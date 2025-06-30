using AcervoEducacional.Domain.Enums;

namespace AcervoEducacional.Domain.Entities;

public class Curso : BaseEntity
{
    public string CodigoCurso { get; set; } = string.Empty;
    public string NomeCurso { get; set; } = string.Empty;
    public string? DescricaoAcademia { get; set; }
    public StatusCurso Status { get; set; } = StatusCurso.Backlog;
    public TipoAmbiente? TipoAmbiente { get; set; }
    public TipoAcesso? TipoAcesso { get; set; }
    public DateTime? DataInicioOperacao { get; set; }
    public OrigemCurso Origem { get; set; } = OrigemCurso.Manual;
    public int? CriadoPor { get; set; }
    public string? ComentariosInternos { get; set; }
    public Dictionary<string, object>? DadosSenior { get; set; }
    public DateTime? SyncedAt { get; set; }

    // Relacionamentos
    public virtual Usuario? UsuarioCriador { get; set; }
    public virtual ICollection<Arquivo> Arquivos { get; set; } = new List<Arquivo>();
    public virtual ICollection<LogAtividade> LogsAtividade { get; set; } = new List<LogAtividade>();
}

