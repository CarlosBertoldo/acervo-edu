using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using AcervoEducacional.Domain.Enums;

namespace AcervoEducacional.Application.Services;

public class FileValidationService
{
    private readonly ILogger<FileValidationService> _logger;
    private readonly Dictionary<string, List<string>> _allowedExtensions = new()
    {
        { "image", new List<string> { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" } },
        { "video", new List<string> { ".mp4", ".avi", ".mov", ".wmv", ".flv", ".webm" } },
        { "audio", new List<string> { ".mp3", ".wav", ".flac", ".aac", ".ogg", ".wma" } },
        { "document", new List<string> { ".pdf", ".doc", ".docx", ".txt", ".rtf", ".odt" } },
        { "presentation", new List<string> { ".ppt", ".pptx", ".odp" } },
        { "spreadsheet", new List<string> { ".xls", ".xlsx", ".csv", ".ods" } },
        { "archive", new List<string> { ".zip", ".rar", ".7z", ".tar", ".gz" } }
    };

    public FileValidationService(ILogger<FileValidationService> logger)
    {
        _logger = logger;
    }

    public bool IsValidFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return false;

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        return _allowedExtensions.Values.Any(extensions => extensions.Contains(extension));
    }

    public CategoriaArquivo GetCategoriaArquivo(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        
        if (_allowedExtensions["image"].Contains(extension))
            return CategoriaArquivo.Imagem;
        if (_allowedExtensions["video"].Contains(extension))
            return CategoriaArquivo.Video;
        if (_allowedExtensions["audio"].Contains(extension))
            return CategoriaArquivo.Audio;
        if (_allowedExtensions["document"].Contains(extension))
            return CategoriaArquivo.Documento;
        if (_allowedExtensions["presentation"].Contains(extension))
            return CategoriaArquivo.Apresentacao;
        if (_allowedExtensions["spreadsheet"].Contains(extension))
            return CategoriaArquivo.Planilha;
        if (_allowedExtensions["archive"].Contains(extension))
            return CategoriaArquivo.Arquivo;
            
        return CategoriaArquivo.Documento;
    }

    public long GetMaxFileSize(CategoriaArquivo categoria)
    {
        return categoria switch
        {
            CategoriaArquivo.Video => 100 * 1024 * 1024, // 100MB
            CategoriaArquivo.Audio => 50 * 1024 * 1024,  // 50MB
            CategoriaArquivo.Imagem => 10 * 1024 * 1024, // 10MB
            CategoriaArquivo.Documento => 20 * 1024 * 1024, // 20MB
            CategoriaArquivo.Apresentacao => 30 * 1024 * 1024, // 30MB
            CategoriaArquivo.Planilha => 15 * 1024 * 1024, // 15MB
            CategoriaArquivo.Arquivo => 50 * 1024 * 1024, // 50MB
            _ => 10 * 1024 * 1024 // 10MB default
        };
    }

    public bool ValidateFileSize(IFormFile file, CategoriaArquivo categoria)
    {
        var maxSize = GetMaxFileSize(categoria);
        return file.Length <= maxSize;
    }

    public async Task<(bool IsValid, string ErrorMessage)> ValidateFileAsync(IFormFile file)
    {
        await Task.CompletedTask; // Para manter assinatura async
        
        if (file == null || file.Length == 0)
            return (false, "Arquivo não pode ser vazio");

        if (!IsValidFile(file))
            return (false, "Tipo de arquivo não permitido");

        var categoria = GetCategoriaArquivo(file.FileName);
        if (!ValidateFileSize(file, categoria))
        {
            var maxSize = GetMaxFileSize(categoria);
            return (false, $"Arquivo muito grande. Tamanho máximo permitido: {FormatFileSize(maxSize)}");
        }

        return (true, string.Empty);
    }

    private string FormatFileSize(long bytes)
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