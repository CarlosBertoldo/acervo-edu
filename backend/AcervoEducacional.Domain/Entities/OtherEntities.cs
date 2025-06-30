using AcervoEducacional.Domain.Enums;
using System.Net;

namespace AcervoEducacional.Domain.Entities;

public class LogAtividade : BaseEntity
{
    public int UsuarioId { get; set; }
    public int? CursoId { get; set; }
    public int? ArquivoId { get; set; }
    public TipoAtividade TipoAtividade { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public Dictionary<string, object>? DadosAnteriores { get; set; }
    public Dictionary<string, object>? DadosNovos { get; set; }
    public IPAddress? EnderecoIp { get; set; }
    public string? UserAgent { get; set; }

    // Relacionamentos
    public virtual Usuario Usuario { get; set; } = null!;
    public virtual Curso? Curso { get; set; }
    public virtual Arquivo? Arquivo { get; set; }
}

public class SessaoUsuario : BaseEntity
{
    public int UsuarioId { get; set; }
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime RefreshExpiresAt { get; set; }
    public IPAddress? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public bool IsRevogada { get; set; } = false;
    public DateTime? RevogedAt { get; set; }

    // Relacionamentos
    public virtual Usuario Usuario { get; set; } = null!;
}

public class TokenRecuperacao : BaseEntity
{
    public int UsuarioId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool IsUsado { get; set; } = false;
    public DateTime? UsadoEm { get; set; }
    public IPAddress? IpAddress { get; set; }

    // Relacionamentos
    public virtual Usuario Usuario { get; set; } = null!;
}

public class ConfiguracaoSistema : BaseEntity
{
    public string Chave { get; set; } = string.Empty;
    public string Valor { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string Tipo { get; set; } = "string";
}

