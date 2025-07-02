using AcervoEducacional.Application.DTOs.Auth;
using AcervoEducacional.Application.DTOs.Usuario;
using AcervoEducacional.Application.DTOs.Curso;
using AcervoEducacional.Application.DTOs.Arquivo;
using AcervoEducacional.Application.DTOs.Common;

namespace AcervoEducacional.Application.Interfaces;

public interface ISecurityService
{
    Task<bool> IsRateLimitExceededAsync(string identifier, string action, int maxAttempts = 5, int windowMinutes = 15);
    Task<string> HashPasswordAsync(string password);
    Task<bool> VerifyPasswordAsync(string password, string hash);
    Task<bool> ValidatePasswordStrengthAsync(string password);
    Task<string> GenerateSecureTokenAsync(int length = 32);
    Task<bool> IsIpAddressBlockedAsync(string ipAddress);
    Task BlockIpAddressAsync(string ipAddress, int durationMinutes = 60, string reason = "");
    Task<bool> ValidateEmailFormatAsync(string email);
    Task<bool> IsEmailDomainAllowedAsync(string email);
    Task LogSecurityEventAsync(int? usuarioId, string eventType, string description, string? ipAddress = null, string? userAgent = null);
    Task<bool> DetectSuspiciousActivityAsync(int usuarioId, string ipAddress, string userAgent);
    Task CleanupExpiredDataAsync();
}

public interface IAuthService
{
    Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto request, string ipAddress, string userAgent);
    Task<ApiResponse<RefreshTokenResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request, string ipAddress, string userAgent);
    Task<ApiResponse<bool>> LogoutAsync(string token);
    Task<ApiResponse<bool>> ForgotPasswordAsync(ForgotPasswordRequestDto request);
    Task<ApiResponse<bool>> ResetPasswordAsync(ResetPasswordRequestDto request);
    Task<ApiResponse<bool>> ChangePasswordAsync(int usuarioId, ChangePasswordRequestDto request);
    Task<ApiResponse<bool>> ValidateTokenAsync(string token);
}

public interface IUsuarioService
{
    Task<ApiResponse<UsuarioResponseDto>> GetByIdAsync(int id);
    Task<ApiResponse<PagedResponse<UsuarioListDto>>> GetAllAsync(UsuarioFilterDto filter);
    Task<ApiResponse<UsuarioResponseDto>> CreateAsync(CreateUsuarioDto dto, int criadoPor);
    Task<ApiResponse<UsuarioResponseDto>> UpdateAsync(int id, UpdateUsuarioDto dto, int atualizadoPor);
    Task<ApiResponse<bool>> DeleteAsync(int id, int deletadoPor);
    Task<ApiResponse<UsuarioResponseDto>> GetByEmailAsync(string email);
}

public interface ICursoService
{
    Task<ApiResponse<CursoResponseDto>> GetByIdAsync(int id);
    Task<ApiResponse<PagedResponse<CursoListDto>>> GetAllAsync(CursoFilterDto filter);
    Task<ApiResponse<List<CursoKanbanDto>>> GetKanbanAsync();
    Task<ApiResponse<CursoResponseDto>> CreateAsync(CreateCursoDto dto, int criadoPor);
    Task<ApiResponse<CursoResponseDto>> UpdateAsync(int id, UpdateCursoDto dto, int atualizadoPor);
    Task<ApiResponse<CursoResponseDto>> UpdateStatusAsync(int id, UpdateStatusCursoDto dto, int atualizadoPor);
    Task<ApiResponse<bool>> DeleteAsync(int id, int deletadoPor);
    Task<ApiResponse<DashboardStatsDto>> GetDashboardStatsAsync();
}

public interface IArquivoService
{
    Task<ApiResponse<ArquivoResponseDto>> GetByIdAsync(int id);
    Task<ApiResponse<List<ArquivosPorCategoriaDto>>> GetByCursoAsync(int cursoId);
    Task<ApiResponse<PagedResponse<ArquivoListDto>>> GetAllAsync(ArquivoFilterDto filter);
    Task<ApiResponse<ArquivoResponseDto>> UploadAsync(int cursoId, CreateArquivoDto dto, int criadoPor);
    Task<ApiResponse<ArquivoResponseDto>> UpdateAsync(int id, UpdateArquivoDto dto, int atualizadoPor);
    Task<ApiResponse<bool>> DeleteAsync(int id, int deletadoPor);
    Task<ApiResponse<DownloadArquivoResponseDto>> GetDownloadUrlAsync(int id, int usuarioId);
    Task<ApiResponse<ShareArquivoResponseDto>> ShareAsync(int id, ShareArquivoDto dto, int usuarioId);
}

public interface IEmailService
{
    Task<bool> SendEmailAsync(string to, string subject, string body);
    Task<bool> SendPasswordResetEmailAsync(string to, string token);
    Task<bool> SendWelcomeEmailAsync(string to, string nome);
}

public interface IAwsS3Service
{
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType);
    Task<string> GetPresignedUrlAsync(string key, TimeSpan expiration);
    Task<bool> DeleteFileAsync(string key);
    Task<bool> FileExistsAsync(string key);
}

public interface IReportService
{
    Task<ApiResponse<DashboardStatsDto>> GetDashboardStatsAsync();
    Task<ApiResponse<PagedResponse<LogAtividadeDto>>> GetLogsAtividadeAsync(int page = 1, int pageSize = 50);
    Task<ApiResponse<byte[]>> ExportCursosAsync(CursoFilterDto filter, string format);
    Task<ApiResponse<byte[]>> ExportArquivosAsync(ArquivoFilterDto filter, string format);
    Task<ApiResponse<byte[]>> ExportLogsAsync(int page, int pageSize, string format);
}

