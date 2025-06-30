using Microsoft.EntityFrameworkCore;
using AcervoEducacional.Domain.Entities;
using AcervoEducacional.Domain.Enums;

namespace AcervoEducacional.Infrastructure.Data;

public class AcervoEducacionalContext : DbContext
{
    public AcervoEducacionalContext(DbContextOptions<AcervoEducacionalContext> options) : base(options)
    {
    }

    // DbSets
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Curso> Cursos { get; set; }
    public DbSet<Arquivo> Arquivos { get; set; }
    public DbSet<LogAtividade> LogsAtividade { get; set; }
    public DbSet<SessaoUsuario> SessoesUsuario { get; set; }
    public DbSet<TokenRecuperacao> TokensRecuperacao { get; set; }
    public DbSet<ConfiguracaoSistema> ConfiguracoesSistema { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuração da entidade Usuario
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.SenhaHash).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Tipo).HasConversion<string>();
            entity.Property(e => e.Status).HasConversion<string>();
            entity.Property(e => e.Departamento).HasMaxLength(100);
            entity.Property(e => e.Cargo).HasMaxLength(100);
            entity.Property(e => e.Telefone).HasMaxLength(20);
            entity.Property(e => e.Avatar).HasMaxLength(500);
            
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Configuração da entidade Curso
        modelBuilder.Entity<Curso>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Codigo).IsRequired().HasMaxLength(50);
            entity.Property(e => e.DescricaoAcademia).HasMaxLength(500);
            entity.Property(e => e.Status).HasConversion<string>();
            entity.Property(e => e.TipoAmbiente).HasConversion<string>();
            entity.Property(e => e.TipoAcesso).HasConversion<string>();
            entity.Property(e => e.Origem).HasConversion<string>();
            entity.Property(e => e.ComentariosInternos).HasMaxLength(1000);
            entity.Property(e => e.Tags).HasMaxLength(500);
            
            entity.HasIndex(e => e.Codigo).IsUnique();
            
            // Relacionamentos
            entity.HasOne(e => e.CriadoPorUsuario)
                  .WithMany()
                  .HasForeignKey(e => e.CriadoPor)
                  .OnDelete(DeleteBehavior.Restrict);
                  
            entity.HasMany(e => e.Arquivos)
                  .WithOne(a => a.Curso)
                  .HasForeignKey(a => a.CursoId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuração da entidade Arquivo
        modelBuilder.Entity<Arquivo>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(255);
            entity.Property(e => e.NomeOriginal).IsRequired().HasMaxLength(255);
            entity.Property(e => e.CaminhoArquivo).IsRequired().HasMaxLength(500);
            entity.Property(e => e.TipoMime).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Categoria).HasConversion<string>();
            entity.Property(e => e.TipoCompartilhamento).HasConversion<string>();
            entity.Property(e => e.Descricao).HasMaxLength(500);
            entity.Property(e => e.Tags).HasMaxLength(500);
            entity.Property(e => e.HashArquivo).HasMaxLength(64);
            entity.Property(e => e.UrlPublica).HasMaxLength(500);
            entity.Property(e => e.CodigoEmbed).HasMaxLength(1000);
            entity.Property(e => e.DominiosPermitidos).HasMaxLength(500);
            entity.Property(e => e.SenhaAcesso).HasMaxLength(255);
            
            // Relacionamentos
            entity.HasOne(e => e.Curso)
                  .WithMany(c => c.Arquivos)
                  .HasForeignKey(e => e.CursoId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasOne(e => e.UploadPorUsuario)
                  .WithMany()
                  .HasForeignKey(e => e.UploadPor)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuração da entidade LogAtividade
        modelBuilder.Entity<LogAtividade>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TipoAcao).HasConversion<string>();
            entity.Property(e => e.Descricao).IsRequired().HasMaxLength(500);
            entity.Property(e => e.EnderecoIp).HasMaxLength(45);
            entity.Property(e => e.UserAgent).HasMaxLength(500);
            entity.Property(e => e.DetalhesJson).HasMaxLength(2000);
            
            // Relacionamentos
            entity.HasOne(e => e.Usuario)
                  .WithMany()
                  .HasForeignKey(e => e.UsuarioId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuração da entidade SessaoUsuario
        modelBuilder.Entity<SessaoUsuario>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Token).IsRequired().HasMaxLength(255);
            entity.Property(e => e.EnderecoIp).HasMaxLength(45);
            entity.Property(e => e.UserAgent).HasMaxLength(500);
            
            entity.HasIndex(e => e.Token).IsUnique();
            
            // Relacionamentos
            entity.HasOne(e => e.Usuario)
                  .WithMany()
                  .HasForeignKey(e => e.UsuarioId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuração da entidade TokenRecuperacao
        modelBuilder.Entity<TokenRecuperacao>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Token).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            
            entity.HasIndex(e => e.Token).IsUnique();
        });

        // Configuração da entidade ConfiguracaoSistema
        modelBuilder.Entity<ConfiguracaoSistema>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Chave).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Valor).HasMaxLength(1000);
            entity.Property(e => e.Descricao).HasMaxLength(500);
            entity.Property(e => e.Tipo).HasConversion<string>();
            
            entity.HasIndex(e => e.Chave).IsUnique();
        });

        // Dados iniciais
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Usuário administrador padrão
        modelBuilder.Entity<Usuario>().HasData(
            new Usuario
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Nome = "Administrador",
                Email = "admin@acervo.com",
                SenhaHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                Tipo = TipoUsuario.Administrador,
                Status = StatusUsuario.Ativo,
                Departamento = "TI",
                Cargo = "Administrador do Sistema",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        );

        // Configurações padrão do sistema
        var configId = 1;
        modelBuilder.Entity<ConfiguracaoSistema>().HasData(
            new ConfiguracaoSistema
            {
                Id = configId++,
                Chave = "SISTEMA_NOME",
                Valor = "Sistema Acervo Educacional",
                Descricao = "Nome do sistema",
                Tipo = TipoConfiguracao.Texto,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new ConfiguracaoSistema
            {
                Id = configId++,
                Chave = "SISTEMA_VERSAO",
                Valor = "1.0.0",
                Descricao = "Versão do sistema",
                Tipo = TipoConfiguracao.Texto,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new ConfiguracaoSistema
            {
                Id = configId++,
                Chave = "JWT_SECRET",
                Valor = "AcervoEducacional2024!@#$%^&*()_+SecretKey",
                Descricao = "Chave secreta para JWT",
                Tipo = TipoConfiguracao.Senha,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new ConfiguracaoSistema
            {
                Id = configId++,
                Chave = "JWT_EXPIRATION_HOURS",
                Valor = "24",
                Descricao = "Tempo de expiração do JWT em horas",
                Tipo = TipoConfiguracao.Numero,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        );
    }
}

