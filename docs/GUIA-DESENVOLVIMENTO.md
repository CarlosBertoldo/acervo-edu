# 👨‍💻 Guia de Desenvolvimento - Sistema Acervo Educacional Ferreira Costa

Este guia é destinado a desenvolvedores que desejam contribuir, expandir ou manter o Sistema Acervo Educacional da Ferreira Costa.

## 🏗️ **Arquitetura do Sistema**

### **Visão Geral**
O sistema segue os princípios de **Clean Architecture** com separação clara de responsabilidades:

```
┌─────────────────────────────────────────────────────────────┐
│                    Presentation Layer                       │
│  ┌─────────────────┐    ┌─────────────────────────────────┐ │
│  │   React Frontend │    │      .NET WebApi Controllers   │ │
│  │   (TailwindCSS)  │    │      (Swagger/OpenAPI)         │ │
│  └─────────────────┘    └─────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────┘
                                    │
┌─────────────────────────────────────────────────────────────┐
│                   Application Layer                         │
│  ┌─────────────────────────────────────────────────────────┐ │
│  │  Services (ArquivoService, UsuarioService, etc.)       │ │
│  │  DTOs, Interfaces, Responses                           │ │
│  └─────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────┘
                                    │
┌─────────────────────────────────────────────────────────────┐
│                     Domain Layer                            │
│  ┌─────────────────────────────────────────────────────────┐ │
│  │  Entities (Usuario, Curso, Arquivo)                    │ │
│  │  Enums, Interfaces, Business Rules                     │ │
│  └─────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────┘
                                    │
┌─────────────────────────────────────────────────────────────┐
│                 Infrastructure Layer                        │
│  ┌─────────────────────────────────────────────────────────┐ │
│  │  Repositories, DbContext, External Services            │ │
│  │  PostgreSQL, Email, AWS S3, Senior Integration         │ │
│  └─────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────┘
```

### **Tecnologias Utilizadas**

#### **Frontend**
- **React 19** - Framework principal
- **Vite** - Build tool e dev server
- **TailwindCSS** - Framework CSS com paleta Ferreira Costa
- **React Router DOM** - Roteamento
- **Lucide React** - Ícones
- **Recharts** - Gráficos

#### **Backend**
- **.NET 8** - Framework principal
- **Entity Framework Core** - ORM
- **PostgreSQL** - Banco de dados
- **JWT Bearer** - Autenticação
- **BCrypt** - Criptografia de senhas
- **Swagger/OpenAPI** - Documentação da API

## 📁 **Estrutura de Pastas Detalhada**

### **Backend (.NET)**
```
backend/
├── AcervoEducacional.Domain/
│   ├── Entities/                    # Entidades do domínio
│   │   ├── Usuario.cs              # Entidade de usuário
│   │   ├── Curso.cs                # Entidade de curso
│   │   ├── Arquivo.cs              # Entidade de arquivo
│   │   └── LogAtividade.cs         # Logs de auditoria
│   ├── Enums/                      # Enumerações
│   │   ├── TipoUsuario.cs          # Admin, Gestor, Usuario
│   │   ├── StatusCurso.cs          # Status do curso
│   │   └── TipoArquivo.cs          # Tipos de arquivo
│   └── Interfaces/                 # Interfaces do domínio
│       └── IRepositories.cs        # Interfaces de repositórios
│
├── AcervoEducacional.Application/
│   ├── DTOs/                       # Data Transfer Objects
│   │   ├── Auth/                   # DTOs de autenticação
│   │   ├── Curso/                  # DTOs de curso
│   │   ├── Usuario/                # DTOs de usuário
│   │   ├── Arquivo/                # DTOs de arquivo
│   │   └── Common/                 # DTOs comuns
│   ├── Interfaces/                 # Interfaces de serviços
│   │   └── IServices.cs            # Interfaces dos services
│   └── Services/                   # Implementação dos serviços
│       ├── AuthService.cs          # Autenticação JWT
│       ├── UsuarioService.cs       # Gestão de usuários
│       ├── CursoService.cs         # Gestão de cursos
│       ├── ArquivoService.cs       # Gestão de arquivos
│       ├── ReportService.cs        # Relatórios
│       ├── EmailService.cs         # Envio de emails
│       └── SecurityService.cs      # Segurança e validações
│
├── AcervoEducacional.Infrastructure/
│   ├── Data/                       # Configuração do banco
│   │   ├── AcervoDbContext.cs      # DbContext principal
│   │   └── Configurations/         # Configurações EF
│   ├── Repositories/               # Implementação dos repositórios
│   │   ├── BaseRepository.cs       # Repositório genérico
│   │   ├── UsuarioRepository.cs    # Repositório de usuários
│   │   ├── CursoRepository.cs      # Repositório de cursos
│   │   └── ArquivoRepository.cs    # Repositório de arquivos
│   └── Services/                   # Serviços de infraestrutura
│       ├── EmailService.cs         # Implementação SMTP
│       └── AwsS3Service.cs         # Integração AWS S3
│
└── AcervoEducacional.WebApi/
    ├── Controllers/                # Controllers da API
    │   ├── AuthController.cs       # Endpoints de autenticação
    │   ├── UsuarioController.cs    # Endpoints de usuários
    │   ├── CursoController.cs      # Endpoints de cursos
    │   ├── ArquivoController.cs    # Endpoints de arquivos
    │   └── ReportController.cs     # Endpoints de relatórios
    ├── Middleware/                 # Middleware customizado
    │   ├── JwtMiddleware.cs        # Autenticação automática
    │   └── ErrorHandlingMiddleware.cs # Tratamento de erros
    ├── Configuration/              # Configurações
    │   └── SwaggerConfiguration.cs # Configuração do Swagger
    ├── HealthChecks/              # Health checks
    │   ├── DatabaseHealthCheck.cs  # Monitoramento do banco
    │   └── EmailHealthCheck.cs     # Monitoramento do email
    └── wwwroot/swagger-ui/        # Assets customizados
        ├── custom.css             # CSS com identidade FC
        └── custom.js              # JavaScript customizado
```

### **Frontend (React)**
```
frontend/
├── src/
│   ├── components/                 # Componentes reutilizáveis
│   │   ├── Layout.jsx             # Layout principal
│   │   ├── Sidebar.jsx            # Barra lateral
│   │   ├── Header.jsx             # Cabeçalho
│   │   └── ui/                    # Componentes de UI
│   ├── pages/                     # Páginas da aplicação
│   │   ├── Dashboard.jsx          # Dashboard principal
│   │   ├── Kanban.jsx             # Sistema Kanban
│   │   ├── Cursos.jsx             # Gestão de cursos
│   │   ├── Usuarios.jsx           # Gestão de usuários
│   │   ├── Logs.jsx               # Logs e relatórios
│   │   └── Configuracoes.jsx      # Configurações
│   ├── services/                  # Serviços e API
│   │   ├── api.js                 # Cliente HTTP
│   │   ├── auth.js                # Serviços de autenticação
│   │   └── endpoints.js           # Endpoints da API
│   ├── contexts/                  # Contextos React
│   │   ├── AuthContext.jsx        # Contexto de autenticação
│   │   └── ThemeContext.jsx       # Contexto de tema
│   ├── hooks/                     # Hooks customizados
│   │   ├── useAuth.js             # Hook de autenticação
│   │   └── useApi.js              # Hook para API
│   ├── utils/                     # Utilitários
│   │   ├── constants.js           # Constantes
│   │   ├── helpers.js             # Funções auxiliares
│   │   └── validators.js          # Validações
│   ├── assets/                    # Assets estáticos
│   │   ├── ferreira-costa-logo.png
│   │   └── ferreira-costa-logo-white.png
│   ├── App.jsx                    # Componente principal
│   ├── App.css                    # Estilos globais
│   ├── index.css                  # Configuração TailwindCSS
│   └── main.jsx                   # Ponto de entrada
├── public/
│   ├── favicon-ferreira-costa.png # Favicon personalizado
│   └── index.html                 # Template HTML
├── package.json                   # Dependências
└── vite.config.js                 # Configuração Vite
```

## 🎨 **Padrões de Desenvolvimento**

### **1. Convenções de Nomenclatura**

#### **Backend (C#)**
```csharp
// Classes: PascalCase
public class UsuarioService { }

// Métodos: PascalCase
public async Task<UsuarioResponse> GetByIdAsync(Guid id) { }

// Propriedades: PascalCase
public string Nome { get; set; }

// Variáveis locais: camelCase
var usuarioAtivo = await _repository.GetByIdAsync(id);

// Constantes: UPPER_CASE
public const string DEFAULT_ROLE = "USUARIO";

// Interfaces: I + PascalCase
public interface IUsuarioService { }
```

#### **Frontend (JavaScript/React)**
```javascript
// Componentes: PascalCase
const Dashboard = () => { }

// Funções: camelCase
const handleSubmit = () => { }

// Variáveis: camelCase
const isLoading = false;

// Constantes: UPPER_CASE
const API_BASE_URL = 'http://localhost:5000';

// Arquivos: kebab-case
// user-profile.jsx, api-client.js
```

### **2. Padrões de Código**

#### **Services (Application Layer)**
```csharp
public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _repository;
    private readonly ISecurityService _securityService;
    private readonly ILogger<UsuarioService> _logger;

    public UsuarioService(
        IUsuarioRepository repository,
        ISecurityService securityService,
        ILogger<UsuarioService> logger)
    {
        _repository = repository;
        _securityService = securityService;
        _logger = logger;
    }

    public async Task<ApiResponse<UsuarioResponse>> GetByIdAsync(Guid id)
    {
        try
        {
            _logger.LogInformation("Buscando usuário {UserId}", id);
            
            var usuario = await _repository.GetByIdAsync(id);
            if (usuario == null)
            {
                _logger.LogWarning("Usuário {UserId} não encontrado", id);
                return ApiResponse<UsuarioResponse>.Error("Usuário não encontrado");
            }

            var response = new UsuarioResponse
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Tipo = usuario.Tipo.ToString(),
                Status = usuario.Status.ToString()
            };

            return ApiResponse<UsuarioResponse>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar usuário {UserId}", id);
            return ApiResponse<UsuarioResponse>.Error("Erro interno do servidor");
        }
    }
}
```

#### **Controllers (WebApi Layer)**
```csharp
[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class UsuarioController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;
    private readonly ILogger<UsuarioController> _logger;

    public UsuarioController(
        IUsuarioService usuarioService,
        ILogger<UsuarioController> logger)
    {
        _usuarioService = usuarioService;
        _logger = logger;
    }

    /// <summary>
    /// Obtém um usuário por ID
    /// </summary>
    /// <param name="id">ID do usuário</param>
    /// <returns>Dados do usuário</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<UsuarioResponse>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _usuarioService.GetByIdAsync(id);
        
        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }
}
```

#### **Componentes React**
```jsx
import React, { useState, useEffect } from 'react';
import { useAuth } from '../hooks/useAuth';
import { api } from '../services/api';

const Dashboard = () => {
  const [metrics, setMetrics] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const { user } = useAuth();

  useEffect(() => {
    const fetchMetrics = async () => {
      try {
        setLoading(true);
        const response = await api.get('/report/dashboard');
        setMetrics(response.data);
      } catch (err) {
        setError('Erro ao carregar métricas');
        console.error('Erro ao buscar métricas:', err);
      } finally {
        setLoading(false);
      }
    };

    fetchMetrics();
  }, []);

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-fc-red"></div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="bg-red-50 border border-red-200 rounded-lg p-4">
        <p className="text-red-600">{error}</p>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold text-gray-900">
          Dashboard - Bem-vindo, {user?.nome}!
        </h1>
      </div>
      
      {/* Métricas */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        {/* Cards de métricas */}
      </div>
    </div>
  );
};

export default Dashboard;
```

### **3. Tratamento de Erros**

#### **Backend**
```csharp
// Middleware de tratamento global
public class ErrorHandlingMiddleware
{
    public async Task InvokeAsync(HttpContext context, Exception exception)
    {
        var response = new ApiResponse<object>
        {
            Success = false,
            Message = GetUserFriendlyMessage(exception),
            Error = _environment.IsDevelopment() ? exception.Message : "Erro interno"
        };

        context.Response.StatusCode = GetStatusCode(exception);
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}

// Exceções customizadas
public class BusinessException : Exception
{
    public string ErrorCode { get; }
    
    public BusinessException(string message, string errorCode = "BUSINESS_ERROR") 
        : base(message)
    {
        ErrorCode = errorCode;
    }
}
```

#### **Frontend**
```javascript
// Hook para tratamento de erros da API
export const useApi = () => {
  const handleApiError = (error) => {
    if (error.response?.status === 401) {
      // Redirecionar para login
      window.location.href = '/login';
    } else if (error.response?.status === 403) {
      // Mostrar mensagem de acesso negado
      toast.error('Acesso negado');
    } else {
      // Erro genérico
      const message = error.response?.data?.message || 'Erro interno do servidor';
      toast.error(message);
    }
  };

  return { handleApiError };
};
```

## 🔐 **Segurança**

### **1. Autenticação JWT**

#### **Geração de Token**
```csharp
public class AuthService
{
    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        // Validar credenciais
        var usuario = await _repository.GetByEmailAsync(request.Email);
        if (usuario == null || !_securityService.VerifyPassword(request.Senha, usuario.SenhaHash))
        {
            await _securityService.LogFailedLoginAttempt(request.Email, GetClientIp());
            return AuthResponse.Error("Credenciais inválidas");
        }

        // Gerar tokens
        var accessToken = GenerateAccessToken(usuario);
        var refreshToken = GenerateRefreshToken();

        // Salvar refresh token
        await _repository.SaveRefreshTokenAsync(usuario.Id, refreshToken);

        return AuthResponse.Success(accessToken, refreshToken);
    }

    private string GenerateAccessToken(Usuario usuario)
    {
        var claims = new[]
        {
            new Claim("userId", usuario.Id.ToString()),
            new Claim("email", usuario.Email),
            new Claim("role", usuario.Tipo.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(_jwtSettings.ExpirationHours),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
```

#### **Middleware de Autenticação**
```csharp
public class JwtMiddleware
{
    public async Task InvokeAsync(HttpContext context, IAuthService authService)
    {
        var token = ExtractToken(context);
        
        if (!string.IsNullOrEmpty(token))
        {
            await AttachUserToContext(context, authService, token);
        }

        await _next(context);
    }

    private async Task AttachUserToContext(HttpContext context, IAuthService authService, string token)
    {
        try
        {
            var principal = ValidateToken(token);
            var userId = principal.FindFirst("userId")?.Value;
            
            if (Guid.TryParse(userId, out var userGuid))
            {
                context.Items["UserId"] = userGuid;
                context.Items["IsAuthenticated"] = true;
                context.User = principal;
            }
        }
        catch (SecurityTokenExpiredException)
        {
            context.Items["TokenExpired"] = true;
        }
        catch (Exception)
        {
            context.Items["TokenInvalid"] = true;
        }
    }
}
```

### **2. Validações e Sanitização**

#### **DTOs com Validação**
```csharp
public class CreateUsuarioRequest
{
    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Nome deve ter entre 2 e 100 caracteres")]
    public string Nome { get; set; }

    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    [StringLength(255, ErrorMessage = "Email deve ter no máximo 255 caracteres")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Senha é obrigatória")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Senha deve ter entre 8 e 100 caracteres")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]", 
        ErrorMessage = "Senha deve conter ao menos: 1 letra minúscula, 1 maiúscula, 1 número e 1 caractere especial")]
    public string Senha { get; set; }

    [Required(ErrorMessage = "Tipo de usuário é obrigatório")]
    [EnumDataType(typeof(TipoUsuario), ErrorMessage = "Tipo de usuário inválido")]
    public TipoUsuario Tipo { get; set; }
}
```

#### **Sanitização de Entrada**
```csharp
public class SecurityService
{
    public string SanitizeInput(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        // Remove caracteres perigosos
        input = input.Trim();
        input = Regex.Replace(input, @"[<>""']", string.Empty);
        input = Regex.Replace(input, @"javascript:", string.Empty, RegexOptions.IgnoreCase);
        input = Regex.Replace(input, @"vbscript:", string.Empty, RegexOptions.IgnoreCase);
        
        return input;
    }

    public bool IsValidPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            return false;

        // Verificar força da senha
        var hasLower = Regex.IsMatch(password, @"[a-z]");
        var hasUpper = Regex.IsMatch(password, @"[A-Z]");
        var hasDigit = Regex.IsMatch(password, @"\d");
        var hasSpecial = Regex.IsMatch(password, @"[@$!%*?&]");

        return hasLower && hasUpper && hasDigit && hasSpecial;
    }
}
```

## 🧪 **Testes**

### **1. Testes Unitários (Backend)**

#### **Configuração**
```xml
<!-- AcervoEducacional.Tests.csproj -->
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
<PackageReference Include="xunit" Version="2.4.2" />
<PackageReference Include="xunit.runner.visualstudio" Version="2.4.5" />
<PackageReference Include="Moq" Version="4.20.69" />
<PackageReference Include="FluentAssertions" Version="6.12.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="8.0.0" />
```

#### **Exemplo de Teste**
```csharp
public class UsuarioServiceTests
{
    private readonly Mock<IUsuarioRepository> _repositoryMock;
    private readonly Mock<ISecurityService> _securityMock;
    private readonly Mock<ILogger<UsuarioService>> _loggerMock;
    private readonly UsuarioService _service;

    public UsuarioServiceTests()
    {
        _repositoryMock = new Mock<IUsuarioRepository>();
        _securityMock = new Mock<ISecurityService>();
        _loggerMock = new Mock<ILogger<UsuarioService>>();
        _service = new UsuarioService(_repositoryMock.Object, _securityMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_UsuarioExiste_DeveRetornarSucesso()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();
        var usuario = new Usuario
        {
            Id = usuarioId,
            Nome = "João Silva",
            Email = "joao@teste.com",
            Tipo = TipoUsuario.Usuario,
            Status = StatusUsuario.Ativo
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(usuarioId))
                      .ReturnsAsync(usuario);

        // Act
        var result = await _service.GetByIdAsync(usuarioId);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Nome.Should().Be("João Silva");
        result.Data.Email.Should().Be("joao@teste.com");
    }

    [Fact]
    public async Task GetByIdAsync_UsuarioNaoExiste_DeveRetornarErro()
    {
        // Arrange
        var usuarioId = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetByIdAsync(usuarioId))
                      .ReturnsAsync((Usuario)null);

        // Act
        var result = await _service.GetByIdAsync(usuarioId);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Usuário não encontrado");
    }
}
```

### **2. Testes de Integração**
```csharp
public class UsuarioControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public UsuarioControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetUsuarios_SemAutenticacao_DeveRetornar401()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/usuario");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetUsuarios_ComAutenticacao_DeveRetornarLista()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/usuario");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ApiResponse<PagedResponse<UsuarioResponse>>>(content);
        result.Success.Should().BeTrue();
    }
}
```

### **3. Testes Frontend (Jest + React Testing Library)**

#### **Configuração**
```json
{
  "devDependencies": {
    "@testing-library/react": "^13.4.0",
    "@testing-library/jest-dom": "^5.16.5",
    "@testing-library/user-event": "^14.4.3",
    "jest": "^29.7.0",
    "jest-environment-jsdom": "^29.7.0"
  }
}
```

#### **Exemplo de Teste**
```javascript
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { AuthProvider } from '../contexts/AuthContext';
import Dashboard from '../pages/Dashboard';

// Mock do serviço de API
jest.mock('../services/api', () => ({
  api: {
    get: jest.fn()
  }
}));

describe('Dashboard', () => {
  const renderWithAuth = (component) => {
    return render(
      <AuthProvider>
        {component}
      </AuthProvider>
    );
  };

  test('deve exibir loading inicialmente', () => {
    renderWithAuth(<Dashboard />);
    
    expect(screen.getByTestId('loading-spinner')).toBeInTheDocument();
  });

  test('deve exibir métricas após carregamento', async () => {
    const mockMetrics = {
      totalCursos: 156,
      totalArquivos: 1247,
      totalUsuarios: 89,
      cursosAtivos: 23
    };

    require('../services/api').api.get.mockResolvedValue({
      data: mockMetrics
    });

    renderWithAuth(<Dashboard />);

    await waitFor(() => {
      expect(screen.getByText('156')).toBeInTheDocument();
      expect(screen.getByText('1247')).toBeInTheDocument();
      expect(screen.getByText('89')).toBeInTheDocument();
      expect(screen.getByText('23')).toBeInTheDocument();
    });
  });

  test('deve exibir erro quando API falha', async () => {
    require('../services/api').api.get.mockRejectedValue(new Error('API Error'));

    renderWithAuth(<Dashboard />);

    await waitFor(() => {
      expect(screen.getByText(/erro ao carregar métricas/i)).toBeInTheDocument();
    });
  });
});
```

## 🚀 **Deploy e CI/CD**

### **1. Docker**

#### **Dockerfile Backend**
```dockerfile
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files
COPY ["AcervoEducacional.WebApi/AcervoEducacional.WebApi.csproj", "AcervoEducacional.WebApi/"]
COPY ["AcervoEducacional.Application/AcervoEducacional.Application.csproj", "AcervoEducacional.Application/"]
COPY ["AcervoEducacional.Domain/AcervoEducacional.Domain.csproj", "AcervoEducacional.Domain/"]
COPY ["AcervoEducacional.Infrastructure/AcervoEducacional.Infrastructure.csproj", "AcervoEducacional.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "AcervoEducacional.WebApi/AcervoEducacional.WebApi.csproj"

# Copy source code
COPY . .

# Build application
WORKDIR "/src/AcervoEducacional.WebApi"
RUN dotnet build "AcervoEducacional.WebApi.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "AcervoEducacional.WebApi.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=publish /app/publish .

# Create non-root user
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

EXPOSE 80
ENTRYPOINT ["dotnet", "AcervoEducacional.WebApi.dll"]
```

#### **Dockerfile Frontend**
```dockerfile
# Build stage
FROM node:18-alpine AS build
WORKDIR /app

# Copy package files
COPY package*.json ./
RUN npm ci --only=production

# Copy source code
COPY . .

# Build application
RUN npm run build

# Runtime stage
FROM nginx:alpine AS runtime

# Copy custom nginx config
COPY nginx.conf /etc/nginx/nginx.conf

# Copy built application
COPY --from=build /app/dist /usr/share/nginx/html

# Create non-root user
RUN addgroup -g 1001 -S nodejs && adduser -S nextjs -u 1001
RUN chown -R nextjs:nodejs /usr/share/nginx/html
USER nextjs

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
```

### **2. GitLab CI/CD**

#### **.gitlab-ci.yml**
```yaml
stages:
  - test
  - build
  - deploy

variables:
  DOCKER_DRIVER: overlay2
  DOCKER_TLS_CERTDIR: "/certs"

# Testes Backend
test-backend:
  stage: test
  image: mcr.microsoft.com/dotnet/sdk:8.0
  script:
    - cd backend
    - dotnet restore
    - dotnet test --collect:"XPlat Code Coverage"
  coverage: '/Total\s*\|\s*(\d+(?:\.\d+)?%)/'
  artifacts:
    reports:
      coverage_report:
        coverage_format: cobertura
        path: backend/*/coverage.cobertura.xml

# Testes Frontend
test-frontend:
  stage: test
  image: node:18-alpine
  script:
    - cd frontend
    - npm ci
    - npm run test:coverage
  coverage: '/All files[^|]*\|[^|]*\s+([\d\.]+)/'
  artifacts:
    reports:
      coverage_report:
        coverage_format: cobertura
        path: frontend/coverage/cobertura-coverage.xml

# Build Backend
build-backend:
  stage: build
  image: docker:latest
  services:
    - docker:dind
  script:
    - cd backend
    - docker build -t $CI_REGISTRY_IMAGE/backend:$CI_COMMIT_SHA .
    - docker push $CI_REGISTRY_IMAGE/backend:$CI_COMMIT_SHA
  only:
    - main
    - develop

# Build Frontend
build-frontend:
  stage: build
  image: docker:latest
  services:
    - docker:dind
  script:
    - cd frontend
    - docker build -t $CI_REGISTRY_IMAGE/frontend:$CI_COMMIT_SHA .
    - docker push $CI_REGISTRY_IMAGE/frontend:$CI_COMMIT_SHA
  only:
    - main
    - develop

# Deploy para Staging
deploy-staging:
  stage: deploy
  image: alpine:latest
  before_script:
    - apk add --no-cache curl
  script:
    - curl -X POST "$WEBHOOK_STAGING" -H "Content-Type: application/json" -d '{"image_tag":"'$CI_COMMIT_SHA'"}'
  environment:
    name: staging
    url: https://acervo-staging.ferreiracosta.com
  only:
    - develop

# Deploy para Produção
deploy-production:
  stage: deploy
  image: alpine:latest
  before_script:
    - apk add --no-cache curl
  script:
    - curl -X POST "$WEBHOOK_PRODUCTION" -H "Content-Type: application/json" -d '{"image_tag":"'$CI_COMMIT_SHA'"}'
  environment:
    name: production
    url: https://acervo.ferreiracosta.com
  when: manual
  only:
    - main
```

## 📊 **Monitoramento e Observabilidade**

### **1. Logs Estruturados (Serilog)**
```csharp
// Program.cs
builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithEnvironmentUserName()
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
        .WriteTo.File("logs/acervo-.txt", 
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 30,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
        .WriteTo.Seq("http://seq:5341"); // Se usando Seq
});
```

### **2. Métricas Customizadas**
```csharp
public class MetricsService
{
    private readonly ILogger<MetricsService> _logger;
    private readonly Counter _requestCounter;
    private readonly Histogram _requestDuration;

    public MetricsService(ILogger<MetricsService> logger)
    {
        _logger = logger;
        _requestCounter = Metrics.CreateCounter("acervo_requests_total", "Total requests", new[] { "method", "endpoint", "status" });
        _requestDuration = Metrics.CreateHistogram("acervo_request_duration_seconds", "Request duration");
    }

    public void RecordRequest(string method, string endpoint, int statusCode, double duration)
    {
        _requestCounter.WithTags(method, endpoint, statusCode.ToString()).Inc();
        _requestDuration.Observe(duration);
        
        _logger.LogInformation("Request recorded: {Method} {Endpoint} {StatusCode} {Duration}ms", 
            method, endpoint, statusCode, duration);
    }
}
```

### **3. Health Checks Avançados**
```csharp
// Startup.cs
services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("database")
    .AddCheck<EmailHealthCheck>("email")
    .AddCheck<DataIntegrityHealthCheck>("data_integrity")
    .AddCheck("external_api", () =>
    {
        // Verificar APIs externas (Senior, AWS S3, etc.)
        return HealthCheckResult.Healthy();
    });

// Endpoint customizado
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        var response = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(x => new
            {
                name = x.Key,
                status = x.Value.Status.ToString(),
                duration = x.Value.Duration.TotalMilliseconds,
                data = x.Value.Data
            }),
            totalDuration = report.TotalDuration.TotalMilliseconds
        };

        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
});
```

## 🔄 **Contribuindo para o Projeto**

### **1. Fluxo de Desenvolvimento**
```bash
# 1. Fork do repositório
git clone https://github.com/seu-usuario/acervo-edu.git
cd acervo-edu

# 2. Criar branch para feature
git checkout -b feature/nova-funcionalidade

# 3. Fazer alterações e commits
git add .
git commit -m "feat: adiciona nova funcionalidade"

# 4. Push da branch
git push origin feature/nova-funcionalidade

# 5. Criar Pull Request no GitHub
```

### **2. Padrões de Commit**
```
feat: nova funcionalidade
fix: correção de bug
docs: atualização de documentação
style: formatação de código
refactor: refatoração sem mudança de funcionalidade
test: adição ou correção de testes
chore: tarefas de manutenção
```

### **3. Code Review Checklist**
- [ ] ✅ Código segue padrões estabelecidos
- [ ] 🧪 Testes unitários implementados
- [ ] 📚 Documentação atualizada
- [ ] 🔒 Validações de segurança implementadas
- [ ] ⚡ Performance considerada
- [ ] 🎨 Identidade visual Ferreira Costa mantida
- [ ] 📝 Logs apropriados adicionados
- [ ] 🔍 Code review aprovado por pelo menos 1 pessoa

---

## 📞 **Suporte ao Desenvolvimento**

### **Documentação Adicional**
- 📚 **README Principal:** `README.md`
- 🏆 **Projeto Finalizado:** `docs/PROJETO-FINALIZADO.md`
- 🌐 **API Documentation:** `docs/API-REST-Documentation.md`
- 🚀 **Guia de Implementação:** `docs/GUIA-IMPLEMENTACAO.md`

### **Ferramentas Recomendadas**
- **IDE:** Visual Studio Code ou Visual Studio 2022
- **Extensions:** C# Dev Kit, ES7+ React/Redux/React-Native snippets
- **Database:** pgAdmin 4 para PostgreSQL
- **API Testing:** Postman ou Insomnia
- **Git GUI:** GitKraken ou SourceTree

### **Contato da Equipe**
- 📧 **Email:** desenvolvimento@ferreiracosta.com
- 🌐 **Website:** https://www.ferreiracosta.com
- 📋 **Issues:** https://github.com/CarlosBertoldo/acervo-edu/issues

**🚀 Pronto para contribuir com o Sistema Acervo Educacional da Ferreira Costa!**

