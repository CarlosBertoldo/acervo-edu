using Microsoft.EntityFrameworkCore;
using AcervoEducacional.Domain.Entities;
using AcervoEducacional.Domain.Interfaces;
using AcervoEducacional.Infrastructure.Data;

namespace AcervoEducacional.Infrastructure.Repositories;

public class UsuarioRepository : BaseRepository<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(SimpleDbContext context) : base(context)
    {
    }

    public async Task<Usuario?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
    }

    public async Task<bool> EmailExistsAsync(string email, int? excludeId = null)
    {
        var query = _dbSet.Where(u => u.Email.ToLower() == email.ToLower());
        
        if (excludeId.HasValue)
        {
            query = query.Where(u => u.Id != excludeId.Value);
        }
        
        return await query.AnyAsync();
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _dbSet.AnyAsync(u => u.Email.ToLower() == email.ToLower());
    }

    public async Task<IEnumerable<Usuario>> GetUsuariosAtivosAsync()
    {
        return await _dbSet.Where(u => u.Status == Domain.Enums.StatusUsuario.Ativo && u.DeletadoEm == null).ToListAsync();
    }

    public async Task<(IEnumerable<Usuario> Items, int TotalCount)> GetPagedAsync(
        int page, 
        int pageSize, 
        string? search = null, 
        Domain.Enums.TipoUsuario? tipoUsuario = null, 
        Domain.Enums.StatusUsuario? status = null, 
        bool? ativo = null, 
        DateTime? criadoApartirDe = null, 
        DateTime? criadoAte = null, 
        string? sortBy = null, 
        string? sortDirection = null)
    {
        var query = _dbSet.AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchTerm = search.ToLower();
            query = query.Where(u => u.Nome.ToLower().Contains(searchTerm) || 
                                   u.Email.ToLower().Contains(searchTerm));
        }

        if (tipoUsuario.HasValue)
        {
            query = query.Where(u => u.Tipo == tipoUsuario.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(u => u.Status == status.Value);
        }

        if (ativo.HasValue)
        {
            if (ativo.Value)
                query = query.Where(u => u.DeletadoEm == null);
            else
                query = query.Where(u => u.DeletadoEm != null);
        }

        if (criadoApartirDe.HasValue)
        {
            query = query.Where(u => u.CriadoEm >= criadoApartirDe.Value);
        }

        if (criadoAte.HasValue)
        {
            query = query.Where(u => u.CriadoEm <= criadoAte.Value);
        }

        // Apply sorting
        if (!string.IsNullOrWhiteSpace(sortBy))
        {
            var isDescending = sortDirection?.ToLower() == "desc";
            query = sortBy.ToLower() switch
            {
                "nome" => isDescending ? query.OrderByDescending(u => u.Nome) : query.OrderBy(u => u.Nome),
                "email" => isDescending ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
                "criadoem" => isDescending ? query.OrderByDescending(u => u.CriadoEm) : query.OrderBy(u => u.CriadoEm),
                "ultimologin" => isDescending ? query.OrderByDescending(u => u.UltimoLogin) : query.OrderBy(u => u.UltimoLogin),
                _ => query.OrderBy(u => u.Nome)
            };
        }
        else
        {
            query = query.OrderBy(u => u.Nome);
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<int> CountActiveAdminsAsync()
    {
        return await _dbSet.CountAsync(u => u.Tipo == Domain.Enums.TipoUsuario.Administrador && 
                                          u.Status == Domain.Enums.StatusUsuario.Ativo && 
                                          u.DeletadoEm == null);
    }

    public async Task<bool> HasDependenciesAsync(int usuarioId)
    {
        // Check if user has created courses
        var hasCursos = await _context.Set<Curso>().AnyAsync(c => c.CriadoPor == usuarioId);
        
        // Check if user has activity logs
        var hasLogs = await _context.Set<LogAtividade>().AnyAsync(l => l.UsuarioId == usuarioId);
        
        return hasCursos || hasLogs;
    }

}

