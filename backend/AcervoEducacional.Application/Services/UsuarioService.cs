using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using BCrypt.Net;
using AcervoEducacional.Application.DTOs.Usuario;
using AcervoEducacional.Application.DTOs.Common;
using AcervoEducacional.Application.Interfaces;
using AcervoEducacional.Domain.Entities;
using AcervoEducacional.Domain.Enums;
using AcervoEducacional.Domain.Interfaces;

namespace AcervoEducacional.Application.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ILogger<UsuarioService> _logger;
    
    // Configurações de segurança
    private const int _maxTentativasLogin = 5;
    private const int _tempoBloqueiMinutos = 30;
    private readonly Regex _emailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
    private readonly Regex _senhaRegex = new(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", RegexOptions.Compiled);

    public UsuarioService(
        IUsuarioRepository usuarioRepository,
        ILogger<UsuarioService> logger)
    {
        _usuarioRepository = usuarioRepository;
        _logger = logger;
    }

    public async Task<ApiResponse<UsuarioResponseDto>> GetByIdAsync(int id)
    {
        try
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario == null)
            {
                return ApiResponse<UsuarioResponseDto>.ErrorResult("Usuário não encontrado");
            }

            var response = MapToResponseDto(usuario);
            return ApiResponse<UsuarioResponseDto>.SuccessResult(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar usuário {Id}", id);
            return ApiResponse<UsuarioResponseDto>.ErrorResult("Erro interno do servidor");
        }
    }

    public async Task<ApiResponse<PagedResponse<UsuarioListDto>>> GetAllAsync(UsuarioFilterDto filter)
    {
        try
        {
            var (usuarios, total) = await _usuarioRepository.GetPagedAsync(
                filter.Page,
                filter.PageSize,
                filter.Search,
                filter.Tipo,
                filter.Status,
                filter.Ativo,
                filter.CriadoApartirDe,
                filter.CriadoAte,
                filter.SortBy,
                filter.SortDirection
            );

            var usuariosDto = usuarios.Select(MapToListDto).ToList();
            
            var pagedResponse = new PagedResponse<UsuarioListDto>(
                usuariosDto,
                total,
                filter.Page,
                filter.PageSize);

            return ApiResponse<PagedResponse<UsuarioListDto>>.SuccessResult(pagedResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar usuários com filtros");
            return ApiResponse<PagedResponse<UsuarioListDto>>.ErrorResult("Erro interno do servidor");
        }
    }

    public async Task<ApiResponse<UsuarioResponseDto>> CreateAsync(CreateUsuarioDto dto, int criadoPor)
    {
        try
        {
            // Validações de negócio
            var validationResult = await ValidateCreateUsuario(dto);
            if (!validationResult.IsSuccess)
            {
                return ApiResponse<UsuarioResponseDto>.ErrorResult(validationResult.Message);
            }

            // Verificar se email já existe
            var existingUser = await _usuarioRepository.GetByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                return ApiResponse<UsuarioResponseDto>.ErrorResult("Email já está em uso por outro usuário");
            }

            // Hash da senha
            var senhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha, BCrypt.Net.BCrypt.GenerateSalt(12));

            // Criar entidade
            var usuario = new Usuario
            {
                Nome = dto.Nome.Trim(),
                Email = dto.Email.Trim().ToLowerInvariant(),
                SenhaHash = senhaHash,
                Tipo = dto.Tipo,
                Status = StatusUsuario.Ativo,
                TentativasLogin = 0,
                CriadoPor = criadoPor.ToString(),
                CriadoEm = DateTime.UtcNow
            };

            // Salvar no banco
            await _usuarioRepository.AddAsync(usuario);

            _logger.LogInformation("Usuário {Email} criado com sucesso por {CriadoPor}", 
                usuario.Email, criadoPor);

            var response = MapToResponseDto(usuario);
            return ApiResponse<UsuarioResponseDto>.SuccessResult(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar usuário {Email}", dto.Email);
            return ApiResponse<UsuarioResponseDto>.ErrorResult("Erro interno do servidor");
        }
    }

    public async Task<ApiResponse<UsuarioResponseDto>> UpdateAsync(int id, UpdateUsuarioDto dto, int atualizadoPor)
    {
        try
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario == null)
            {
                return ApiResponse<UsuarioResponseDto>.ErrorResult("Usuário não encontrado");
            }

            // Validações de negócio
            var validationResult = await ValidateUpdateUsuario(dto, id);
            if (!validationResult.IsSuccess)
            {
                return ApiResponse<UsuarioResponseDto>.ErrorResult(validationResult.Message);
            }

            // Verificar se email já existe em outro usuário
            if (dto.Email.ToLowerInvariant() != usuario.Email)
            {
                var existingUser = await _usuarioRepository.GetByEmailAsync(dto.Email);
                if (existingUser != null && existingUser.Id != id)
                {
                    return ApiResponse<UsuarioResponseDto>.ErrorResult("Email já está em uso por outro usuário");
                }
            }

            // Atualizar propriedades
            usuario.Nome = dto.Nome.Trim();
            usuario.Email = dto.Email.Trim().ToLowerInvariant();
            usuario.Tipo = dto.Tipo;
            usuario.Status = dto.Status;
            usuario.AtualizadoPor = atualizadoPor.ToString();
            usuario.AtualizadoEm = DateTime.UtcNow;

            // Se o status mudou para Bloqueado, definir data de bloqueio
            if (dto.Status == StatusUsuario.Bloqueado && usuario.Status != StatusUsuario.Bloqueado)
            {
                usuario.BloqueadoAte = DateTime.UtcNow.AddMinutes(_tempoBloqueiMinutos);
            }
            else if (dto.Status != StatusUsuario.Bloqueado)
            {
                usuario.BloqueadoAte = null;
                usuario.TentativasLogin = 0;
            }

            await _usuarioRepository.UpdateAsync(usuario);

            _logger.LogInformation("Usuário {Id} atualizado com sucesso por {AtualizadoPor}", 
                id, atualizadoPor);

            var response = MapToResponseDto(usuario);
            return ApiResponse<UsuarioResponseDto>.SuccessResult(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar usuário {Id}", id);
            return ApiResponse<UsuarioResponseDto>.ErrorResult("Erro interno do servidor");
        }
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id, int deletadoPor)
    {
        try
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario == null)
            {
                return ApiResponse<bool>.ErrorResult("Usuário não encontrado");
            }

            // Verificar se é o último administrador
            if (usuario.Tipo == Tipo.Administrador)
            {
                var adminCount = await _usuarioRepository.CountActiveAdminsAsync();
                if (adminCount <= 1)
                {
                    return ApiResponse<bool>.ErrorResult("Não é possível deletar o último administrador do sistema");
                }
            }

            // Verificar se usuário tem dependências
            var hasDependencies = await _usuarioRepository.HasDependenciesAsync(id);
            if (hasDependencies)
            {
                // Soft delete - apenas desativar
                usuario.Status = StatusUsuario.Inativo;
                usuario.AtualizadoPor = deletadoPor.ToString();
                usuario.AtualizadoEm = DateTime.UtcNow;
                await _usuarioRepository.UpdateAsync(usuario);

                _logger.LogInformation("Usuário {Id} desativado (soft delete) por {DeletadoPor}", 
                    id, deletadoPor);
            }
            else
            {
                // Hard delete - remover completamente
                usuario.DeletadoPor = deletadoPor;
                usuario.DeletadoEm = DateTime.UtcNow;
                await _usuarioRepository.UpdateAsync(usuario);

                _logger.LogInformation("Usuário {Id} deletado por {DeletadoPor}", 
                    id, deletadoPor);
            }

            return ApiResponse<bool>.SuccessResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao deletar usuário {Id}", id);
            return ApiResponse<bool>.ErrorResult("Erro interno do servidor");
        }
    }

    public async Task<ApiResponse<UsuarioResponseDto>> GetByEmailAsync(string email)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return ApiResponse<UsuarioResponseDto>.ErrorResult("Email é obrigatório");
            }

            var usuario = await _usuarioRepository.GetByEmailAsync(email.Trim().ToLowerInvariant());
            if (usuario == null)
            {
                return ApiResponse<UsuarioResponseDto>.ErrorResult("Usuário não encontrado");
            }

            var response = MapToResponseDto(usuario);
            return ApiResponse<UsuarioResponseDto>.SuccessResult(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar usuário por email {Email}", email);
            return ApiResponse<UsuarioResponseDto>.ErrorResult("Erro interno do servidor");
        }
    }

    #region Métodos Auxiliares

    public async Task<ApiResponse<bool>> ChangePasswordAsync(int usuarioId, string senhaAtual, string novaSenha)
    {
        try
        {
            var usuario = await _usuarioRepository.GetByIdAsync(usuarioId);
            if (usuario == null)
            {
                return ApiResponse<bool>.ErrorResult("Usuário não encontrado");
            }

            // Verificar senha atual
            if (!BCrypt.Net.BCrypt.Verify(senhaAtual, usuario.SenhaHash))
            {
                return ApiResponse<bool>.ErrorResult("Senha atual incorreta");
            }

            // Validar nova senha
            if (!ValidatePassword(novaSenha))
            {
                return ApiResponse<bool>.ErrorResult("Nova senha não atende aos critérios de segurança");
            }

            // Atualizar senha
            usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(novaSenha, BCrypt.Net.BCrypt.GenerateSalt(12));
            usuario.AtualizadoPor = usuarioId.ToString();
            usuario.AtualizadoEm = DateTime.UtcNow;

            await _usuarioRepository.UpdateAsync(usuario);

            _logger.LogInformation("Senha alterada com sucesso para usuário {Id}", usuarioId);

            return ApiResponse<bool>.SuccessResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao alterar senha do usuário {Id}", usuarioId);
            return ApiResponse<bool>.ErrorResult("Erro interno do servidor");
        }
    }

    public async Task<ApiResponse<bool>> ResetPasswordAsync(int usuarioId, string novaSenha, int resetadoPor)
    {
        try
        {
            var usuario = await _usuarioRepository.GetByIdAsync(usuarioId);
            if (usuario == null)
            {
                return ApiResponse<bool>.ErrorResult("Usuário não encontrado");
            }

            // Validar nova senha
            if (!ValidatePassword(novaSenha))
            {
                return ApiResponse<bool>.ErrorResult("Nova senha não atende aos critérios de segurança");
            }

            // Atualizar senha e resetar tentativas
            usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(novaSenha, BCrypt.Net.BCrypt.GenerateSalt(12));
            usuario.TentativasLogin = 0;
            usuario.BloqueadoAte = null;
            usuario.Status = StatusUsuario.Ativo;
            usuario.AtualizadoPor = resetadoPor.ToString();
            usuario.AtualizadoEm = DateTime.UtcNow;

            await _usuarioRepository.UpdateAsync(usuario);

            _logger.LogInformation("Senha resetada com sucesso para usuário {Id} por {ResetadoPor}", 
                usuarioId, resetadoPor);

            return ApiResponse<bool>.SuccessResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao resetar senha do usuário {Id}", usuarioId);
            return ApiResponse<bool>.ErrorResult("Erro interno do servidor");
        }
    }

    public async Task<ApiResponse<bool>> UpdateLastLoginAsync(int usuarioId, string ipAddress, string userAgent)
    {
        try
        {
            var usuario = await _usuarioRepository.GetByIdAsync(usuarioId);
            if (usuario == null)
            {
                return ApiResponse<bool>.ErrorResult("Usuário não encontrado");
            }

            usuario.UltimoLogin = DateTime.UtcNow;
            usuario.UltimoIp = ipAddress;
            usuario.UltimoUserAgent = userAgent;
            usuario.TentativasLogin = 0; // Reset tentativas após login bem-sucedido

            await _usuarioRepository.UpdateAsync(usuario);

            return ApiResponse<bool>.SuccessResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar último login do usuário {Id}", usuarioId);
            return ApiResponse<bool>.ErrorResult("Erro interno do servidor");
        }
    }

    public async Task<ApiResponse<bool>> IncrementLoginAttemptsAsync(int usuarioId)
    {
        try
        {
            var usuario = await _usuarioRepository.GetByIdAsync(usuarioId);
            if (usuario == null)
            {
                return ApiResponse<bool>.ErrorResult("Usuário não encontrado");
            }

            usuario.TentativasLogin++;

            // Bloquear usuário se exceder tentativas máximas
            if (usuario.TentativasLogin >= _maxTentativasLogin)
            {
                usuario.Status = StatusUsuario.Bloqueado;
                usuario.BloqueadoAte = DateTime.UtcNow.AddMinutes(_tempoBloqueiMinutos);
            }

            await _usuarioRepository.UpdateAsync(usuario);

            return ApiResponse<bool>.SuccessResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao incrementar tentativas de login do usuário {Id}", usuarioId);
            return ApiResponse<bool>.ErrorResult("Erro interno do servidor");
        }
    }

    #endregion

    #region Private Methods

    private async Task<(bool IsSuccess, string Message)> ValidateCreateUsuario(CreateUsuarioDto dto)
    {
        // Validar email
        if (!_emailRegex.IsMatch(dto.Email))
        {
            return (false, "Email deve ter um formato válido");
        }

        // Validar senha
        if (!ValidatePassword(dto.Senha))
        {
            return (false, "Senha deve ter pelo menos 8 caracteres, incluindo maiúscula, minúscula, número e símbolo");
        }

        // Validar nome
        if (string.IsNullOrWhiteSpace(dto.Nome) || dto.Nome.Trim().Length < 2)
        {
            return (false, "Nome deve ter pelo menos 2 caracteres");
        }

        return (true, string.Empty);
    }

    private async Task<(bool IsSuccess, string Message)> ValidateUpdateUsuario(UpdateUsuarioDto dto, int usuarioId)
    {
        // Validar email
        if (!_emailRegex.IsMatch(dto.Email))
        {
            return (false, "Email deve ter um formato válido");
        }

        // Validar nome
        if (string.IsNullOrWhiteSpace(dto.Nome) || dto.Nome.Trim().Length < 2)
        {
            return (false, "Nome deve ter pelo menos 2 caracteres");
        }

        return (true, string.Empty);
    }

    private bool ValidatePassword(string senha)
    {
        if (string.IsNullOrWhiteSpace(senha))
            return false;

        // Pelo menos 8 caracteres, 1 maiúscula, 1 minúscula, 1 número, 1 símbolo
        return _senhaRegex.IsMatch(senha);
    }

    private UsuarioResponseDto MapToResponseDto(Usuario usuario)
    {
        return new UsuarioResponseDto
        {
            Id = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email,
            Tipo = usuario.Tipo,
            Status = usuario.Status,
            UltimoLogin = usuario.UltimoLogin,
            UltimoIp = usuario.UltimoIp,
            CriadoEm = usuario.CriadoEm,
            AtualizadoEm = usuario.AtualizadoEm,
            Ativo = usuario.DeletadoEm == null && usuario.Status == StatusUsuario.Ativo
        };
    }

    private UsuarioListDto MapToListDto(Usuario usuario)
    {
        return new UsuarioListDto
        {
            Id = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email,
            Tipo = usuario.Tipo,
            Status = usuario.Status,
            UltimoLogin = usuario.UltimoLogin,
            CriadoEm = usuario.CriadoEm,
            Ativo = usuario.DeletadoEm == null && usuario.Status == StatusUsuario.Ativo
        };
    }

    #endregion
}

