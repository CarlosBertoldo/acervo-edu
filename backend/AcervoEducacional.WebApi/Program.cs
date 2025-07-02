using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AcervoEducacional.Infrastructure.Data;
using AcervoEducacional.Application.Interfaces;
using AcervoEducacional.Infrastructure.Services;
using AcervoEducacional.Domain.Interfaces;
using AcervoEducacional.Infrastructure.Repositories;
using Hangfire;
using Hangfire.PostgreSql;
using Serilog;
using AspNetCoreRateLimit;
using AcervoEducacional.WebApi.Middleware;
using AcervoEducacional.WebApi.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Configurar Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/acervo-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Configurar serviço de credenciais seguras
builder.Services.AddCredentialsService();

// Add services to the container.
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configurar Entity Framework com credenciais seguras
builder.Services.AddDbContext<AcervoEducacionalContext>((serviceProvider, options) =>
{
    var credentialsService = serviceProvider.GetRequiredService<ICredentialsService>();
    var connectionString = credentialsService.GetDatabaseConnectionString();
    options.UseNpgsql(connectionString);
});

// Configurar JWT com credenciais seguras
var jwtSettings = builder.Configuration.GetSection("JwtSettings");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                builder.Services.BuildServiceProvider().GetRequiredService<ICredentialsService>().GetJwtSecretKey()
            )),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// Configurar AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Configurar CORS com base nas flags de segurança
builder.Services.AddCors(options =>
{
    var enableStrictCors = builder.Configuration.GetValue<bool>("Security:EnableStrictCors", false);
    
    if (enableStrictCors)
    {
        // CORS restrito para produção
        var allowedOrigins = Environment.GetEnvironmentVariable("CORS_ALLOWED_ORIGINS")?.Split(',') 
            ?? new[] { "https://acervo.ferreiracosta.com", "https://app.ferreiracosta.com" };
            
        options.AddPolicy("Production", policy =>
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        });
        
        // Log da configuração
        var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();
        logger.LogInformation("🔒 CORS RESTRITO ativado para domínios: {Origins}", string.Join(", ", allowedOrigins));
    }
    else
    {
        // CORS permissivo para desenvolvimento
        options.AddPolicy("Development", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
        
        // Log da configuração
        var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();
        logger.LogInformation("🛠️ CORS PERMISSIVO ativado para desenvolvimento");
    }
});

// Configurar Rate Limiting
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.Configure<IpRateLimitPolicies>(builder.Configuration.GetSection("IpRateLimitPolicies"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

// Configurar Hangfire
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHangfireServer();

// Registrar serviços
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICursoService, CursoService>();
builder.Services.AddScoped<IArquivoService, ArquivoService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IFileStorageService, AwsS3Service>();

// Registrar repositórios
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<ICursoRepository, CursoRepository>();
builder.Services.AddScoped<IArquivoRepository, ArquivoRepository>();
builder.Services.AddScoped<ILogAtividadeRepository, LogAtividadeRepository>();
builder.Services.AddScoped<ISessaoUsuarioRepository, SessaoUsuarioRepository>();
builder.Services.AddScoped<ITokenRecuperacaoRepository, TokenRecuperacaoRepository>();
builder.Services.AddScoped<IConfiguracaoSistemaRepository, ConfiguracaoSistemaRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Aplicar Headers de Segurança
app.UseSecurityHeaders();

// Aplicar CORS baseado na configuração de segurança
var enableStrictCors = app.Configuration.GetValue<bool>("Security:EnableStrictCors", false);
if (enableStrictCors)
{
    app.UseCors("Production");
}
else
{
    app.UseCors("Development");
}

// Aplicar Rate Limiting
app.UseIpRateLimiting();

app.UseAuthentication();

// Aplicar proteção BOLA (ativada apenas em produção via flag)
app.UseObjectLevelAuthorization();

app.UseAuthorization();

app.UseHangfireDashboard("/hangfire");

app.MapControllers();

// Aplicar migrations automaticamente em desenvolvimento
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AcervoEducacionalContext>();
    context.Database.Migrate();
}

app.Run();

