using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace AcervoEducacional.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CursoController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        await Task.Delay(1); // Simular async
        
        return Ok(new { 
            data = new[] { 
                new { Id = 1, Nome = "Curso de Gestão", Status = "Ativo", Codigo = "CGT001" },
                new { Id = 2, Nome = "Curso de Liderança", Status = "Backlog", Codigo = "CLD002" }
            },
            totalCount = 2 
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        await Task.Delay(1); // Simular async
        
        return Ok(new { 
            Id = id,
            Nome = "Curso de Gestão",
            Status = "Ativo",
            Codigo = "CGT001",
            DescricaoAcademia = "Curso completo de gestão empresarial"
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] object dto)
    {
        await Task.Delay(1); // Simular async
        
        return Ok(new { 
            success = true, 
            message = "Curso criado com sucesso",
            data = new { Id = 3, Nome = "Novo Curso" }
        });
    }
}