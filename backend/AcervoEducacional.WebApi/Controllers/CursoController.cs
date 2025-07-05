using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace AcervoEducacional.WebApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[Authorize]
public class CursoController : ControllerBase
{
    private readonly ILogger<CursoController> _logger;

    public CursoController(ILogger<CursoController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Obter curso por ID - Temporário com dados mock
    /// </summary>
    /// <param name="id">ID do curso</param>
    /// <returns>Dados completos do curso</returns>
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

    /// <summary>
    /// Listar cursos - Temporário com dados mock
    /// </summary>
    /// <returns>Lista de cursos</returns>
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

    /// <summary>
    /// Criar novo curso - Temporário com dados mock
    /// </summary>
    /// <param name="dto">Dados do curso a ser criado</param>
    /// <returns>Curso criado</returns>
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