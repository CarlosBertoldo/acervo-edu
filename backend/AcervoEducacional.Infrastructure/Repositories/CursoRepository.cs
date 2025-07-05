using Microsoft.EntityFrameworkCore;
using AcervoEducacional.Domain.Entities;
using AcervoEducacional.Domain.Interfaces;
using AcervoEducacional.Infrastructure.Data;

namespace AcervoEducacional.Infrastructure.Repositories;

public class CursoRepository : BaseRepository<Curso>, ICursoRepository
{
    public CursoRepository(SimpleDbContext context) : base(context)
    {
    }

    public async Task<Curso?> GetByCodigoAsync(string codigo)
    {
        return await _dbSet
            .Include(c => c.UsuarioCriador)
            .Include(c => c.Arquivos)
            .FirstOrDefaultAsync(c => c.Codigo.ToLower() == codigo.ToLower());
    }

    public async Task<bool> CodigoExistsAsync(string codigo, int? excludeId = null)
    {
        var query = _dbSet.Where(c => c.Codigo.ToLower() == codigo.ToLower());
        
        if (excludeId.HasValue)
        {
            query = query.Where(c => c.Id != excludeId.Value);
        }
        
        return await query.AnyAsync();
    }

    public async Task<bool> CodigoExistsAsync(string codigo)
    {
        return await _dbSet.AnyAsync(c => c.Codigo.ToLower() == codigo.ToLower());
    }

    public async Task<IEnumerable<Curso>> GetByStatusAsync(Domain.Enums.StatusCurso status)
    {
        return await _dbSet
            .Include(c => c.UsuarioCriador)
            .Include(c => c.Arquivos)
            .Where(c => c.Status == status)
            .OrderBy(c => c.NomeCurso)
            .ToListAsync();
    }

    public async Task<IEnumerable<Curso>> GetByOrigemAsync(Domain.Enums.OrigemCurso origem)
    {
        return await _dbSet
            .Include(c => c.UsuarioCriador)
            .Include(c => c.Arquivos)
            .Where(c => c.Origem == origem)
            .OrderBy(c => c.NomeCurso)
            .ToListAsync();
    }

    public async Task<IEnumerable<Curso>> GetWithArquivosAsync()
    {
        return await _dbSet
            .Include(c => c.UsuarioCriador)
            .Include(c => c.Arquivos)
            .OrderBy(c => c.NomeCurso)
            .ToListAsync();
    }

    public async Task<Curso?> GetWithArquivosByIdAsync(int id)
    {
        return await _dbSet
            .Include(c => c.UsuarioCriador)
            .Include(c => c.Arquivos)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Curso>> SearchAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await GetWithArquivosAsync();

        var term = searchTerm.ToLower();
        return await _dbSet
            .Include(c => c.UsuarioCriador)
            .Include(c => c.Arquivos)
            .Where(c => c.NomeCurso.ToLower().Contains(term) ||
                       c.Codigo.ToLower().Contains(term) ||
                       (c.DescricaoAcademia != null && c.DescricaoAcademia.ToLower().Contains(term)) ||
                       (c.UsuarioCriador != null && c.UsuarioCriador.Nome.ToLower().Contains(term)))
            .OrderBy(c => c.NomeCurso)
            .ToListAsync();
    }

    public async Task<(IEnumerable<Curso> Items, int TotalCount)> GetPagedWithFiltersAsync(
        int page,
        int pageSize,
        string? searchTerm = null,
        Domain.Enums.StatusCurso? status = null,
        Domain.Enums.OrigemCurso? origem = null,
        Domain.Enums.TipoAmbiente? tipoAmbiente = null,
        Domain.Enums.TipoAcesso? tipoAcesso = null)
    {
        var query = _dbSet
            .Include(c => c.UsuarioCriador)
            .Include(c => c.Arquivos)
            .AsQueryable();

        // Aplicar filtros
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower();
            query = query.Where(c => c.NomeCurso.ToLower().Contains(term) ||
                               c.Codigo.ToLower().Contains(term) ||
                               (c.DescricaoAcademia != null && c.DescricaoAcademia.ToLower().Contains(term)) ||
                               (c.UsuarioCriador != null && c.UsuarioCriador.Nome.ToLower().Contains(term)));
        }

        if (status.HasValue)
        {
            query = query.Where(c => c.Status == status.Value);
        }

        if (origem.HasValue)
        {
            query = query.Where(c => c.Origem == origem.Value);
        }

        if (tipoAmbiente.HasValue)
        {
            query = query.Where(c => c.TipoAmbiente == tipoAmbiente.Value);
        }

        if (tipoAcesso.HasValue)
        {
            query = query.Where(c => c.TipoAcesso == tipoAcesso.Value);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(c => c.NomeCurso)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Dictionary<string, int>> GetStatisticsAsync()
    {
        var total = await _dbSet.CountAsync();
        var backlog = await _dbSet.CountAsync(c => c.Status == Domain.Enums.StatusCurso.Backlog);
        var emDesenvolvimento = await _dbSet.CountAsync(c => c.Status == Domain.Enums.StatusCurso.EmDesenvolvimento);
        var veiculado = await _dbSet.CountAsync(c => c.Status == Domain.Enums.StatusCurso.Veiculado);
        var manuais = await _dbSet.CountAsync(c => c.Origem == Domain.Enums.OrigemCurso.Manual);
        var senior = await _dbSet.CountAsync(c => c.Origem == Domain.Enums.OrigemCurso.Senior);

        return new Dictionary<string, int>
        {
            { "Total", total },
            { "Backlog", backlog },
            { "EmDesenvolvimento", emDesenvolvimento },
            { "Veiculado", veiculado },
            { "Manuais", manuais },
            { "Senior", senior }
        };
    }

    public async Task<IEnumerable<Curso>> GetKanbanDataAsync()
    {
        return await _dbSet
            .Include(c => c.UsuarioCriador)
            .Include(c => c.Arquivos)
            .OrderBy(c => c.Status)
            .ThenBy(c => c.NomeCurso)
            .ToListAsync();
    }

    public async Task<IEnumerable<Curso>> GetRecentAsync(int count = 10)
    {
        return await _dbSet
            .Include(c => c.UsuarioCriador)
            .Include(c => c.Arquivos)
            .OrderByDescending(c => c.AtualizadoEm)
            .Take(count)
            .ToListAsync();
    }
}

