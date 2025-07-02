using Microsoft.Extensions.Diagnostics.HealthChecks;
using AcervoEducacional.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace AcervoEducacional.WebApi.HealthChecks
{
    /// <summary>
    /// Health Check para monitoramento da conectividade e performance do banco de dados
    /// </summary>
    public class DatabaseHealthCheck : IHealthCheck
    {
        private readonly AcervoDbContext _context;
        private readonly ILogger<DatabaseHealthCheck> _logger;

        public DatabaseHealthCheck(AcervoDbContext context, ILogger<DatabaseHealthCheck> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();
                
                // Testa conectividade básica
                var canConnect = await _context.Database.CanConnectAsync(cancellationToken);
                
                if (!canConnect)
                {
                    _logger.LogError("Falha na conectividade com o banco de dados");
                    return HealthCheckResult.Unhealthy("Não foi possível conectar ao banco de dados");
                }

                // Testa uma query simples para verificar responsividade
                var userCount = await _context.Usuarios.CountAsync(cancellationToken);
                var cursoCount = await _context.Cursos.CountAsync(cancellationToken);
                var arquivoCount = await _context.Arquivos.CountAsync(cancellationToken);
                
                stopwatch.Stop();
                var responseTime = stopwatch.ElapsedMilliseconds;

                // Coleta informações adicionais
                var connectionString = _context.Database.GetConnectionString();
                var databaseName = _context.Database.GetDbConnection().Database;
                var providerName = _context.Database.ProviderName;

                var data = new Dictionary<string, object>
                {
                    ["database_name"] = databaseName ?? "Unknown",
                    ["provider"] = providerName ?? "Unknown",
                    ["response_time_ms"] = responseTime,
                    ["total_usuarios"] = userCount,
                    ["total_cursos"] = cursoCount,
                    ["total_arquivos"] = arquivoCount,
                    ["connection_state"] = _context.Database.GetDbConnection().State.ToString(),
                    ["last_check"] = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss UTC")
                };

                // Determina o status baseado na performance
                if (responseTime > 5000) // > 5 segundos
                {
                    _logger.LogWarning("Banco de dados respondendo lentamente: {ResponseTime}ms", responseTime);
                    return HealthCheckResult.Degraded(
                        $"Banco de dados respondendo lentamente ({responseTime}ms)", 
                        data: data);
                }
                
                if (responseTime > 2000) // > 2 segundos
                {
                    _logger.LogInformation("Banco de dados com performance reduzida: {ResponseTime}ms", responseTime);
                    return HealthCheckResult.Degraded(
                        $"Performance do banco reduzida ({responseTime}ms)", 
                        data: data);
                }

                _logger.LogDebug("Health check do banco de dados bem-sucedido em {ResponseTime}ms", responseTime);
                
                return HealthCheckResult.Healthy(
                    $"Banco de dados operacional ({responseTime}ms)", 
                    data: data);
            }
            catch (TaskCanceledException)
            {
                _logger.LogWarning("Health check do banco de dados cancelado por timeout");
                return HealthCheckResult.Unhealthy("Timeout na verificação do banco de dados");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante health check do banco de dados");
                
                var errorData = new Dictionary<string, object>
                {
                    ["error_message"] = ex.Message,
                    ["error_type"] = ex.GetType().Name,
                    ["last_check"] = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss UTC")
                };

                return HealthCheckResult.Unhealthy(
                    $"Erro no banco de dados: {ex.Message}", 
                    ex, 
                    data: errorData);
            }
        }
    }

    /// <summary>
    /// Health Check específico para verificar migrações pendentes
    /// </summary>
    public class DatabaseMigrationHealthCheck : IHealthCheck
    {
        private readonly AcervoDbContext _context;
        private readonly ILogger<DatabaseMigrationHealthCheck> _logger;

        public DatabaseMigrationHealthCheck(AcervoDbContext context, ILogger<DatabaseMigrationHealthCheck> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var pendingMigrations = await _context.Database.GetPendingMigrationsAsync(cancellationToken);
                var appliedMigrations = await _context.Database.GetAppliedMigrationsAsync(cancellationToken);

                var data = new Dictionary<string, object>
                {
                    ["applied_migrations_count"] = appliedMigrations.Count(),
                    ["pending_migrations_count"] = pendingMigrations.Count(),
                    ["applied_migrations"] = appliedMigrations.ToArray(),
                    ["pending_migrations"] = pendingMigrations.ToArray(),
                    ["last_check"] = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss UTC")
                };

                if (pendingMigrations.Any())
                {
                    _logger.LogWarning("Existem {Count} migrações pendentes: {Migrations}", 
                        pendingMigrations.Count(), 
                        string.Join(", ", pendingMigrations));
                    
                    return HealthCheckResult.Degraded(
                        $"{pendingMigrations.Count()} migrações pendentes encontradas", 
                        data: data);
                }

                _logger.LogDebug("Todas as migrações estão aplicadas. Total: {Count}", appliedMigrations.Count());
                
                return HealthCheckResult.Healthy(
                    $"Banco atualizado - {appliedMigrations.Count()} migrações aplicadas", 
                    data: data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar migrações do banco de dados");
                
                return HealthCheckResult.Unhealthy(
                    $"Erro ao verificar migrações: {ex.Message}", 
                    ex);
            }
        }
    }

    /// <summary>
    /// Health Check para verificar integridade dos dados críticos
    /// </summary>
    public class DataIntegrityHealthCheck : IHealthCheck
    {
        private readonly AcervoDbContext _context;
        private readonly ILogger<DataIntegrityHealthCheck> _logger;

        public DataIntegrityHealthCheck(AcervoDbContext context, ILogger<DataIntegrityHealthCheck> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var issues = new List<string>();
                var warnings = new List<string>();

                // Verifica usuários órfãos ou com dados inconsistentes
                var usuariosSemEmail = await _context.Usuarios
                    .Where(u => string.IsNullOrEmpty(u.Email))
                    .CountAsync(cancellationToken);

                if (usuariosSemEmail > 0)
                {
                    issues.Add($"{usuariosSemEmail} usuários sem email");
                }

                // Verifica cursos sem código ou com códigos duplicados
                var cursosSemCodigo = await _context.Cursos
                    .Where(c => string.IsNullOrEmpty(c.Codigo))
                    .CountAsync(cancellationToken);

                if (cursosSemCodigo > 0)
                {
                    issues.Add($"{cursosSemCodigo} cursos sem código");
                }

                var codigosDuplicados = await _context.Cursos
                    .GroupBy(c => c.Codigo)
                    .Where(g => g.Count() > 1)
                    .CountAsync(cancellationToken);

                if (codigosDuplicados > 0)
                {
                    issues.Add($"{codigosDuplicados} códigos de curso duplicados");
                }

                // Verifica arquivos órfãos
                var arquivosOrfaos = await _context.Arquivos
                    .Where(a => a.CursoId != null && !_context.Cursos.Any(c => c.Id == a.CursoId))
                    .CountAsync(cancellationToken);

                if (arquivosOrfaos > 0)
                {
                    warnings.Add($"{arquivosOrfaos} arquivos órfãos encontrados");
                }

                // Verifica logs muito antigos (mais de 1 ano)
                var logsAntigos = await _context.LogsAtividade
                    .Where(l => l.DataHora < DateTime.UtcNow.AddYears(-1))
                    .CountAsync(cancellationToken);

                if (logsAntigos > 10000)
                {
                    warnings.Add($"{logsAntigos} logs antigos (>1 ano) - considere limpeza");
                }

                var data = new Dictionary<string, object>
                {
                    ["usuarios_sem_email"] = usuariosSemEmail,
                    ["cursos_sem_codigo"] = cursosSemCodigo,
                    ["codigos_duplicados"] = codigosDuplicados,
                    ["arquivos_orfaos"] = arquivosOrfaos,
                    ["logs_antigos"] = logsAntigos,
                    ["issues_count"] = issues.Count,
                    ["warnings_count"] = warnings.Count,
                    ["last_check"] = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss UTC")
                };

                if (issues.Any())
                {
                    var message = $"Problemas de integridade encontrados: {string.Join(", ", issues)}";
                    _logger.LogError("Problemas de integridade de dados: {Issues}", string.Join("; ", issues));
                    
                    return HealthCheckResult.Unhealthy(message, data: data);
                }

                if (warnings.Any())
                {
                    var message = $"Avisos de integridade: {string.Join(", ", warnings)}";
                    _logger.LogWarning("Avisos de integridade de dados: {Warnings}", string.Join("; ", warnings));
                    
                    return HealthCheckResult.Degraded(message, data: data);
                }

                _logger.LogDebug("Verificação de integridade de dados concluída sem problemas");
                
                return HealthCheckResult.Healthy("Integridade dos dados verificada", data: data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante verificação de integridade dos dados");
                
                return HealthCheckResult.Unhealthy(
                    $"Erro na verificação de integridade: {ex.Message}", 
                    ex);
            }
        }
    }
}

