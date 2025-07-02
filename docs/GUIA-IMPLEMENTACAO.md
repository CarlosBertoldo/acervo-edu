# 🚀 Guia de Implementação - Sistema Acervo Educacional Ferreira Costa

Este guia fornece instruções passo a passo para implementar e executar o Sistema Acervo Educacional da Ferreira Costa em diferentes ambientes.

## 📋 **Pré-requisitos**

### **Software Necessário**
- **Node.js 18+** - Para o frontend React
- **.NET 8 SDK** - Para o backend
- **PostgreSQL 14+** - Banco de dados
- **Git** - Controle de versão
- **Docker** (opcional) - Para containerização
- **Visual Studio Code** (recomendado) - Editor

### **Conhecimentos Técnicos**
- Básico de React e JavaScript
- Conhecimento de .NET Core e C#
- Familiaridade com PostgreSQL
- Conceitos de API REST
- Git e GitHub

## 🔧 **Configuração do Ambiente**

### **1. Clonando o Repositório**
```bash
# Clone o repositório
git clone https://github.com/CarlosBertoldo/acervo-edu.git
cd acervo-edu

# Mude para a branch de desenvolvimento
git checkout jul02
```

### **2. Configuração do Banco de Dados**

#### **Opção A: PostgreSQL Local**
```bash
# Instalar PostgreSQL (Ubuntu/Debian)
sudo apt update
sudo apt install postgresql postgresql-contrib

# Criar banco de dados
sudo -u postgres psql
CREATE DATABASE acervo_educacional;
CREATE USER acervo_user WITH PASSWORD 'sua_senha_aqui';
GRANT ALL PRIVILEGES ON DATABASE acervo_educacional TO acervo_user;
\q
```

#### **Opção B: Docker PostgreSQL**
```bash
# Executar PostgreSQL em container
docker run --name postgres-acervo \
  -e POSTGRES_DB=acervo_educacional \
  -e POSTGRES_USER=acervo_user \
  -e POSTGRES_PASSWORD=sua_senha_aqui \
  -p 5432:5432 \
  -d postgres:14
```

### **3. Configuração das Variáveis de Ambiente**

#### **Backend (.NET)**
Crie `backend/AcervoEducacional.WebApi/appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=acervo_educacional;Username=acervo_user;Password=sua_senha_aqui"
  },
  "Jwt": {
    "SecretKey": "sua_chave_secreta_muito_longa_e_segura_aqui_com_pelo_menos_32_caracteres",
    "Issuer": "AcervoEducacional",
    "Audience": "AcervoEducacional.Users",
    "ExpirationHours": 1,
    "RefreshExpirationDays": 7
  },
  "Email": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "EnableSsl": true,
    "Username": "seu_email@gmail.com",
    "Password": "sua_senha_app"
  },
  "RateLimit": {
    "LoginAttempts": 5,
    "LoginWindowMinutes": 15,
    "ResetAttempts": 3,
    "ResetWindowHours": 1
  }
}
```

#### **Frontend (React)**
Crie `frontend/.env.local`:
```env
VITE_API_BASE_URL=http://localhost:5000/api/v1
VITE_APP_NAME=Acervo Educacional - Ferreira Costa
VITE_COMPANY_NAME=Ferreira Costa
```

## 🚀 **Executando o Sistema**

### **1. Backend (.NET)**
```bash
# Navegar para o diretório do backend
cd backend

# Restaurar dependências
dotnet restore

# Executar migrações do banco
dotnet ef database update --project AcervoEducacional.Infrastructure --startup-project AcervoEducacional.WebApi

# Executar a aplicação
dotnet run --project AcervoEducacional.WebApi

# ✅ Backend rodando em: http://localhost:5000
# 📚 Swagger disponível em: http://localhost:5000/api/docs
# 🏥 Health checks em: http://localhost:5000/health
```

### **2. Frontend (React)**
```bash
# Em um novo terminal, navegar para o frontend
cd frontend

# Instalar dependências
npm install

# Executar em modo desenvolvimento
npm run dev

# ✅ Frontend rodando em: http://localhost:5173
```

### **3. Verificação da Instalação**

#### **Checklist de Verificação**
- [ ] ✅ Backend respondendo em http://localhost:5000
- [ ] 📚 Swagger acessível em http://localhost:5000/api/docs
- [ ] 🏥 Health checks retornando "Healthy" em http://localhost:5000/health
- [ ] ⚛️ Frontend carregando em http://localhost:5173
- [ ] 🎨 Identidade visual Ferreira Costa aplicada
- [ ] 🔐 Tela de login funcional
- [ ] 📊 Dashboard com métricas carregando

## 🔐 **Configuração de Autenticação**

### **1. Usuário Administrador Padrão**
```sql
-- Conectar ao banco PostgreSQL
psql -h localhost -U acervo_user -d acervo_educacional

-- Inserir usuário administrador (senha: Admin@123)
INSERT INTO "Usuarios" ("Id", "Nome", "Email", "SenhaHash", "Tipo", "Status", "DataCriacao", "DataAtualizacao")
VALUES (
  gen_random_uuid(),
  'Administrador',
  'admin@ferreiracosta.com',
  '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewdBPj3bp.VpO/iG',
  'Admin',
  'Ativo',
  NOW(),
  NOW()
);
```

### **2. Testando Autenticação**
```bash
# Teste de login via curl
curl -X POST http://localhost:5000/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@ferreiracosta.com",
    "senha": "Admin@123"
  }'

# Resposta esperada:
# {
#   "success": true,
#   "data": {
#     "accessToken": "eyJ...",
#     "refreshToken": "eyJ...",
#     "expiresIn": 3600
#   }
# }
```

## 🐳 **Execução com Docker**

### **1. Docker Compose (Recomendado)**
```bash
# Executar todo o sistema com Docker
docker-compose up -d

# Verificar status dos containers
docker-compose ps

# Logs do sistema
docker-compose logs -f

# Parar o sistema
docker-compose down
```

### **2. Configuração Docker Personalizada**
```yaml
# docker-compose.override.yml
version: '3.8'
services:
  backend:
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Host=postgres;Database=acervo_educacional;Username=acervo_user;Password=sua_senha
    ports:
      - "5000:80"
  
  frontend:
    environment:
      - VITE_API_BASE_URL=http://localhost:5000/api/v1
    ports:
      - "3000:80"
  
  postgres:
    environment:
      - POSTGRES_PASSWORD=sua_senha_segura
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data

volumes:
  postgres_data:
```

## 🔧 **Configurações Avançadas**

### **1. Configuração de Email**

#### **Gmail (Recomendado para desenvolvimento)**
```json
{
  "Email": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "EnableSsl": true,
    "Username": "seu_email@gmail.com",
    "Password": "sua_senha_de_app_do_gmail"
  }
}
```

**Passos para configurar Gmail:**
1. Ativar autenticação de 2 fatores
2. Gerar senha de app específica
3. Usar a senha de app no campo "Password"

#### **Outlook/Hotmail**
```json
{
  "Email": {
    "SmtpHost": "smtp-mail.outlook.com",
    "SmtpPort": 587,
    "EnableSsl": true,
    "Username": "seu_email@outlook.com",
    "Password": "sua_senha"
  }
}
```

### **2. Configuração de Logs**

#### **Serilog (Recomendado)**
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Console"
      },
      {
        "Name": "File",
        "Args": {
          "path": "logs/acervo-.txt",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 30
        }
      }
    ]
  }
}
```

### **3. Configuração de Performance**

#### **Entity Framework**
```json
{
  "EntityFramework": {
    "EnableSensitiveDataLogging": false,
    "EnableDetailedErrors": false,
    "CommandTimeout": 30,
    "MaxRetryCount": 3,
    "MaxRetryDelay": "00:00:30"
  }
}
```

#### **Rate Limiting Personalizado**
```json
{
  "RateLimit": {
    "LoginAttempts": 5,
    "LoginWindowMinutes": 15,
    "ResetAttempts": 3,
    "ResetWindowHours": 1,
    "GeneralRequestsPerMinute": 100,
    "BurstRequestsPerSecond": 10
  }
}
```

## 🧪 **Testes e Validação**

### **1. Testes de API**

#### **Postman Collection**
```bash
# Importar collection do Postman
# Arquivo: docs/postman/Acervo-Educacional-API.postman_collection.json
```

#### **Testes manuais via Swagger**
1. Acesse http://localhost:5000/api/docs
2. Clique em "Authorize"
3. Faça login em `/api/v1/auth/login`
4. Copie o token retornado
5. Cole no campo "Value" como "Bearer {token}"
6. Teste os endpoints disponíveis

### **2. Testes de Frontend**
```bash
# Executar testes unitários (quando implementados)
cd frontend
npm test

# Executar testes E2E (quando implementados)
npm run test:e2e

# Build de produção
npm run build
npm run preview
```

### **3. Testes de Integração**
```bash
# Testes de health checks
curl http://localhost:5000/health

# Teste de conectividade do banco
curl http://localhost:5000/health/database

# Teste de serviço de email
curl http://localhost:5000/health/email
```

## 🚀 **Deploy em Produção**

### **1. Preparação para Produção**

#### **Configurações de Segurança**
```json
{
  "Jwt": {
    "SecretKey": "chave_super_secreta_de_producao_com_pelo_menos_64_caracteres_muito_segura",
    "ExpirationHours": 1,
    "RefreshExpirationDays": 7
  },
  "AllowedHosts": ["acervo.ferreiracosta.com"],
  "Cors": {
    "AllowedOrigins": ["https://acervo.ferreiracosta.com"]
  }
}
```

#### **Build de Produção**
```bash
# Frontend
cd frontend
npm run build
# Arquivos gerados em: dist/

# Backend
cd backend
dotnet publish -c Release -o publish/
# Arquivos gerados em: publish/
```

### **2. Deploy com Docker**
```dockerfile
# Dockerfile.prod
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY publish/ .
EXPOSE 80
ENTRYPOINT ["dotnet", "AcervoEducacional.WebApi.dll"]
```

```bash
# Build e deploy
docker build -f Dockerfile.prod -t acervo-educacional:latest .
docker run -d -p 80:80 --name acervo-prod acervo-educacional:latest
```

## 🔍 **Troubleshooting**

### **Problemas Comuns**

#### **1. Erro de Conexão com Banco**
```
Error: Could not connect to database
```
**Solução:**
- Verificar se PostgreSQL está rodando
- Confirmar string de conexão
- Testar conectividade: `psql -h localhost -U acervo_user -d acervo_educacional`

#### **2. Erro de CORS no Frontend**
```
Access to XMLHttpRequest blocked by CORS policy
```
**Solução:**
- Verificar configuração CORS no backend
- Confirmar URL da API no frontend (.env.local)
- Verificar se backend está rodando

#### **3. Erro de JWT**
```
401 Unauthorized - Invalid token
```
**Solução:**
- Verificar se token não expirou
- Confirmar formato: "Bearer {token}"
- Verificar chave secreta JWT

#### **4. Erro de Migração**
```
Unable to create migration
```
**Solução:**
```bash
# Limpar migrações
dotnet ef database drop --force
dotnet ef migrations remove
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### **Logs e Monitoramento**
```bash
# Logs do backend
tail -f logs/acervo-*.txt

# Logs do Docker
docker-compose logs -f backend

# Health checks
curl http://localhost:5000/health
```

## 📞 **Suporte**

### **Documentação Adicional**
- 📚 **API Documentation:** `/api/docs` (Swagger)
- 🏆 **Projeto Finalizado:** `docs/PROJETO-FINALIZADO.md`
- 🌐 **API REST:** `docs/API-REST-Documentation.md`
- 🔧 **Documentação Técnica:** `docs/documentacao-tecnica.md`

### **Contato**
- 📧 **Email:** desenvolvimento@ferreiracosta.com
- 🌐 **Website:** https://www.ferreiracosta.com
- 📋 **Issues:** https://github.com/CarlosBertoldo/acervo-edu/issues

---

## ✅ **Checklist de Implementação Completa**

- [ ] ✅ Repositório clonado e branch `jul02` ativa
- [ ] 🗄️ PostgreSQL configurado e rodando
- [ ] 🔧 Variáveis de ambiente configuradas
- [ ] 📦 Dependências instaladas (backend e frontend)
- [ ] 🔄 Migrações do banco executadas
- [ ] 🚀 Backend rodando em http://localhost:5000
- [ ] ⚛️ Frontend rodando em http://localhost:5173
- [ ] 📚 Swagger acessível em http://localhost:5000/api/docs
- [ ] 🏥 Health checks retornando "Healthy"
- [ ] 🔐 Login funcionando com usuário admin
- [ ] 🎨 Identidade visual Ferreira Costa carregada
- [ ] 📊 Dashboard com métricas funcionando
- [ ] 📋 Sistema Kanban operacional
- [ ] 📚 Gestão de cursos funcional
- [ ] 👥 Gestão de usuários operacional
- [ ] 📊 Relatórios e logs funcionando

**🎉 Parabéns! O Sistema Acervo Educacional da Ferreira Costa está funcionando perfeitamente!**

