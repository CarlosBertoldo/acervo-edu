using Microsoft.Extensions.Logging;
using Hangfire;
using AcervoEducacional.Application.Interfaces;
using AcervoEducacional.Domain.Interfaces;
using AcervoEducacional.Domain.Enums;

namespace AcervoEducacional.Application.Services;

/// <summary>
/// Service para gerenciar background jobs essenciais do sistema
/// Jobs executados pelo Hangfire para manutenção e monitoramento
/// </summary>
public interface IBackgroundJobService
{
    Task CleanupExpiredDataAsync();
    Task SecurityAnalysisAsync();
    Task GenerateSystemReportsAsync();
    Task DatabaseMaintenanceAsync();
}

public class BackgroundJobService : IBackgroundJobService
{
    private readonly ISecurityService _securityService;
    private readonly ILogAtividadeRepository _logRepository;
    private readonly ISessaoUsuarioRepository _sessaoRepository;
    private readonly ITokenRecuperacaoRepository _tokenRepository;
    private readonly ILogger<BackgroundJobService> _logger;

    public BackgroundJobService(
        ISecurityService securityService,
        ILogAtividadeRepository logRepository,
        ISessaoUsuarioRepository sessaoRepository,
        ITokenRecuperacaoRepository tokenRepository,
        ILogger<BackgroundJobService> logger)
    {
        _securityService = securityService;
        _logRepository = logRepository;
        _sessaoRepository = sessaoRepository;
        _tokenRepository = tokenRepository;
        _logger = logger;
    }

    /// <summary>
    /// Job executado diariamente às 2:00 AM para limpeza de dados expirados
    /// </summary>
    [AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 60, 300, 900 })]
    [Queue("background")]
    public async Task CleanupExpiredDataAsync()
    {
        try
        {
            _logger.LogInformation("🧹 Iniciando limpeza de dados expirados");
            var startTime = DateTime.UtcNow;

            // 1. Limpar cache de segurança
            await _securityService.CleanupExpiredDataAsync();
            _logger.LogInformation("✅ Cache de segurança limpo");

            // 2. Limpar sessões expiradas (mais de 7 dias)
            var cutoffDate = DateTime.UtcNow.AddDays(-7);
            var expiredSessions = await _sessaoRepository.GetExpiredSessionsAsync(cutoffDate);
            
            if (expiredSessions.Any())
            {
                foreach (var session in expiredSessions)
                {
                    await _sessaoRepository.DeleteAsync(session.Id);
                }
                _logger.LogInformation("✅ {Count} sessões expiradas removidas", expiredSessions.Count());
            }

            // 3. Limpar tokens de recuperação expirados
            var expiredTokens = await _tokenRepository.GetExpiredTokensAsync();
            
            if (expiredTokens.Any())
            {
                foreach (var token in expiredTokens)
                {
                    await _tokenRepository.DeleteAsync(token.Id);
                }
                _logger.LogInformation("✅ {Count} tokens de recuperação expirados removidos", expiredTokens.Count());
            }

            // 4. Limpar logs antigos (mais de 90 dias)
            var logCutoffDate = DateTime.UtcNow.AddDays(-90);
            var deletedLogsCount = await _logRepository.DeleteOldLogsAsync(logCutoffDate);
            _logger.LogInformation("✅ {Count} logs antigos removidos", deletedLogsCount);

            var duration = DateTime.UtcNow - startTime;
            _logger.LogInformation("🎉 Limpeza concluída em {Duration}ms", duration.TotalMilliseconds);

            // Log da atividade
            await _logRepository.AddAsync(new Domain.Entities.LogAtividade
            {
                UsuarioId = 0, // Sistema
                TipoAtividade = TipoAtividade.ManutencaoSistema,
                Descricao = $"Limpeza automática concluída. Duração: {duration.TotalSeconds:F2}s",
                CriadoEm = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro durante limpeza de dados expirados");
            
            // Log do erro
            await _logRepository.AddAsync(new Domain.Entities.LogAtividade
            {
                UsuarioId = 0,
                TipoAtividade = TipoAtividade.ErroSistema,
                Descricao = $"Erro na limpeza automática: {ex.Message}",
                CriadoEm = DateTime.UtcNow
            });
            
            throw; // Re-throw para que o Hangfire registre a falha
        }
    }

    /// <summary>
    /// Job executado a cada 4 horas para análise de segurança
    /// </summary>
    [AutomaticRetry(Attempts = 2, DelaysInSeconds = new[] { 120, 600 })]
    [Queue("critical")]
    public async Task SecurityAnalysisAsync()
    {
        try
        {
            _logger.LogInformation("🔍 Iniciando análise de segurança");
            var startTime = DateTime.UtcNow;

            // 1. Buscar atividades suspeitas nas últimas 4 horas
            var cutoffTime = DateTime.UtcNow.AddHours(-4);
            var recentLogs = await _logRepository.GetLogsSinceAsync(cutoffTime);

            // 2. Detectar padrões suspeitos
            var suspiciousActivities = new List<string>();

            // 2.1 Muitas tentativas de login falhadas
            var failedLogins = recentLogs.Count(l => l.TipoAtividade == TipoAtividade.LoginFalhou);
            if (failedLogins > 50)
            {
                suspiciousActivities.Add($"Alto número de tentativas de login falhadas: {failedLogins}");
            }

            // 2.2 Muitos IPs únicos
            var uniqueIps = recentLogs
                .Where(l => l.EnderecoIp != null)
                .Select(l => l.EnderecoIp!.ToString())
                .Distinct()
                .Count();
            
            if (uniqueIps > 100)
            {
                suspiciousActivities.Add($"Alto número de IPs únicos: {uniqueIps}");
            }

            // 2.3 Verificar sessões ativas suspeitas
            var activeSessions = await _sessaoRepository.GetActiveSessionsAsync();
            var suspiciousSessions = 0;
            
            foreach (var session in activeSessions)
            {
                if (await _securityService.DetectSuspiciousActivityAsync(session.UsuarioId, 
                    session.IpAddress?.ToString() ?? "", session.UserAgent ?? ""))
                {
                    suspiciousSessions++;
                }
            }

            if (suspiciousSessions > 0)
            {
                suspiciousActivities.Add($"Sessões com atividade suspeita: {suspiciousSessions}");
            }

            // 3. Log dos resultados
            if (suspiciousActivities.Any())
            {
                var alertMessage = "🚨 ALERTA DE SEGURANÇA: " + string.Join("; ", suspiciousActivities);
                _logger.LogWarning(alertMessage);
                
                await _logRepository.AddAsync(new Domain.Entities.LogAtividade
                {
                    UsuarioId = 0,
                    TipoAtividade = TipoAtividade.AlertaSeguranca,
                    Descricao = alertMessage,
                    CriadoEm = DateTime.UtcNow
                });
            }
            else
            {
                _logger.LogInformation("✅ Nenhuma atividade suspeita detectada");
            }

            var duration = DateTime.UtcNow - startTime;
            _logger.LogInformation("🔍 Análise de segurança concluída em {Duration}ms", duration.TotalMilliseconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro durante análise de segurança");
            throw;
        }
    }

    /// <summary>
    /// Job executado semanalmente para gerar relatórios do sistema
    /// </summary>
    [AutomaticRetry(Attempts = 2)]
    [Queue("background")]
    public async Task GenerateSystemReportsAsync()
    {
        try
        {
            _logger.LogInformation("📊 Gerando relatórios semanais do sistema");
            var startTime = DateTime.UtcNow;

            // 1. Estatísticas da semana passada
            var weekAgo = DateTime.UtcNow.AddDays(-7);
            var weeklyLogs = await _logRepository.GetLogsSinceAsync(weekAgo);

            // 2. Métricas básicas
            var metrics = new
            {
                TotalLogins = weeklyLogs.Count(l => l.TipoAtividade == TipoAtividade.Login),
                FailedLogins = weeklyLogs.Count(l => l.TipoAtividade == TipoAtividade.LoginFalhou),
                UniqueUsers = weeklyLogs.Where(l => l.UsuarioId > 0).Select(l => l.UsuarioId).Distinct().Count(),
                SecurityEvents = weeklyLogs.Count(l => l.TipoAtividade == TipoAtividade.EventoSeguranca),
                SystemErrors = weeklyLogs.Count(l => l.TipoAtividade == TipoAtividade.ErroSistema)
            };

            // 3. Log do relatório
            var reportContent = $"Relatório Semanal: " +
                              $"Logins: {metrics.TotalLogins}, " +
                              $"Falhas: {metrics.FailedLogins}, " +
                              $"Usuários: {metrics.UniqueUsers}, " +
                              $"Eventos Segurança: {metrics.SecurityEvents}, " +
                              $"Erros: {metrics.SystemErrors}";

            await _logRepository.AddAsync(new Domain.Entities.LogAtividade
            {
                UsuarioId = 0,
                TipoAtividade = TipoAtividade.RelatorioSistema,
                Descricao = reportContent,
                CriadoEm = DateTime.UtcNow
            });

            var duration = DateTime.UtcNow - startTime;
            _logger.LogInformation("📊 Relatório semanal gerado em {Duration}ms", duration.TotalMilliseconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao gerar relatórios do sistema");
            throw;
        }
    }

    /// <summary>
    /// Job executado diariamente para manutenção do banco de dados
    /// </summary>
    [AutomaticRetry(Attempts = 2)]
    [Queue("background")]
    public async Task DatabaseMaintenanceAsync()
    {
        try
        {
            _logger.LogInformation("🔧 Iniciando manutenção do banco de dados");
            var startTime = DateTime.UtcNow;

            // 1. Verificar saúde das conexões
            var activeConnections = await _sessaoRepository.CountActiveSessionsAsync();
            _logger.LogInformation("📊 Conexões ativas: {Count}", activeConnections);

            // 2. Verificar crescimento dos logs
            var totalLogs = await _logRepository.CountTotalLogsAsync();
            _logger.LogInformation("📊 Total de logs: {Count}", totalLogs);

            // 3. Log da manutenção
            await _logRepository.AddAsync(new Domain.Entities.LogAtividade
            {
                UsuarioId = 0,
                TipoAtividade = TipoAtividade.ManutencaoSistema,
                Descricao = $"Manutenção DB: {activeConnections} conexões, {totalLogs} logs",
                CriadoEm = DateTime.UtcNow
            });

            var duration = DateTime.UtcNow - startTime;
            _logger.LogInformation("🔧 Manutenção concluída em {Duration}ms", duration.TotalMilliseconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro durante manutenção do banco");
            throw;
        }
    }
}