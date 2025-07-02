using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AcervoEducacional.Application.Interfaces;

namespace AcervoEducacional.WebApi.Middleware
{
    /// <summary>
    /// Middleware para validação automática de tokens JWT em todas as requisições
    /// </summary>
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private readonly ILogger<JwtMiddleware> _logger;

        public JwtMiddleware(RequestDelegate next, IConfiguration configuration, ILogger<JwtMiddleware> logger)
        {
            _next = next;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IAuthService authService)
        {
            var token = ExtractToken(context);
            
            if (!string.IsNullOrEmpty(token))
            {
                await AttachUserToContext(context, authService, token);
            }

            await _next(context);
        }

        /// <summary>
        /// Extrai o token JWT do header Authorization
        /// </summary>
        private string? ExtractToken(HttpContext context)
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            
            if (authHeader != null && authHeader.StartsWith("Bearer "))
            {
                return authHeader.Substring("Bearer ".Length).Trim();
            }

            return null;
        }

        /// <summary>
        /// Valida o token e anexa o usuário ao contexto da requisição
        /// </summary>
        private async Task AttachUserToContext(HttpContext context, IAuthService authService, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"]!);
                
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                
                if (validatedToken is JwtSecurityToken jwtToken)
                {
                    var userIdClaim = principal.FindFirst("userId")?.Value;
                    var emailClaim = principal.FindFirst("email")?.Value;
                    var roleClaim = principal.FindFirst("role")?.Value;
                    
                    if (!string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out var userId))
                    {
                        // Anexa informações do usuário ao contexto
                        context.Items["UserId"] = userId;
                        context.Items["UserEmail"] = emailClaim;
                        context.Items["UserRole"] = roleClaim;
                        context.Items["IsAuthenticated"] = true;
                        
                        // Cria ClaimsPrincipal para uso com [Authorize]
                        var claims = new List<Claim>
                        {
                            new Claim("userId", userIdClaim),
                            new Claim("email", emailClaim ?? ""),
                            new Claim("role", roleClaim ?? "Usuario"),
                            new Claim(ClaimTypes.NameIdentifier, userIdClaim),
                            new Claim(ClaimTypes.Email, emailClaim ?? ""),
                            new Claim(ClaimTypes.Role, roleClaim ?? "Usuario")
                        };
                        
                        var identity = new ClaimsIdentity(claims, "jwt");
                        context.User = new ClaimsPrincipal(identity);
                        
                        _logger.LogDebug("Token JWT válido para usuário {UserId} ({Email})", userId, emailClaim);
                    }
                }
            }
            catch (SecurityTokenExpiredException)
            {
                _logger.LogWarning("Token JWT expirado na requisição {Path}", context.Request.Path);
                context.Items["TokenExpired"] = true;
            }
            catch (SecurityTokenException ex)
            {
                _logger.LogWarning("Token JWT inválido na requisição {Path}: {Error}", context.Request.Path, ex.Message);
                context.Items["TokenInvalid"] = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar token JWT na requisição {Path}", context.Request.Path);
                context.Items["TokenError"] = true;
            }
        }
    }

    /// <summary>
    /// Extensão para registrar o JwtMiddleware
    /// </summary>
    public static class JwtMiddlewareExtensions
    {
        public static IApplicationBuilder UseJwtMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<JwtMiddleware>();
        }
    }

    /// <summary>
    /// Atributo para endpoints que requerem autenticação
    /// </summary>
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[]? _roles;

        public AuthorizeAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var isAuthenticated = context.HttpContext.Items["IsAuthenticated"] as bool? ?? false;
            
            if (!isAuthenticated)
            {
                context.Result = new Microsoft.AspNetCore.Mvc.UnauthorizedObjectResult(new
                {
                    success = false,
                    message = "Token de acesso requerido",
                    error = "UNAUTHORIZED"
                });
                return;
            }

            // Verifica roles se especificadas
            if (_roles != null && _roles.Length > 0)
            {
                var userRole = context.HttpContext.Items["UserRole"] as string;
                
                if (string.IsNullOrEmpty(userRole) || !_roles.Contains(userRole))
                {
                    context.Result = new Microsoft.AspNetCore.Mvc.ForbiddenObjectResult(new
                    {
                        success = false,
                        message = "Acesso negado. Permissões insuficientes",
                        error = "FORBIDDEN",
                        requiredRoles = _roles
                    });
                    return;
                }
            }
        }
    }

    /// <summary>
    /// Extensões para acessar informações do usuário autenticado
    /// </summary>
    public static class HttpContextExtensions
    {
        public static Guid? GetUserId(this HttpContext context)
        {
            return context.Items["UserId"] as Guid?;
        }

        public static string? GetUserEmail(this HttpContext context)
        {
            return context.Items["UserEmail"] as string;
        }

        public static string? GetUserRole(this HttpContext context)
        {
            return context.Items["UserRole"] as string;
        }

        public static bool IsAuthenticated(this HttpContext context)
        {
            return context.Items["IsAuthenticated"] as bool? ?? false;
        }

        public static bool IsTokenExpired(this HttpContext context)
        {
            return context.Items["TokenExpired"] as bool? ?? false;
        }

        public static bool IsTokenInvalid(this HttpContext context)
        {
            return context.Items["TokenInvalid"] as bool? ?? false;
        }

        public static bool HasTokenError(this HttpContext context)
        {
            return context.Items["TokenError"] as bool? ?? false;
        }
    }
}

