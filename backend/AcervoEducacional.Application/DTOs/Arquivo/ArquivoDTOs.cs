using System.ComponentModel.DataAnnotations;
using AcervoEducacional.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace AcervoEducacional.Application.DTOs.Arquivo;

public class CreateArquivoDto
{
    [Required(ErrorMessage = "Arquivo é obrigatório")]
    public IFormFile Arquivo { get; set; } = null!;

    [Required(ErrorMessage = "Categoria é obrigatória")]
    public CategoriaArquivo Categoria { get; set; }

    public bool IsPublico { get; set; } = false;
    public DateTime? DataExpiracao { get; set; }
    public string[]? DominiosPermitidos { get; set; }
    public Dictionary<string, object>? BloqueiosAtivos { get; set; }
    public Dictionary<string, object>? Metadados { get; set; }
}

public class UpdateArquivoDto
{
    [Required(ErrorMessage = "Nome é obrigatório")]
    [MaxLength(500, ErrorMessage = "Nome deve ter no máximo 500 caracteres")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Categoria é obrigatória")]
    public CategoriaArquivo Categoria { get; set; }

    public bool IsPublico { get; set; }
    public DateTime? DataExpiracao { get; set; }
    public string[]? DominiosPermitidos { get; set; }
    public Dictionary<string, object>? BloqueiosAtivos { get; set; }
    public Dictionary<string, object>? Metadados { get; set; }
}

public class ArquivoResponseDto
{
    public int Id { get; set; }
    public int CursoId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public CategoriaArquivo Categoria { get; set; }
    public string TipoMime { get; set; } = string.Empty;
    public long Tamanho { get; set; }
    public string TamanhoFormatado { get; set; } = string.Empty;
    public bool IsPublico { get; set; }
    public DateTime? DataExpiracao { get; set; }
    public string[]? DominiosPermitidos { get; set; }
    public Dictionary<string, object>? BloqueiosAtivos { get; set; }
    public Dictionary<string, object>? Metadados { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime? AtualizadoEm { get; set; }
    public string? UrlDownload { get; set; }
    public string? UrlVisualizacao { get; set; }
    public string? UrlCompartilhamento { get; set; }
}

public class ArquivoListDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public CategoriaArquivo Categoria { get; set; }
    public string TipoMime { get; set; } = string.Empty;
    public long Tamanho { get; set; }
    public string TamanhoFormatado { get; set; } = string.Empty;
    public bool IsPublico { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime? AtualizadoEm { get; set; }
}

public class ArquivosPorCategoriaDto
{
    public CategoriaArquivo Categoria { get; set; }
    public List<ArquivoListDto> Arquivos { get; set; } = new();
    public int Total { get; set; }
    public long TamanhoTotal { get; set; }
    public string TamanhoTotalFormatado { get; set; } = string.Empty;
}

public class ShareArquivoDto
{
    public bool IsPublico { get; set; }
    public DateTime? DataExpiracao { get; set; }
    public string[]? DominiosPermitidos { get; set; }
    public bool GerarCodigoEmbed { get; set; } = false;
    public Dictionary<string, object>? BloqueiosAtivos { get; set; }
}

public class ShareArquivoResponseDto
{
    public string ShareUrl { get; set; } = string.Empty;
    public string? EmbedCode { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsPublico { get; set; }
    public string[]? DominiosPermitidos { get; set; }
    public string Token { get; set; } = string.Empty;
}

public class DownloadArquivoResponseDto
{
    public string DownloadUrl { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public string Nome { get; set; } = string.Empty;
    public long Tamanho { get; set; }
    public string TipoMime { get; set; } = string.Empty;
}

public class ArquivoFilterDto
{
    public int? CursoId { get; set; }
    public CategoriaArquivo? Categoria { get; set; }
    public string? Search { get; set; }
    public string? TipoMime { get; set; }
    public bool? IsPublico { get; set; }
    public DateTime? CriadoApartirDe { get; set; }
    public DateTime? CriadoAte { get; set; }
    public long? TamanhoMin { get; set; }
    public long? TamanhoMax { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; } = "CriadoEm";
    public string? SortDirection { get; set; } = "desc";
}

