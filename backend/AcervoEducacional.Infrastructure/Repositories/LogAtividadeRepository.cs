using Microsoft.EntityFrameworkCore;
using AcervoEducacional.Domain.Entities;
using AcervoEducacional.Domain.Interfaces;
using AcervoEducacional.Infrastructure.Data;

namespace AcervoEducacional.Infrastructure.Repositories;

public class LogAtividadeRepository : BaseRepository<LogAtividade>, ILogAtividadeRepository
{
    public LogAtividadeRepository(SimpleDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<LogAtividade>> GetByUsuarioIdAsync(int usuarioId)
    {
        return await _dbSet
            .Include(l => l.Usuario)
            .Include(l => l.Curso)
            .Where(l => l.UsuarioId == usuarioId)
            .OrderByDescending(l => l.CriadoEm)
            .ToListAsync();
    }

    public async Task<IEnumerable<LogAtividade>> GetRecentAsync(int count = 50)
    {
        return await _dbSet
            .Include(l => l.Usuario)
            .Include(l => l.Curso)
            .OrderByDescending(l => l.CriadoEm)
            .Take(count)
            .ToListAsync();
    }

    public async Task<(IEnumerable<LogAtividade> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        int? usuarioId = null,
        DateTime? dataInicio = null,
        DateTime? dataFim = null)
    {
        var query = _dbSet
            .Include(l => l.Usuario)
            .Include(l => l.Curso)
            .AsQueryable();

        if (usuarioId.HasValue)
        {
            query = query.Where(l => l.UsuarioId == usuarioId.Value);
        }

        if (dataInicio.HasValue)
        {
            query = query.Where(l => l.CriadoEm >= dataInicio.Value);
        }

        if (dataFim.HasValue)
        {
            query = query.Where(l => l.CriadoEm <= dataFim.Value);
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(l => l.CriadoEm)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<IEnumerable<LogAtividade>> GetByCursoIdAsync(int cursoId)
    {
        return await _dbSet
            .Include(l => l.Usuario)
            .Include(l => l.Curso)
            .Where(l => l.CursoId == cursoId)
            .OrderByDescending(l => l.CriadoEm)
            .ToListAsync();
    }

    public async Task<IEnumerable<LogAtividade>> GetByTipoAtividadeAsync(Domain.Enums.TipoAtividade tipoAtividade)
    {
        return await _dbSet
            .Include(l => l.Usuario)
            .Include(l => l.Curso)
            .Where(l => l.TipoAtividade == tipoAtividade)
            .OrderByDescending(l => l.CriadoEm)
            .ToListAsync();
    }

    public async Task<IEnumerable<LogAtividade>> GetRecentesAsync(int count = 50)
    {
        return await GetRecentAsync(count);
    }

    public async Task<IEnumerable<LogAtividade>> GetLogsSinceAsync(DateTime since)
    {
        return await _dbSet
            .Where(l => l.CriadoEm >= since)
            .OrderByDescending(l => l.CriadoEm)
            .ToListAsync();
    }

    public async Task<int> DeleteOldLogsAsync(DateTime cutoffDate)
    {
        var oldLogs = await _dbSet
            .Where(l => l.CriadoEm < cutoffDate)
            .ToListAsync();

        if (oldLogs.Any())
        {
            _dbSet.RemoveRange(oldLogs);
            await _context.SaveChangesAsync();
        }

        return oldLogs.Count;
    }

    public async Task<int> CountTotalLogsAsync()
    {
        return await _dbSet.CountAsync();
    }
}