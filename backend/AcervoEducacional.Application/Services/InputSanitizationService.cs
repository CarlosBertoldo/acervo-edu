using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.Extensions.Logging;

namespace AcervoEducacional.Application.Services
{
    /// <summary>
    /// Serviço para sanitização de inputs e proteção contra XSS, injeção SQL e outros ataques
    /// </summary>
    public class InputSanitizationService
    {
        private readonly ILogger<InputSanitizationService> _logger;

        // Padrões perigosos para detecção
        private static readonly Regex _sqlInjectionPattern = new(
            @"(\b(ALTER|CREATE|DELETE|DROP|EXEC(UTE)?|INSERT( +INTO)?|MERGE|SELECT|UPDATE|UNION( +ALL)?)\b)|" +
            @"(\b(AND|OR)\b.{1,6}?(=|>|<|\!|\||&))|" +
            @"(\b(CHAR|NCHAR|VARCHAR|NVARCHAR)\s*\(\s*\d+\s*\))|" +
            @"(\b(COUNT|SUM|AVG|MIN|MAX)\s*\()|" +
            @"(\*\s*(FROM|WHERE))|" +
            @"(\bUNION\b.{1,20}\bSELECT\b)|" +
            @"(\b(EXEC|EXECUTE)\b.{1,20}\b(SP_|XP_))|" +
            @"(\b(CAST|CONVERT)\s*\()",
            RegexOptions.IgnoreCase | RegexOptions.Compiled
        );

        private static readonly Regex _xssPattern = new(
            @"<\s*script[^>]*>.*?</\s*script\s*>|" +
            @"<\s*iframe[^>]*>.*?</\s*iframe\s*>|" +
            @"<\s*object[^>]*>.*?</\s*object\s*>|" +
            @"<\s*embed[^>]*>.*?</\s*embed\s*>|" +
            @"<\s*link[^>]*>|" +
            @"<\s*meta[^>]*>|" +
            @"javascript\s*:|" +
            @"vbscript\s*:|" +
            @"data\s*:|" +
            @"on\w+\s*=|" +
            @"expression\s*\(|" +
            @"url\s*\(|" +
            @"@import|" +
            @"<\s*style[^>]*>.*?</\s*style\s*>",
            RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline
        );

        private static readonly Regex _pathTraversalPattern = new(
            @"(\.\./|\.\.\\|%2e%2e%2f|%2e%2e%5c|\.\.%2f|\.\.%5c)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled
        );

        private static readonly Regex _commandInjectionPattern = new(
            @"(\b(cmd|command|exec|system|shell|powershell|bash|sh|zsh)\b)|" +
            @"(\||&|;|`|\$\(|\$\{)|" +
            @"(\b(rm|del|format|fdisk|mkfs)\b)|" +
            @"(\b(wget|curl|nc|netcat|telnet|ssh)\b)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled
        );

        // Tags HTML permitidas para conteúdo rico (se necessário)
        private static readonly HashSet<string> _allowedHtmlTags = new(StringComparer.OrdinalIgnoreCase)
        {
            "p", "br", "strong", "b", "em", "i", "u", "ul", "ol", "li", "h1", "h2", "h3", "h4", "h5", "h6"
        };

        // Caracteres especiais que devem ser escapados
        private static readonly Dictionary<char, string> _htmlEscapeMap = new()
        {
            { '<', "&lt;" },
            { '>', "&gt;" },
            { '"', "&quot;" },
            { '\'', "&#x27;" },
            { '&', "&amp;" },
            { '/', "&#x2F;" }
        };

        public InputSanitizationService(ILogger<InputSanitizationService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Sanitiza uma string removendo conteúdo malicioso
        /// </summary>
        public string SanitizeInput(string input, SanitizationOptions options = null)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            options ??= new SanitizationOptions();
            var sanitized = input;

            try
            {
                // 1. Detectar e bloquear padrões maliciosos
                if (options.DetectMaliciousPatterns && DetectMaliciousContent(sanitized))
                {
                    _logger.LogWarning("Conteúdo malicioso detectado e bloqueado: {Input}", 
                        input.Length > 100 ? input.Substring(0, 100) + "..." : input);
                    return options.ThrowOnMalicious ? 
                        throw new SecurityException("Conteúdo malicioso detectado") : 
                        string.Empty;
                }

                // 2. Remover caracteres de controle
                if (options.RemoveControlCharacters)
                {
                    sanitized = RemoveControlCharacters(sanitized);
                }

                // 3. Escapar HTML
                if (options.EscapeHtml)
                {
                    sanitized = EscapeHtml(sanitized);
                }

                // 4. Remover tags HTML (exceto permitidas)
                if (options.StripHtml)
                {
                    sanitized = StripHtml(sanitized, options.AllowedHtmlTags);
                }

                // 5. Normalizar espaços em branco
                if (options.NormalizeWhitespace)
                {
                    sanitized = NormalizeWhitespace(sanitized);
                }

                // 6. Truncar se necessário
                if (options.MaxLength > 0 && sanitized.Length > options.MaxLength)
                {
                    sanitized = sanitized.Substring(0, options.MaxLength);
                }

                // 7. Validar caracteres permitidos
                if (options.AllowedCharactersPattern != null)
                {
                    sanitized = Regex.Replace(sanitized, options.AllowedCharactersPattern, "", RegexOptions.Compiled);
                }

                return sanitized;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante sanitização do input");
                return options.ThrowOnError ? throw : string.Empty;
            }
        }

        /// <summary>
        /// Sanitiza múltiplas strings de uma vez
        /// </summary>
        public Dictionary<string, string> SanitizeInputs(Dictionary<string, string> inputs, SanitizationOptions options = null)
        {
            var result = new Dictionary<string, string>();
            
            foreach (var kvp in inputs)
            {
                result[kvp.Key] = SanitizeInput(kvp.Value, options);
            }

            return result;
        }

        /// <summary>
        /// Valida se um input é seguro sem modificá-lo
        /// </summary>
        public ValidationResult ValidateInput(string input, SanitizationOptions options = null)
        {
            if (string.IsNullOrEmpty(input))
                return new ValidationResult { IsValid = true };

            options ??= new SanitizationOptions();
            var result = new ValidationResult { IsValid = true };

            // Verificar padrões maliciosos
            if (options.DetectMaliciousPatterns && DetectMaliciousContent(input))
            {
                result.IsValid = false;
                result.Errors.Add("Conteúdo malicioso detectado");
            }

            // Verificar tamanho
            if (options.MaxLength > 0 && input.Length > options.MaxLength)
            {
                result.IsValid = false;
                result.Errors.Add($"Texto muito longo. Máximo permitido: {options.MaxLength} caracteres");
            }

            // Verificar caracteres permitidos
            if (options.AllowedCharactersPattern != null)
            {
                var invalidChars = Regex.Matches(input, options.AllowedCharactersPattern);
                if (invalidChars.Count > 0)
                {
                    result.IsValid = false;
                    result.Errors.Add("Contém caracteres não permitidos");
                }
            }

            return result;
        }

        #region Métodos Privados

        private bool DetectMaliciousContent(string input)
        {
            var lowerInput = input.ToLowerInvariant();

            // Verificar SQL Injection
            if (_sqlInjectionPattern.IsMatch(lowerInput))
            {
                _logger.LogWarning("Possível SQL Injection detectado");
                return true;
            }

            // Verificar XSS
            if (_xssPattern.IsMatch(lowerInput))
            {
                _logger.LogWarning("Possível XSS detectado");
                return true;
            }

            // Verificar Path Traversal
            if (_pathTraversalPattern.IsMatch(lowerInput))
            {
                _logger.LogWarning("Possível Path Traversal detectado");
                return true;
            }

            // Verificar Command Injection
            if (_commandInjectionPattern.IsMatch(lowerInput))
            {
                _logger.LogWarning("Possível Command Injection detectado");
                return true;
            }

            return false;
        }

        private string RemoveControlCharacters(string input)
        {
            var sb = new StringBuilder();
            foreach (char c in input)
            {
                // Manter apenas caracteres imprimíveis e alguns caracteres de controle úteis
                if (!char.IsControl(c) || c == '\n' || c == '\r' || c == '\t')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        private string EscapeHtml(string input)
        {
            var sb = new StringBuilder();
            foreach (char c in input)
            {
                if (_htmlEscapeMap.TryGetValue(c, out string escaped))
                {
                    sb.Append(escaped);
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        private string StripHtml(string input, HashSet<string> allowedTags = null)
        {
            allowedTags ??= _allowedHtmlTags;
            
            // Remove todas as tags HTML exceto as permitidas
            var tagPattern = @"<\s*/?([a-zA-Z][a-zA-Z0-9]*)\b[^>]*>";
            
            return Regex.Replace(input, tagPattern, match =>
            {
                var tagName = match.Groups[1].Value.ToLowerInvariant();
                return allowedTags.Contains(tagName) ? match.Value : "";
            }, RegexOptions.IgnoreCase);
        }

        private string NormalizeWhitespace(string input)
        {
            // Substituir múltiplos espaços por um único espaço
            var normalized = Regex.Replace(input, @"\s+", " ", RegexOptions.Compiled);
            return normalized.Trim();
        }

        #endregion
    }

    /// <summary>
    /// Opções para configurar a sanitização
    /// </summary>
    public class SanitizationOptions
    {
        public bool DetectMaliciousPatterns { get; set; } = true;
        public bool RemoveControlCharacters { get; set; } = true;
        public bool EscapeHtml { get; set; } = true;
        public bool StripHtml { get; set; } = false;
        public bool NormalizeWhitespace { get; set; } = true;
        public bool ThrowOnMalicious { get; set; } = false;
        public bool ThrowOnError { get; set; } = false;
        public int MaxLength { get; set; } = 0; // 0 = sem limite
        public string AllowedCharactersPattern { get; set; } = null;
        public HashSet<string> AllowedHtmlTags { get; set; } = null;
    }

    /// <summary>
    /// Resultado da validação de input
    /// </summary>
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
    }

    /// <summary>
    /// Exceção para conteúdo malicioso detectado
    /// </summary>
    public class SecurityException : Exception
    {
        public SecurityException(string message) : base(message) { }
        public SecurityException(string message, Exception innerException) : base(message, innerException) { }
    }
}

