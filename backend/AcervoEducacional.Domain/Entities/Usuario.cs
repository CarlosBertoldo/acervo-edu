using AcervoEducacional.Domain.Enums;

namespace AcervoEducacional.Domain.Entities;

public class Usuario : BaseEntity
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public TipoUsuario Tipo { get; set; } = TipoUsuario.Usuario;
    public string? Departamento { get; set; }
    public string? Cargo { get; set; }
    public string? Telefone { get; set; }
    public string? Avatar { get; set; }
    public StatusUsuario Status { get; set; } = StatusUsuario.Ativo;
    public DateTime? UltimoLogin { get; set; }
    public string? UltimoIp { get; set; }
    public string? UltimoUserAgent { get; set; }
    public int TentativasLogin { get; set; } = 0;
    public DateTime? BloqueadoAte { get; set; }
    public new int? DeletadoPor { get; set; }
    public new DateTime? DeletadoEm { get; set; }

    // Relacionamentos
    public virtual ICollection<Curso> CursosCriados { get; set; } = new List<Curso>();
    public virtual ICollection<LogAtividade> LogsAtividade { get; set; } = new List<LogAtividade>();
    public virtual ICollection<SessaoUsuario> Sessoes { get; set; } = new List<SessaoUsuario>();
    public virtual ICollection<TokenRecuperacao> TokensRecuperacao { get; set; } = new List<TokenRecuperacao>();
}

