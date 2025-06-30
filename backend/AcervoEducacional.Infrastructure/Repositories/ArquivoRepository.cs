using Microsoft.EntityFrameworkCore;
using AcervoEducacional.Domain.Entities;
using AcervoEducacional.Domain.Interfaces;
using AcervoEducacional.Infrastructure.Data;

namespace AcervoEducacional.Infrastructure.Repositories;

public class ArquivoRepository : BaseRepository<Arquivo>, IArquivoRepository
{
    public ArquivoRepository(AcervoEducacionalContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Arquivo>> GetByCursoIdAsync(Guid cursoId)
    {
        return await _dbSet
            .Include(a => a.Curso)
            .Include(a => a.UploadPorUsuario)
            .Where(a => a.CursoId == cursoId)
            .OrderBy(a => a.Categoria)
            .ThenBy(a => a.Nome)
            .ToListAsync();
    }

    public async Task<IEnumerable<Arquivo>> GetByCategoriaAsync(Domain.Enums.CategoriaArquivo categoria)
    {
        return await _dbSet
            .Include(a => a.Curso)
            .Include(a => a.UploadPorUsuario)
            .Where(a => a.Categoria == categoria)
            .OrderBy(a => a.Nome)
            .ToListAsync();
    }

    public async Task<IEnumerable<Arquivo>> GetByTipoCompartilhamentoAsync(Domain.Enums.TipoCompartilhamento tipo)
    {
        return await _dbSet
            .Include(a => a.Curso)
            .Include(a => a.UploadPorUsuario)
            .Where(a => a.TipoCompartilhamento == tipo)
            .OrderBy(a => a.Nome)
            .ToListAsync();
    }

    public async Task<Arquivo?> GetByHashAsync(string hash)
    {
        return await _dbSet
            .Include(a => a.Curso)
            .Include(a => a.UploadPorUsuario)
            .FirstOrDefaultAsync(a => a.HashArquivo == hash);
    }

    public async Task<bool> HashExistsAsync(string hash, Guid? excludeId = null)
    {
        var query = _dbSet.Where(a => a.HashArquivo == hash);
        
        if (excludeId.HasValue)
        {
            query = query.Where(a => a.Id != excludeId.Value);
        }
        
        return await query.AnyAsync();
    }

    public async Task<IEnumerable<Arquivo>> SearchAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await GetAllWithIncludesAsync();

        var term = searchTerm.ToLower();
        return await _dbSet
            .Include(a => a.Curso)
            .Include(a => a.UploadPorUsuario)
            .Where(a => a.Nome.ToLower().Contains(term) ||
                       a.NomeOriginal.ToLower().Contains(term) ||
                       (a.Descricao != null && a.Descricao.ToLower().Contains(term)) ||
                       (a.Tags != null && a.Tags.ToLower().Contains(term)) ||
                       (a.Curso != null && a.Curso.Nome.ToLower().Contains(term)))
            .OrderBy(a => a.Nome)
            .ToListAsync();
    }

    public async Task<(IEnumerable<Arquivo> Items, int TotalCount)> GetPagedWithFiltersAsync(
        int page,
        int pageSize,
        string? searchTerm = null,
        Domain.Enums.CategoriaArquivo? categoria = null,
        Domain.Enums.TipoCompartilhamento? tipoCompartilhamento = null,
        Guid? cursoId = null,
        string? tipoMime = null)
    {
        var query = _dbSet
            .Include(a => a.Curso)
            .Include(a => a.UploadPorUsuario)
            .AsQueryable();

        // Aplicar filtros
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower();
            query = query.Where(a => a.Nome.ToLower().Contains(term) ||
                               a.NomeOriginal.ToLower().Contains(term) ||
                               (a.Descricao != null && a.Descricao.ToLower().Contains(term)) ||
                               (a.Tags != null && a.Tags.ToLower().Contains(term)) ||
                               (a.Curso != null && a.Curso.Nome.ToLower().Contains(term)));
        }

        if (categoria.HasValue)
        {
            query = query.Where(a => a.Categoria == categoria.Value);
        }

        if (tipoCompartilhamento.HasValue)
        {
            query = query.Where(a => a.TipoCompartilhamento == tipoCompartilhamento.Value);
        }

        if (cursoId.HasValue)
        {
            query = query.Where(a => a.CursoId == cursoId.Value);
        }

        if (!string.IsNullOrWhiteSpace(tipoMime))
        {
            query = query.Where(a => a.TipoMime.ToLower().Contains(tipoMime.ToLower()));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(a => a.Categoria)
            .ThenBy(a => a.Nome)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Dictionary<string, int>> GetStatisticsAsync()
    {
        var total = await _dbSet.CountAsync();
        var totalSize = await _dbSet.SumAsync(a => a.Tamanho);
        var downloads = await _dbSet.SumAsync(a => a.Downloads);
        var visualizacoes = await _dbSet.SumAsync(a => a.Visualizacoes);
        var compartilhados = await _dbSet.CountAsync(a => a.TipoCompartilhamento != Domain.Enums.TipoCompartilhamento.Privado);

        return new Dictionary<string, int>
        {
            { "Total", total },
            { "TotalSize", (int)(totalSize / 1024 / 1024) }, // MB
            { "Downloads", downloads },
            { "Visualizacoes", visualizacoes },
            { "Compartilhados", compartilhados }
        };
    }

    public async Task<Dictionary<Domain.Enums.CategoriaArquivo, int>> GetStatisticsByCategoriaAsync()
    {
        var stats = await _dbSet
            .GroupBy(a => a.Categoria)
            .Select(g => new { Categoria = g.Key, Count = g.Count() })
            .ToListAsync();

        return stats.ToDictionary(s => s.Categoria, s => s.Count);
    }

    public async Task<IEnumerable<Arquivo>> GetRecentAsync(int count = 10)
    {
        return await _dbSet
            .Include(a => a.Curso)
            .Include(a => a.UploadPorUsuario)
            .OrderByDescending(a => a.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<Arquivo>> GetAllWithIncludesAsync()
    {
        return await _dbSet
            .Include(a => a.Curso)
            .Include(a => a.UploadPorUsuario)
            .OrderBy(a => a.Categoria)
            .ThenBy(a => a.Nome)
            .ToListAsync();
    }

    public async Task IncrementDownloadAsync(Guid id)
    {
        var arquivo = await _dbSet.FindAsync(id);
        if (arquivo != null)
        {
            arquivo.Downloads++;
            arquivo.UpdatedAt = DateTime.UtcNow;
            _dbSet.Update(arquivo);
        }
    }

    public async Task IncrementVisualizacaoAsync(Guid id)
    {
        var arquivo = await _dbSet.FindAsync(id);
        if (arquivo != null)
        {
            arquivo.Visualizacoes++;
            arquivo.UpdatedAt = DateTime.UtcNow;
            _dbSet.Update(arquivo);
        }
    }
}

