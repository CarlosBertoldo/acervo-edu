using System.ComponentModel.DataAnnotations;
using AcervoEducacional.Domain.Enums;

namespace AcervoEducacional.Application.DTOs.Usuario;

public class CreateUsuarioDto
{
    [Required(ErrorMessage = "Nome é obrigatório")]
    [MaxLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email deve ter um formato válido")]
    [MaxLength(200, ErrorMessage = "Email deve ter no máximo 200 caracteres")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Senha é obrigatória")]
    [MinLength(6, ErrorMessage = "Senha deve ter pelo menos 6 caracteres")]
    public string Senha { get; set; } = string.Empty;

    public TipoUsuario TipoUsuario { get; set; } = TipoUsuario.Usuario;
}

public class UpdateUsuarioDto
{
    [Required(ErrorMessage = "Nome é obrigatório")]
    [MaxLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email deve ter um formato válido")]
    [MaxLength(200, ErrorMessage = "Email deve ter no máximo 200 caracteres")]
    public string Email { get; set; } = string.Empty;

    public TipoUsuario TipoUsuario { get; set; }
    public StatusUsuario Status { get; set; }
}

public class UsuarioResponseDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public TipoUsuario TipoUsuario { get; set; }
    public StatusUsuario Status { get; set; }
    public DateTime? UltimoLogin { get; set; }
    public string? UltimoIp { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime? AtualizadoEm { get; set; }
    public bool Ativo { get; set; }
}

public class UsuarioListDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public TipoUsuario TipoUsuario { get; set; }
    public StatusUsuario Status { get; set; }
    public DateTime? UltimoLogin { get; set; }
    public DateTime CriadoEm { get; set; }
    public bool Ativo { get; set; }
}

public class UsuarioFilterDto
{
    public string? Search { get; set; }
    public TipoUsuario? TipoUsuario { get; set; }
    public StatusUsuario? Status { get; set; }
    public bool? Ativo { get; set; }
    public DateTime? CriadoApartirDe { get; set; }
    public DateTime? CriadoAte { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; } = "CriadoEm";
    public string? SortDirection { get; set; } = "desc";
}

