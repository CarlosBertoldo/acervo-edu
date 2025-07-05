using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using AcervoEducacional.Application.DTOs.Common;
using AcervoEducacional.Application.Interfaces;
using AcervoEducacional.Domain.Entities;
using AcervoEducacional.Domain.Enums;
using AcervoEducacional.Domain.Interfaces;

namespace AcervoEducacional.Application.Services;

public class SecurityService : ISecurityService
{
    private readonly ILogAtividadeRepository _logRepository;
    private readonly IConfiguracaoSistemaRepository _configRepository;
    private readonly ILogger<SecurityService> _logger;
    private readonly IConfiguration _configuration;
    
    // Cache para rate limiting
    private static readonly Dictionary<string, List<DateTime>> _rateLimitCache = new();
    private static readonly Dictionary<string, DateTime> _blockedIps = new();
    private static readonly object _cacheLock = new();
    
    // Regex patterns
    private static readonly Regex _emailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
    private static readonly Regex _strongPasswordRegex = new(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", RegexOptions.Compiled);
    
    // Domínios suspeitos comuns
    private static readonly HashSet<string> _suspiciousDomains = new()
    {
        "10minutemail.com", "guerrillamail.com", "mailinator.com", "tempmail.org",
        "throwaway.email", "temp-mail.org", "getnada.com", "maildrop.cc"
    };

    public SecurityService(
        ILogAtividadeRepository logRepository,
        IConfiguracaoSistemaRepository configRepository,
        ILogger<SecurityService> logger,
        IConfiguration configuration)
    {
        _logRepository = logRepository;
        _configRepository = configRepository;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<bool> IsRateLimitExceededAsync(string identifier, string action, int maxAttempts = 5, int windowMinutes = 15)
    {
        try
        {
            var key = $"{identifier}:{action}";
            var now = DateTime.UtcNow;
            var windowStart = now.AddMinutes(-windowMinutes);

            lock (_cacheLock)
            {
                if (!_rateLimitCache.ContainsKey(key))
                {
                    _rateLimitCache[key] = new List<DateTime>();
                }

                var attempts = _rateLimitCache[key];
                
                // Remove tentativas antigas
                attempts.RemoveAll(attempt => attempt < windowStart);
                
                // Verificar se excedeu o limite
                if (attempts.Count >= maxAttempts)
                {
                    return true;
                }
                
                // Adicionar tentativa atual
                attempts.Add(now);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar rate limit para {Identifier}:{Action}", identifier, action);
            return false; // Em caso de erro, permitir a operação
        }
    }

    public async Task<string> HashPasswordAsync(string password)
    {
        return await Task.Run(() => BCrypt.Net.BCrypt.HashPassword(password, 12));
    }

    public async Task<bool> VerifyPasswordAsync(string password, string hash)
    {
        return await Task.Run(() => BCrypt.Net.BCrypt.Verify(password, hash));
    }

    public async Task<bool> ValidatePasswordStrengthAsync(string password)
    {
        await Task.CompletedTask; // Para manter a assinatura async
        
        if (string.IsNullOrWhiteSpace(password))
            return false;
            
        // Verificar comprimento mínimo
        if (password.Length < 8)
            return false;
            
        // Verificar padrão de força
        if (!_strongPasswordRegex.IsMatch(password))
            return false;
            
        // Verificar se não é uma senha comum
        var commonPasswords = new[]
        {
            "password", "123456", "123456789", "qwerty", "abc123",
            "password123", "admin", "letmein", "welcome", "monkey"
        };
        
        if (commonPasswords.Contains(password.ToLower()))
            return false;
            
        return true;
    }

    public async Task<string> GenerateSecureTokenAsync(int length = 32)
    {
        return await Task.Run(() =>
        {
            var bytes = new byte[length];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "")
                .Substring(0, Math.Min(length, Convert.ToBase64String(bytes).Length));
        });
    }

    public async Task<bool> IsIpAddressBlockedAsync(string ipAddress)
    {
        await Task.CompletedTask;
        
        if (string.IsNullOrWhiteSpace(ipAddress))
            return false;
            
        lock (_cacheLock)
        {
            if (_blockedIps.TryGetValue(ipAddress, out var blockedUntil))
            {
                if (blockedUntil > DateTime.UtcNow)
                {
                    return true;
                }
                else
                {
                    _blockedIps.Remove(ipAddress);
                }
            }
        }
        
        return false;
    }

    public async Task BlockIpAddressAsync(string ipAddress, int durationMinutes = 60, string reason = "")
    {
        await Task.CompletedTask;
        
        if (string.IsNullOrWhiteSpace(ipAddress))
            return;
            
        var blockedUntil = DateTime.UtcNow.AddMinutes(durationMinutes);
        
        lock (_cacheLock)
        {
            _blockedIps[ipAddress] = blockedUntil;
        }
        
        await LogSecurityEventAsync(null, "IP_BLOCKED", 
            $"IP {ipAddress} bloqueado por {durationMinutes} minutos. Motivo: {reason}", ipAddress);
        
        _logger.LogWarning("IP {IpAddress} bloqueado até {BlockedUntil}. Motivo: {Reason}", 
            ipAddress, blockedUntil, reason);
    }

    public async Task<bool> ValidateEmailFormatAsync(string email)
    {
        await Task.CompletedTask;
        
        if (string.IsNullOrWhiteSpace(email))
            return false;
            
        return _emailRegex.IsMatch(email);
    }

    public async Task<bool> IsEmailDomainAllowedAsync(string email)
    {
        await Task.CompletedTask;
        
        if (!await ValidateEmailFormatAsync(email))
            return false;
            
        var domain = email.Split('@')[1].ToLower();
        
        // Verificar se é um domínio suspeito
        if (_suspiciousDomains.Contains(domain))
        {
            await LogSecurityEventAsync(null, "SUSPICIOUS_EMAIL_DOMAIN", 
                $"Tentativa de uso de email com domínio suspeito: {domain}");
            return false;
        }
        
        // Verificar lista de domínios permitidos (se configurada)
        var allowedDomains = await _configRepository.GetValorAsync("ALLOWED_EMAIL_DOMAINS");
        if (!string.IsNullOrEmpty(allowedDomains))
        {
            var domains = allowedDomains.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(d => d.Trim().ToLower());
                
            return domains.Contains(domain);
        }
        
        return true;
    }

    public async Task LogSecurityEventAsync(int? usuarioId, string eventType, string description, string? ipAddress = null, string? userAgent = null)
    {
        try
        {
            var log = new LogAtividade
            {
                UsuarioId = usuarioId ?? 0,
                TipoAtividade = TipoAtividade.EventoSeguranca,
                Descricao = $"[{eventType}] {description}",
                EnderecoIp = IPAddress.TryParse(ipAddress, out var ip) ? ip : null,
                UserAgent = userAgent,
                CriadoEm = DateTime.UtcNow
            };

            await _logRepository.AddAsync(log);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao registrar evento de segurança");
        }
    }

    public async Task<bool> DetectSuspiciousActivityAsync(int usuarioId, string ipAddress, string userAgent)
    {
        try
        {
            var now = DateTime.UtcNow;
            var last24Hours = now.AddHours(-24);
            
            // Buscar atividades recentes do usuário
            var recentLogs = await _logRepository.GetByUsuarioIdAsync(usuarioId);
            var recentActivity = recentLogs.Where(l => l.CriadoEm >= last24Hours).ToList();
            
            // Detectar múltiplos IPs em pouco tempo
            var uniqueIps = recentActivity
                .Where(l => l.EnderecoIp != null)
                .Select(l => l.EnderecoIp!.ToString())
                .Distinct()
                .Count();
                
            if (uniqueIps > 3)
            {
                await LogSecurityEventAsync(usuarioId, "MULTIPLE_IPS", 
                    $"Usuário acessou de {uniqueIps} IPs diferentes nas últimas 24h", ipAddress, userAgent);
                return true;
            }
            
            // Detectar muitas tentativas de login
            var loginAttempts = recentActivity
                .Count(l => l.TipoAtividade == TipoAtividade.LoginFalhou);
                
            if (loginAttempts > 10)
            {
                await LogSecurityEventAsync(usuarioId, "EXCESSIVE_LOGIN_ATTEMPTS", 
                    $"Usuário teve {loginAttempts} tentativas de login falhadas nas últimas 24h", ipAddress, userAgent);
                return true;
            }
            
            // Detectar mudanças suspeitas de User-Agent
            var userAgents = recentActivity
                .Where(l => !string.IsNullOrEmpty(l.UserAgent))
                .Select(l => l.UserAgent)
                .Distinct()
                .Count();
                
            if (userAgents > 5)
            {
                await LogSecurityEventAsync(usuarioId, "MULTIPLE_USER_AGENTS", 
                    $"Usuário usou {userAgents} User-Agents diferentes nas últimas 24h", ipAddress, userAgent);
                return true;
            }
            
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao detectar atividade suspeita para usuário {UsuarioId}", usuarioId);
            return false;
        }
    }

    public async Task CleanupExpiredDataAsync()
    {
        try
        {
            var now = DateTime.UtcNow;
            
            // Limpar cache de rate limiting
            lock (_cacheLock)
            {
                var expiredKeys = new List<string>();
                
                foreach (var kvp in _rateLimitCache)
                {
                    kvp.Value.RemoveAll(attempt => attempt < now.AddHours(-1));
                    if (kvp.Value.Count == 0)
                    {
                        expiredKeys.Add(kvp.Key);
                    }
                }
                
                foreach (var key in expiredKeys)
                {
                    _rateLimitCache.Remove(key);
                }
                
                // Limpar IPs bloqueados expirados
                var expiredIps = _blockedIps
                    .Where(kvp => kvp.Value <= now)
                    .Select(kvp => kvp.Key)
                    .ToList();
                    
                foreach (var ip in expiredIps)
                {
                    _blockedIps.Remove(ip);
                }
            }
            
            _logger.LogInformation("Limpeza de dados de segurança expirados concluída");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante limpeza de dados de segurança");
        }
        
        await Task.CompletedTask;
    }
}

