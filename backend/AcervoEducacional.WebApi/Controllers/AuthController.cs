using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using AcervoEducacional.Infrastructure.Data;
using AcervoEducacional.Domain.Entities;

namespace AcervoEducacional.WebApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly SimpleDbContext _context;
    private readonly ILogger<AuthController> _logger;
    private readonly IConfiguration _configuration;

    public AuthController(SimpleDbContext context, ILogger<AuthController> logger, IConfiguration configuration)
    {
        _context = context;
        _logger = logger;
        _configuration = configuration;
    }

    /// <summary>
    /// Realizar login no sistema - Versão simplificada com banco
    /// </summary>
    /// <param name="request">Dados de login</param>
    /// <returns>Token de acesso e informações do usuário</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] dynamic request)
    {
        try
        {
            string email = request.email ?? request.Email ?? "";
            string senha = request.senha ?? request.password ?? "";

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(senha))
            {
                return BadRequest(new { message = "Email e senha são obrigatórios" });
            }

            // Buscar usuário no banco
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
            
            if (usuario == null)
            {
                _logger.LogWarning("Tentativa de login com email inexistente: {Email}", email);
                return BadRequest(new { message = "Email ou senha inválidos" });
            }

            // Verificar senha com BCrypt
            if (!BCrypt.Net.BCrypt.Verify(senha, usuario.SenhaHash))
            {
                _logger.LogWarning("Tentativa de login com senha incorreta para: {Email}", email);
                return BadRequest(new { message = "Email ou senha inválidos" });
            }

            // Login bem-sucedido - gerar token JWT
            var token = GenerateJwtToken(usuario);

            _logger.LogInformation("Login realizado com sucesso para: {Email}", email);

            return Ok(new
            {
                success = true,
                token = token,
                user = new
                {
                    Id = usuario.Id,
                    Nome = usuario.Nome,
                    Email = usuario.Email,
                    Tipo = usuario.Tipo.ToString()
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante login");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Obter informações do usuário logado
    /// </summary>
    /// <returns>Dados do usuário atual</returns>
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        try
        {
            var usuarioId = GetCurrentUserId();
            if (usuarioId == 0)
            {
                return Unauthorized(new { message = "Usuário não identificado" });
            }

            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario == null)
            {
                return Unauthorized(new { message = "Usuário não encontrado" });
            }

            return Ok(new
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Tipo = usuario.Tipo.ToString(),
                Status = usuario.Status.ToString()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter dados do usuário");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Logout simples
    /// </summary>
    /// <returns>Confirmação de logout</returns>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await Task.Delay(1); // Simular async
        _logger.LogInformation("Logout realizado");
        return Ok(new { success = true, message = "Logout realizado com sucesso" });
    }

    #region Métodos Auxiliares

    private string GenerateJwtToken(Usuario usuario)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["JWT:SecretKey"] ?? "AcervoEducacional2024!@#$%^&*()_+SecretKeyForProduction");
        
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Name, usuario.Nome),
            new(ClaimTypes.Email, usuario.Email),
            new("tipo_usuario", usuario.Tipo.ToString()),
            new("status", usuario.Status.ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(24),
            Issuer = _configuration["JWT:Issuer"] ?? "AcervoEducacional",
            Audience = _configuration["JWT:Audience"] ?? "AcervoEducacionalUsers",
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub") ?? User.FindFirst("id");
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
        {
            return userId;
        }
        return 0;
    }

    #endregion
}