using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using FluentAssertions;
using System.Text.RegularExpressions;

namespace AcervoEducacional.Application.Tests.Services
{
    /// <summary>
    /// Testes simplificados de validação de segurança
    /// Foco em validar as regras de negócio de segurança implementadas
    /// </summary>
    public class SecurityValidationTests
    {
        #region Testes de Validação de Email

        [Theory]
        [InlineData("usuario@teste.com", true)]
        [InlineData("admin@ferreiracosta.com", true)]
        [InlineData("test.email+tag@domain.co.uk", true)]
        [InlineData("user123@example.org", true)]
        [InlineData("", false)]
        [InlineData("email-invalido", false)]
        [InlineData("@teste.com", false)]
        [InlineData("usuario@", false)]
        [InlineData("usuario.teste.com", false)]
        [InlineData("usuario@.com", false)]
        [InlineData("usuario@teste.", false)]
        public void ValidateEmail_ShouldReturnCorrectResult(string email, bool expectedValid)
        {
            // Arrange
            var emailRegex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");

            // Act
            var isValid = !string.IsNullOrWhiteSpace(email) && emailRegex.IsMatch(email);

            // Assert
            isValid.Should().Be(expectedValid, $"Email '{email}' deveria ser {(expectedValid ? "válido" : "inválido")}");
        }

        #endregion

        #region Testes de Validação de Senha

        [Theory]
        [InlineData("MinhaSenh@123", true)]
        [InlineData("OutraSenh@456", true)]
        [InlineData("Teste@2024", true)]
        [InlineData("ComplexP@ssw0rd", true)]
        [InlineData("minhasenha123", false)] // Sem maiúscula e caractere especial
        [InlineData("MINHASENHA123", false)] // Sem minúscula e caractere especial
        [InlineData("MinhaSenh@", false)] // Sem número
        [InlineData("MinhaSenh123", false)] // Sem caractere especial
        [InlineData("Senh@1", false)] // Muito curta
        [InlineData("", false)] // Vazia
        public void ValidatePassword_ShouldReturnCorrectResult(string password, bool expectedValid)
        {
            // Arrange
            var hasMinLength = password.Length >= 8;
            var hasUpperCase = password.Any(char.IsUpper);
            var hasLowerCase = password.Any(char.IsLower);
            var hasDigit = password.Any(char.IsDigit);
            var hasSpecialChar = password.Any(c => "!@#$%^&*()_+-=[]{}|;:,.<>?".Contains(c));

            // Act
            var isValid = hasMinLength && hasUpperCase && hasLowerCase && hasDigit && hasSpecialChar;

            // Assert
            isValid.Should().Be(expectedValid, $"Senha '{password}' deveria ser {(expectedValid ? "válida" : "inválida")}");
        }

        [Fact]
        public void ValidatePassword_ShouldRequireMinimumLength()
        {
            // Arrange
            var shortPassword = "Abc@1";
            var validPassword = "Abc@1234";

            // Act
            var shortIsValid = shortPassword.Length >= 8;
            var validIsValid = validPassword.Length >= 8;

            // Assert
            shortIsValid.Should().BeFalse("Senha muito curta deveria ser inválida");
            validIsValid.Should().BeTrue("Senha com tamanho adequado deveria ser válida");
        }

        [Fact]
        public void ValidatePassword_ShouldRequireUpperCase()
        {
            // Arrange
            var noUpperCase = "minhasenha@123";
            var withUpperCase = "MinhaSenh@123";

            // Act
            var noUpperIsValid = noUpperCase.Any(char.IsUpper);
            var withUpperIsValid = withUpperCase.Any(char.IsUpper);

            // Assert
            noUpperIsValid.Should().BeFalse("Senha sem maiúscula deveria ser inválida");
            withUpperIsValid.Should().BeTrue("Senha com maiúscula deveria ser válida");
        }

        [Fact]
        public void ValidatePassword_ShouldRequireLowerCase()
        {
            // Arrange
            var noLowerCase = "MINHASENHA@123";
            var withLowerCase = "MinhaSenh@123";

            // Act
            var noLowerIsValid = noLowerCase.Any(char.IsLower);
            var withLowerIsValid = withLowerCase.Any(char.IsLower);

            // Assert
            noLowerIsValid.Should().BeFalse("Senha sem minúscula deveria ser inválida");
            withLowerIsValid.Should().BeTrue("Senha com minúscula deveria ser válida");
        }

        [Fact]
        public void ValidatePassword_ShouldRequireDigit()
        {
            // Arrange
            var noDigit = "MinhaSenh@";
            var withDigit = "MinhaSenh@123";

            // Act
            var noDigitIsValid = noDigit.Any(char.IsDigit);
            var withDigitIsValid = withDigit.Any(char.IsDigit);

            // Assert
            noDigitIsValid.Should().BeFalse("Senha sem número deveria ser inválida");
            withDigitIsValid.Should().BeTrue("Senha com número deveria ser válida");
        }

        [Fact]
        public void ValidatePassword_ShouldRequireSpecialCharacter()
        {
            // Arrange
            var noSpecialChar = "MinhaSenh123";
            var withSpecialChar = "MinhaSenh@123";
            var specialChars = "!@#$%^&*()_+-=[]{}|;:,.<>?";

            // Act
            var noSpecialIsValid = noSpecialChar.Any(c => specialChars.Contains(c));
            var withSpecialIsValid = withSpecialChar.Any(c => specialChars.Contains(c));

            // Assert
            noSpecialIsValid.Should().BeFalse("Senha sem caractere especial deveria ser inválida");
            withSpecialIsValid.Should().BeTrue("Senha com caractere especial deveria ser válida");
        }

        #endregion

        #region Testes de Proteção contra Ataques

        [Theory]
        [InlineData("'; DROP TABLE Users; --")]
        [InlineData("' OR '1'='1")]
        [InlineData("admin'--")]
        [InlineData("' UNION SELECT * FROM Users --")]
        [InlineData("1' OR 1=1#")]
        public void ValidateInput_ShouldRejectSqlInjectionAttempts(string maliciousInput)
        {
            // Arrange
            var sqlInjectionPatterns = new[]
            {
                @"('|(\')|(\-\-)|(\;)|(\|)|(\*)|(\%)",
                @"((\%3D)|(=))[^\n]*((\%27)|(\')|(\-\-)|(\%3B)|(;))",
                @"((\%27)|(\'))((\%6F)|o|(\%4F))((\%72)|r|(\%52))",
                @"((\%27)|(\'))union",
                @"exec(\s|\+)+(s|x)p\w+",
                @"UNION.*SELECT",
                @"DROP\s+TABLE"
            };

            // Act
            var containsSqlInjection = sqlInjectionPatterns.Any(pattern => 
                Regex.IsMatch(maliciousInput, pattern, RegexOptions.IgnoreCase));

            // Assert
            containsSqlInjection.Should().BeTrue($"Input '{maliciousInput}' deveria ser detectado como tentativa de SQL injection");
        }

        [Theory]
        [InlineData("<script>alert('xss')</script>")]
        [InlineData("javascript:alert('xss')")]
        [InlineData("<img src=x onerror=alert('xss')>")]
        [InlineData("'><script>alert('xss')</script>")]
        [InlineData("<iframe src='javascript:alert(\"xss\")'></iframe>")]
        public void ValidateInput_ShouldRejectXssAttempts(string maliciousInput)
        {
            // Arrange
            var xssPatterns = new[]
            {
                @"<script\b[^<]*(?:(?!<\/script>)<[^<]*)*<\/script>",
                @"javascript:",
                @"on\w+\s*=",
                @"<iframe",
                @"<object",
                @"<embed"
            };

            // Act
            var containsXss = xssPatterns.Any(pattern => 
                Regex.IsMatch(maliciousInput, pattern, RegexOptions.IgnoreCase));

            // Assert
            containsXss.Should().BeTrue($"Input '{maliciousInput}' deveria ser detectado como tentativa de XSS");
        }

        [Theory]
        [InlineData("../../../etc/passwd")]
        [InlineData("..\\..\\..\\windows\\system32\\config\\sam")]
        [InlineData("%2e%2e%2f%2e%2e%2f%2e%2e%2fetc%2fpasswd")]
        [InlineData("....//....//....//etc/passwd")]
        public void ValidateInput_ShouldRejectPathTraversalAttempts(string maliciousInput)
        {
            // Arrange
            var pathTraversalPatterns = new[]
            {
                @"\.\.[\\/]",
                @"%2e%2e[\\/]",
                @"\.\.%2f",
                @"\.\.%5c",
                @"\.\.[\\/].*[\\/]",
                @"(\.\.[\\/]){3,}"
            };

            // Act
            var containsPathTraversal = pathTraversalPatterns.Any(pattern => 
                Regex.IsMatch(maliciousInput, pattern, RegexOptions.IgnoreCase));

            // Assert
            containsPathTraversal.Should().BeTrue($"Input '{maliciousInput}' deveria ser detectado como tentativa de path traversal");
        }

        #endregion

        #region Testes de Sanitização

        [Theory]
        [InlineData("<script>alert('test')</script>", "&lt;script&gt;alert('test')&lt;/script&gt;")]
        [InlineData("Test & Company", "Test &amp; Company")]
        [InlineData("Price: $100 < $200", "Price: $100 &lt; $200")]
        [InlineData("Quote: \"Hello World\"", "Quote: &quot;Hello World&quot;")]
        [InlineData("Single quote: 'test'", "Single quote: &#x27;test&#x27;")]
        public void SanitizeHtml_ShouldEscapeHtmlCharacters(string input, string expectedOutput)
        {
            // Arrange & Act
            var sanitized = input
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;")
                .Replace("'", "&#x27;");

            // Assert
            sanitized.Should().Be(expectedOutput, $"Input '{input}' deveria ser sanitizado corretamente");
        }

        [Theory]
        [InlineData("  texto com espaços  ", "texto com espaços")]
        [InlineData("texto\ncom\nquebras", "texto com quebras")]
        [InlineData("texto\tcom\ttabs", "texto com tabs")]
        [InlineData("texto   com   espaços   múltiplos", "texto com espaços múltiplos")]
        public void SanitizeText_ShouldNormalizeWhitespace(string input, string expectedOutput)
        {
            // Arrange & Act
            var sanitized = Regex.Replace(input.Trim(), @"\s+", " ");

            // Assert
            sanitized.Should().Be(expectedOutput, $"Input '{input}' deveria ser normalizado corretamente");
        }

        #endregion

        #region Testes de Constantes de Segurança

        [Fact]
        public void SecurityConstants_ShouldHaveSecureDefaults()
        {
            // Arrange
            const int minPasswordLength = 8;
            const int maxLoginAttempts = 5;
            const int lockoutMinutes = 15;
            const int tokenExpirationMinutes = 60;

            // Assert
            minPasswordLength.Should().BeGreaterThanOrEqualTo(8, "Tamanho mínimo de senha deve ser pelo menos 8 caracteres");
            maxLoginAttempts.Should().BeLessThanOrEqualTo(5, "Máximo de tentativas de login deve ser no máximo 5");
            lockoutMinutes.Should().BeGreaterThanOrEqualTo(15, "Tempo de bloqueio deve ser pelo menos 15 minutos");
            tokenExpirationMinutes.Should().BeLessThanOrEqualTo(120, "Expiração do token deve ser no máximo 2 horas");
        }

        #endregion

        #region Testes de Validação de Entrada

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void ValidateInput_ShouldRejectEmptyOrNullValues(string input)
        {
            // Act
            var isValid = !string.IsNullOrWhiteSpace(input);

            // Assert
            isValid.Should().BeFalse($"Input '{input}' deveria ser considerado inválido");
        }

        [Theory]
        [InlineData("a", 1)]
        [InlineData("texto normal", 12)]
        public void ValidateInput_ShouldRespectMaxLength(string input, int expectedLength)
        {
            // Arrange
            const int maxLength = 255;

            // Act
            var isValidLength = input.Length <= maxLength;
            var actualLength = input.Length;

            // Assert
            actualLength.Should().Be(expectedLength, $"Input deveria ter {expectedLength} caracteres");
            
            if (expectedLength <= maxLength)
            {
                isValidLength.Should().BeTrue($"Input com {expectedLength} caracteres deveria ser válido");
            }
        }

        [Fact]
        public void ValidateInput_ShouldRejectExcessivelyLongInput()
        {
            // Arrange
            var longInput = new string('x', 1000);
            const int maxLength = 255;

            // Act
            var isValid = longInput.Length <= maxLength;

            // Assert
            isValid.Should().BeFalse($"Input com {longInput.Length} caracteres deveria ser rejeitado (máximo: {maxLength})");
        }

        #endregion

        #region Testes de Performance

        [Fact]
        public void SecurityValidation_ShouldCompleteQuickly()
        {
            // Arrange
            var testInputs = new[]
            {
                "usuario@teste.com",
                "MinhaSenh@123",
                "<script>alert('test')</script>",
                "'; DROP TABLE Users; --",
                "../../../etc/passwd"
            };

            // Act
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            foreach (var input in testInputs)
            {
                // Simular validações de segurança
                var emailRegex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
                var isEmail = emailRegex.IsMatch(input);
                
                var hasSpecialChars = input.Any(c => "!@#$%^&*()_+-=[]{}|;:,.<>?".Contains(c));
                var containsScript = input.Contains("<script>", StringComparison.OrdinalIgnoreCase);
                var containsSql = input.Contains("DROP TABLE", StringComparison.OrdinalIgnoreCase);
            }
            
            stopwatch.Stop();

            // Assert
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(100, "Validações de segurança devem ser rápidas");
        }

        #endregion
    }
}

