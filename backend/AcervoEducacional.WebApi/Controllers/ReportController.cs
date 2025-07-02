using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AcervoEducacional.Application.DTOs.Report;
using AcervoEducacional.Application.Interfaces;

namespace AcervoEducacional.WebApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[Authorize]
public class ReportController : ControllerBase
{
    private readonly IReportService _reportService;
    private readonly ILogger<ReportController> _logger;

    public ReportController(IReportService reportService, ILogger<ReportController> logger)
    {
        _reportService = reportService;
        _logger = logger;
    }

    /// <summary>
    /// Obter estatísticas gerais do dashboard
    /// </summary>
    /// <returns>Estatísticas consolidadas do sistema</returns>
    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboardStats()
    {
        try
        {
            var result = await _reportService.GetDashboardStatsAsync();

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

    /// <summary>
    /// Obter estatísticas de cursos
    /// </summary>
    /// <param name="filter">Filtros para as estatísticas</param>
    /// <returns>Estatísticas detalhadas de cursos</returns>
    [HttpGet("cursos")]
    public async Task<IActionResult> GetCursoStats([FromQuery] ReportFilterDto filter)
    {
        try
        {
            var result = await _reportService.GetCursoStatsAsync(filter);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar estatísticas de cursos");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Obter estatísticas de arquivos
    /// </summary>
    /// <param name="filter">Filtros para as estatísticas</param>
    /// <returns>Estatísticas detalhadas de arquivos</returns>
    [HttpGet("arquivos")]
    public async Task<IActionResult> GetArquivoStats([FromQuery] ReportFilterDto filter)
    {
        try
        {
            var result = await _reportService.GetArquivoStatsAsync(filter);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar estatísticas de arquivos");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Obter estatísticas de usuários
    /// </summary>
    /// <param name="filter">Filtros para as estatísticas</param>
    /// <returns>Estatísticas detalhadas de usuários</returns>
    [HttpGet("usuarios")]
    [Authorize(Roles = "Admin,Gestor")]
    public async Task<IActionResult> GetUsuarioStats([FromQuery] ReportFilterDto filter)
    {
        try
        {
            var result = await _reportService.GetUsuarioStatsAsync(filter);

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
    /// Exportar relatório de cursos
    /// </summary>
    /// <param name="filter">Filtros para exportação</param>
    /// <param name="format">Formato de exportação (pdf, excel)</param>
    /// <returns>Arquivo do relatório</returns>
    [HttpPost("export/cursos")]
    public async Task<IActionResult> ExportCursos([FromBody] ReportFilterDto filter, [FromQuery] string format = "excel")
    {
        try
        {
            var usuarioId = GetCurrentUserId();
            if (usuarioId == 0)
            {
                return Unauthorized(new { message = "Usuário não identificado" });
            }

            var result = await _reportService.ExportCursosAsync(filter, format, usuarioId);

            if (result.Success)
            {
                var fileBytes = result.Data.FileBytes;
                var fileName = result.Data.FileName;
                var contentType = format.ToLower() == "pdf" ? "application/pdf" : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                return File(fileBytes, contentType, fileName);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao exportar relatório de cursos");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Exportar relatório de arquivos
    /// </summary>
    /// <param name="filter">Filtros para exportação</param>
    /// <param name="format">Formato de exportação (pdf, excel)</param>
    /// <returns>Arquivo do relatório</returns>
    [HttpPost("export/arquivos")]
    public async Task<IActionResult> ExportArquivos([FromBody] ReportFilterDto filter, [FromQuery] string format = "excel")
    {
        try
        {
            var usuarioId = GetCurrentUserId();
            if (usuarioId == 0)
            {
                return Unauthorized(new { message = "Usuário não identificado" });
            }

            var result = await _reportService.ExportArquivosAsync(filter, format, usuarioId);

            if (result.Success)
            {
                var fileBytes = result.Data.FileBytes;
                var fileName = result.Data.FileName;
                var contentType = format.ToLower() == "pdf" ? "application/pdf" : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                return File(fileBytes, contentType, fileName);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao exportar relatório de arquivos");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Exportar relatório de usuários
    /// </summary>
    /// <param name="filter">Filtros para exportação</param>
    /// <param name="format">Formato de exportação (pdf, excel)</param>
    /// <returns>Arquivo do relatório</returns>
    [HttpPost("export/usuarios")]
    [Authorize(Roles = "Admin,Gestor")]
    public async Task<IActionResult> ExportUsuarios([FromBody] ReportFilterDto filter, [FromQuery] string format = "excel")
    {
        try
        {
            var usuarioId = GetCurrentUserId();
            if (usuarioId == 0)
            {
                return Unauthorized(new { message = "Usuário não identificado" });
            }

            var result = await _reportService.ExportUsuariosAsync(filter, format, usuarioId);

            if (result.Success)
            {
                var fileBytes = result.Data.FileBytes;
                var fileName = result.Data.FileName;
                var contentType = format.ToLower() == "pdf" ? "application/pdf" : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                return File(fileBytes, contentType, fileName);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao exportar relatório de usuários");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Exportar logs de atividade
    /// </summary>
    /// <param name="filter">Filtros para exportação</param>
    /// <param name="format">Formato de exportação (pdf, excel)</param>
    /// <returns>Arquivo do relatório</returns>
    [HttpPost("export/logs")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ExportLogs([FromBody] ReportFilterDto filter, [FromQuery] string format = "excel")
    {
        try
        {
            var usuarioId = GetCurrentUserId();
            if (usuarioId == 0)
            {
                return Unauthorized(new { message = "Usuário não identificado" });
            }

            var result = await _reportService.ExportLogsAsync(filter, format, usuarioId);

            if (result.Success)
            {
                var fileBytes = result.Data.FileBytes;
                var fileName = result.Data.FileName;
                var contentType = format.ToLower() == "pdf" ? "application/pdf" : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                return File(fileBytes, contentType, fileName);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao exportar logs de atividade");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Obter dados para gráficos do dashboard
    /// </summary>
    /// <param name="tipo">Tipo de gráfico (cursos_status, arquivos_categoria, usuarios_perfil)</param>
    /// <param name="periodo">Período para análise (7d, 30d, 90d, 1y)</param>
    /// <returns>Dados formatados para gráficos</returns>
    [HttpGet("charts/{tipo}")]
    public async Task<IActionResult> GetChartData(string tipo, [FromQuery] string periodo = "30d")
    {
        try
        {
            var result = await _reportService.GetChartDataAsync(tipo, periodo);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar dados do gráfico {Tipo}", tipo);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Obter relatório de atividades recentes
    /// </summary>
    /// <param name="dias">Número de dias para buscar atividades (padrão: 7)</param>
    /// <param name="limite">Limite de registros (padrão: 50)</param>
    /// <returns>Lista de atividades recentes</returns>
    [HttpGet("activities")]
    public async Task<IActionResult> GetRecentActivities([FromQuery] int dias = 7, [FromQuery] int limite = 50)
    {
        try
        {
            var result = await _reportService.GetRecentActivitiesAsync(dias, limite);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar atividades recentes");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Obter relatório de performance do sistema
    /// </summary>
    /// <param name="periodo">Período para análise (24h, 7d, 30d)</param>
    /// <returns>Métricas de performance</returns>
    [HttpGet("performance")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetPerformanceReport([FromQuery] string periodo = "24h")
    {
        try
        {
            var result = await _reportService.GetPerformanceReportAsync(periodo);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar relatório de performance");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Agendar geração de relatório
    /// </summary>
    /// <param name="dto">Configurações do agendamento</param>
    /// <returns>Confirmação do agendamento</returns>
    [HttpPost("schedule")]
    [Authorize(Roles = "Admin,Gestor")]
    public async Task<IActionResult> ScheduleReport([FromBody] ScheduleReportDto dto)
    {
        try
        {
            var usuarioId = GetCurrentUserId();
            if (usuarioId == 0)
            {
                return Unauthorized(new { message = "Usuário não identificado" });
            }

            // Esta funcionalidade seria implementada com Hangfire ou similar
            // Por enquanto, retorna não implementado
            return StatusCode(501, new { message = "Funcionalidade em desenvolvimento" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao agendar relatório");
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

