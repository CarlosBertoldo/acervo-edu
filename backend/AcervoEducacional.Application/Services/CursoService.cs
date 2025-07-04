using Microsoft.Extensions.Logging;
using AcervoEducacional.Application.DTOs.Common;
using AcervoEducacional.Application.DTOs.Curso;
using AcervoEducacional.Application.Interfaces;
using AcervoEducacional.Domain.Entities;
using AcervoEducacional.Domain.Enums;
using AcervoEducacional.Domain.Interfaces;

namespace AcervoEducacional.Application.Services;

public class CursoService : ICursoService
{
    private readonly ICursoRepository _cursoRepository;
    private readonly IArquivoRepository _arquivoRepository;
    private readonly ILogAtividadeRepository _logRepository;
    private readonly ILogger<CursoService> _logger;

    public CursoService(
        ICursoRepository cursoRepository,
        IArquivoRepository arquivoRepository,
        ILogAtividadeRepository logRepository,
        ILogger<CursoService> logger)
    {
        _cursoRepository = cursoRepository;
        _arquivoRepository = arquivoRepository;
        _logRepository = logRepository;
        _logger = logger;
    }

    public async Task<ApiResponse<CursoResponseDto>> GetByIdAsync(int id)
    {
        try
        {
            var curso = await _cursoRepository.GetByIdAsync(id);
            if (curso == null)
            {
                return ApiResponse<CursoResponseDto>.ErrorResult("Curso não encontrado");
            }

            var response = MapToResponseDto(curso);
            return ApiResponse<CursoResponseDto>.SuccessResult(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar curso por ID {CursoId}", id);
            return ApiResponse<CursoResponseDto>.ErrorResult("Erro interno do servidor");
        }
    }

    public async Task<ApiResponse<PagedResponse<CursoListDto>>> GetAllAsync(CursoFilterDto filter)
    {
        try
        {
            var cursos = await _cursoRepository.GetAllAsync();
            
            // Aplicar filtros
            var query = cursos.AsQueryable();

            if (!string.IsNullOrEmpty(filter.Search))
            {
                var searchLower = filter.Search.ToLower();
                query = query.Where(c => 
                    c.Nome.ToLower().Contains(searchLower) ||
                    (c.DescricaoAcademia != null && c.DescricaoAcademia.ToLower().Contains(searchLower)) ||
                    c.Codigo.ToLower().Contains(searchLower));
            }

            if (filter.Status.HasValue)
            {
                query = query.Where(c => c.Status == filter.Status.Value);
            }

            if (filter.Origem.HasValue)
            {
                query = query.Where(c => c.Origem == filter.Origem.Value);
            }

            if (filter.CriadoApartirDe.HasValue)
            {
                query = query.Where(c => c.CriadoEm >= filter.CriadoApartirDe.Value);
            }

            if (filter.CriadoAte.HasValue)
            {
                query = query.Where(c => c.CriadoEm <= filter.CriadoAte.Value);
            }

            // Ordenação
            query = filter.OrderBy?.ToLower() switch
            {
                "titulo" => filter.OrderDirection == "desc" 
                    ? query.OrderByDescending(c => c.Nome)
                    : query.OrderBy(c => c.Nome),
                "status" => filter.OrderDirection == "desc"
                    ? query.OrderByDescending(c => c.Status)
                    : query.OrderBy(c => c.Status),
                "criadoem" => filter.OrderDirection == "desc"
                    ? query.OrderByDescending(c => c.CriadoEm)
                    : query.OrderBy(c => c.CriadoEm),
                _ => query.OrderByDescending(c => c.CriadoEm)
            };

            var totalItems = query.Count();
            var items = query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(MapToListDto)
                .ToList();

            var pagedResponse = new PagedResponse<CursoListDto>(
                items,
                totalItems,
                filter.Page,
                filter.PageSize);

            return ApiResponse<PagedResponse<CursoListDto>>.SuccessResult(pagedResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar cursos com filtros");
            return ApiResponse<PagedResponse<CursoListDto>>.ErrorResult("Erro interno do servidor");
        }
    }

    public async Task<ApiResponse<List<CursoKanbanDto>>> GetKanbanAsync()
    {
        try
        {
            var cursos = await _cursoRepository.GetAllAsync();
            
            var kanbanData = cursos
                .GroupBy(c => c.Status)
                .Select(g => new CursoKanbanDto
                {
                    Status = g.Key,
                    StatusNome = GetStatusNome(g.Key),
                    Cursos = g.Select(MapToKanbanItemDto).ToList(),
                    TotalCursos = g.Count()
                })
                .OrderBy(k => (int)k.Status)
                .ToList();

            // Garantir que todas as colunas existam, mesmo vazias
            var todosStatus = Enum.GetValues<StatusCurso>();
            foreach (var status in todosStatus)
            {
                if (!kanbanData.Any(k => k.Status == status))
                {
                    kanbanData.Add(new CursoKanbanDto
                    {
                        Status = status,
                        StatusNome = GetStatusNome(status),
                        Cursos = new List<CursoKanbanItemDto>(),
                        TotalCursos = 0
                    });
                }
            }

            kanbanData = kanbanData.OrderBy(k => (int)k.Status).ToList();

            return ApiResponse<List<CursoKanbanDto>>.SuccessResult(kanbanData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar dados do Kanban");
            return ApiResponse<List<CursoKanbanDto>>.ErrorResult("Erro interno do servidor");
        }
    }

    public async Task<ApiResponse<CursoResponseDto>> CreateAsync(CreateCursoDto dto, int criadoPor)
    {
        try
        {
            // Validações
            var validationResult = await ValidateCreateCursoAsync(dto);
            if (!validationResult.Success)
            {
                return ApiResponse<CursoResponseDto>.ErrorResult(validationResult.Message);
            }

            // Verificar se código já existe
            var codigoExists = await _cursoRepository.CodigoExistsAsync(dto.Codigo);
            if (codigoExists)
            {
                return ApiResponse<CursoResponseDto>.ErrorResult("Já existe um curso com este código");
            }

            var curso = new Curso
            {
                Nome = dto.Titulo.Trim(),
                DescricaoAcademia = dto.Descricao?.Trim(),
                Codigo = dto.Codigo.Trim().ToUpper(),
                Status = dto.Status,
                Origem = dto.Origem,
                ComentariosInternos = dto.Observacoes?.Trim(),
                CriadoPor = criadoPor,
                CriadoEm = DateTime.UtcNow
            };

            var cursoCreated = await _cursoRepository.AddAsync(curso);

            await LogAtividadeAsync(criadoPor, TipoAtividade.CriacaoCurso, 
                $"Curso '{curso.Nome}' criado", cursoCreated.Id);

            var response = MapToResponseDto(cursoCreated);
            return ApiResponse<CursoResponseDto>.SuccessResult(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar curso");
            return ApiResponse<CursoResponseDto>.ErrorResult("Erro interno do servidor");
        }
    }

    public async Task<ApiResponse<CursoResponseDto>> UpdateAsync(int id, UpdateCursoDto dto, int atualizadoPor)
    {
        try
        {
            var curso = await _cursoRepository.GetByIdAsync(id);
            if (curso == null)
            {
                return ApiResponse<CursoResponseDto>.ErrorResult("Curso não encontrado");
            }

            // Validações
            var validationResult = await ValidateUpdateCursoAsync(dto, id);
            if (!validationResult.Success)
            {
                return ApiResponse<CursoResponseDto>.ErrorResult(validationResult.Message);
            }

            // Verificar se código já existe (exceto para o próprio curso)
            if (dto.Codigo != curso.Codigo)
            {
                var codigoExists = await _cursoRepository.CodigoExistsAsync(dto.Codigo);
                if (codigoExists)
                {
                    return ApiResponse<CursoResponseDto>.ErrorResult("Já existe um curso com este código");
                }
            }

            // Atualizar campos
            curso.Nome = dto.Titulo.Trim();
            curso.DescricaoAcademia = dto.Descricao?.Trim();
            curso.Codigo = dto.Codigo.Trim().ToUpper();
            curso.ComentariosInternos = dto.Observacoes?.Trim();
            curso.AtualizadoPor = atualizadoPor.ToString();
            curso.AtualizadoEm = DateTime.UtcNow;

            var cursoUpdated = await _cursoRepository.UpdateAsync(curso);

            await LogAtividadeAsync(atualizadoPor, TipoAtividade.EdicaoCurso, 
                $"Curso '{curso.Nome}' atualizado", curso.Id);

            var response = MapToResponseDto(cursoUpdated);
            return ApiResponse<CursoResponseDto>.SuccessResult(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar curso {CursoId}", id);
            return ApiResponse<CursoResponseDto>.ErrorResult("Erro interno do servidor");
        }
    }

    public async Task<ApiResponse<CursoResponseDto>> UpdateStatusAsync(int id, UpdateStatusCursoDto dto, int atualizadoPor)
    {
        try
        {
            var curso = await _cursoRepository.GetByIdAsync(id);
            if (curso == null)
            {
                return ApiResponse<CursoResponseDto>.ErrorResult("Curso não encontrado");
            }

            var statusAnterior = curso.Status;
            curso.Status = dto.NovoStatus;
            curso.AtualizadoPor = atualizadoPor.ToString();
            curso.AtualizadoEm = DateTime.UtcNow;

            var cursoUpdated = await _cursoRepository.UpdateAsync(curso);

            await LogAtividadeAsync(atualizadoPor, TipoAtividade.AlteracaoStatusCurso, 
                $"Status do curso '{curso.Nome}' alterado de {GetStatusNome(statusAnterior)} para {GetStatusNome(dto.NovoStatus)}", 
                curso.Id);

            var response = MapToResponseDto(cursoUpdated);
            return ApiResponse<CursoResponseDto>.SuccessResult(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar status do curso {CursoId}", id);
            return ApiResponse<CursoResponseDto>.ErrorResult("Erro interno do servidor");
        }
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id, int deletadoPor)
    {
        try
        {
            var curso = await _cursoRepository.GetByIdAsync(id);
            if (curso == null)
            {
                return ApiResponse<bool>.ErrorResult("Curso não encontrado");
            }

            // Verificar se há arquivos associados
            var arquivos = await _arquivoRepository.GetByCursoIdAsync(id);
            if (arquivos.Any())
            {
                return ApiResponse<bool>.ErrorResult("Não é possível excluir um curso que possui arquivos associados");
            }

            await _cursoRepository.DeleteAsync(id);

            await LogAtividadeAsync(deletadoPor, TipoAtividade.ExclusaoCurso, 
                $"Curso '{curso.Nome}' excluído", curso.Id);

            return ApiResponse<bool>.SuccessResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir curso {CursoId}", id);
            return ApiResponse<bool>.ErrorResult("Erro interno do servidor");
        }
    }

    public async Task<ApiResponse<DashboardStatsDto>> GetDashboardStatsAsync()
    {
        try
        {
            var cursos = await _cursoRepository.GetAllAsync();
            var arquivos = await _arquivoRepository.GetAllAsync();

            var stats = new DashboardStatsDto
            {
                TotalCursos = cursos.Count(),
                TotalArquivos = arquivos.Count(),
                CursosAtivos = cursos.Count(c => c.Status == StatusCurso.Ativo),
                CursosPorStatus = cursos
                    .GroupBy(c => c.Status)
                    .ToDictionary(g => g.Key.ToString(), g => g.Count()),
                CursosPorOrigem = cursos
                    .GroupBy(c => c.Origem)
                    .ToDictionary(g => g.Key.ToString(), g => g.Count()),
                ArquivosPorCategoria = arquivos
                    .GroupBy(a => a.Categoria)
                    .ToDictionary(g => g.Key.ToString(), g => g.Count())
            };

            return ApiResponse<DashboardStatsDto>.SuccessResult(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar estatísticas do dashboard");
            return ApiResponse<DashboardStatsDto>.ErrorResult("Erro interno do servidor");
        }
    }

    #region Métodos Privados

    private async Task<(bool Success, string Message)> ValidateCreateCursoAsync(CreateCursoDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Titulo))
            return (false, "Título é obrigatório");

        if (dto.Titulo.Length > 200)
            return (false, "Título deve ter no máximo 200 caracteres");

        if (string.IsNullOrWhiteSpace(dto.Codigo))
            return (false, "Código é obrigatório");

        if (dto.Codigo.Length > 20)
            return (false, "Código deve ter no máximo 20 caracteres");

        if (dto.CargaHoraria.HasValue && dto.CargaHoraria <= 0)
            return (false, "Carga horária deve ser maior que zero");

        if (dto.DataInicio.HasValue && dto.DataFim.HasValue && dto.DataInicio > dto.DataFim)
            return (false, "Data de início não pode ser posterior à data de fim");

        if (!string.IsNullOrEmpty(dto.Descricao) && dto.Descricao.Length > 1000)
            return (false, "Descrição deve ter no máximo 1000 caracteres");

        if (!string.IsNullOrEmpty(dto.Instrutor) && dto.Instrutor.Length > 100)
            return (false, "Nome do instrutor deve ter no máximo 100 caracteres");

        if (!string.IsNullOrEmpty(dto.Observacoes) && dto.Observacoes.Length > 500)
            return (false, "Observações devem ter no máximo 500 caracteres");

        return (true, string.Empty);
    }

    private async Task<(bool Success, string Message)> ValidateUpdateCursoAsync(UpdateCursoDto dto, int cursoId)
    {
        if (string.IsNullOrWhiteSpace(dto.Titulo))
            return (false, "Título é obrigatório");

        if (dto.Titulo.Length > 200)
            return (false, "Título deve ter no máximo 200 caracteres");

        if (string.IsNullOrWhiteSpace(dto.Codigo))
            return (false, "Código é obrigatório");

        if (dto.Codigo.Length > 20)
            return (false, "Código deve ter no máximo 20 caracteres");

        if (dto.CargaHoraria.HasValue && dto.CargaHoraria <= 0)
            return (false, "Carga horária deve ser maior que zero");

        if (dto.DataInicio.HasValue && dto.DataFim.HasValue && dto.DataInicio > dto.DataFim)
            return (false, "Data de início não pode ser posterior à data de fim");

        if (!string.IsNullOrEmpty(dto.Descricao) && dto.Descricao.Length > 1000)
            return (false, "Descrição deve ter no máximo 1000 caracteres");

        if (!string.IsNullOrEmpty(dto.Instrutor) && dto.Instrutor.Length > 100)
            return (false, "Nome do instrutor deve ter no máximo 100 caracteres");

        if (!string.IsNullOrEmpty(dto.Observacoes) && dto.Observacoes.Length > 500)
            return (false, "Observações devem ter no máximo 500 caracteres");

        return (true, string.Empty);
    }

    private async Task LogAtividadeAsync(int usuarioId, TipoAtividade tipo, string descricao, int? cursoId = null)
    {
        try
        {
            var log = new LogAtividade
            {
                UsuarioId = usuarioId,
                TipoAtividade = tipo,
                Descricao = descricao,
                CursoId = cursoId,
                CriadoEm = DateTime.UtcNow
            };

            await _logRepository.AddAsync(log);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao registrar log de atividade");
        }
    }

    private static string GetStatusNome(StatusCurso status)
    {
        return status switch
        {
            StatusCurso.Rascunho => "Rascunho",
            StatusCurso.Planejamento => "Planejamento",
            StatusCurso.EmDesenvolvimento => "Em Desenvolvimento",
            StatusCurso.Revisao => "Revisão",
            StatusCurso.Aprovado => "Aprovado",
            StatusCurso.Publicado => "Publicado",
            StatusCurso.Ativo => "Ativo",
            StatusCurso.Pausado => "Pausado",
            StatusCurso.Concluido => "Concluído",
            StatusCurso.Arquivado => "Arquivado",
            _ => status.ToString()
        };
    }

    private static CursoResponseDto MapToResponseDto(Curso curso)
    {
        return new CursoResponseDto
        {
            Id = curso.Id,
            Titulo = curso.Nome,
            Descricao = curso.DescricaoAcademia,
            Codigo = curso.Codigo,
            Status = curso.Status,
            StatusNome = GetStatusNome(curso.Status),
            Origem = curso.Origem,
            OrigemNome = curso.Origem.ToString(),
            Observacoes = curso.ComentariosInternos,
            CriadoPor = curso.CriadoPor,
            CriadoEm = curso.CriadoEm,
            AtualizadoPor = int.TryParse(curso.AtualizadoPor, out int atualizadoPor) ? atualizadoPor : null,
            AtualizadoEm = curso.AtualizadoEm
        };
    }

    private static CursoListDto MapToListDto(Curso curso)
    {
        return new CursoListDto
        {
            Id = curso.Id,
            Titulo = curso.Nome,
            Codigo = curso.Codigo,
            Status = curso.Status,
            StatusNome = GetStatusNome(curso.Status),
            Origem = curso.Origem,
            OrigemNome = curso.Origem.ToString(),
            CriadoEm = curso.CriadoEm
        };
    }

    private static CursoKanbanItemDto MapToKanbanItemDto(Curso curso)
    {
        return new CursoKanbanItemDto
        {
            Id = curso.Id,
            Titulo = curso.Nome,
            Codigo = curso.Codigo,
            Origem = curso.Origem,
            OrigemNome = curso.Origem.ToString(),
            CriadoEm = curso.CriadoEm
        };
    }

    #endregion
}

