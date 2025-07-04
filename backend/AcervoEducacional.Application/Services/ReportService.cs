using System.Text;
using Microsoft.Extensions.Logging;
using AcervoEducacional.Application.DTOs.Common;
using AcervoEducacional.Application.DTOs.Curso;
using AcervoEducacional.Application.DTOs.Arquivo;
using AcervoEducacional.Application.Interfaces;
using AcervoEducacional.Domain.Interfaces;
using AcervoEducacional.Domain.Enums;

namespace AcervoEducacional.Application.Services;

public class ReportService : IReportService
{
    private readonly ICursoRepository _cursoRepository;
    private readonly IArquivoRepository _arquivoRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ILogAtividadeRepository _logAtividadeRepository;
    private readonly ILogger<ReportService> _logger;

    public ReportService(
        ICursoRepository cursoRepository,
        IArquivoRepository arquivoRepository,
        IUsuarioRepository usuarioRepository,
        ILogAtividadeRepository logAtividadeRepository,
        ILogger<ReportService> logger)
    {
        _cursoRepository = cursoRepository;
        _arquivoRepository = arquivoRepository;
        _usuarioRepository = usuarioRepository;
        _logAtividadeRepository = logAtividadeRepository;
        _logger = logger;
    }

    public async Task<ApiResponse<DashboardStatsDto>> GetDashboardStatsAsync()
    {
        try
        {
            _logger.LogInformation("Gerando estatísticas do dashboard");

            // Buscar dados básicos
            var cursos = await _cursoRepository.GetAllAsync();
            var arquivos = await _arquivoRepository.GetAllAsync();
            var usuarios = await _usuarioRepository.GetAllAsync();
            var logsRecentes = (await _logAtividadeRepository.GetAllAsync()).OrderByDescending(l => l.CriadoEm).Take(10);

            var stats = new DashboardStatsDto
            {
                TotalCursos = cursos.Count(),
                CursosAtivos = cursos.Count(c => c.Status == StatusCurso.Ativo),
                CursosPorStatus = cursos.GroupBy(c => c.Status).ToDictionary(g => g.Key.ToString(), g => g.Count()),
                CursosPorOrigem = cursos.GroupBy(c => c.Origem).ToDictionary(g => g.Key.ToString(), g => g.Count()),
                TotalArquivos = arquivos.Count(),
                ArquivosPorCategoria = arquivos.GroupBy(a => a.Categoria).ToDictionary(g => g.Key.ToString(), g => g.Count()),
                TamanhoTotalArquivos = arquivos.Sum(a => a.Tamanho),
                UsuariosAtivos = usuarios.Count(u => u.DeletadoEm == null && u.Status == StatusUsuario.Ativo),
                AtividadesRecentes = logsRecentes.Select(l => new AtividadeRecenteDto
                {
                    Id = l.Id,
                    UsuarioNome = l.Usuario?.Nome ?? "Sistema",
                    TipoAtividade = l.TipoAtividade.ToString(),
                    Descricao = l.Descricao,
                    CriadoEm = l.CriadoEm
                }).ToList(),
                UltimaSincronizacao = cursos.Any() ? cursos.Max(c => c.SyncedAt) : null
            };

            stats.TamanhoTotalFormatado = FormatFileSize(stats.TamanhoTotalArquivos);

            _logger.LogInformation("Estatísticas do dashboard geradas com sucesso");

            return ApiResponse<DashboardStatsDto>.SuccessResult(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar estatísticas do dashboard");
            return ApiResponse<DashboardStatsDto>.ErrorResult("Erro interno do servidor");
        }
    }

    public async Task<ApiResponse<PagedResponse<LogAtividadeDto>>> GetLogsAtividadeAsync(int page = 1, int pageSize = 50)
    {
        try
        {
            _logger.LogInformation("Buscando logs de atividade - Página {Page}, Tamanho {PageSize}", page, pageSize);

            var allLogs = await _logAtividadeRepository.GetAllAsync();
            var totalCount = allLogs.Count();
            var logs = allLogs.OrderByDescending(l => l.CriadoEm)
                             .Skip((page - 1) * pageSize)
                             .Take(pageSize)
                             .ToList();

            var logsDto = logs.Select(MapToLogAtividadeDto).ToList();

            var pagedResponse = new PagedResponse<LogAtividadeDto>(
                logsDto,
                totalCount,
                page,
                pageSize);

            return ApiResponse<PagedResponse<LogAtividadeDto>>.SuccessResult(pagedResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar logs de atividade");
            return ApiResponse<PagedResponse<LogAtividadeDto>>.ErrorResult("Erro interno do servidor");
        }
    }

    public async Task<ApiResponse<byte[]>> ExportCursosAsync(CursoFilterDto filter, string format)
    {
        try
        {
            _logger.LogInformation("Exportando cursos no formato {Format}", format);

            var allCursos = await _cursoRepository.GetAllAsync();
            
            // Apply filters manually
            var cursosQuery = allCursos.AsQueryable();
            
            if (!string.IsNullOrEmpty(filter.Search))
            {
                var searchLower = filter.Search.ToLower();
                cursosQuery = cursosQuery.Where(c => 
                    c.Nome.ToLower().Contains(searchLower) ||
                    (c.DescricaoAcademia != null && c.DescricaoAcademia.ToLower().Contains(searchLower)) ||
                    c.CodigoCurso.ToLower().Contains(searchLower));
            }

            if (filter.Status.HasValue)
            {
                cursosQuery = cursosQuery.Where(c => c.Status == filter.Status.Value);
            }

            if (filter.Origem.HasValue)
            {
                cursosQuery = cursosQuery.Where(c => c.Origem == filter.Origem.Value);
            }

            if (filter.CriadoApartirDe.HasValue)
            {
                cursosQuery = cursosQuery.Where(c => c.CriadoEm >= filter.CriadoApartirDe.Value);
            }

            if (filter.CriadoAte.HasValue)
            {
                cursosQuery = cursosQuery.Where(c => c.CriadoEm <= filter.CriadoAte.Value);
            }

            var cursos = cursosQuery.ToList();

            byte[] data = format.ToLower() switch
            {
                "excel" => await ExportCursosToExcel(cursos),
                "pdf" => await ExportCursosToPdf(cursos),
                "csv" => await ExportCursosToCsv(cursos),
                _ => throw new ArgumentException($"Formato {format} não suportado")
            };

            _logger.LogInformation("Exportação de cursos concluída - {Count} registros", cursos.Count);

            return ApiResponse<byte[]>.SuccessResult(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao exportar cursos no formato {Format}", format);
            return ApiResponse<byte[]>.ErrorResult("Erro interno do servidor");
        }
    }

    public async Task<ApiResponse<byte[]>> ExportArquivosAsync(ArquivoFilterDto filter, string format)
    {
        try
        {
            _logger.LogInformation("Exportando arquivos no formato {Format}", format);

            var allArquivos = await _arquivoRepository.GetAllAsync();
            
            // Apply filters manually
            var arquivosQuery = allArquivos.AsQueryable();
            
            if (!string.IsNullOrEmpty(filter.Search))
            {
                var searchLower = filter.Search.ToLower();
                arquivosQuery = arquivosQuery.Where(a => a.Nome.ToLower().Contains(searchLower));
            }

            if (filter.CursoId.HasValue)
            {
                arquivosQuery = arquivosQuery.Where(a => a.CursoId == filter.CursoId.Value);
            }

            if (filter.Categoria.HasValue)
            {
                arquivosQuery = arquivosQuery.Where(a => a.Categoria == filter.Categoria.Value);
            }

            if (filter.IsPublico.HasValue)
            {
                arquivosQuery = arquivosQuery.Where(a => a.IsPublico == filter.IsPublico.Value);
            }

            if (filter.CriadoApartirDe.HasValue)
            {
                arquivosQuery = arquivosQuery.Where(a => a.CriadoEm >= filter.CriadoApartirDe.Value);
            }

            if (filter.CriadoAte.HasValue)
            {
                arquivosQuery = arquivosQuery.Where(a => a.CriadoEm <= filter.CriadoAte.Value);
            }

            var arquivos = arquivosQuery.ToList();

            byte[] data = format.ToLower() switch
            {
                "excel" => await ExportArquivosToExcel(arquivos),
                "pdf" => await ExportArquivosToPdf(arquivos),
                "csv" => await ExportArquivosToCsv(arquivos),
                _ => throw new ArgumentException($"Formato {format} não suportado")
            };

            _logger.LogInformation("Exportação de arquivos concluída - {Count} registros", arquivos.Count);

            return ApiResponse<byte[]>.SuccessResult(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao exportar arquivos no formato {Format}", format);
            return ApiResponse<byte[]>.ErrorResult("Erro interno do servidor");
        }
    }

    public async Task<ApiResponse<byte[]>> ExportLogsAsync(int page, int pageSize, string format)
    {
        try
        {
            _logger.LogInformation("Exportando logs no formato {Format}", format);

            var logs = (await _logAtividadeRepository.GetAllAsync()).ToList(); // Buscar todos

            byte[] data = format.ToLower() switch
            {
                "excel" => await ExportLogsToExcel(logs),
                "pdf" => await ExportLogsToPdf(logs),
                "csv" => await ExportLogsToCsv(logs),
                _ => throw new ArgumentException($"Formato {format} não suportado")
            };

            _logger.LogInformation("Exportação de logs concluída - {Count} registros", logs.Count);

            return ApiResponse<byte[]>.SuccessResult(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao exportar logs no formato {Format}", format);
            return ApiResponse<byte[]>.ErrorResult("Erro interno do servidor");
        }
    }

    #region Métodos de Exportação

    private async Task<byte[]> ExportCursosToExcel(List<Domain.Entities.Curso> cursos)
    {
        // Implementação simplificada - em produção usar EPPlus ou similar
        var csv = await ExportCursosToCsv(cursos);
        return csv; // Por enquanto retorna CSV, implementar Excel depois
    }

    private async Task<byte[]> ExportCursosToPdf(List<Domain.Entities.Curso> cursos)
    {
        // Implementação simplificada - em produção usar iTextSharp ou similar
        var html = GenerateCursosHtml(cursos);
        return Encoding.UTF8.GetBytes(html);
    }

    private async Task<byte[]> ExportCursosToCsv(List<Domain.Entities.Curso> cursos)
    {
        var csv = new StringBuilder();
        
        // Cabeçalho
        csv.AppendLine("Código,Nome,Academia,Status,Tipo Ambiente,Tipo Acesso,Data Início,Origem,Criado Em");
        
        // Dados
        foreach (var curso in cursos)
        {
            csv.AppendLine($"\"{curso.CodigoCurso}\",\"{curso.Nome}\",\"{curso.DescricaoAcademia}\"," +
                          $"\"{curso.Status}\",\"{curso.TipoAmbiente}\",\"{curso.TipoAcesso}\"," +
                          $"\"{curso.DataInicioOperacao?.ToString("dd/MM/yyyy")}\",\"{curso.Origem}\"," +
                          $"\"{curso.CriadoEm:dd/MM/yyyy HH:mm}\"");
        }
        
        return Encoding.UTF8.GetBytes(csv.ToString());
    }

    private async Task<byte[]> ExportArquivosToExcel(List<Domain.Entities.Arquivo> arquivos)
    {
        var csv = await ExportArquivosToCsv(arquivos);
        return csv;
    }

    private async Task<byte[]> ExportArquivosToPdf(List<Domain.Entities.Arquivo> arquivos)
    {
        var html = GenerateArquivosHtml(arquivos);
        return Encoding.UTF8.GetBytes(html);
    }

    private async Task<byte[]> ExportArquivosToCsv(List<Domain.Entities.Arquivo> arquivos)
    {
        var csv = new StringBuilder();
        
        // Cabeçalho
        csv.AppendLine("Nome,Categoria,Tipo MIME,Tamanho,Público,Data Expiração,Criado Em");
        
        // Dados
        foreach (var arquivo in arquivos)
        {
            csv.AppendLine($"\"{arquivo.Nome}\",\"{arquivo.Categoria}\",\"{arquivo.TipoMime}\"," +
                          $"\"{FormatFileSize(arquivo.Tamanho)}\",\"{(arquivo.IsPublico ? "Sim" : "Não")}\"," +
                          $"\"{arquivo.DataExpiracao?.ToString("dd/MM/yyyy")}\",\"{arquivo.CriadoEm:dd/MM/yyyy HH:mm}\"");
        }
        
        return Encoding.UTF8.GetBytes(csv.ToString());
    }

    private async Task<byte[]> ExportLogsToExcel(List<Domain.Entities.LogAtividade> logs)
    {
        var csv = await ExportLogsToCsv(logs);
        return csv;
    }

    private async Task<byte[]> ExportLogsToPdf(List<Domain.Entities.LogAtividade> logs)
    {
        var html = GenerateLogsHtml(logs);
        return Encoding.UTF8.GetBytes(html);
    }

    private async Task<byte[]> ExportLogsToCsv(List<Domain.Entities.LogAtividade> logs)
    {
        var csv = new StringBuilder();
        
        // Cabeçalho
        csv.AppendLine("Usuário,Tipo Ação,Descrição,IP,Data/Hora");
        
        // Dados
        foreach (var log in logs)
        {
            csv.AppendLine($"\"{log.Usuario?.Nome}\",\"{log.TipoAtividade}\",\"{log.Descricao}\"," +
                          $"\"{log.EnderecoIp}\",\"{log.CriadoEm:dd/MM/yyyy HH:mm:ss}\"");
        }
        
        return Encoding.UTF8.GetBytes(csv.ToString());
    }

    #endregion

    #region Métodos de Geração HTML

    private string GenerateCursosHtml(List<Domain.Entities.Curso> cursos)
    {
        var html = new StringBuilder();
        html.AppendLine("<html><head><title>Relatório de Cursos</title></head><body>");
        html.AppendLine("<h1>Relatório de Cursos</h1>");
        html.AppendLine("<table border='1'>");
        html.AppendLine("<tr><th>Código</th><th>Nome</th><th>Academia</th><th>Status</th><th>Origem</th></tr>");
        
        foreach (var curso in cursos)
        {
            html.AppendLine($"<tr><td>{curso.CodigoCurso}</td><td>{curso.Nome}</td>" +
                           $"<td>{curso.DescricaoAcademia}</td><td>{curso.Status}</td><td>{curso.Origem}</td></tr>");
        }
        
        html.AppendLine("</table></body></html>");
        return html.ToString();
    }

    private string GenerateArquivosHtml(List<Domain.Entities.Arquivo> arquivos)
    {
        var html = new StringBuilder();
        html.AppendLine("<html><head><title>Relatório de Arquivos</title></head><body>");
        html.AppendLine("<h1>Relatório de Arquivos</h1>");
        html.AppendLine("<table border='1'>");
        html.AppendLine("<tr><th>Nome</th><th>Categoria</th><th>Tamanho</th><th>Público</th></tr>");
        
        foreach (var arquivo in arquivos)
        {
            html.AppendLine($"<tr><td>{arquivo.Nome}</td><td>{arquivo.Categoria}</td>" +
                           $"<td>{FormatFileSize(arquivo.Tamanho)}</td><td>{(arquivo.IsPublico ? "Sim" : "Não")}</td></tr>");
        }
        
        html.AppendLine("</table></body></html>");
        return html.ToString();
    }

    private string GenerateLogsHtml(List<Domain.Entities.LogAtividade> logs)
    {
        var html = new StringBuilder();
        html.AppendLine("<html><head><title>Relatório de Logs</title></head><body>");
        html.AppendLine("<h1>Relatório de Logs de Atividade</h1>");
        html.AppendLine("<table border='1'>");
        html.AppendLine("<tr><th>Usuário</th><th>Ação</th><th>Descrição</th><th>Data/Hora</th></tr>");
        
        foreach (var log in logs)
        {
            html.AppendLine($"<tr><td>{log.Usuario?.Nome}</td><td>{log.TipoAtividade}</td>" +
                           $"<td>{log.Descricao}</td><td>{log.CriadoEm:dd/MM/yyyy HH:mm}</td></tr>");
        }
        
        html.AppendLine("</table></body></html>");
        return html.ToString();
    }

    #endregion

    #region Métodos Auxiliares

    private LogAtividadeDto MapToLogAtividadeDto(Domain.Entities.LogAtividade log)
    {
        return new LogAtividadeDto
        {
            Id = log.Id,
            UsuarioNome = log.Usuario?.Nome ?? "Sistema",
            TipoAcao = log.TipoAtividade.ToString(),
            Descricao = log.Descricao,
            DadosAnteriores = log.DadosAnteriores,
            DadosNovos = log.DadosNovos,
            EnderecoIp = log.EnderecoIp?.ToString(),
            UserAgent = log.UserAgent,
            CriadoEm = log.CriadoEm,
            CursoNome = log.Curso?.Nome,
            ArquivoNome = log.Arquivo?.Nome
        };
    }


    private string FormatFileSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        double len = bytes;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }

    #endregion
}

