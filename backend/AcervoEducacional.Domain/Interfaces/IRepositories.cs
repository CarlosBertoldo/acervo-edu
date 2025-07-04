using AcervoEducacional.Domain.Entities;
using AcervoEducacional.Domain.Enums;
using System.Linq.Expressions;

namespace AcervoEducacional.Domain.Interfaces;

public interface IBaseRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
}

public interface IUsuarioRepository : IBaseRepository<Usuario>
{
    Task<Usuario?> GetByEmailAsync(string email);
    Task<bool> EmailExistsAsync(string email);
    Task<IEnumerable<Usuario>> GetUsuariosAtivosAsync();
    Task<(IEnumerable<Usuario> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string? search = null, TipoUsuario? tipoUsuario = null, StatusUsuario? status = null, bool? ativo = null, DateTime? criadoApartirDe = null, DateTime? criadoAte = null, string? sortBy = null, string? sortDirection = null);
    Task<int> CountActiveAdminsAsync();
    Task<bool> HasDependenciesAsync(int usuarioId);
}

public interface ICursoRepository : IBaseRepository<Curso>
{
    Task<Curso?> GetByCodigoAsync(string codigo);
    Task<bool> CodigoExistsAsync(string codigo);
    Task<IEnumerable<Curso>> GetByStatusAsync(StatusCurso status);
    Task<IEnumerable<Curso>> GetByOrigemAsync(OrigemCurso origem);
    Task<IEnumerable<Curso>> SearchAsync(string searchTerm);
}

public interface IArquivoRepository : IBaseRepository<Arquivo>
{
    Task<IEnumerable<Arquivo>> GetByCursoIdAsync(int cursoId);
    Task<IEnumerable<Arquivo>> GetByCategoriaAsync(CategoriaArquivo categoria);
    Task<IEnumerable<Arquivo>> GetPublicosAsync();
    Task<long> GetTamanhoTotalByCursoAsync(int cursoId);
    Task<Arquivo?> GetByHashAsync(string hash);
    Task<(IEnumerable<Arquivo> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string? search = null, int? cursoId = null, CategoriaArquivo? categoria = null, string? tipoMime = null, bool? isPublico = null, DateTime? criadoApartirDe = null, DateTime? criadoAte = null, long? tamanhoMin = null, long? tamanhoMax = null, string? sortBy = null, string? sortDirection = null);
}

public interface ILogAtividadeRepository : IBaseRepository<LogAtividade>
{
    Task<IEnumerable<LogAtividade>> GetByUsuarioIdAsync(int usuarioId);
    Task<IEnumerable<LogAtividade>> GetByCursoIdAsync(int cursoId);
    Task<IEnumerable<LogAtividade>> GetByTipoAtividadeAsync(TipoAtividade tipo);
    Task<IEnumerable<LogAtividade>> GetRecentesAsync(int count = 10);
}

public interface ISessaoUsuarioRepository : IBaseRepository<SessaoUsuario>
{
    Task<SessaoUsuario?> GetByTokenAsync(string token);
    Task<SessaoUsuario?> GetByRefreshTokenAsync(string refreshToken);
    Task<IEnumerable<SessaoUsuario>> GetByUsuarioIdAsync(int usuarioId);
    Task RevogarSessoesUsuarioAsync(int usuarioId);
    Task LimparSessoesExpiradasAsync();
}

public interface ITokenRecuperacaoRepository : IBaseRepository<TokenRecuperacao>
{
    Task<TokenRecuperacao?> GetByTokenAsync(string token);
    Task<IEnumerable<TokenRecuperacao>> GetByUsuarioIdAsync(int usuarioId);
    Task InvalidarTokensUsuarioAsync(int usuarioId);
    Task LimparTokensExpiradosAsync();
}

public interface IConfiguracaoSistemaRepository : IBaseRepository<ConfiguracaoSistema>
{
    Task<ConfiguracaoSistema?> GetByChaveAsync(string chave);
    Task<string?> GetValorAsync(string chave);
    Task SetValorAsync(string chave, string valor);
}

public interface IUnitOfWork : IDisposable
{
    IUsuarioRepository Usuarios { get; }
    ICursoRepository Cursos { get; }
    IArquivoRepository Arquivos { get; }
    ILogAtividadeRepository LogsAtividade { get; }
    ISessaoUsuarioRepository SessoesUsuario { get; }
    ITokenRecuperacaoRepository TokensRecuperacao { get; }
    IConfiguracaoSistemaRepository ConfiguracoesSistema { get; }
    
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}

