namespace AcervoEducacional.Application.DTOs.Common;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public static ApiResponse<T> SuccessResult(T data, string message = "Operação realizada com sucesso")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    public static ApiResponse<T> ErrorResult(string message, List<string>? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = errors ?? new List<string>()
        };
    }

    public static ApiResponse<T> ErrorResult(List<string> errors)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = "Erro na validação dos dados",
            Errors = errors
        };
    }
}

public class PagedResponse<T>
{
    public List<T> Data { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }

    public PagedResponse()
    {
    }

    public PagedResponse(List<T> data, int totalCount, int page, int pageSize)
    {
        Data = data;
        TotalCount = totalCount;
        Page = page;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        HasNextPage = page < TotalPages;
        HasPreviousPage = page > 1;
    }
}

public class DashboardStatsDto
{
    public int TotalCursos { get; set; }
    public int CursosAtivos { get; set; }
    public Dictionary<string, int> CursosPorStatus { get; set; } = new();
    public Dictionary<string, int> CursosPorOrigem { get; set; } = new();
    public int TotalArquivos { get; set; }
    public Dictionary<string, int> ArquivosPorCategoria { get; set; } = new();
    public int UsuariosAtivos { get; set; }
    public DateTime? UltimaSincronizacao { get; set; }
    public long TamanhoTotalArquivos { get; set; }
    public string TamanhoTotalFormatado { get; set; } = string.Empty;
    public List<AtividadeRecenteDto> AtividadesRecentes { get; set; } = new();
}

public class AtividadeRecenteDto
{
    public int Id { get; set; }
    public string UsuarioNome { get; set; } = string.Empty;
    public string TipoAtividade { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; }
    public string? CursoNome { get; set; }
    public string? ArquivoNome { get; set; }
}

public class LogAtividadeDto
{
    public int Id { get; set; }
    public string UsuarioNome { get; set; } = string.Empty;
    public string TipoAcao { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public Dictionary<string, object>? DadosAnteriores { get; set; }
    public Dictionary<string, object>? DadosNovos { get; set; }
    public string? EnderecoIp { get; set; }
    public string? UserAgent { get; set; }
    public DateTime CriadoEm { get; set; }
    public string? CursoNome { get; set; }
    public string? ArquivoNome { get; set; }
}

public class ExportRequestDto
{
    public string Format { get; set; } = "excel"; // excel, pdf, csv
    public Dictionary<string, object>? Filters { get; set; }
    public List<string>? Columns { get; set; }
    public string? Title { get; set; }
}

public class ExportResponseDto
{
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public byte[] Data { get; set; } = Array.Empty<byte>();
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}

