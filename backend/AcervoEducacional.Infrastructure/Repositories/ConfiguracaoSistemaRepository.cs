using Microsoft.EntityFrameworkCore;
using AcervoEducacional.Domain.Entities;
using AcervoEducacional.Domain.Interfaces;
using AcervoEducacional.Infrastructure.Data;

namespace AcervoEducacional.Infrastructure.Repositories;

public class ConfiguracaoSistemaRepository : BaseRepository<ConfiguracaoSistema>, IConfiguracaoSistemaRepository
{
    public ConfiguracaoSistemaRepository(SimpleDbContext context) : base(context)
    {
    }

    public async Task<ConfiguracaoSistema?> GetByChaveAsync(string chave)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.Chave == chave);
    }

    public async Task<string?> GetValueAsync(string chave)
    {
        var config = await GetByChaveAsync(chave);
        return config?.Valor;
    }

    public async Task SetValueAsync(string chave, string valor, string? descricao = null)
    {
        var config = await GetByChaveAsync(chave);
        if (config != null)
        {
            config.Valor = valor;
            config.AtualizadoEm = DateTime.UtcNow;
            _dbSet.Update(config);
        }
        else
        {
            config = new ConfiguracaoSistema
            {
                Chave = chave,
                Valor = valor,
                Descricao = descricao,
                CriadoEm = DateTime.UtcNow
            };
            await _dbSet.AddAsync(config);
        }
    }

    public async Task<string?> GetValorAsync(string chave)
    {
        return await GetValueAsync(chave);
    }

    public async Task SetValorAsync(string chave, string valor)
    {
        await SetValueAsync(chave, valor);
    }
}