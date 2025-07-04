namespace AcervoEducacional.Domain.Entities;

public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? AtualizadoEm { get; set; }
    public string? CriadoPor { get; set; }
    public string? AtualizadoPor { get; set; }
    public string? DeletadoPor { get; set; }
    public DateTime? DeletadoEm { get; set; }
    public bool Ativo { get; set; } = true;
}

