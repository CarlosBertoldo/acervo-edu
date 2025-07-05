using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using BCrypt.Net;
using AcervoEducacional.Application.DTOs.Auth;
using AcervoEducacional.Application.DTOs.Common;
using AcervoEducacional.Application.Interfaces;
using AcervoEducacional.Domain.Entities;
using AcervoEducacional.Domain.Enums;
using AcervoEducacional.Domain.Interfaces;

namespace AcervoEducacional.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ISessaoUsuarioRepository _sessaoRepository;
    private readonly ITokenRecuperacaoRepository _tokenRepository;
    private readonly ILogAtividadeRepository _logRepository;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;
    
    // Configurações de segurança
    private const int _maxTentativasLogin = 5;
    private const int _tempoBloqueiMinutos = 30;
    private const int _tokenExpirationMinutes = 60;
    private const int _refreshTokenExpirationDays = 7;
    private const int _resetTokenExpirationHours = 2;
    
    private readonly Regex _emailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
    private readonly Regex _senhaRegex = new(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", RegexOptions.Compiled);

    public AuthService(
        IUsuarioRepository usuarioRepository,
        ISessaoUsuarioRepository sessaoRepository,
        ITokenRecuperacaoRepository tokenRepository,
        ILogAtividadeRepository logRepository,
        IEmailService emailService,
        IConfiguration configuration,
        ILogger<AuthService> logger)
    {
        _usuarioRepository = usuarioRepository;
        _sessaoRepository = sessaoRepository;
        _tokenRepository = tokenRepository;
        _logRepository = logRepository;
        _emailService = emailService;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto request, string ipAddress, string userAgent)
    {
        try
        {
            // Validar entrada
            if (!_emailRegex.IsMatch(request.Email))
            {
                return ApiResponse<LoginResponseDto>.ErrorResult("Email inválido");
            }

            // Buscar usuário
            var usuario = await _usuarioRepository.GetByEmailAsync(request.Email);
            if (usuario == null)
            {
                await LogAtividadeAsync(null, TipoAtividade.LoginFalhou, 
                    $"Tentativa de login com email inexistente: {request.Email}", ipAddress, userAgent);
                return ApiResponse<LoginResponseDto>.ErrorResult("Email ou senha inválidos");
            }

            // Verificar se usuário está ativo
            if (usuario.Status != StatusUsuario.Ativo)
            {
                await LogAtividadeAsync(usuario.Id, TipoAtividade.LoginFalhou, 
                    "Tentativa de login com usuário inativo", ipAddress, userAgent);
                return ApiResponse<LoginResponseDto>.ErrorResult("Usuário inativo");
            }

            // Verificar bloqueio por tentativas
            if (await IsUsuarioBloqueadoAsync(usuario.Id))
            {
                await LogAtividadeAsync(usuario.Id, TipoAtividade.LoginFalhou, 
                    "Tentativa de login com usuário bloqueado", ipAddress, userAgent);
                return ApiResponse<LoginResponseDto>.ErrorResult("Usuário temporariamente bloqueado devido a muitas tentativas de login");
            }

            // Verificar senha
            if (!BCrypt.Net.BCrypt.Verify(request.Senha, usuario.SenhaHash))
            {
                await RegistrarTentativaLoginAsync(usuario.Id, false, ipAddress, userAgent);
                await LogAtividadeAsync(usuario.Id, TipoAtividade.LoginFalhou, 
                    "Senha incorreta", ipAddress, userAgent);
                return ApiResponse<LoginResponseDto>.ErrorResult("Email ou senha inválidos");
            }

            // Login bem-sucedido
            await RegistrarTentativaLoginAsync(usuario.Id, true, ipAddress, userAgent);

            // Gerar tokens
            var token = GenerateJwtToken(usuario);
            var refreshToken = GenerateRefreshToken();

            // Salvar sessão
            var sessao = new SessaoUsuario
            {
                UsuarioId = usuario.Id,
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_tokenExpirationMinutes),
                RefreshExpiresAt = DateTime.UtcNow.AddDays(_refreshTokenExpirationDays),
                IpAddress = IPAddress.TryParse(ipAddress, out var ip) ? ip : null,
                UserAgent = userAgent,
                CriadoEm = DateTime.UtcNow
            };

            await _sessaoRepository.AddAsync(sessao);

            // Atualizar último login
            usuario.UltimoLogin = DateTime.UtcNow;
            usuario.TentativasLogin = 0;
            usuario.BloqueadoAte = null;
            await _usuarioRepository.UpdateAsync(usuario);

            await LogAtividadeAsync(usuario.Id, TipoAtividade.Login, 
                "Login realizado com sucesso", ipAddress, userAgent);

            var response = new LoginResponseDto
            {
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = sessao.ExpiresAt,
                Usuario = MapToUsuarioDto(usuario)
            };

            return ApiResponse<LoginResponseDto>.SuccessResult(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante login para email {Email}", request.Email);
            return ApiResponse<LoginResponseDto>.ErrorResult("Erro interno do servidor");
        }
    }

    public async Task<ApiResponse<RefreshTokenResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request, string ipAddress, string userAgent)
    {
        try
        {
            // Buscar sessão ativa
            var sessao = await _sessaoRepository.GetByRefreshTokenAsync(request.RefreshToken);
            if (sessao == null || sessao.IsRevogada || sessao.RefreshExpiresAt < DateTime.UtcNow)
            {
                return ApiResponse<RefreshTokenResponseDto>.ErrorResult("Refresh token inválido ou expirado");
            }

            // Verificar usuário
            var usuario = await _usuarioRepository.GetByIdAsync(sessao.UsuarioId);
            if (usuario == null || usuario.Status != StatusUsuario.Ativo)
            {
                await RevogarSessaoAsync(sessao);
                return ApiResponse<RefreshTokenResponseDto>.ErrorResult("Usuário inválido");
            }

            // Gerar novos tokens
            var novoToken = GenerateJwtToken(usuario);
            var novoRefreshToken = GenerateRefreshToken();

            // Atualizar sessão
            sessao.Token = novoToken;
            sessao.RefreshToken = novoRefreshToken;
            sessao.ExpiresAt = DateTime.UtcNow.AddMinutes(_tokenExpirationMinutes);
            sessao.RefreshExpiresAt = DateTime.UtcNow.AddDays(_refreshTokenExpirationDays);
            sessao.AtualizadoEm = DateTime.UtcNow;

            await _sessaoRepository.UpdateAsync(sessao);

            await LogAtividadeAsync(usuario.Id, TipoAtividade.RefreshToken, 
                "Token renovado com sucesso", ipAddress, userAgent);

            var response = new RefreshTokenResponseDto
            {
                Token = novoToken,
                RefreshToken = novoRefreshToken,
                ExpiresAt = sessao.ExpiresAt
            };

            return ApiResponse<RefreshTokenResponseDto>.SuccessResult(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante refresh token");
            return ApiResponse<RefreshTokenResponseDto>.ErrorResult("Erro interno do servidor");
        }
    }

    public async Task<ApiResponse<bool>> LogoutAsync(string token)
    {
        try
        {
            var sessao = await _sessaoRepository.GetByTokenAsync(token);
            if (sessao != null)
            {
                await RevogarSessaoAsync(sessao);
                
                await LogAtividadeAsync(sessao.UsuarioId, TipoAtividade.Logout, 
                    "Logout realizado", null, null);
            }

            return ApiResponse<bool>.SuccessResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante logout");
            return ApiResponse<bool>.ErrorResult("Erro interno do servidor");
        }
    }

    public async Task<ApiResponse<bool>> ForgotPasswordAsync(ForgotPasswordRequestDto request)
    {
        try
        {
            if (!_emailRegex.IsMatch(request.Email))
            {
                return ApiResponse<bool>.ErrorResult("Email inválido");
            }

            var usuario = await _usuarioRepository.GetByEmailAsync(request.Email);
            if (usuario == null)
            {
                // Por segurança, sempre retornar sucesso mesmo se email não existir
                return ApiResponse<bool>.SuccessResult(true);
            }

            // Invalidar tokens anteriores
            await _tokenRepository.InvalidarTokensUsuarioAsync(usuario.Id);

            // Gerar novo token
            var resetToken = GenerateSecureToken();
            var tokenRecuperacao = new TokenRecuperacao
            {
                UsuarioId = usuario.Id,
                Token = resetToken,
                ExpiresAt = DateTime.UtcNow.AddHours(_resetTokenExpirationHours),
                CriadoEm = DateTime.UtcNow
            };

            await _tokenRepository.AddAsync(tokenRecuperacao);

            await LogAtividadeAsync(usuario.Id, TipoAtividade.SolicitacaoRecuperacaoSenha, 
                "Token de recuperação de senha gerado", null, null);

            // Enviar email com token
            var emailEnviado = await _emailService.SendPasswordResetEmailAsync(usuario.Email, resetToken);
            if (!emailEnviado)
            {
                _logger.LogWarning("Falha ao enviar email de recuperação para usuário {UsuarioId}", usuario.Id);
                // Não retornar erro para não revelar se o email existe
            }

            return ApiResponse<bool>.SuccessResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante solicitação de recuperação de senha");
            return ApiResponse<bool>.ErrorResult("Erro interno do servidor");
        }
    }

    public async Task<ApiResponse<bool>> ResetPasswordAsync(ResetPasswordRequestDto request)
    {
        try
        {
            if (!_senhaRegex.IsMatch(request.NovaSenha))
            {
                return ApiResponse<bool>.ErrorResult("Senha deve ter pelo menos 8 caracteres, incluindo maiúscula, minúscula, número e símbolo");
            }

            var tokenRecuperacao = await _tokenRepository.GetByTokenAsync(request.Token);
            if (tokenRecuperacao == null || tokenRecuperacao.IsUsado || tokenRecuperacao.ExpiresAt < DateTime.UtcNow)
            {
                return ApiResponse<bool>.ErrorResult("Token inválido ou expirado");
            }

            var usuario = await _usuarioRepository.GetByIdAsync(tokenRecuperacao.UsuarioId);
            if (usuario == null)
            {
                return ApiResponse<bool>.ErrorResult("Usuário não encontrado");
            }

            // Atualizar senha
            usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(request.NovaSenha);
            usuario.TentativasLogin = 0;
            usuario.BloqueadoAte = null;
            usuario.AtualizadoEm = DateTime.UtcNow;

            await _usuarioRepository.UpdateAsync(usuario);

            // Marcar token como usado
            tokenRecuperacao.IsUsado = true;
            tokenRecuperacao.UsadoEm = DateTime.UtcNow;
            await _tokenRepository.UpdateAsync(tokenRecuperacao);

            // Revogar todas as sessões ativas
            await _sessaoRepository.RevogarSessoesUsuarioAsync(usuario.Id);

            await LogAtividadeAsync(usuario.Id, TipoAtividade.AlteracaoSenha, 
                "Senha alterada via recuperação", null, null);

            return ApiResponse<bool>.SuccessResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante reset de senha");
            return ApiResponse<bool>.ErrorResult("Erro interno do servidor");
        }
    }

    public async Task<ApiResponse<bool>> ChangePasswordAsync(int usuarioId, ChangePasswordRequestDto request)
    {
        try
        {
            var usuario = await _usuarioRepository.GetByIdAsync(usuarioId);
            if (usuario == null)
            {
                return ApiResponse<bool>.ErrorResult("Usuário não encontrado");
            }

            // Verificar senha atual
            if (!BCrypt.Net.BCrypt.Verify(request.SenhaAtual, usuario.SenhaHash))
            {
                return ApiResponse<bool>.ErrorResult("Senha atual incorreta");
            }

            if (!_senhaRegex.IsMatch(request.NovaSenha))
            {
                return ApiResponse<bool>.ErrorResult("Nova senha deve ter pelo menos 8 caracteres, incluindo maiúscula, minúscula, número e símbolo");
            }

            // Atualizar senha
            usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(request.NovaSenha);
            usuario.AtualizadoEm = DateTime.UtcNow;

            await _usuarioRepository.UpdateAsync(usuario);

            await LogAtividadeAsync(usuario.Id, TipoAtividade.AlteracaoSenha, 
                "Senha alterada pelo usuário", null, null);

            return ApiResponse<bool>.SuccessResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante alteração de senha");
            return ApiResponse<bool>.ErrorResult("Erro interno do servidor");
        }
    }

    public async Task<ApiResponse<bool>> ValidateTokenAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? "default-secret-key");

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            // Verificar se sessão ainda está ativa
            var sessao = await _sessaoRepository.GetByTokenAsync(token);
            if (sessao == null || sessao.IsRevogada || sessao.ExpiresAt < DateTime.UtcNow)
            {
                return ApiResponse<bool>.SuccessResult(false);
            }

            return ApiResponse<bool>.SuccessResult(true);
        }
        catch
        {
            return ApiResponse<bool>.SuccessResult(false);
        }
    }

    #region Métodos Privados

    private string GenerateJwtToken(Usuario usuario)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? "default-secret-key");
        
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
            Expires = DateTime.UtcNow.AddMinutes(_tokenExpirationMinutes),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    private static string GenerateSecureToken()
    {
        var randomBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
    }

    private async Task<bool> IsUsuarioBloqueadoAsync(int usuarioId)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(usuarioId);
        return usuario?.BloqueadoAte > DateTime.UtcNow;
    }

    private async Task RegistrarTentativaLoginAsync(int usuarioId, bool sucesso, string ipAddress, string userAgent)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(usuarioId);
        if (usuario == null) return;

        if (sucesso)
        {
            usuario.TentativasLogin = 0;
            usuario.BloqueadoAte = null;
        }
        else
        {
            usuario.TentativasLogin++;
            if (usuario.TentativasLogin >= _maxTentativasLogin)
            {
                usuario.BloqueadoAte = DateTime.UtcNow.AddMinutes(_tempoBloqueiMinutos);
            }
        }

        await _usuarioRepository.UpdateAsync(usuario);
    }

    private async Task RevogarSessaoAsync(SessaoUsuario sessao)
    {
        sessao.IsRevogada = true;
        sessao.RevogedAt = DateTime.UtcNow;
        await _sessaoRepository.UpdateAsync(sessao);
    }

    private async Task LogAtividadeAsync(int? usuarioId, TipoAtividade tipo, string descricao, string? ipAddress, string? userAgent)
    {
        try
        {
            var log = new LogAtividade
            {
                UsuarioId = usuarioId ?? 0,
                TipoAtividade = tipo,
                Descricao = descricao,
                EnderecoIp = IPAddress.TryParse(ipAddress, out var ip) ? ip : null,
                UserAgent = userAgent,
                CriadoEm = DateTime.UtcNow
            };

            await _logRepository.AddAsync(log);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao registrar log de atividade");
        }
    }

    private static UsuarioDto MapToUsuarioDto(Usuario usuario)
    {
        return new UsuarioDto
        {
            Id = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email,
            TipoUsuario = usuario.Tipo.ToString(),
            Status = usuario.Status.ToString(),
            UltimoLogin = usuario.UltimoLogin,
            CriadoEm = usuario.CriadoEm
        };
    }

    #endregion
}

