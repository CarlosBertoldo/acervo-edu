using Microsoft.EntityFrameworkCore;
using AcervoEducacional.Domain.Entities;
using AcervoEducacional.Domain.Interfaces;
using AcervoEducacional.Infrastructure.Data;

namespace AcervoEducacional.Infrastructure.Repositories;

public class TokenRecuperacaoRepository : BaseRepository<TokenRecuperacao>, ITokenRecuperacaoRepository
{
    public TokenRecuperacaoRepository(AcervoEducacionalContext context) : base(context)
    {
    }

    public async Task<TokenRecuperacao?> GetByTokenAsync(string token)
    {
        return await _dbSet.FirstOrDefaultAsync(t => t.Token == token && !t.IsUsado && t.ExpiresAt > DateTime.UtcNow);
    }

    public async Task<IEnumerable<TokenRecuperacao>> GetByUsuarioIdAsync(int usuarioId)
    {
        return await _dbSet.Where(t => t.UsuarioId == usuarioId).ToListAsync();
    }

    public async Task InvalidateAllByUsuarioIdAsync(int usuarioId)
    {
        var tokens = await _dbSet.Where(t => t.UsuarioId == usuarioId && !t.IsUsado).ToListAsync();
        foreach (var token in tokens)
        {
            token.IsUsado = true;
            token.AtualizadoEm = DateTime.UtcNow;
        }
        _dbSet.UpdateRange(tokens);
    }

    public async Task InvalidarTokensUsuarioAsync(int usuarioId)
    {
        await InvalidateAllByUsuarioIdAsync(usuarioId);
    }

    public async Task LimparTokensExpiradosAsync()
    {
        var tokensExpirados = await _dbSet.Where(t => t.ExpiresAt < DateTime.UtcNow).ToListAsync();
        _dbSet.RemoveRange(tokensExpirados);
    }
}