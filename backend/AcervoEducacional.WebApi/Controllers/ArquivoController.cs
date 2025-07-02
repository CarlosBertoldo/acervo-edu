using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AcervoEducacional.Application.DTOs.Arquivo;
using AcervoEducacional.Application.Interfaces;

namespace AcervoEducacional.WebApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[Authorize]
public class ArquivoController : ControllerBase
{
    private readonly IArquivoService _arquivoService;
    private readonly ILogger<ArquivoController> _logger;

    public ArquivoController(IArquivoService arquivoService, ILogger<ArquivoController> logger)
    {
        _arquivoService = arquivoService;
        _logger = logger;
    }

    /// <summary>
    /// Obter arquivo por ID
    /// </summary>
    /// <param name="id">ID do arquivo</param>
    /// <returns>Dados completos do arquivo</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var result = await _arquivoService.GetByIdAsync(id);

            if (result.Success)
            {
                return Ok(result);
            }

            return NotFound(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar arquivo {ArquivoId}", id);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Listar arquivos com filtros e paginação
    /// </summary>
    /// <param name="filter">Filtros de busca</param>
    /// <returns>Lista paginada de arquivos</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ArquivoFilterDto filter)
    {
        try
        {
            var result = await _arquivoService.GetAllAsync(filter);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar arquivos com filtros");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Obter arquivos por curso, organizados por categoria
    /// </summary>
    /// <param name="cursoId">ID do curso</param>
    /// <returns>Arquivos organizados por categoria</returns>
    [HttpGet("curso/{cursoId}")]
    public async Task<IActionResult> GetByCurso(int cursoId)
    {
        try
        {
            var result = await _arquivoService.GetByCursoAsync(cursoId);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar arquivos do curso {CursoId}", cursoId);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Upload de arquivo
    /// </summary>
    /// <param name="cursoId">ID do curso</param>
    /// <param name="file">Arquivo a ser enviado</param>
    /// <param name="dto">Dados do arquivo</param>
    /// <returns>Arquivo criado</returns>
    [HttpPost("upload/{cursoId}")]
    [RequestSizeLimit(100_000_000)] // 100MB
    [RequestFormLimits(MultipartBodyLengthLimit = 100_000_000)]
    public async Task<IActionResult> Upload(int cursoId, IFormFile file, [FromForm] CreateArquivoDto dto)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "Arquivo é obrigatório" });
            }

            var usuarioId = GetCurrentUserId();
            if (usuarioId == 0)
            {
                return Unauthorized(new { message = "Usuário não identificado" });
            }

            // Validações básicas do arquivo
            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".txt", ".jpg", ".jpeg", ".png", ".gif", ".mp4", ".avi", ".zip", ".rar" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            
            if (!allowedExtensions.Contains(fileExtension))
            {
                return BadRequest(new { message = "Tipo de arquivo não permitido" });
            }

            if (file.Length > 100_000_000) // 100MB
            {
                return BadRequest(new { message = "Arquivo muito grande. Máximo permitido: 100MB" });
            }

            // Preparar DTO com dados do arquivo
            dto.NomeOriginal = file.FileName;
            dto.TamanhoBytes = file.Length;
            dto.TipoConteudo = file.ContentType;

            using var stream = file.OpenReadStream();
            var result = await _arquivoService.UploadAsync(cursoId, dto, usuarioId);

            if (result.Success)
            {
                return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao fazer upload de arquivo para curso {CursoId}", cursoId);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Atualizar metadados do arquivo
    /// </summary>
    /// <param name="id">ID do arquivo</param>
    /// <param name="dto">Dados atualizados</param>
    /// <returns>Arquivo atualizado</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateArquivoDto dto)
    {
        try
        {
            var usuarioId = GetCurrentUserId();
            if (usuarioId == 0)
            {
                return Unauthorized(new { message = "Usuário não identificado" });
            }

            var result = await _arquivoService.UpdateAsync(id, dto, usuarioId);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar arquivo {ArquivoId}", id);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Excluir arquivo
    /// </summary>
    /// <param name="id">ID do arquivo</param>
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

            var result = await _arquivoService.DeleteAsync(id, usuarioId);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir arquivo {ArquivoId}", id);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Obter URL de download do arquivo
    /// </summary>
    /// <param name="id">ID do arquivo</param>
    /// <returns>URL pré-assinada para download</returns>
    [HttpGet("{id}/download")]
    public async Task<IActionResult> GetDownloadUrl(int id)
    {
        try
        {
            var usuarioId = GetCurrentUserId();
            if (usuarioId == 0)
            {
                return Unauthorized(new { message = "Usuário não identificado" });
            }

            var result = await _arquivoService.GetDownloadUrlAsync(id, usuarioId);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar URL de download para arquivo {ArquivoId}", id);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Compartilhar arquivo
    /// </summary>
    /// <param name="id">ID do arquivo</param>
    /// <param name="dto">Configurações de compartilhamento</param>
    /// <returns>Link de compartilhamento</returns>
    [HttpPost("{id}/share")]
    public async Task<IActionResult> Share(int id, [FromBody] ShareArquivoDto dto)
    {
        try
        {
            var usuarioId = GetCurrentUserId();
            if (usuarioId == 0)
            {
                return Unauthorized(new { message = "Usuário não identificado" });
            }

            var result = await _arquivoService.ShareAsync(id, dto, usuarioId);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao compartilhar arquivo {ArquivoId}", id);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Download direto do arquivo (público)
    /// </summary>
    /// <param name="token">Token de compartilhamento</param>
    /// <returns>Arquivo para download</returns>
    [HttpGet("public/{token}")]
    [AllowAnonymous]
    public async Task<IActionResult> PublicDownload(string token)
    {
        try
        {
            // Esta funcionalidade seria implementada com validação de token público
            // Por enquanto, retorna não implementado
            return StatusCode(501, new { message = "Funcionalidade em desenvolvimento" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro no download público com token {Token}", token);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Visualizar arquivo (embed)
    /// </summary>
    /// <param name="id">ID do arquivo</param>
    /// <returns>URL para visualização embed</returns>
    [HttpGet("{id}/preview")]
    public async Task<IActionResult> GetPreviewUrl(int id)
    {
        try
        {
            var usuarioId = GetCurrentUserId();
            if (usuarioId == 0)
            {
                return Unauthorized(new { message = "Usuário não identificado" });
            }

            // Esta funcionalidade seria implementada com geração de URL de preview
            // Por enquanto, retorna não implementado
            return StatusCode(501, new { message = "Funcionalidade em desenvolvimento" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar preview para arquivo {ArquivoId}", id);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Obter estatísticas de downloads do arquivo
    /// </summary>
    /// <param name="id">ID do arquivo</param>
    /// <returns>Estatísticas de acesso</returns>
    [HttpGet("{id}/stats")]
    public async Task<IActionResult> GetStats(int id)
    {
        try
        {
            // Esta funcionalidade seria implementada com contadores de download
            // Por enquanto, retorna não implementado
            return StatusCode(501, new { message = "Funcionalidade em desenvolvimento" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar estatísticas do arquivo {ArquivoId}", id);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Listar versões do arquivo
    /// </summary>
    /// <param name="id">ID do arquivo</param>
    /// <returns>Histórico de versões</returns>
    [HttpGet("{id}/versions")]
    public async Task<IActionResult> GetVersions(int id)
    {
        try
        {
            // Esta funcionalidade seria implementada com controle de versões
            // Por enquanto, retorna não implementado
            return StatusCode(501, new { message = "Funcionalidade em desenvolvimento" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar versões do arquivo {ArquivoId}", id);
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

