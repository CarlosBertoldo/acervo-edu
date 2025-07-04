using AcervoEducacional.Domain.Enums;

namespace AcervoEducacional.Domain.Entities;

public class Arquivo : BaseEntity
{
    public int CursoId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string NomeOriginal { get; set; } = string.Empty;
    public string CaminhoArquivo { get; set; } = string.Empty;
    public string TipoMime { get; set; } = string.Empty;
    public long Tamanho { get; set; }
    public CategoriaArquivo Categoria { get; set; }
    public TipoCompartilhamento TipoCompartilhamento { get; set; } = TipoCompartilhamento.Restrito;
    public string? Descricao { get; set; }
    public string? Tags { get; set; }
    public string? HashArquivo { get; set; }
    public string? UrlPublica { get; set; }
    public string? CodigoEmbed { get; set; }
    public string? DominiosPermitidos { get; set; }
    public string? SenhaAcesso { get; set; }
    public DateTime? DataExpiracao { get; set; }
    public bool IsPublico { get; set; } = false;
    public int UploadPor { get; set; }
    public int DownloadCount { get; set; } = 0;
    public DateTime? UltimoDownload { get; set; }
    
    // Propriedades para compatibilidade com serviços existentes
    public string NomeArmazenamento { get; set; } = string.Empty;
    public string UrlS3 { get; set; } = string.Empty;
    public Dictionary<string, object>? BloqueiosAtivos { get; set; }
    public Dictionary<string, object>? Metadados { get; set; }

    // Relacionamentos
    public virtual Curso Curso { get; set; } = null!;
    public virtual Usuario UploadPorUsuario { get; set; } = null!;
}

