using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace AcervoEducacional.WebApi.Middleware
{
    /// <summary>
    /// Middleware responsável por adicionar headers de segurança em todas as respostas da API
    /// </summary>
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Adicionar headers de segurança
            AddSecurityHeaders(context.Response);

            await _next(context);
        }

        private static void AddSecurityHeaders(HttpResponse response)
        {
            // X-Content-Type-Options: Previne MIME type sniffing
            if (!response.Headers.ContainsKey("X-Content-Type-Options"))
            {
                response.Headers.Add("X-Content-Type-Options", "nosniff");
            }

            // X-Frame-Options: Previne clickjacking
            if (!response.Headers.ContainsKey("X-Frame-Options"))
            {
                response.Headers.Add("X-Frame-Options", "DENY");
            }

            // X-XSS-Protection: Ativa proteção XSS do browser
            if (!response.Headers.ContainsKey("X-XSS-Protection"))
            {
                response.Headers.Add("X-XSS-Protection", "1; mode=block");
            }

            // Referrer-Policy: Controla informações de referrer
            if (!response.Headers.ContainsKey("Referrer-Policy"))
            {
                response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
            }

            // Content-Security-Policy: Política de segurança de conteúdo básica para API
            if (!response.Headers.ContainsKey("Content-Security-Policy"))
            {
                response.Headers.Add("Content-Security-Policy", "default-src 'self'; frame-ancestors 'none';");
            }

            // Permissions-Policy: Controla APIs do browser
            if (!response.Headers.ContainsKey("Permissions-Policy"))
            {
                response.Headers.Add("Permissions-Policy", "camera=(), microphone=(), geolocation=()");
            }

            // X-Permitted-Cross-Domain-Policies: Controla políticas cross-domain
            if (!response.Headers.ContainsKey("X-Permitted-Cross-Domain-Policies"))
            {
                response.Headers.Add("X-Permitted-Cross-Domain-Policies", "none");
            }

            // Cache-Control: Controla cache para endpoints sensíveis
            if (IsSensitiveEndpoint(response.HttpContext.Request.Path))
            {
                if (!response.Headers.ContainsKey("Cache-Control"))
                {
                    response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
                }
                if (!response.Headers.ContainsKey("Pragma"))
                {
                    response.Headers.Add("Pragma", "no-cache");
                }
                if (!response.Headers.ContainsKey("Expires"))
                {
                    response.Headers.Add("Expires", "0");
                }
            }
        }

        private static bool IsSensitiveEndpoint(PathString path)
        {
            var sensitiveEndpoints = new[]
            {
                "/api/v1/auth/login",
                "/api/v1/auth/refresh",
                "/api/v1/auth/logout",
                "/api/v1/auth/forgot-password",
                "/api/v1/auth/reset-password",
                "/api/v1/usuarios/profile",
                "/hangfire"
            };

            return sensitiveEndpoints.Any(endpoint => 
                path.Value?.StartsWith(endpoint, StringComparison.OrdinalIgnoreCase) == true);
        }
    }

    /// <summary>
    /// Extensão para facilitar o registro do middleware
    /// </summary>
    public static class SecurityHeadersMiddlewareExtensions
    {
        public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SecurityHeadersMiddleware>();
        }
    }
}

