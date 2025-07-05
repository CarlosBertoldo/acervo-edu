using Hangfire.Dashboard;

namespace AcervoEducacional.WebApi.Filters;

/// <summary>
/// Filtro de autorização para o dashboard do Hangfire
/// Permite acesso apenas para usuários autenticados como Administrador
/// </summary>
public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();
        
        // Em desenvolvimento, permitir acesso livre
        if (httpContext.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
        {
            return true;
        }
        
        // Em produção, verificar se usuário está autenticado e é administrador
        return httpContext.User.Identity?.IsAuthenticated == true && 
               httpContext.User.IsInRole("Administrador");
    }
}