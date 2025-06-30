using Microsoft.EntityFrameworkCore;
using AcervoEducacional.Domain.Entities;
using AcervoEducacional.Domain.Interfaces;
using AcervoEducacional.Infrastructure.Data;

namespace AcervoEducacional.Infrastructure.Repositories;

public class UsuarioRepository : BaseRepository<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(AcervoEducacionalContext context) : base(context)
    {
    }

    public async Task<Usuario?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
    }

    public async Task<bool> EmailExistsAsync(string email, Guid? excludeId = null)
    {
        var query = _dbSet.Where(u => u.Email.ToLower() == email.ToLower());
        
        if (excludeId.HasValue)
        {
            query = query.Where(u => u.Id != excludeId.Value);
        }
        
        return await query.AnyAsync();
    }

    public async Task<IEnumerable<Usuario>> GetByTipoAsync(Domain.Enums.TipoUsuario tipo)
    {
        return await _dbSet.Where(u => u.Tipo == tipo).ToListAsync();
    }

    public async Task<IEnumerable<Usuario>> GetAtivosAsync()
    {
        return await _dbSet.Where(u => u.Status == Domain.Enums.StatusUsuario.Ativo).ToListAsync();
    }

    public async Task<IEnumerable<Usuario>> SearchAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await GetAllAsync();

        var term = searchTerm.ToLower();
        return await _dbSet
            .Where(u => u.Nome.ToLower().Contains(term) ||
                       u.Email.ToLower().Contains(term) ||
                       (u.Departamento != null && u.Departamento.ToLower().Contains(term)) ||
                       (u.Cargo != null && u.Cargo.ToLower().Contains(term)))
            .ToListAsync();
    }

    public async Task<(IEnumerable<Usuario> Items, int TotalCount)> GetPagedWithFiltersAsync(
        int page,
        int pageSize,
        string? searchTerm = null,
        Domain.Enums.TipoUsuario? tipo = null,
        Domain.Enums.StatusUsuario? status = null,
        string? departamento = null)
    {
        var query = _dbSet.AsQueryable();

        // Aplicar filtros
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower();
            query = query.Where(u => u.Nome.ToLower().Contains(term) ||
                                   u.Email.ToLower().Contains(term) ||
                                   (u.Departamento != null && u.Departamento.ToLower().Contains(term)) ||
                                   (u.Cargo != null && u.Cargo.ToLower().Contains(term)));
        }

        if (tipo.HasValue)
        {
            query = query.Where(u => u.Tipo == tipo.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(u => u.Status == status.Value);
        }

        if (!string.IsNullOrWhiteSpace(departamento))
        {
            query = query.Where(u => u.Departamento != null && u.Departamento.ToLower().Contains(departamento.ToLower()));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(u => u.Nome)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Dictionary<string, int>> GetStatisticsAsync()
    {
        var total = await _dbSet.CountAsync();
        var ativos = await _dbSet.CountAsync(u => u.Status == Domain.Enums.StatusUsuario.Ativo);
        var admins = await _dbSet.CountAsync(u => u.Tipo == Domain.Enums.TipoUsuario.Administrador);
        var novos = await _dbSet.CountAsync(u => u.CreatedAt >= DateTime.UtcNow.AddDays(-30));

        return new Dictionary<string, int>
        {
            { "Total", total },
            { "Ativos", ativos },
            { "Administradores", admins },
            { "Novos", novos }
        };
    }
}

