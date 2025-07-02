using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text;

namespace AcervoEducacional.Application.Services
{
    /// <summary>
    /// Serviço especializado para validação avançada de arquivos
    /// Implementa verificação de assinatura, conteúdo e proteção contra malware básica
    /// </summary>
    public class FileValidationService
    {
        private readonly ILogger<FileValidationService> _logger;

        // Assinaturas de arquivo (Magic Numbers) para validação de conteúdo real
        private static readonly Dictionary<string, List<byte[]>> _fileSignatures = new()
        {
            // Documentos PDF
            { ".pdf", new List<byte[]> { new byte[] { 0x25, 0x50, 0x44, 0x46 } } }, // %PDF

            // Imagens
            { ".jpg", new List<byte[]> { 
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 }, // JPEG JFIF
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 }, // JPEG EXIF
                new byte[] { 0xFF, 0xD8, 0xFF, 0xDB }  // JPEG
            }},
            { ".jpeg", new List<byte[]> { 
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 },
                new byte[] { 0xFF, 0xD8, 0xFF, 0xDB }
            }},
            { ".png", new List<byte[]> { new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A } } },
            { ".gif", new List<byte[]> { 
                new byte[] { 0x47, 0x49, 0x46, 0x38, 0x37, 0x61 }, // GIF87a
                new byte[] { 0x47, 0x49, 0x46, 0x38, 0x39, 0x61 }  // GIF89a
            }},
            { ".webp", new List<byte[]> { new byte[] { 0x52, 0x49, 0x46, 0x46 } } }, // RIFF (WebP)

            // Documentos Office
            { ".docx", new List<byte[]> { new byte[] { 0x50, 0x4B, 0x03, 0x04 } } }, // ZIP (DOCX é ZIP)
            { ".xlsx", new List<byte[]> { new byte[] { 0x50, 0x4B, 0x03, 0x04 } } }, // ZIP (XLSX é ZIP)
            { ".pptx", new List<byte[]> { new byte[] { 0x50, 0x4B, 0x03, 0x04 } } }, // ZIP (PPTX é ZIP)
            { ".doc", new List<byte[]> { new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 } } }, // OLE
            { ".xls", new List<byte[]> { new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 } } }, // OLE
            { ".ppt", new List<byte[]> { new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 } } }, // OLE

            // Vídeos
            { ".mp4", new List<byte[]> { 
                new byte[] { 0x00, 0x00, 0x00, 0x18, 0x66, 0x74, 0x79, 0x70 }, // ftyp
                new byte[] { 0x00, 0x00, 0x00, 0x20, 0x66, 0x74, 0x79, 0x70 }
            }},
            { ".avi", new List<byte[]> { new byte[] { 0x52, 0x49, 0x46, 0x46 } } }, // RIFF
            { ".mov", new List<byte[]> { new byte[] { 0x00, 0x00, 0x00, 0x14, 0x66, 0x74, 0x79, 0x70 } } },

            // Áudios
            { ".mp3", new List<byte[]> { 
                new byte[] { 0xFF, 0xFB }, // MP3
                new byte[] { 0x49, 0x44, 0x33 } // ID3
            }},
            { ".wav", new List<byte[]> { new byte[] { 0x52, 0x49, 0x46, 0x46 } } }, // RIFF

            // Texto
            { ".txt", new List<byte[]>() }, // Texto pode ter qualquer conteúdo
            { ".csv", new List<byte[]>() }  // CSV pode ter qualquer conteúdo
        };

        // Extensões perigosas que devem ser sempre bloqueadas
        private static readonly HashSet<string> _extensoesProibidas = new(StringComparer.OrdinalIgnoreCase)
        {
            ".exe", ".bat", ".cmd", ".com", ".scr", ".pif", ".vbs", ".js", ".jar",
            ".msi", ".dll", ".sys", ".drv", ".ocx", ".cpl", ".inf", ".reg",
            ".ps1", ".psm1", ".psd1", ".ps1xml", ".psc1", ".pssc",
            ".sh", ".bash", ".zsh", ".fish", ".csh", ".tcsh",
            ".php", ".asp", ".aspx", ".jsp", ".py", ".rb", ".pl"
        };

        // Padrões suspeitos no conteúdo do arquivo
        private static readonly string[] _padroesmaliciosos = new[]
        {
            "eval(", "exec(", "system(", "shell_exec(", "passthru(",
            "base64_decode(", "gzinflate(", "str_rot13(",
            "<script", "javascript:", "vbscript:",
            "cmd.exe", "powershell", "/bin/sh", "/bin/bash",
            "CreateObject(", "WScript.Shell", "Shell.Application"
        };

        public FileValidationService(ILogger<FileValidationService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Valida um arquivo de forma abrangente
        /// </summary>
        public async Task<FileValidationResult> ValidateFileAsync(IFormFile file)
        {
            var result = new FileValidationResult { IsValid = true };

            try
            {
                // 1. Validações básicas
                if (!ValidateBasicProperties(file, result))
                    return result;

                // 2. Validar extensão
                if (!ValidateExtension(file.FileName, result))
                    return result;

                // 3. Validar tamanho
                if (!ValidateSize(file.Length, result))
                    return result;

                // 4. Validar assinatura do arquivo (magic numbers)
                if (!await ValidateFileSignatureAsync(file, result))
                    return result;

                // 5. Verificar conteúdo malicioso
                if (!await ValidateMaliciousContentAsync(file, result))
                    return result;

                _logger.LogInformation("Arquivo {FileName} validado com sucesso", file.FileName);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante validação do arquivo {FileName}", file.FileName);
                result.IsValid = false;
                result.ErrorMessage = "Erro interno durante validação do arquivo";
                return result;
            }
        }

        private bool ValidateBasicProperties(IFormFile file, FileValidationResult result)
        {
            if (file == null)
            {
                result.IsValid = false;
                result.ErrorMessage = "Arquivo não fornecido";
                return false;
            }

            if (string.IsNullOrWhiteSpace(file.FileName))
            {
                result.IsValid = false;
                result.ErrorMessage = "Nome do arquivo é obrigatório";
                return false;
            }

            if (file.Length == 0)
            {
                result.IsValid = false;
                result.ErrorMessage = "Arquivo está vazio";
                return false;
            }

            return true;
        }

        private bool ValidateExtension(string fileName, FileValidationResult result)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(extension))
            {
                result.IsValid = false;
                result.ErrorMessage = "Arquivo deve ter uma extensão válida";
                return false;
            }

            if (_extensoesProibidas.Contains(extension))
            {
                result.IsValid = false;
                result.ErrorMessage = $"Tipo de arquivo '{extension}' não é permitido por motivos de segurança";
                _logger.LogWarning("Tentativa de upload de arquivo com extensão proibida: {Extension}", extension);
                return false;
            }

            if (!_fileSignatures.ContainsKey(extension))
            {
                result.IsValid = false;
                result.ErrorMessage = $"Tipo de arquivo '{extension}' não é suportado";
                return false;
            }

            return true;
        }

        private bool ValidateSize(long fileSize, FileValidationResult result)
        {
            const long maxSize = 500 * 1024 * 1024; // 500MB

            if (fileSize > maxSize)
            {
                result.IsValid = false;
                result.ErrorMessage = $"Arquivo muito grande. Tamanho máximo permitido: {FormatFileSize(maxSize)}";
                return false;
            }

            return true;
        }

        private async Task<bool> ValidateFileSignatureAsync(IFormFile file, FileValidationResult result)
        {
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var expectedSignatures = _fileSignatures[extension];

            // Se não há assinaturas definidas (como .txt, .csv), pular validação
            if (!expectedSignatures.Any())
                return true;

            try
            {
                using var stream = file.OpenReadStream();
                var buffer = new byte[32]; // Ler primeiros 32 bytes
                var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                if (bytesRead == 0)
                {
                    result.IsValid = false;
                    result.ErrorMessage = "Não foi possível ler o conteúdo do arquivo";
                    return false;
                }

                // Verificar se alguma assinatura esperada corresponde
                var isValidSignature = expectedSignatures.Any(signature =>
                    signature.Length <= bytesRead &&
                    buffer.Take(signature.Length).SequenceEqual(signature));

                if (!isValidSignature)
                {
                    result.IsValid = false;
                    result.ErrorMessage = $"O conteúdo do arquivo não corresponde ao tipo '{extension}' declarado";
                    _logger.LogWarning("Arquivo {FileName} com assinatura inválida para extensão {Extension}", 
                        file.FileName, extension);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar assinatura do arquivo {FileName}", file.FileName);
                result.IsValid = false;
                result.ErrorMessage = "Erro ao validar conteúdo do arquivo";
                return false;
            }
        }

        private async Task<bool> ValidateMaliciousContentAsync(IFormFile file, FileValidationResult result)
        {
            try
            {
                using var stream = file.OpenReadStream();
                using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);

                // Ler uma amostra do arquivo (primeiros 64KB)
                var buffer = new char[65536];
                var charsRead = await reader.ReadAsync(buffer, 0, buffer.Length);
                var content = new string(buffer, 0, charsRead).ToLowerInvariant();

                // Verificar padrões maliciosos
                foreach (var pattern in _padroesmaliciosos)
                {
                    if (content.Contains(pattern.ToLowerInvariant()))
                    {
                        result.IsValid = false;
                        result.ErrorMessage = "Arquivo contém conteúdo potencialmente malicioso";
                        _logger.LogWarning("Arquivo {FileName} contém padrão suspeito: {Pattern}", 
                            file.FileName, pattern);
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                // Se não conseguir ler como texto, provavelmente é binário - ok para imagens/vídeos
                _logger.LogDebug("Não foi possível ler arquivo {FileName} como texto: {Error}", 
                    file.FileName, ex.Message);
                return true;
            }
        }

        private static string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }

    /// <summary>
    /// Resultado da validação de arquivo
    /// </summary>
    public class FileValidationResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public List<string> Warnings { get; set; } = new();
    }
}

