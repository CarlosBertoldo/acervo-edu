using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ========== CONFIGURAÇÃO DA AUTENTICAÇÃO JWT ==========
var jwtKey = builder.Configuration["JWT:SecretKey"] ?? "AcervoEducacional2024!@#$%^&*()_+SecretKeyForProduction";
var jwtIssuer = builder.Configuration["JWT:Issuer"] ?? "AcervoEducacional";
var jwtAudience = builder.Configuration["JWT:Audience"] ?? "AcervoEducacionalUsers";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// ========== CONFIGURAÇÃO BÁSICA DOS SERVIÇOS ==========

// ========== CONFIGURAÇÃO DO CORS ==========
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultPolicy", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        }
        else
        {
            policy.WithOrigins(
                      "https://acervo.ferreiracosta.com",
                      "https://www.ferreiracosta.com"
                  )
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        }
    });
});

// ========== CONFIGURAÇÃO DO MVC ==========
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        options.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
    });

builder.Services.AddEndpointsApiExplorer();

// ========== CONFIGURAÇÃO DO SWAGGER ==========
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Acervo Educacional API", 
        Version = "v1.0.0",
        Description = "Sistema de Acervo Educacional Ferreira Costa - API REST completa para gestão de cursos e arquivos educacionais",
        Contact = new OpenApiContact
        {
            Name = "Ferreira Costa",
            Url = new Uri("https://www.ferreiracosta.com")
        }
    });
    
    // Configuração de autenticação JWT para Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Exemplo: 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// ========== VERSÃO SIMPLIFICADA SEM BANCO ==========

// ========== CONFIGURAÇÃO DO PIPELINE HTTP ==========
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Acervo Educacional API v1");
        c.RoutePrefix = "swagger";
        c.DisplayRequestDuration();
        c.EnableDeepLinking();
    });
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Rate Limiting básico (removido por simplicidade)

// CORS
app.UseCors("DefaultPolicy");

// Security Headers
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
    await next();
});

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Controllers
app.MapControllers();

// Hangfire removido por simplicidade

// Health Check
app.MapGet("/health", () => new { 
    Status = "Healthy", 
    DateTime = DateTime.UtcNow,
    Environment = app.Environment.EnvironmentName,
    Version = "1.0.0"
}).WithName("HealthCheck").WithTags("Health");

// Root endpoint
app.MapGet("/", () => new {
    Message = "🚀 Sistema Acervo Educacional Ferreira Costa - API REST",
    Version = "1.0.0",
    Environment = app.Environment.EnvironmentName,
    Documentation = "/swagger"
}).WithName("Root").WithTags("Info");

// ========== INICIALIZAÇÃO ==========
var port = builder.Configuration["ASPNETCORE_URLS"]?.Split(':').LastOrDefault()?.TrimEnd('/') ?? "5000";

Console.WriteLine("🚀 Sistema Acervo Educacional Ferreira Costa");
Console.WriteLine("===============================================");
Console.WriteLine($"🌐 API: http://localhost:{port}");
Console.WriteLine($"📖 Swagger: http://localhost:{port}/swagger");
// Hangfire removido da versão simplificada
Console.WriteLine($"🏥 Health: http://localhost:{port}/health");
Console.WriteLine($"🎯 Environment: {app.Environment.EnvironmentName}");
Console.WriteLine("===============================================");

app.Run("http://0.0.0.0:5105");