using AcervoEducacional.Domain.Interfaces;
using AcervoEducacional.Infrastructure.Data;

namespace AcervoEducacional.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AcervoEducacionalContext _context;
    private bool _disposed = false;

    // Repositórios
    private IUsuarioRepository? _usuarioRepository;
    private ICursoRepository? _cursoRepository;
    private IArquivoRepository? _arquivoRepository;
    private ILogAtividadeRepository? _logAtividadeRepository;
    private IConfiguracaoSistemaRepository? _configuracaoSistemaRepository;

    public UnitOfWork(AcervoEducacionalContext context)
    {
        _context = context;
    }

    public IUsuarioRepository UsuarioRepository
    {
        get
        {
            _usuarioRepository ??= new UsuarioRepository(_context);
            return _usuarioRepository;
        }
    }

    public ICursoRepository CursoRepository
    {
        get
        {
            _cursoRepository ??= new CursoRepository(_context);
            return _cursoRepository;
        }
    }

    public IArquivoRepository ArquivoRepository
    {
        get
        {
            _arquivoRepository ??= new ArquivoRepository(_context);
            return _arquivoRepository;
        }
    }

    public ILogAtividadeRepository LogAtividadeRepository
    {
        get
        {
            _logAtividadeRepository ??= new LogAtividadeRepository(_context);
            return _logAtividadeRepository;
        }
    }

    public IConfiguracaoSistemaRepository ConfiguracaoSistemaRepository
    {
        get
        {
            _configuracaoSistemaRepository ??= new ConfiguracaoSistemaRepository(_context);
            return _configuracaoSistemaRepository;
        }
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_context.Database.CurrentTransaction != null)
        {
            await _context.Database.CommitTransactionAsync();
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_context.Database.CurrentTransaction != null)
        {
            await _context.Database.RollbackTransactionAsync();
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }
        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}

