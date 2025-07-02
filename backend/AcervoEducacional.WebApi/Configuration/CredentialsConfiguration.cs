using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AcervoEducacional.WebApi.Configuration
{
    /// <summary>
    /// Serviço para gerenciar credenciais de forma segura
    /// Usa variáveis de ambiente em produção e fallback para desenvolvimento
    /// </summary>
    public interface ICredentialsService
    {
        string GetJwtSecretKey();
        string GetAwsAccessKey();
        string GetAwsSecretKey();
        string GetEmailPassword();
        string GetDatabaseConnectionString();
        bool IsProductionMode();
    }

    public class CredentialsService : ICredentialsService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<CredentialsService> _logger;
        private readonly bool _useEnvironmentCredentials;
        private readonly bool _isProductionMode;

        public CredentialsService(IConfiguration configuration, ILogger<CredentialsService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            
            // Verificar se deve usar credenciais de ambiente
            _useEnvironmentCredentials = _configuration.GetValue<bool>("Security:UseEnvironmentCredentials", false);
            _isProductionMode = _configuration.GetValue<bool>("Security:ProductionMode", false);
            
            LogCredentialsMode();
        }

        public bool IsProductionMode() => _isProductionMode;

        public string GetJwtSecretKey()
        {
            if (_useEnvironmentCredentials)
            {
                var envKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
                if (!string.IsNullOrEmpty(envKey))
                {
                    _logger.LogInformation("JWT Secret carregada de variável de ambiente");
                    return envKey;
                }
                
                if (_isProductionMode)
                {
                    throw new InvalidOperationException("JWT_SECRET_KEY não encontrada nas variáveis de ambiente (modo produção)");
                }
                
                _logger.LogWarning("JWT_SECRET_KEY não encontrada em variáveis de ambiente, usando fallback de desenvolvimento");
            }

            // Fallback para desenvolvimento
            var fallbackKey = _configuration["JwtSettings:SecretKey"];
            if (string.IsNullOrEmpty(fallbackKey))
            {
                throw new InvalidOperationException("JWT SecretKey não configurada");
            }

            _logger.LogInformation("JWT Secret carregada do appsettings (modo desenvolvimento)");
            return fallbackKey;
        }

        public string GetAwsAccessKey()
        {
            if (_useEnvironmentCredentials)
            {
                var envKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID");
                if (!string.IsNullOrEmpty(envKey))
                {
                    _logger.LogInformation("AWS Access Key carregada de variável de ambiente");
                    return envKey;
                }
                
                if (_isProductionMode)
                {
                    throw new InvalidOperationException("AWS_ACCESS_KEY_ID não encontrada nas variáveis de ambiente (modo produção)");
                }
                
                _logger.LogWarning("AWS_ACCESS_KEY_ID não encontrada em variáveis de ambiente, usando fallback de desenvolvimento");
            }

            // Fallback para desenvolvimento
            var fallbackKey = _configuration["AwsSettings:AccessKey"];
            if (string.IsNullOrEmpty(fallbackKey))
            {
                throw new InvalidOperationException("AWS AccessKey não configurada");
            }

            _logger.LogInformation("AWS Access Key carregada do appsettings (modo desenvolvimento)");
            return fallbackKey;
        }

        public string GetAwsSecretKey()
        {
            if (_useEnvironmentCredentials)
            {
                var envKey = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY");
                if (!string.IsNullOrEmpty(envKey))
                {
                    _logger.LogInformation("AWS Secret Key carregada de variável de ambiente");
                    return envKey;
                }
                
                if (_isProductionMode)
                {
                    throw new InvalidOperationException("AWS_SECRET_ACCESS_KEY não encontrada nas variáveis de ambiente (modo produção)");
                }
                
                _logger.LogWarning("AWS_SECRET_ACCESS_KEY não encontrada em variáveis de ambiente, usando fallback de desenvolvimento");
            }

            // Fallback para desenvolvimento
            var fallbackKey = _configuration["AwsSettings:SecretKey"];
            if (string.IsNullOrEmpty(fallbackKey))
            {
                throw new InvalidOperationException("AWS SecretKey não configurada");
            }

            _logger.LogInformation("AWS Secret Key carregada do appsettings (modo desenvolvimento)");
            return fallbackKey;
        }

        public string GetEmailPassword()
        {
            if (_useEnvironmentCredentials)
            {
                var envPassword = Environment.GetEnvironmentVariable("EMAIL_PASSWORD");
                if (!string.IsNullOrEmpty(envPassword))
                {
                    _logger.LogInformation("Email Password carregada de variável de ambiente");
                    return envPassword;
                }
                
                if (_isProductionMode)
                {
                    throw new InvalidOperationException("EMAIL_PASSWORD não encontrada nas variáveis de ambiente (modo produção)");
                }
                
                _logger.LogWarning("EMAIL_PASSWORD não encontrada em variáveis de ambiente, usando fallback de desenvolvimento");
            }

            // Fallback para desenvolvimento
            var fallbackPassword = _configuration["EmailSettings:SmtpPassword"];
            if (string.IsNullOrEmpty(fallbackPassword))
            {
                throw new InvalidOperationException("Email Password não configurada");
            }

            _logger.LogInformation("Email Password carregada do appsettings (modo desenvolvimento)");
            return fallbackPassword;
        }

        public string GetDatabaseConnectionString()
        {
            if (_useEnvironmentCredentials)
            {
                var envConnectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING");
                if (!string.IsNullOrEmpty(envConnectionString))
                {
                    _logger.LogInformation("Database Connection String carregada de variável de ambiente");
                    return envConnectionString;
                }
                
                if (_isProductionMode)
                {
                    throw new InvalidOperationException("DATABASE_CONNECTION_STRING não encontrada nas variáveis de ambiente (modo produção)");
                }
                
                _logger.LogWarning("DATABASE_CONNECTION_STRING não encontrada em variáveis de ambiente, usando fallback de desenvolvimento");
            }

            // Fallback para desenvolvimento
            var fallbackConnectionString = _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(fallbackConnectionString))
            {
                throw new InvalidOperationException("Database Connection String não configurada");
            }

            _logger.LogInformation("Database Connection String carregada do appsettings (modo desenvolvimento)");
            return fallbackConnectionString;
        }

        private void LogCredentialsMode()
        {
            if (_isProductionMode)
            {
                _logger.LogInformation("🔒 MODO PRODUÇÃO ATIVADO - Credenciais obrigatórias de variáveis de ambiente");
            }
            else if (_useEnvironmentCredentials)
            {
                _logger.LogInformation("🔧 MODO HÍBRIDO - Tentará variáveis de ambiente com fallback para appsettings");
            }
            else
            {
                _logger.LogInformation("🛠️ MODO DESENVOLVIMENTO - Usando credenciais do appsettings.json");
            }
        }
    }

    /// <summary>
    /// Extensões para facilitar o registro do serviço
    /// </summary>
    public static class CredentialsServiceExtensions
    {
        public static IServiceCollection AddCredentialsService(this IServiceCollection services)
        {
            services.AddSingleton<ICredentialsService, CredentialsService>();
            return services;
        }
    }
}

