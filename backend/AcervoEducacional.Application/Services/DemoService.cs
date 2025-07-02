using AcervoEducacional.Application.DTOs.Common;

namespace AcervoEducacional.Application.Services;

public class DemoService
{
    public async Task<ApiResponse<string>> GetStatusAsync()
    {
        await Task.Delay(100); // Simular operação async
        
        return new ApiResponse<string>
        {
            Success = true,
            Data = "Sistema Acervo Educacional Ferreira Costa está funcionando!",
            Message = "API operacional"
        };
    }
    
    public async Task<ApiResponse<object>> GetInfoAsync()
    {
        await Task.Delay(50);
        
        var info = new
        {
            Sistema = "Acervo Educacional Ferreira Costa",
            Versao = "1.0.0",
            Status = "Operacional",
            Timestamp = DateTime.UtcNow,
            Funcionalidades = new[]
            {
                "Gestão de Usuários",
                "Gestão de Cursos", 
                "Gestão de Arquivos",
                "Sistema de Relatórios",
                "Autenticação JWT",
                "Swagger/OpenAPI",
                "Health Checks",
                "Hangfire Jobs"
            }
        };
        
        return new ApiResponse<object>
        {
            Success = true,
            Data = info,
            Message = "Informações do sistema"
        };
    }
}
