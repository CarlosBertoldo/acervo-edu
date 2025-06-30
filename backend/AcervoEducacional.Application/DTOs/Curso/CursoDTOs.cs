using System.ComponentModel.DataAnnotations;
using AcervoEducacional.Domain.Enums;

namespace AcervoEducacional.Application.DTOs.Curso;

public class CreateCursoDto
{
    [Required(ErrorMessage = "Código do curso é obrigatório")]
    [MaxLength(50, ErrorMessage = "Código deve ter no máximo 50 caracteres")]
    public string CodigoCurso { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nome do curso é obrigatório")]
    [MaxLength(500, ErrorMessage = "Nome deve ter no máximo 500 caracteres")]
    public string NomeCurso { get; set; } = string.Empty;

    [MaxLength(1000, ErrorMessage = "Descrição deve ter no máximo 1000 caracteres")]
    public string? DescricaoAcademia { get; set; }

    public StatusCurso Status { get; set; } = StatusCurso.Backlog;
    public TipoAmbiente? TipoAmbiente { get; set; }
    public TipoAcesso? TipoAcesso { get; set; }
    public DateTime? DataInicioOperacao { get; set; }

    [MaxLength(2000, ErrorMessage = "Comentários devem ter no máximo 2000 caracteres")]
    public string? ComentariosInternos { get; set; }

    public OrigemCurso Origem { get; set; } = OrigemCurso.Manual;
    public Dictionary<string, object>? DadosSenior { get; set; }
}

public class UpdateCursoDto
{
    [Required(ErrorMessage = "Nome do curso é obrigatório")]
    [MaxLength(500, ErrorMessage = "Nome deve ter no máximo 500 caracteres")]
    public string NomeCurso { get; set; } = string.Empty;

    [MaxLength(1000, ErrorMessage = "Descrição deve ter no máximo 1000 caracteres")]
    public string? DescricaoAcademia { get; set; }

    public TipoAmbiente? TipoAmbiente { get; set; }
    public TipoAcesso? TipoAcesso { get; set; }
    public DateTime? DataInicioOperacao { get; set; }

    [MaxLength(2000, ErrorMessage = "Comentários devem ter no máximo 2000 caracteres")]
    public string? ComentariosInternos { get; set; }
}

public class UpdateStatusCursoDto
{
    [Required(ErrorMessage = "Status é obrigatório")]
    public StatusCurso Status { get; set; }

    [MaxLength(500, ErrorMessage = "Comentário deve ter no máximo 500 caracteres")]
    public string? Comentario { get; set; }
}

public class CursoResponseDto
{
    public int Id { get; set; }
    public string CodigoCurso { get; set; } = string.Empty;
    public string NomeCurso { get; set; } = string.Empty;
    public string? DescricaoAcademia { get; set; }
    public StatusCurso Status { get; set; }
    public TipoAmbiente? TipoAmbiente { get; set; }
    public TipoAcesso? TipoAcesso { get; set; }
    public DateTime? DataInicioOperacao { get; set; }
    public OrigemCurso Origem { get; set; }
    public int? CriadoPor { get; set; }
    public string? CriadoPorNome { get; set; }
    public string? ComentariosInternos { get; set; }
    public Dictionary<string, object>? DadosSenior { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime? AtualizadoEm { get; set; }
    public DateTime? SyncedAt { get; set; }
    public int TotalArquivos { get; set; }
    public long TamanhoTotalBytes { get; set; }
    public bool Ativo { get; set; }
}

public class CursoListDto
{
    public int Id { get; set; }
    public string CodigoCurso { get; set; } = string.Empty;
    public string NomeCurso { get; set; } = string.Empty;
    public string? DescricaoAcademia { get; set; }
    public StatusCurso Status { get; set; }
    public TipoAmbiente? TipoAmbiente { get; set; }
    public TipoAcesso? TipoAcesso { get; set; }
    public OrigemCurso Origem { get; set; }
    public string? CriadoPorNome { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime? AtualizadoEm { get; set; }
    public int TotalArquivos { get; set; }
    public long TamanhoTotalBytes { get; set; }
}

public class CursoKanbanDto
{
    public int Id { get; set; }
    public string CodigoCurso { get; set; } = string.Empty;
    public string NomeCurso { get; set; } = string.Empty;
    public string? DescricaoAcademia { get; set; }
    public StatusCurso Status { get; set; }
    public TipoAmbiente? TipoAmbiente { get; set; }
    public TipoAcesso? TipoAcesso { get; set; }
    public DateTime? DataInicioOperacao { get; set; }
    public OrigemCurso Origem { get; set; }
    public string? CriadoPorNome { get; set; }
    public DateTime? AtualizadoEm { get; set; }
    public int TotalArquivos { get; set; }
    public Dictionary<CategoriaArquivo, int> ArquivosPorCategoria { get; set; } = new();
}

public class CursoFilterDto
{
    public string? Search { get; set; }
    public StatusCurso? Status { get; set; }
    public OrigemCurso? Origem { get; set; }
    public TipoAmbiente? TipoAmbiente { get; set; }
    public TipoAcesso? TipoAcesso { get; set; }
    public DateTime? DataInicioMin { get; set; }
    public DateTime? DataInicioMax { get; set; }
    public DateTime? CriadoApartirDe { get; set; }
    public DateTime? CriadoAte { get; set; }
    public int? CriadoPor { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; } = "AtualizadoEm";
    public string? SortDirection { get; set; } = "desc";
}

