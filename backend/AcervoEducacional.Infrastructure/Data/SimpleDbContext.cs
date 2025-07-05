using Microsoft.EntityFrameworkCore;
using AcervoEducacional.Domain.Entities;
using AcervoEducacional.Domain.Enums;

namespace AcervoEducacional.Infrastructure.Data;

public class SimpleDbContext : DbContext
{
    public SimpleDbContext(DbContextOptions<SimpleDbContext> options) : base(options)
    {
    }

    // DbSets básicos para autenticação
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<LogAtividade> LogsAtividade { get; set; }
    public DbSet<SessaoUsuario> SessoesUsuario { get; set; }
    public DbSet<TokenRecuperacao> TokensRecuperacao { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuração simplificada da entidade Usuario
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

        // Configuração simplificada da entidade LogAtividade
        modelBuilder.Entity<LogAtividade>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TipoAtividade).HasConversion<string>();
            entity.Property(e => e.Descricao).IsRequired().HasMaxLength(500);
            entity.Property(e => e.EnderecoIp).HasMaxLength(45);
            entity.Property(e => e.UserAgent).HasMaxLength(500);
        });

        // Configuração simplificada da entidade SessaoUsuario
        modelBuilder.Entity<SessaoUsuario>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Token).IsRequired().HasMaxLength(255);
            entity.Property(e => e.IpAddress).HasMaxLength(45);
            entity.Property(e => e.UserAgent).HasMaxLength(500);
            
            entity.HasIndex(e => e.Token).IsUnique();
        });

        // Configuração simplificada da entidade TokenRecuperacao
        modelBuilder.Entity<TokenRecuperacao>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Token).IsRequired().HasMaxLength(255);
            
            entity.HasIndex(e => e.Token).IsUnique();
        });

        // Dados iniciais - usuário administrador
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Usuário administrador padrão
        modelBuilder.Entity<Usuario>().HasData(
            new Usuario
            {
                Id = 1,
                Nome = "Administrador",
                Email = "admin@acervo.com",
                SenhaHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                Tipo = TipoUsuario.Administrador,
                Status = StatusUsuario.Ativo,
                Departamento = "TI",
                Cargo = "Administrador do Sistema",
                CriadoEm = DateTime.UtcNow,
                AtualizadoEm = DateTime.UtcNow
            }
        );
    }
}