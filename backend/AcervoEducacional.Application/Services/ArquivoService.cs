using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using AcervoEducacional.Application.DTOs.Arquivo;
using AcervoEducacional.Application.DTOs.Common;
using AcervoEducacional.Application.Interfaces;
using AcervoEducacional.Domain.Entities;
using AcervoEducacional.Domain.Enums;
using AcervoEducacional.Domain.Interfaces;

namespace AcervoEducacional.Application.Services;

public class ArquivoService : IArquivoService
{
    private readonly IArquivoRepository _arquivoRepository;
    private readonly ICursoRepository _cursoRepository;
    private readonly IAwsS3Service _s3Service;
    private readonly ILogger<ArquivoService> _logger;
    
    // Configurações de upload
    private readonly string[] _tiposPermitidos = {
        // Documentos
        "application/pdf", "application/msword", "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "application/vnd.ms-excel", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        "application/vnd.ms-powerpoint", "application/vnd.openxmlformats-officedocument.presentationml.presentation",
        "text/plain", "text/csv",
        // Imagens
        "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp", "image/svg+xml",
        // Vídeos
        "video/mp4", "video/avi", "video/mov", "video/wmv", "video/flv", "video/webm",
        // Áudios
        "audio/mp3", "audio/wav", "audio/ogg", "audio/m4a", "audio/aac"
    };
    
    private const long _tamanhoMaximo = 500 * 1024 * 1024; // 500MB

    public ArquivoService(
        IArquivoRepository arquivoRepository,
        ICursoRepository cursoRepository,
        IAwsS3Service s3Service,
        ILogger<ArquivoService> logger)
    {
        _arquivoRepository = arquivoRepository;
        _cursoRepository = cursoRepository;
        _s3Service = s3Service;
        _logger = logger;
    }

    public async Task<ApiResponse<ArquivoResponseDto>> GetByIdAsync(int id)
    {
        try
        {
            var arquivo = await _arquivoRepository.GetByIdAsync(id);
            if (arquivo == null)
            {
                return ApiResponse<ArquivoResponseDto>.Error("Arquivo não encontrado");
            }

            var response = await MapToResponseDto(arquivo);
            return ApiResponse<ArquivoResponseDto>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar arquivo {Id}", id);
            return ApiResponse<ArquivoResponseDto>.Error("Erro interno do servidor");
        }
    }

    public async Task<ApiResponse<List<ArquivosPorCategoriaDto>>> GetByCursoAsync(int cursoId)
    {
        try
        {
            var arquivos = await _arquivoRepository.GetByCursoIdAsync(cursoId);
            
            var arquivosPorCategoria = arquivos
                .GroupBy(a => a.Categoria)
                .Select(g => new ArquivosPorCategoriaDto
                {
                    Categoria = g.Key,
                    Arquivos = g.Select(MapToListDto).ToList(),
                    Total = g.Count(),
                    TamanhoTotal = g.Sum(a => a.Tamanho),
                    TamanhoTotalFormatado = FormatFileSize(g.Sum(a => a.Tamanho))
                })
                .ToList();

            return ApiResponse<List<ArquivosPorCategoriaDto>>.Success(arquivosPorCategoria);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar arquivos do curso {CursoId}", cursoId);
            return ApiResponse<List<ArquivosPorCategoriaDto>>.Error("Erro interno do servidor");
        }
    }

    public async Task<ApiResponse<PagedResponse<ArquivoListDto>>> GetAllAsync(ArquivoFilterDto filter)
    {
        try
        {
            var (arquivos, total) = await _arquivoRepository.GetPagedAsync(
                filter.Page, 
                filter.PageSize, 
                filter.Search,
                filter.CursoId,
                filter.Categoria,
                filter.TipoMime,
                filter.IsPublico,
                filter.CriadoApartirDe,
                filter.CriadoAte,
                filter.TamanhoMin,
                filter.TamanhoMax,
                filter.SortBy,
                filter.SortDirection
            );

            var arquivosDto = arquivos.Select(MapToListDto).ToList();
            
            var pagedResponse = new PagedResponse<ArquivoListDto>
            {
                Data = arquivosDto,
                Page = filter.Page,
                PageSize = filter.PageSize,
                TotalCount = total,
                TotalPages = (int)Math.Ceiling((double)total / filter.PageSize)
            };

            return ApiResponse<PagedResponse<ArquivoListDto>>.Success(pagedResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar arquivos com filtros");
            return ApiResponse<PagedResponse<ArquivoListDto>>.Error("Erro interno do servidor");
        }
    }

    public async Task<ApiResponse<ArquivoResponseDto>> UploadAsync(int cursoId, CreateArquivoDto dto, int criadoPor)
    {
        try
        {
            // Validar curso
            var curso = await _cursoRepository.GetByIdAsync(cursoId);
            if (curso == null)
            {
                return ApiResponse<ArquivoResponseDto>.Error("Curso não encontrado");
            }

            // Validar arquivo
            var validationResult = ValidateFile(dto.Arquivo);
            if (!validationResult.IsSuccess)
            {
                return ApiResponse<ArquivoResponseDto>.Error(validationResult.Message);
            }

            // Calcular hash SHA256 para detectar duplicatas
            var fileHash = await CalculateFileHashAsync(dto.Arquivo.OpenReadStream());
            var existingFile = await _arquivoRepository.GetByHashAsync(fileHash);
            
            if (existingFile != null)
            {
                _logger.LogWarning("Tentativa de upload de arquivo duplicado. Hash: {Hash}", fileHash);
                return ApiResponse<ArquivoResponseDto>.Error("Arquivo já existe no sistema");
            }

            // Gerar nome único com GUID
            var fileExtension = Path.GetExtension(dto.Arquivo.FileName);
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            var s3Key = $"cursos/{curso.Codigo}/{uniqueFileName}";

            // Upload para S3
            var s3Url = await _s3Service.UploadFileAsync(
                dto.Arquivo.OpenReadStream(), 
                s3Key, 
                dto.Arquivo.ContentType
            );

            // Criar entidade
            var arquivo = new Arquivo
            {
                CursoId = cursoId,
                Nome = dto.Arquivo.FileName,
                NomeArmazenamento = uniqueFileName,
                Categoria = dto.Categoria,
                TipoMime = dto.Arquivo.ContentType,
                Tamanho = dto.Arquivo.Length,
                UrlS3 = s3Url,
                IsPublico = dto.IsPublico,
                DataExpiracao = dto.DataExpiracao,
                DominiosPermitidos = dto.DominiosPermitidos,
                BloqueiosAtivos = dto.BloqueiosAtivos ?? new Dictionary<string, object>(),
                Metadados = CreateMetadata(dto.Arquivo, fileHash),
                CriadoPor = criadoPor,
                CriadoEm = DateTime.UtcNow
            };

            // Salvar no banco
            await _arquivoRepository.AddAsync(arquivo);

            _logger.LogInformation("Arquivo {Nome} uploaded com sucesso para o curso {CursoId}", 
                arquivo.Nome, cursoId);

            var response = await MapToResponseDto(arquivo);
            return ApiResponse<ArquivoResponseDto>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao fazer upload do arquivo para o curso {CursoId}", cursoId);
            return ApiResponse<ArquivoResponseDto>.Error("Erro interno do servidor");
        }
    }

    public async Task<ApiResponse<ArquivoResponseDto>> UpdateAsync(int id, UpdateArquivoDto dto, int atualizadoPor)
    {
        try
        {
            var arquivo = await _arquivoRepository.GetByIdAsync(id);
            if (arquivo == null)
            {
                return ApiResponse<ArquivoResponseDto>.Error("Arquivo não encontrado");
            }

            // Atualizar propriedades
            arquivo.Nome = dto.Nome;
            arquivo.Categoria = dto.Categoria;
            arquivo.IsPublico = dto.IsPublico;
            arquivo.DataExpiracao = dto.DataExpiracao;
            arquivo.DominiosPermitidos = dto.DominiosPermitidos;
            arquivo.BloqueiosAtivos = dto.BloqueiosAtivos ?? new Dictionary<string, object>();
            arquivo.Metadados = dto.Metadados ?? arquivo.Metadados;
            arquivo.AtualizadoPor = atualizadoPor;
            arquivo.AtualizadoEm = DateTime.UtcNow;

            await _arquivoRepository.UpdateAsync(arquivo);

            _logger.LogInformation("Arquivo {Id} atualizado com sucesso", id);

            var response = await MapToResponseDto(arquivo);
            return ApiResponse<ArquivoResponseDto>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar arquivo {Id}", id);
            return ApiResponse<ArquivoResponseDto>.Error("Erro interno do servidor");
        }
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id, int deletadoPor)
    {
        try
        {
            var arquivo = await _arquivoRepository.GetByIdAsync(id);
            if (arquivo == null)
            {
                return ApiResponse<bool>.Error("Arquivo não encontrado");
            }

            // Deletar do S3
            var s3Key = ExtractS3KeyFromUrl(arquivo.UrlS3);
            await _s3Service.DeleteFileAsync(s3Key);

            // Soft delete no banco
            arquivo.DeletadoPor = deletadoPor;
            arquivo.DeletadoEm = DateTime.UtcNow;
            await _arquivoRepository.UpdateAsync(arquivo);

            _logger.LogInformation("Arquivo {Id} deletado com sucesso", id);

            return ApiResponse<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao deletar arquivo {Id}", id);
            return ApiResponse<bool>.Error("Erro interno do servidor");
        }
    }

    public async Task<ApiResponse<DownloadArquivoResponseDto>> GetDownloadUrlAsync(int id, int usuarioId)
    {
        try
        {
            var arquivo = await _arquivoRepository.GetByIdAsync(id);
            if (arquivo == null)
            {
                return ApiResponse<DownloadArquivoResponseDto>.Error("Arquivo não encontrado");
            }

            // Verificar permissões e expiração
            if (!arquivo.IsPublico && !await HasPermissionToAccess(arquivo, usuarioId))
            {
                return ApiResponse<DownloadArquivoResponseDto>.Error("Acesso negado");
            }

            if (arquivo.DataExpiracao.HasValue && arquivo.DataExpiracao.Value < DateTime.UtcNow)
            {
                return ApiResponse<DownloadArquivoResponseDto>.Error("Arquivo expirado");
            }

            // Gerar URL pré-assinada (1 hora de validade)
            var s3Key = ExtractS3KeyFromUrl(arquivo.UrlS3);
            var downloadUrl = await _s3Service.GetPresignedUrlAsync(s3Key, TimeSpan.FromHours(1));

            // Registrar log de acesso
            await LogFileAccess(arquivo.Id, usuarioId, "download");

            var response = new DownloadArquivoResponseDto
            {
                DownloadUrl = downloadUrl,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                Nome = arquivo.Nome,
                Tamanho = arquivo.Tamanho,
                TipoMime = arquivo.TipoMime
            };

            return ApiResponse<DownloadArquivoResponseDto>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar URL de download para arquivo {Id}", id);
            return ApiResponse<DownloadArquivoResponseDto>.Error("Erro interno do servidor");
        }
    }

    public async Task<ApiResponse<ShareArquivoResponseDto>> ShareAsync(int id, ShareArquivoDto dto, int usuarioId)
    {
        try
        {
            var arquivo = await _arquivoRepository.GetByIdAsync(id);
            if (arquivo == null)
            {
                return ApiResponse<ShareArquivoResponseDto>.Error("Arquivo não encontrado");
            }

            // Atualizar configurações de compartilhamento
            arquivo.IsPublico = dto.IsPublico;
            arquivo.DataExpiracao = dto.DataExpiracao;
            arquivo.DominiosPermitidos = dto.DominiosPermitidos;
            arquivo.BloqueiosAtivos = dto.BloqueiosAtivos ?? new Dictionary<string, object>();
            arquivo.AtualizadoPor = usuarioId;
            arquivo.AtualizadoEm = DateTime.UtcNow;

            await _arquivoRepository.UpdateAsync(arquivo);

            // Gerar token único de 32 bytes
            var shareToken = GenerateShareToken();
            
            // URL de compartilhamento
            var shareUrl = $"https://rhlivzrm.manus.space/share/{shareToken}";

            // Gerar código embed se solicitado
            string? embedCode = null;
            if (dto.GerarCodigoEmbed)
            {
                embedCode = GenerateEmbedCode(arquivo, shareUrl);
            }

            // Registrar log de compartilhamento
            await LogFileAccess(arquivo.Id, usuarioId, "share");

            var response = new ShareArquivoResponseDto
            {
                ShareUrl = shareUrl,
                EmbedCode = embedCode,
                ExpiresAt = dto.DataExpiracao,
                IsPublico = dto.IsPublico,
                DominiosPermitidos = dto.DominiosPermitidos,
                Token = shareToken
            };

            _logger.LogInformation("Arquivo {Id} compartilhado com sucesso", id);

            return ApiResponse<ShareArquivoResponseDto>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao compartilhar arquivo {Id}", id);
            return ApiResponse<ShareArquivoResponseDto>.Error("Erro interno do servidor");
        }
    }

    #region Private Methods

    private (bool IsSuccess, string Message) ValidateFile(Microsoft.AspNetCore.Http.IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return (false, "Arquivo é obrigatório");
        }

        if (file.Length > _tamanhoMaximo)
        {
            return (false, $"Arquivo muito grande. Tamanho máximo: {FormatFileSize(_tamanhoMaximo)}");
        }

        if (!_tiposPermitidos.Contains(file.ContentType.ToLower()))
        {
            return (false, "Tipo de arquivo não permitido");
        }

        return (true, string.Empty);
    }

    private async Task<string> CalculateFileHashAsync(Stream stream)
    {
        using var sha256 = SHA256.Create();
        var hash = await sha256.ComputeHashAsync(stream);
        return Convert.ToBase64String(hash);
    }

    private Dictionary<string, object> CreateMetadata(Microsoft.AspNetCore.Http.IFormFile file, string hash)
    {
        return new Dictionary<string, object>
        {
            ["originalFileName"] = file.FileName,
            ["contentType"] = file.ContentType,
            ["uploadedAt"] = DateTime.UtcNow,
            ["sha256Hash"] = hash,
            ["fileSize"] = file.Length,
            ["fileSizeFormatted"] = FormatFileSize(file.Length)
        };
    }

    private async Task<ArquivoResponseDto> MapToResponseDto(Arquivo arquivo)
    {
        var dto = new ArquivoResponseDto
        {
            Id = arquivo.Id,
            CursoId = arquivo.CursoId,
            Nome = arquivo.Nome,
            Categoria = arquivo.Categoria,
            TipoMime = arquivo.TipoMime,
            Tamanho = arquivo.Tamanho,
            TamanhoFormatado = FormatFileSize(arquivo.Tamanho),
            IsPublico = arquivo.IsPublico,
            DataExpiracao = arquivo.DataExpiracao,
            DominiosPermitidos = arquivo.DominiosPermitidos,
            BloqueiosAtivos = arquivo.BloqueiosAtivos,
            Metadados = arquivo.Metadados,
            CriadoEm = arquivo.CriadoEm,
            AtualizadoEm = arquivo.AtualizadoEm
        };

        // Gerar URLs se o arquivo for público ou não expirado
        if (arquivo.IsPublico && (!arquivo.DataExpiracao.HasValue || arquivo.DataExpiracao.Value > DateTime.UtcNow))
        {
            var s3Key = ExtractS3KeyFromUrl(arquivo.UrlS3);
            dto.UrlDownload = await _s3Service.GetPresignedUrlAsync(s3Key, TimeSpan.FromHours(1));
            dto.UrlVisualizacao = $"https://rhlivzrm.manus.space/view/{arquivo.Id}";
            dto.UrlCompartilhamento = $"https://rhlivzrm.manus.space/share/{arquivo.Id}";
        }

        return dto;
    }

    private ArquivoListDto MapToListDto(Arquivo arquivo)
    {
        return new ArquivoListDto
        {
            Id = arquivo.Id,
            Nome = arquivo.Nome,
            Categoria = arquivo.Categoria,
            TipoMime = arquivo.TipoMime,
            Tamanho = arquivo.Tamanho,
            TamanhoFormatado = FormatFileSize(arquivo.Tamanho),
            IsPublico = arquivo.IsPublico,
            CriadoEm = arquivo.CriadoEm,
            AtualizadoEm = arquivo.AtualizadoEm
        };
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

    private string ExtractS3KeyFromUrl(string s3Url)
    {
        var uri = new Uri(s3Url);
        return uri.AbsolutePath.TrimStart('/');
    }

    private async Task<bool> HasPermissionToAccess(Arquivo arquivo, int usuarioId)
    {
        // Implementar lógica de permissões baseada no curso e usuário
        // Por enquanto, retorna true para usuários autenticados
        return usuarioId > 0;
    }

    private async Task LogFileAccess(int arquivoId, int usuarioId, string acao)
    {
        // Implementar log de acesso
        _logger.LogInformation("Arquivo {ArquivoId} acessado por usuário {UsuarioId} - Ação: {Acao}", 
            arquivoId, usuarioId, acao);
    }

    private string GenerateShareToken()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[32];
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
    }

    private string GenerateEmbedCode(Arquivo arquivo, string shareUrl)
    {
        var embedUrl = shareUrl.Replace("/share/", "/embed/");
        
        return arquivo.TipoMime.StartsWith("video/") 
            ? $@"<iframe src=""{embedUrl}"" width=""640"" height=""360"" frameborder=""0"" allowfullscreen></iframe>"
            : $@"<iframe src=""{embedUrl}"" width=""100%"" height=""600"" frameborder=""0""></iframe>";
    }

    #endregion
}

