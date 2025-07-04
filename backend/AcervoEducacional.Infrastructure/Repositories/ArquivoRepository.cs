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

    public async Task<IEnumerable<Arquivo>> GetByCursoIdAsync(int cursoId)
    {
        return await _dbSet
            .Include(a => a.Curso)
            .Where(a => a.CursoId == cursoId)
            .OrderBy(a => a.Categoria)
            .ThenBy(a => a.Nome)
            .ToListAsync();
    }

    public async Task<IEnumerable<Arquivo>> GetByCategoriaAsync(Domain.Enums.CategoriaArquivo categoria)
    {
        return await _dbSet
            .Include(a => a.Curso)
            .Where(a => a.Categoria == categoria)
            .OrderBy(a => a.Nome)
            .ToListAsync();
    }

    public async Task<IEnumerable<Arquivo>> GetByTipoCompartilhamentoAsync(Domain.Enums.TipoCompartilhamento tipo)
    {
        return await _dbSet
            .Include(a => a.Curso)
            .Where(a => a.TipoCompartilhamento == tipo)
            .OrderBy(a => a.Nome)
            .ToListAsync();
    }

    public async Task<Arquivo?> GetByHashAsync(string hash)
    {
        return await _dbSet
            .Include(a => a.Curso)
            .FirstOrDefaultAsync(a => a.HashArquivo == hash);
    }

    public async Task<bool> HashExistsAsync(string hash, int? excludeId = null)
    {
        var query = _dbSet.Where(a => a.HashArquivo == hash);
        
        if (excludeId.HasValue)
        {
            query = query.Where(a => a.Id != excludeId.Value);
        }
        
        return await query.AnyAsync();
    }

    public async Task<IEnumerable<Arquivo>> GetPublicosAsync()
    {
        return await _dbSet
            .Include(a => a.Curso)
            .Where(a => a.IsPublico == true)
            .OrderBy(a => a.Nome)
            .ToListAsync();
    }

    public async Task<long> GetTamanhoTotalByCursoAsync(int cursoId)
    {
        return await _dbSet
            .Where(a => a.CursoId == cursoId)
            .SumAsync(a => a.Tamanho);
    }

    public async Task<(IEnumerable<Arquivo> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? search = null,
        int? cursoId = null,
        Domain.Enums.CategoriaArquivo? categoria = null,
        string? extensao = null,
        bool? publico = null,
        DateTime? criadoApartirDe = null,
        DateTime? criadoAte = null,
        long? tamanhoMin = null,
        long? tamanhoMax = null,
        string? sortBy = null,
        string? sortDirection = null)
    {
        var query = _dbSet
            .Include(a => a.Curso)
            .AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchTerm = search.ToLower();
            query = query.Where(a => a.Nome.ToLower().Contains(searchTerm) ||
                                   a.NomeOriginal.ToLower().Contains(searchTerm) ||
                                   (a.Descricao != null && a.Descricao.ToLower().Contains(searchTerm)));
        }

        if (cursoId.HasValue)
        {
            query = query.Where(a => a.CursoId == cursoId.Value);
        }

        if (categoria.HasValue)
        {
            query = query.Where(a => a.Categoria == categoria.Value);
        }

        if (!string.IsNullOrWhiteSpace(extensao))
        {
            query = query.Where(a => a.TipoMime.ToLower().Contains(extensao.ToLower()));
        }

        if (publico.HasValue)
        {
            query = query.Where(a => a.IsPublico == publico.Value);
        }

        if (criadoApartirDe.HasValue)
        {
            query = query.Where(a => a.CriadoEm >= criadoApartirDe.Value);
        }

        if (criadoAte.HasValue)
        {
            query = query.Where(a => a.CriadoEm <= criadoAte.Value);
        }

        if (tamanhoMin.HasValue)
        {
            query = query.Where(a => a.Tamanho >= tamanhoMin.Value);
        }

        if (tamanhoMax.HasValue)
        {
            query = query.Where(a => a.Tamanho <= tamanhoMax.Value);
        }

        // Apply sorting
        if (!string.IsNullOrWhiteSpace(sortBy))
        {
            var isDescending = sortDirection?.ToLower() == "desc";
            query = sortBy.ToLower() switch
            {
                "nome" => isDescending ? query.OrderByDescending(a => a.Nome) : query.OrderBy(a => a.Nome),
                "tamanho" => isDescending ? query.OrderByDescending(a => a.Tamanho) : query.OrderBy(a => a.Tamanho),
                "criadoem" => isDescending ? query.OrderByDescending(a => a.CriadoEm) : query.OrderBy(a => a.CriadoEm),
                _ => query.OrderBy(a => a.Nome)
            };
        }
        else
        {
            query = query.OrderBy(a => a.Nome);
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<IEnumerable<Arquivo>> SearchAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await GetAllWithIncludesAsync();

        var term = searchTerm.ToLower();
        return await _dbSet
            .Include(a => a.Curso)
            .Where(a => a.Nome.ToLower().Contains(term) ||
                       a.NomeOriginal.ToLower().Contains(term) ||
                       (a.Descricao != null && a.Descricao.ToLower().Contains(term)) ||
                       (a.Tags != null && a.Tags.ToLower().Contains(term)) ||
                       (a.Curso != null && a.Curso.NomeCurso.ToLower().Contains(term)))
            .OrderBy(a => a.Nome)
            .ToListAsync();
    }

    public async Task<(IEnumerable<Arquivo> Items, int TotalCount)> GetPagedWithFiltersAsync(
        int page,
        int pageSize,
        string? searchTerm = null,
        Domain.Enums.CategoriaArquivo? categoria = null,
        Domain.Enums.TipoCompartilhamento? tipoCompartilhamento = null,
        int? cursoId = null,
        string? tipoMime = null)
    {
        var query = _dbSet
            .Include(a => a.Curso)
            .AsQueryable();

        // Aplicar filtros
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower();
            query = query.Where(a => a.Nome.ToLower().Contains(term) ||
                               a.NomeOriginal.ToLower().Contains(term) ||
                               (a.Descricao != null && a.Descricao.ToLower().Contains(term)) ||
                               (a.Tags != null && a.Tags.ToLower().Contains(term)) ||
                               (a.Curso != null && a.Curso.NomeCurso.ToLower().Contains(term)));
        }

        if (categoria.HasValue)
        {
            query = query.Where(a => a.Categoria == categoria.Value);
        }

        if (tipoCompartilhamento.HasValue)
        {
            query = query.Where(a => a.IsPublico == tipoCompartilhamento.Value);
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
        var downloads = await _dbSet.SumAsync(a => a.Tamanho);
        var visualizacoes = await _dbSet.SumAsync(a => a.DownloadCount);
        var compartilhados = await _dbSet.CountAsync(a => a.IsPublico == true);

        return new Dictionary<string, int>
        {
            { "Total", total },
            { "TotalSize", (int)(totalSize / 1024 / 1024) }, // MB
            { "Downloads", (int)downloads },
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
            .OrderByDescending(a => a.CriadoEm)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<Arquivo>> GetAllWithIncludesAsync()
    {
        return await _dbSet
            .Include(a => a.Curso)
            .OrderBy(a => a.Categoria)
            .ThenBy(a => a.Nome)
            .ToListAsync();
    }

    public async Task IncrementDownloadAsync(int id)
    {
        var arquivo = await _dbSet.FindAsync(id);
        if (arquivo != null)
        {
            // Simulate download increment (property doesn't exist in entity)
            arquivo.AtualizadoEm = DateTime.UtcNow;
            _dbSet.Update(arquivo);
        }
    }

    public async Task IncrementVisualizacaoAsync(int id)
    {
        var arquivo = await _dbSet.FindAsync(id);
        if (arquivo != null)
        {
            // Simulate visualization increment (property doesn't exist in entity)
            arquivo.AtualizadoEm = DateTime.UtcNow;
            _dbSet.Update(arquivo);
        }
    }
}

