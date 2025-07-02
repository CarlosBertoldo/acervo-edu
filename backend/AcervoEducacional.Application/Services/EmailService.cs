using System.Net;
using System.Net.Mail;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using AcervoEducacional.Application.Interfaces;

namespace AcervoEducacional.Application.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;
    
    private readonly string _smtpHost;
    private readonly int _smtpPort;
    private readonly string _smtpUsername;
    private readonly string _smtpPassword;
    private readonly string _fromEmail;
    private readonly string _fromName;
    private readonly bool _enableSsl;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        
        // Configurações SMTP
        _smtpHost = _configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
        _smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
        _smtpUsername = _configuration["Email:Username"] ?? "";
        _smtpPassword = _configuration["Email:Password"] ?? "";
        _fromEmail = _configuration["Email:FromEmail"] ?? "noreply@ferreiracosta.com";
        _fromName = _configuration["Email:FromName"] ?? "Acervo Educacional - Ferreira Costa";
        _enableSsl = bool.Parse(_configuration["Email:EnableSsl"] ?? "true");
    }

    public async Task<bool> SendEmailAsync(string to, string subject, string body)
    {
        try
        {
            if (string.IsNullOrEmpty(_smtpUsername) || string.IsNullOrEmpty(_smtpPassword))
            {
                _logger.LogWarning("Configurações de email não encontradas. Email não enviado para {To}", to);
                return false;
            }

            using var client = new SmtpClient(_smtpHost, _smtpPort)
            {
                Credentials = new NetworkCredential(_smtpUsername, _smtpPassword),
                EnableSsl = _enableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false
            };

            using var message = new MailMessage
            {
                From = new MailAddress(_fromEmail, _fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
                BodyEncoding = Encoding.UTF8,
                SubjectEncoding = Encoding.UTF8
            };

            message.To.Add(to);

            await client.SendMailAsync(message);
            
            _logger.LogInformation("Email enviado com sucesso para {To} com assunto '{Subject}'", to, subject);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar email para {To} com assunto '{Subject}'", to, subject);
            return false;
        }
    }

    public async Task<bool> SendPasswordResetEmailAsync(string to, string token)
    {
        try
        {
            var resetUrl = $"{_configuration["Frontend:BaseUrl"]}/reset-password?token={token}";
            
            var subject = "Recuperação de Senha - Acervo Educacional Ferreira Costa";
            
            var body = GeneratePasswordResetEmailTemplate(resetUrl);
            
            return await SendEmailAsync(to, subject, body);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar email de recuperação de senha para {To}", to);
            return false;
        }
    }

    public async Task<bool> SendWelcomeEmailAsync(string to, string nome)
    {
        try
        {
            var subject = "Bem-vindo ao Acervo Educacional - Ferreira Costa";
            
            var body = GenerateWelcomeEmailTemplate(nome);
            
            return await SendEmailAsync(to, subject, body);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar email de boas-vindas para {To}", to);
            return false;
        }
    }

    #region Templates de Email

    private string GeneratePasswordResetEmailTemplate(string resetUrl)
    {
        return $@"
<!DOCTYPE html>
<html lang='pt-BR'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Recuperação de Senha</title>
    <style>
        body {{
            font-family: 'Barlow', Arial, sans-serif;
            line-height: 1.6;
            color: #333;
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
            background-color: #f5f5f5;
        }}
        .container {{
            background-color: white;
            padding: 30px;
            border-radius: 10px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
        }}
        .header {{
            text-align: center;
            margin-bottom: 30px;
            padding-bottom: 20px;
            border-bottom: 3px solid #DC2626;
        }}
        .logo {{
            color: #DC2626;
            font-size: 24px;
            font-weight: bold;
            margin-bottom: 10px;
        }}
        .content {{
            margin-bottom: 30px;
        }}
        .button {{
            display: inline-block;
            background-color: #DC2626;
            color: white;
            padding: 12px 30px;
            text-decoration: none;
            border-radius: 5px;
            font-weight: bold;
            margin: 20px 0;
        }}
        .button:hover {{
            background-color: #B91C1C;
        }}
        .footer {{
            text-align: center;
            margin-top: 30px;
            padding-top: 20px;
            border-top: 1px solid #eee;
            font-size: 12px;
            color: #666;
        }}
        .warning {{
            background-color: #FEF3C7;
            border: 1px solid #F59E0B;
            padding: 15px;
            border-radius: 5px;
            margin: 20px 0;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <div class='logo'>Ferreira Costa</div>
            <h2>Acervo Educacional</h2>
        </div>
        
        <div class='content'>
            <h3>Recuperação de Senha</h3>
            <p>Olá!</p>
            <p>Recebemos uma solicitação para redefinir a senha da sua conta no Acervo Educacional da Ferreira Costa.</p>
            <p>Para criar uma nova senha, clique no botão abaixo:</p>
            
            <div style='text-align: center;'>
                <a href='{resetUrl}' class='button'>Redefinir Senha</a>
            </div>
            
            <div class='warning'>
                <strong>⚠️ Importante:</strong>
                <ul>
                    <li>Este link é válido por apenas 2 horas</li>
                    <li>Se você não solicitou esta alteração, ignore este email</li>
                    <li>Nunca compartilhe este link com outras pessoas</li>
                </ul>
            </div>
            
            <p>Se o botão não funcionar, copie e cole o link abaixo no seu navegador:</p>
            <p style='word-break: break-all; background-color: #f8f9fa; padding: 10px; border-radius: 5px;'>
                {resetUrl}
            </p>
        </div>
        
        <div class='footer'>
            <p>Este é um email automático, não responda.</p>
            <p>© 2025 Ferreira Costa - Acervo Educacional</p>
        </div>
    </div>
</body>
</html>";
    }

    private string GenerateWelcomeEmailTemplate(string nome)
    {
        return $@"
<!DOCTYPE html>
<html lang='pt-BR'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Bem-vindo ao Acervo Educacional</title>
    <style>
        body {{
            font-family: 'Barlow', Arial, sans-serif;
            line-height: 1.6;
            color: #333;
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
            background-color: #f5f5f5;
        }}
        .container {{
            background-color: white;
            padding: 30px;
            border-radius: 10px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
        }}
        .header {{
            text-align: center;
            margin-bottom: 30px;
            padding-bottom: 20px;
            border-bottom: 3px solid #DC2626;
        }}
        .logo {{
            color: #DC2626;
            font-size: 24px;
            font-weight: bold;
            margin-bottom: 10px;
        }}
        .content {{
            margin-bottom: 30px;
        }}
        .features {{
            background-color: #f8f9fa;
            padding: 20px;
            border-radius: 5px;
            margin: 20px 0;
        }}
        .feature-item {{
            margin: 10px 0;
            padding-left: 20px;
            position: relative;
        }}
        .feature-item:before {{
            content: '✓';
            position: absolute;
            left: 0;
            color: #16A34A;
            font-weight: bold;
        }}
        .button {{
            display: inline-block;
            background-color: #DC2626;
            color: white;
            padding: 12px 30px;
            text-decoration: none;
            border-radius: 5px;
            font-weight: bold;
            margin: 20px 0;
        }}
        .footer {{
            text-align: center;
            margin-top: 30px;
            padding-top: 20px;
            border-top: 1px solid #eee;
            font-size: 12px;
            color: #666;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <div class='logo'>Ferreira Costa</div>
            <h2>Acervo Educacional</h2>
        </div>
        
        <div class='content'>
            <h3>Bem-vindo, {nome}!</h3>
            <p>É um prazer tê-lo conosco no Acervo Educacional da Ferreira Costa!</p>
            <p>Sua conta foi criada com sucesso e você já pode começar a explorar nossa plataforma.</p>
            
            <div class='features'>
                <h4>O que você pode fazer:</h4>
                <div class='feature-item'>Gerenciar cursos educacionais</div>
                <div class='feature-item'>Organizar conteúdo no sistema Kanban</div>
                <div class='feature-item'>Fazer upload e compartilhar arquivos</div>
                <div class='feature-item'>Acompanhar relatórios e estatísticas</div>
                <div class='feature-item'>Colaborar com sua equipe</div>
            </div>
            
            <div style='text-align: center;'>
                <a href='{_configuration["Frontend:BaseUrl"]}' class='button'>Acessar Plataforma</a>
            </div>
            
            <p>Se você tiver alguma dúvida ou precisar de ajuda, nossa equipe de suporte está sempre disponível.</p>
        </div>
        
        <div class='footer'>
            <p>Este é um email automático, não responda.</p>
            <p>© 2025 Ferreira Costa - Acervo Educacional</p>
        </div>
    </div>
</body>
</html>";
    }

    #endregion
}

