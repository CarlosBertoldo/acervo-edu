using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AcervoEducacional.Application.DTOs.Curso;
using AcervoEducacional.Application.Interfaces;

namespace AcervoEducacional.WebApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[Authorize]
public class CursoController : ControllerBase
{
    private readonly ICursoService _cursoService;
    private readonly ILogger<CursoController> _logger;

    public CursoController(ICursoService cursoService, ILogger<CursoController> logger)
    {
        _cursoService = cursoService;
        _logger = logger;
    }

    /// <summary>
    /// Obter curso por ID
    /// </summary>
    /// <param name="id">ID do curso</param>
    /// <returns>Dados completos do curso</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var result = await _cursoService.GetByIdAsync(id);

            if (result.Success)
            {
                return Ok(result);
            }

            return NotFound(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar curso {CursoId}", id);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Listar cursos com filtros e paginação
    /// </summary>
    /// <param name="filter">Filtros de busca</param>
    /// <returns>Lista paginada de cursos</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] CursoFilterDto filter)
    {
        try
        {
            var result = await _cursoService.GetAllAsync(filter);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar cursos com filtros");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Obter dados do Kanban de cursos
    /// </summary>
    /// <returns>Cursos organizados por status para visualização Kanban</returns>
    [HttpGet("kanban")]
    public async Task<IActionResult> GetKanban()
    {
        try
        {
            var result = await _cursoService.GetKanbanAsync();

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar dados do Kanban");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Criar novo curso
    /// </summary>
    /// <param name="dto">Dados do curso a ser criado</param>
    /// <returns>Curso criado</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCursoDto dto)
    {
        try
        {
            var usuarioId = GetCurrentUserId();
            if (usuarioId == 0)
            {
                return Unauthorized(new { message = "Usuário não identificado" });
            }

            var result = await _cursoService.CreateAsync(dto, usuarioId);

            if (result.Success)
            {
                return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar curso");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Atualizar curso existente
    /// </summary>
    /// <param name="id">ID do curso</param>
    /// <param name="dto">Dados atualizados do curso</param>
    /// <returns>Curso atualizado</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCursoDto dto)
    {
        try
        {
            var usuarioId = GetCurrentUserId();
            if (usuarioId == 0)
            {
                return Unauthorized(new { message = "Usuário não identificado" });
            }

            var result = await _cursoService.UpdateAsync(id, dto, usuarioId);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar curso {CursoId}", id);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Atualizar status do curso
    /// </summary>
    /// <param name="id">ID do curso</param>
    /// <param name="dto">Novo status</param>
    /// <returns>Curso com status atualizado</returns>
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusCursoDto dto)
    {
        try
        {
            var usuarioId = GetCurrentUserId();
            if (usuarioId == 0)
            {
                return Unauthorized(new { message = "Usuário não identificado" });
            }

            var result = await _cursoService.UpdateStatusAsync(id, dto, usuarioId);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar status do curso {CursoId}", id);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Excluir curso
    /// </summary>
    /// <param name="id">ID do curso</param>
    /// <returns>Confirmação de exclusão</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var usuarioId = GetCurrentUserId();
            if (usuarioId == 0)
            {
                return Unauthorized(new { message = "Usuário não identificado" });
            }

            var result = await _cursoService.DeleteAsync(id, usuarioId);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir curso {CursoId}", id);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Obter estatísticas do dashboard
    /// </summary>
    /// <returns>Estatísticas gerais dos cursos</returns>
    [HttpGet("dashboard/stats")]
    public async Task<IActionResult> GetDashboardStats()
    {
        try
        {
            var result = await _cursoService.GetDashboardStatsAsync();

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar estatísticas do dashboard");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    #region Métodos Auxiliares

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst("sub") ?? User.FindFirst("id") ?? User.FindFirst("nameid");
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
        {
            return userId;
        }
        return 0;
    }

    #endregion
}

