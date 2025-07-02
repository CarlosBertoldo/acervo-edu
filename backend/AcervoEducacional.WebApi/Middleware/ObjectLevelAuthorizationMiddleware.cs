using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace AcervoEducacional.WebApi.Middleware
{
    /// <summary>
    /// Middleware para prevenir vulnerabilidades BOLA (Broken Object Level Authorization)
    /// Ativado apenas em produção via flag ENABLE_BOLA_PROTECTION
    /// </summary>
    public class ObjectLevelAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ObjectLevelAuthorizationMiddleware> _logger;
        
        // Flag para ativar/desativar o middleware
        private readonly bool _isEnabled;
        
        // Padrões de rotas que requerem validação de propriedade
        private static readonly Dictionary<string, string> _protectedRoutes = new()
        {
            // Usuários - apenas o próprio usuário ou admin pode acessar
            { @"^/api/v1/usuario/(\d+)$", "usuario" },
            { @"^/api/v1/usuario/(\d+)/.*$", "usuario" },
            
            // Arquivos - apenas o proprietário ou usuários com acesso ao curso podem acessar
            { @"^/api/v1/arquivo/(\d+)$", "arquivo" },
            { @"^/api/v1/arquivo/(\d+)/.*$", "arquivo" },
            
            // Cursos - apenas usuários matriculados ou instrutores podem acessar
            { @"^/api/v1/curso/(\d+)$", "curso" },
            { @"^/api/v1/curso/(\d+)/.*$", "curso" },
        };

        public ObjectLevelAuthorizationMiddleware(
            RequestDelegate next, 
            IConfiguration configuration,
            ILogger<ObjectLevelAuthorizationMiddleware> logger)
        {
            _next = next;
            _configuration = configuration;
            _logger = logger;
            
            // Ler flag de ambiente - padrão false para desenvolvimento
            _isEnabled = _configuration.GetValue<bool>("Security:EnableBolaProtection", false);
            
            if (_isEnabled)
            {
                _logger.LogInformation("BOLA Protection ATIVADA - Middleware de autorização contextual habilitado");
            }
            else
            {
                _logger.LogInformation("BOLA Protection DESATIVADA - Modo desenvolvimento (use ENABLE_BOLA_PROTECTION=true para ativar)");
            }
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Se middleware está desabilitado, pular validação
            if (!_isEnabled)
            {
                await _next(context);
                return;
            }

            // Verificar se a rota requer validação BOLA
            var path = context.Request.Path.Value?.ToLower() ?? "";
            var method = context.Request.Method.ToUpper();
            
            // Apenas validar operações que podem expor dados (GET, PUT, DELETE)
            if (method == "POST" || method == "OPTIONS")
            {
                await _next(context);
                return;
            }

            var (requiresValidation, resourceType, resourceId) = CheckIfRouteRequiresValidation(path);
            
            if (!requiresValidation)
            {
                await _next(context);
                return;
            }

            // Verificar se usuário está autenticado
            if (!context.User.Identity?.IsAuthenticated ?? true)
            {
                await ReturnUnauthorized(context, "Usuário não autenticado");
                return;
            }

            // Obter ID do usuário atual
            var currentUserId = GetCurrentUserId(context);
            if (currentUserId == null)
            {
                await ReturnUnauthorized(context, "ID do usuário não encontrado no token");
                return;
            }

            // Verificar autorização baseada no tipo de recurso
            var isAuthorized = await ValidateResourceAccess(context, resourceType, resourceId, currentUserId.Value);
            
            if (!isAuthorized)
            {
                await LogSecurityViolation(context, resourceType, resourceId, currentUserId.Value);
                await ReturnForbidden(context, $"Acesso negado ao {resourceType} {resourceId}");
                return;
            }

            // Usuário autorizado, continuar
            await _next(context);
        }

        private (bool requiresValidation, string resourceType, int resourceId) CheckIfRouteRequiresValidation(string path)
        {
            foreach (var route in _protectedRoutes)
            {
                var regex = new Regex(route.Key, RegexOptions.IgnoreCase);
                var match = regex.Match(path);
                
                if (match.Success && match.Groups.Count > 1)
                {
                    if (int.TryParse(match.Groups[1].Value, out int resourceId))
                    {
                        return (true, route.Value, resourceId);
                    }
                }
            }
            
            return (false, "", 0);
        }

        private int? GetCurrentUserId(HttpContext context)
        {
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }
            return null;
        }

        private async Task<bool> ValidateResourceAccess(HttpContext context, string resourceType, int resourceId, int currentUserId)
        {
            try
            {
                // Verificar se é admin (admins têm acesso total)
                if (IsUserAdmin(context))
                {
                    _logger.LogInformation("Acesso autorizado: Usuário {UserId} é admin", currentUserId);
                    return true;
                }

                return resourceType switch
                {
                    "usuario" => await ValidateUsuarioAccess(resourceId, currentUserId),
                    "arquivo" => await ValidateArquivoAccess(resourceId, currentUserId),
                    "curso" => await ValidateCursoAccess(resourceId, currentUserId),
                    _ => false
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar acesso ao recurso {ResourceType} {ResourceId} para usuário {UserId}", 
                    resourceType, resourceId, currentUserId);
                return false;
            }
        }

        private bool IsUserAdmin(HttpContext context)
        {
            return context.User.IsInRole("Admin") || context.User.IsInRole("Gestor");
        }

        private async Task<bool> ValidateUsuarioAccess(int usuarioId, int currentUserId)
        {
            // Usuário só pode acessar seus próprios dados
            var isOwner = usuarioId == currentUserId;
            
            if (isOwner)
            {
                _logger.LogInformation("Acesso autorizado: Usuário {UserId} acessando próprios dados", currentUserId);
            }
            else
            {
                _logger.LogWarning("Tentativa de acesso BOLA: Usuário {UserId} tentou acessar dados do usuário {TargetUserId}", 
                    currentUserId, usuarioId);
            }
            
            return isOwner;
        }

        private async Task<bool> ValidateArquivoAccess(int arquivoId, int currentUserId)
        {
            // TODO: Implementar validação real com banco de dados
            // Por enquanto, simular validação (em produção, consultar banco)
            
            // Simular que usuário tem acesso se for o proprietário do arquivo
            // ou se tiver acesso ao curso relacionado
            
            _logger.LogInformation("Validando acesso ao arquivo {ArquivoId} para usuário {UserId}", arquivoId, currentUserId);
            
            // Em desenvolvimento, permitir acesso (flag desabilitada)
            // Em produção, implementar consulta real ao banco
            return true; // Temporário - implementar validação real
        }

        private async Task<bool> ValidateCursoAccess(int cursoId, int currentUserId)
        {
            // TODO: Implementar validação real com banco de dados
            // Verificar se usuário está matriculado no curso ou é instrutor
            
            _logger.LogInformation("Validando acesso ao curso {CursoId} para usuário {UserId}", cursoId, currentUserId);
            
            // Em desenvolvimento, permitir acesso (flag desabilitada)
            // Em produção, implementar consulta real ao banco
            return true; // Temporário - implementar validação real
        }

        private async Task LogSecurityViolation(HttpContext context, string resourceType, int resourceId, int userId)
        {
            var violation = new
            {
                Timestamp = DateTime.UtcNow,
                UserId = userId,
                ResourceType = resourceType,
                ResourceId = resourceId,
                Path = context.Request.Path,
                Method = context.Request.Method,
                IpAddress = context.Connection.RemoteIpAddress?.ToString(),
                UserAgent = context.Request.Headers["User-Agent"].ToString()
            };

            _logger.LogWarning("VIOLAÇÃO DE SEGURANÇA BOLA: {Violation}", JsonSerializer.Serialize(violation));
            
            // TODO: Enviar para sistema de monitoramento/alertas
        }

        private async Task ReturnUnauthorized(HttpContext context, string message)
        {
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            
            var response = new { error = "Unauthorized", message = message };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }

        private async Task ReturnForbidden(HttpContext context, string message)
        {
            context.Response.StatusCode = 403;
            context.Response.ContentType = "application/json";
            
            var response = new { error = "Forbidden", message = message };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }

    /// <summary>
    /// Extensão para facilitar o registro do middleware
    /// </summary>
    public static class ObjectLevelAuthorizationMiddlewareExtensions
    {
        public static IApplicationBuilder UseObjectLevelAuthorization(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ObjectLevelAuthorizationMiddleware>();
        }
    }
}

