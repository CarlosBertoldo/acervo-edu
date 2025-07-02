using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AcervoEducacional.Application.DTOs.Usuario;
using AcervoEducacional.Application.Interfaces;

namespace AcervoEducacional.WebApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[Authorize]
public class UsuarioController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;
    private readonly ILogger<UsuarioController> _logger;

    public UsuarioController(IUsuarioService usuarioService, ILogger<UsuarioController> logger)
    {
        _usuarioService = usuarioService;
        _logger = logger;
    }

    /// <summary>
    /// Obter usuário por ID
    /// </summary>
    /// <param name="id">ID do usuário</param>
    /// <returns>Dados completos do usuário</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var result = await _usuarioService.GetByIdAsync(id);

            if (result.Success)
            {
                return Ok(result);
            }

            return NotFound(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar usuário {UsuarioId}", id);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Listar usuários com filtros e paginação
    /// </summary>
    /// <param name="filter">Filtros de busca</param>
    /// <returns>Lista paginada de usuários</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] UsuarioFilterDto filter)
    {
        try
        {
            var result = await _usuarioService.GetAllAsync(filter);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar usuários com filtros");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Obter usuário por email
    /// </summary>
    /// <param name="email">Email do usuário</param>
    /// <returns>Dados do usuário</returns>
    [HttpGet("email/{email}")]
    public async Task<IActionResult> GetByEmail(string email)
    {
        try
        {
            var result = await _usuarioService.GetByEmailAsync(email);

            if (result.Success)
            {
                return Ok(result);
            }

            return NotFound(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar usuário por email {Email}", email);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Criar novo usuário
    /// </summary>
    /// <param name="dto">Dados do usuário a ser criado</param>
    /// <returns>Usuário criado</returns>
    [HttpPost]
    [Authorize(Roles = "Admin,Gestor")]
    public async Task<IActionResult> Create([FromBody] CreateUsuarioDto dto)
    {
        try
        {
            var criadoPor = GetCurrentUserId();
            if (criadoPor == 0)
            {
                return Unauthorized(new { message = "Usuário não identificado" });
            }

            var result = await _usuarioService.CreateAsync(dto, criadoPor);

            if (result.Success)
            {
                return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar usuário");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Atualizar usuário existente
    /// </summary>
    /// <param name="id">ID do usuário</param>
    /// <param name="dto">Dados atualizados do usuário</param>
    /// <returns>Usuário atualizado</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUsuarioDto dto)
    {
        try
        {
            var atualizadoPor = GetCurrentUserId();
            if (atualizadoPor == 0)
            {
                return Unauthorized(new { message = "Usuário não identificado" });
            }

            // Verificar se o usuário pode atualizar este registro
            if (!CanUpdateUser(id, atualizadoPor))
            {
                return Forbid("Você não tem permissão para atualizar este usuário");
            }

            var result = await _usuarioService.UpdateAsync(id, dto, atualizadoPor);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar usuário {UsuarioId}", id);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Excluir usuário (soft delete)
    /// </summary>
    /// <param name="id">ID do usuário</param>
    /// <returns>Confirmação de exclusão</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var deletadoPor = GetCurrentUserId();
            if (deletadoPor == 0)
            {
                return Unauthorized(new { message = "Usuário não identificado" });
            }

            // Não permitir auto-exclusão
            if (id == deletadoPor)
            {
                return BadRequest(new { message = "Não é possível excluir seu próprio usuário" });
            }

            var result = await _usuarioService.DeleteAsync(id, deletadoPor);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir usuário {UsuarioId}", id);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Alterar senha do usuário
    /// </summary>
    /// <param name="id">ID do usuário</param>
    /// <param name="dto">Dados para alteração de senha</param>
    /// <returns>Confirmação de alteração</returns>
    [HttpPatch("{id}/password")]
    public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordDto dto)
    {
        try
        {
            var usuarioLogado = GetCurrentUserId();
            if (usuarioLogado == 0)
            {
                return Unauthorized(new { message = "Usuário não identificado" });
            }

            // Verificar se o usuário pode alterar a senha
            if (!CanChangePassword(id, usuarioLogado))
            {
                return Forbid("Você não tem permissão para alterar a senha deste usuário");
            }

            var result = await _usuarioService.ChangePasswordAsync(id, dto, usuarioLogado);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao alterar senha do usuário {UsuarioId}", id);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Ativar/Desativar usuário
    /// </summary>
    /// <param name="id">ID do usuário</param>
    /// <param name="dto">Status de ativação</param>
    /// <returns>Usuário com status atualizado</returns>
    [HttpPatch("{id}/status")]
    [Authorize(Roles = "Admin,Gestor")]
    public async Task<IActionResult> ToggleStatus(int id, [FromBody] ToggleUsuarioStatusDto dto)
    {
        try
        {
            var atualizadoPor = GetCurrentUserId();
            if (atualizadoPor == 0)
            {
                return Unauthorized(new { message = "Usuário não identificado" });
            }

            // Não permitir auto-desativação
            if (id == atualizadoPor && !dto.Ativo)
            {
                return BadRequest(new { message = "Não é possível desativar seu próprio usuário" });
            }

            var result = await _usuarioService.ToggleStatusAsync(id, dto, atualizadoPor);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao alterar status do usuário {UsuarioId}", id);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Obter perfil do usuário logado
    /// </summary>
    /// <returns>Dados do perfil</returns>
    [HttpGet("me")]
    public async Task<IActionResult> GetProfile()
    {
        try
        {
            var usuarioId = GetCurrentUserId();
            if (usuarioId == 0)
            {
                return Unauthorized(new { message = "Usuário não identificado" });
            }

            var result = await _usuarioService.GetByIdAsync(usuarioId);

            if (result.Success)
            {
                return Ok(result);
            }

            return NotFound(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar perfil do usuário");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Atualizar perfil do usuário logado
    /// </summary>
    /// <param name="dto">Dados do perfil</param>
    /// <returns>Perfil atualizado</returns>
    [HttpPut("me")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdatePerfilDto dto)
    {
        try
        {
            var usuarioId = GetCurrentUserId();
            if (usuarioId == 0)
            {
                return Unauthorized(new { message = "Usuário não identificado" });
            }

            var result = await _usuarioService.UpdatePerfilAsync(usuarioId, dto);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar perfil do usuário");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Obter estatísticas de usuários
    /// </summary>
    /// <returns>Estatísticas gerais</returns>
    [HttpGet("stats")]
    [Authorize(Roles = "Admin,Gestor")]
    public async Task<IActionResult> GetStats()
    {
        try
        {
            var result = await _usuarioService.GetStatsAsync();

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar estatísticas de usuários");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Verificar disponibilidade de email
    /// </summary>
    /// <param name="email">Email a ser verificado</param>
    /// <returns>Disponibilidade do email</returns>
    [HttpGet("check-email/{email}")]
    [AllowAnonymous]
    public async Task<IActionResult> CheckEmailAvailability(string email)
    {
        try
        {
            var result = await _usuarioService.CheckEmailAvailabilityAsync(email);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar disponibilidade do email {Email}", email);
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

    private bool CanUpdateUser(int targetUserId, int currentUserId)
    {
        // Usuário pode atualizar a si mesmo
        if (targetUserId == currentUserId)
            return true;

        // Admin e Gestor podem atualizar outros usuários
        return User.IsInRole("Admin") || User.IsInRole("Gestor");
    }

    private bool CanChangePassword(int targetUserId, int currentUserId)
    {
        // Usuário pode alterar sua própria senha
        if (targetUserId == currentUserId)
            return true;

        // Apenas Admin pode alterar senha de outros usuários
        return User.IsInRole("Admin");
    }

    #endregion
}

