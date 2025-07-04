using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace AcervoEducacional.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        await Task.Delay(1); // Simular async
        
        return Ok(new { 
            TotalCursos = 15,
            CursosPorStatus = new { 
                Ativo = 8,
                Backlog = 4,
                EmDesenvolvimento = 2,
                Concluido = 1
            },
            TotalArquivos = 127,
            UsuariosAtivos = 12,
            ArquivosPorCategoria = new {
                Videos = 45,
                Documentos = 38,
                Apresentacoes = 24,
                Outros = 20
            }
        });
    }

    [HttpGet("recent-activity")]
    public async Task<IActionResult> GetRecentActivity()
    {
        await Task.Delay(1); // Simular async
        
        return Ok(new { 
            data = new[] { 
                new { 
                    Id = 1, 
                    Acao = "Upload de arquivo",
                    Usuario = "João Silva",
                    Curso = "Gestão de Pessoas",
                    Timestamp = DateTime.UtcNow.AddMinutes(-15)
                },
                new { 
                    Id = 2, 
                    Acao = "Criação de curso",
                    Usuario = "Maria Santos",
                    Curso = "Liderança Estratégica",
                    Timestamp = DateTime.UtcNow.AddHours(-2)
                }
            }
        });
    }
}