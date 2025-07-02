using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net.Mail;
using System.Net;
using System.Diagnostics;

namespace AcervoEducacional.WebApi.HealthChecks
{
    /// <summary>
    /// Health Check para monitoramento do serviço de email SMTP
    /// </summary>
    public class EmailHealthCheck : IHealthCheck
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailHealthCheck> _logger;

        public EmailHealthCheck(IConfiguration configuration, ILogger<EmailHealthCheck> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();
                
                // Obtém configurações SMTP
                var smtpHost = _configuration["Email:SmtpHost"];
                var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
                var enableSsl = bool.Parse(_configuration["Email:EnableSsl"] ?? "true");
                var username = _configuration["Email:Username"];
                var password = _configuration["Email:Password"];

                if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(username))
                {
                    return HealthCheckResult.Unhealthy("Configurações SMTP não encontradas");
                }

                // Testa conectividade SMTP
                using var client = new SmtpClient(smtpHost, smtpPort)
                {
                    EnableSsl = enableSsl,
                    Credentials = new NetworkCredential(username, password),
                    Timeout = 10000 // 10 segundos
                };

                // Tenta conectar ao servidor SMTP
                await Task.Run(() => client.Send(CreateTestMessage(username)), cancellationToken);
                
                stopwatch.Stop();
                var responseTime = stopwatch.ElapsedMilliseconds;

                var data = new Dictionary<string, object>
                {
                    ["smtp_host"] = smtpHost,
                    ["smtp_port"] = smtpPort,
                    ["enable_ssl"] = enableSsl,
                    ["username"] = MaskEmail(username),
                    ["response_time_ms"] = responseTime,
                    ["last_check"] = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss UTC")
                };

                // Determina status baseado na performance
                if (responseTime > 10000) // > 10 segundos
                {
                    _logger.LogWarning("Serviço de email respondendo lentamente: {ResponseTime}ms", responseTime);
                    return HealthCheckResult.Degraded(
                        $"Email respondendo lentamente ({responseTime}ms)", 
                        data: data);
                }

                _logger.LogDebug("Health check do email bem-sucedido em {ResponseTime}ms", responseTime);
                
                return HealthCheckResult.Healthy(
                    $"Serviço de email operacional ({responseTime}ms)", 
                    data: data);
            }
            catch (SmtpException smtpEx)
            {
                _logger.LogError(smtpEx, "Erro SMTP durante health check do email");
                
                var errorData = new Dictionary<string, object>
                {
                    ["error_message"] = smtpEx.Message,
                    ["error_type"] = "SMTP Error",
                    ["status_code"] = smtpEx.StatusCode.ToString(),
                    ["last_check"] = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss UTC")
                };

                return HealthCheckResult.Unhealthy(
                    $"Erro SMTP: {smtpEx.Message}", 
                    smtpEx, 
                    data: errorData);
            }
            catch (TaskCanceledException)
            {
                _logger.LogWarning("Health check do email cancelado por timeout");
                return HealthCheckResult.Unhealthy("Timeout na verificação do serviço de email");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante health check do email");
                
                var errorData = new Dictionary<string, object>
                {
                    ["error_message"] = ex.Message,
                    ["error_type"] = ex.GetType().Name,
                    ["last_check"] = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss UTC")
                };

                return HealthCheckResult.Unhealthy(
                    $"Erro no serviço de email: {ex.Message}", 
                    ex, 
                    data: errorData);
            }
        }

        /// <summary>
        /// Cria uma mensagem de teste para verificar o SMTP
        /// </summary>
        private MailMessage CreateTestMessage(string fromEmail)
        {
            var message = new MailMessage
            {
                From = new MailAddress(fromEmail, "Acervo Educacional - Health Check"),
                Subject = "Health Check - Teste de Conectividade",
                Body = @"
                    <html>
                    <body style='font-family: Arial, sans-serif;'>
                        <h2 style='color: #DC2626;'>🏥 Health Check - Serviço de Email</h2>
                        <p>Esta é uma mensagem de teste automática do sistema de monitoramento.</p>
                        <p><strong>Sistema:</strong> Acervo Educacional - Ferreira Costa</p>
                        <p><strong>Data/Hora:</strong> " + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss UTC") + @"</p>
                        <p><strong>Status:</strong> ✅ Serviço de email operacional</p>
                        <hr>
                        <small style='color: #666;'>
                            Esta mensagem foi gerada automaticamente pelo sistema de health checks.<br>
                            Não é necessário responder a este email.
                        </small>
                    </body>
                    </html>",
                IsBodyHtml = true
            };

            // Envia para o próprio remetente como teste
            message.To.Add(fromEmail);
            
            return message;
        }

        /// <summary>
        /// Mascara o email para logs de segurança
        /// </summary>
        private string MaskEmail(string email)
        {
            if (string.IsNullOrEmpty(email) || !email.Contains('@'))
                return "***";

            var parts = email.Split('@');
            var username = parts[0];
            var domain = parts[1];

            if (username.Length <= 2)
                return $"***@{domain}";

            return $"{username.Substring(0, 2)}***@{domain}";
        }
    }

    /// <summary>
    /// Health Check para verificar templates de email
    /// </summary>
    public class EmailTemplateHealthCheck : IHealthCheck
    {
        private readonly ILogger<EmailTemplateHealthCheck> _logger;
        private readonly IWebHostEnvironment _environment;

        public EmailTemplateHealthCheck(ILogger<EmailTemplateHealthCheck> logger, IWebHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var templatesPath = Path.Combine(_environment.ContentRootPath, "Templates", "Email");
                var requiredTemplates = new[]
                {
                    "RecuperacaoSenha.html",
                    "BoasVindas.html",
                    "NotificacaoCurso.html",
                    "RelatorioSemanal.html"
                };

                var missingTemplates = new List<string>();
                var corruptedTemplates = new List<string>();
                var templateSizes = new Dictionary<string, long>();

                foreach (var template in requiredTemplates)
                {
                    var templatePath = Path.Combine(templatesPath, template);
                    
                    if (!File.Exists(templatePath))
                    {
                        missingTemplates.Add(template);
                        continue;
                    }

                    try
                    {
                        var content = await File.ReadAllTextAsync(templatePath, cancellationToken);
                        templateSizes[template] = content.Length;

                        // Verifica se o template tem estrutura HTML básica
                        if (!content.Contains("<html") || !content.Contains("</html>"))
                        {
                            corruptedTemplates.Add($"{template} (estrutura HTML inválida)");
                        }

                        // Verifica se contém placeholders esperados
                        if (template == "RecuperacaoSenha.html" && !content.Contains("{{resetLink}}"))
                        {
                            corruptedTemplates.Add($"{template} (placeholder resetLink ausente)");
                        }

                        if (template == "BoasVindas.html" && !content.Contains("{{nomeUsuario}}"))
                        {
                            corruptedTemplates.Add($"{template} (placeholder nomeUsuario ausente)");
                        }
                    }
                    catch (Exception ex)
                    {
                        corruptedTemplates.Add($"{template} (erro de leitura: {ex.Message})");
                    }
                }

                var data = new Dictionary<string, object>
                {
                    ["templates_path"] = templatesPath,
                    ["required_templates"] = requiredTemplates,
                    ["missing_templates"] = missingTemplates.ToArray(),
                    ["corrupted_templates"] = corruptedTemplates.ToArray(),
                    ["template_sizes"] = templateSizes,
                    ["total_templates"] = requiredTemplates.Length,
                    ["missing_count"] = missingTemplates.Count,
                    ["corrupted_count"] = corruptedTemplates.Count,
                    ["last_check"] = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss UTC")
                };

                if (missingTemplates.Any())
                {
                    var message = $"Templates de email ausentes: {string.Join(", ", missingTemplates)}";
                    _logger.LogError("Templates de email ausentes: {Templates}", string.Join("; ", missingTemplates));
                    
                    return HealthCheckResult.Unhealthy(message, data: data);
                }

                if (corruptedTemplates.Any())
                {
                    var message = $"Templates de email corrompidos: {string.Join(", ", corruptedTemplates)}";
                    _logger.LogWarning("Templates de email com problemas: {Templates}", string.Join("; ", corruptedTemplates));
                    
                    return HealthCheckResult.Degraded(message, data: data);
                }

                _logger.LogDebug("Todos os {Count} templates de email estão disponíveis e válidos", requiredTemplates.Length);
                
                return HealthCheckResult.Healthy(
                    $"Todos os {requiredTemplates.Length} templates de email estão válidos", 
                    data: data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante verificação dos templates de email");
                
                return HealthCheckResult.Unhealthy(
                    $"Erro na verificação de templates: {ex.Message}", 
                    ex);
            }
        }
    }

    /// <summary>
    /// Health Check para verificar quota e limites de email
    /// </summary>
    public class EmailQuotaHealthCheck : IHealthCheck
    {
        private readonly ILogger<EmailQuotaHealthCheck> _logger;
        private readonly IConfiguration _configuration;

        public EmailQuotaHealthCheck(ILogger<EmailQuotaHealthCheck> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                // Simula verificação de quota (em produção, consultaria o provedor de email)
                var dailyLimit = int.Parse(_configuration["Email:DailyLimit"] ?? "1000");
                var hourlyLimit = int.Parse(_configuration["Email:HourlyLimit"] ?? "100");
                
                // Em produção, estes valores viriam de um cache ou banco de dados
                var emailsSentToday = await GetEmailsSentToday();
                var emailsSentThisHour = await GetEmailsSentThisHour();
                
                var dailyUsagePercent = (emailsSentToday * 100.0) / dailyLimit;
                var hourlyUsagePercent = (emailsSentThisHour * 100.0) / hourlyLimit;

                var data = new Dictionary<string, object>
                {
                    ["daily_limit"] = dailyLimit,
                    ["hourly_limit"] = hourlyLimit,
                    ["emails_sent_today"] = emailsSentToday,
                    ["emails_sent_this_hour"] = emailsSentThisHour,
                    ["daily_usage_percent"] = Math.Round(dailyUsagePercent, 2),
                    ["hourly_usage_percent"] = Math.Round(hourlyUsagePercent, 2),
                    ["daily_remaining"] = dailyLimit - emailsSentToday,
                    ["hourly_remaining"] = hourlyLimit - emailsSentThisHour,
                    ["last_check"] = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss UTC")
                };

                // Verifica se está próximo dos limites
                if (dailyUsagePercent >= 90 || hourlyUsagePercent >= 90)
                {
                    var message = $"Quota de email crítica - Diário: {dailyUsagePercent:F1}%, Horário: {hourlyUsagePercent:F1}%";
                    _logger.LogError("Quota de email crítica: {DailyPercent}% diário, {HourlyPercent}% horário", 
                        dailyUsagePercent, hourlyUsagePercent);
                    
                    return HealthCheckResult.Unhealthy(message, data: data);
                }

                if (dailyUsagePercent >= 75 || hourlyUsagePercent >= 75)
                {
                    var message = $"Quota de email alta - Diário: {dailyUsagePercent:F1}%, Horário: {hourlyUsagePercent:F1}%";
                    _logger.LogWarning("Quota de email alta: {DailyPercent}% diário, {HourlyPercent}% horário", 
                        dailyUsagePercent, hourlyUsagePercent);
                    
                    return HealthCheckResult.Degraded(message, data: data);
                }

                _logger.LogDebug("Quota de email normal: {DailyPercent}% diário, {HourlyPercent}% horário", 
                    dailyUsagePercent, hourlyUsagePercent);
                
                return HealthCheckResult.Healthy(
                    $"Quota normal - Diário: {dailyUsagePercent:F1}%, Horário: {hourlyUsagePercent:F1}%", 
                    data: data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante verificação de quota de email");
                
                return HealthCheckResult.Unhealthy(
                    $"Erro na verificação de quota: {ex.Message}", 
                    ex);
            }
        }

        /// <summary>
        /// Obtém número de emails enviados hoje (simulado)
        /// Em produção, consultaria logs ou banco de dados
        /// </summary>
        private async Task<int> GetEmailsSentToday()
        {
            await Task.Delay(10); // Simula consulta assíncrona
            
            // Simula contagem baseada na hora do dia
            var hour = DateTime.Now.Hour;
            return hour * 5; // Simula 5 emails por hora
        }

        /// <summary>
        /// Obtém número de emails enviados na última hora (simulado)
        /// Em produção, consultaria logs ou banco de dados
        /// </summary>
        private async Task<int> GetEmailsSentThisHour()
        {
            await Task.Delay(10); // Simula consulta assíncrona
            
            // Simula contagem baseada nos minutos
            var minute = DateTime.Now.Minute;
            return minute / 10; // Simula emails baseado nos minutos
        }
    }
}

