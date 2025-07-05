using Microsoft.EntityFrameworkCore;
using AcervoEducacional.Domain.Entities;
using AcervoEducacional.Domain.Interfaces;
using AcervoEducacional.Infrastructure.Data;

namespace AcervoEducacional.Infrastructure.Repositories;

public class SessaoUsuarioRepository : BaseRepository<SessaoUsuario>, ISessaoUsuarioRepository
{
    public SessaoUsuarioRepository(SimpleDbContext context) : base(context)
    {
    }

    public async Task<SessaoUsuario?> GetByTokenAsync(string token)
    {
        return await _dbSet.FirstOrDefaultAsync(s => s.Token == token);
    }

    public async Task<IEnumerable<SessaoUsuario>> GetByUsuarioIdAsync(int usuarioId)
    {
        return await _dbSet.Where(s => s.UsuarioId == usuarioId).ToListAsync();
    }

    public async Task InvalidateAllByUsuarioIdAsync(int usuarioId)
    {
        var sessoes = await _dbSet.Where(s => s.UsuarioId == usuarioId && !s.IsRevogada).ToListAsync();
        foreach (var sessao in sessoes)
        {
            sessao.IsRevogada = true;
            sessao.AtualizadoEm = DateTime.UtcNow;
        }
        _dbSet.UpdateRange(sessoes);
    }

    public async Task<SessaoUsuario?> GetByRefreshTokenAsync(string refreshToken)
    {
        return await _dbSet.FirstOrDefaultAsync(s => s.RefreshToken == refreshToken && !s.IsRevogada);
    }

    public async Task RevogarSessoesUsuarioAsync(int usuarioId)
    {
        await InvalidateAllByUsuarioIdAsync(usuarioId);
    }

    public async Task LimparSessoesExpiradasAsync()
    {
        var sessoesExpiradas = await _dbSet.Where(s => s.ExpiresAt < DateTime.UtcNow).ToListAsync();
        foreach (var sessao in sessoesExpiradas)
        {
            sessao.IsRevogada = true;
            sessao.AtualizadoEm = DateTime.UtcNow;
        }
        _dbSet.UpdateRange(sessoesExpiradas);
    }
}