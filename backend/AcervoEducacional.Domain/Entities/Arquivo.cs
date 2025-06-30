using AcervoEducacional.Domain.Enums;

namespace AcervoEducacional.Domain.Entities;

public class Arquivo : BaseEntity
{
    public int CursoId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string NomeArmazenamento { get; set; } = string.Empty;
    public CategoriaArquivo Categoria { get; set; }
    public string TipoMime { get; set; } = string.Empty;
    public long Tamanho { get; set; }
    public string UrlS3 { get; set; } = string.Empty;
    public bool IsPublico { get; set; } = false;
    public DateTime? DataExpiracao { get; set; }
    public string[]? DominiosPermitidos { get; set; }
    public Dictionary<string, object>? BloqueiosAtivos { get; set; }
    public Dictionary<string, object>? Metadados { get; set; }

    // Relacionamentos
    public virtual Curso Curso { get; set; } = null!;
    public virtual ICollection<LogAtividade> LogsAtividade { get; set; } = new List<LogAtividade>();
}

