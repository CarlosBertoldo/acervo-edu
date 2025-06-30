# 🎓 Sistema Acervo Educacional

Sistema completo de gestão educacional com painel administrativo Kanban para gerenciamento de cursos e arquivos educacionais.

## 🌐 **DEMONSTRAÇÃO AO VIVO**
**Frontend funcionando:** https://fldtrzta.manus.space

## 📋 **Visão Geral**

O Sistema Acervo Educacional é uma plataforma completa para gestão de cursos educacionais, desenvolvida com arquitetura moderna e interface intuitiva em formato Kanban.

### ✨ **Principais Funcionalidades**

- 📊 **Painel Kanban** com 3 colunas fixas (Backlog, Em Desenvolvimento, Veiculado)
- 🎯 **Dashboard executivo** com métricas e gráficos
- 📚 **Gestão completa de cursos** com CRUD
- 👥 **Gerenciamento de usuários** e permissões
- 📁 **Sistema de arquivos** com categorização
- 🔐 **Player de vídeo protegido** com restrições
- 🔗 **Compartilhamento avançado** com links e embeds
- 📊 **Relatórios e logs** detalhados
- ⚙️ **Configurações** e integrações (Senior, AWS S3, Email)
- 🔄 **Integração Senior** para sincronização automática

## 🏗️ **Arquitetura**

### **Frontend - React + TailwindCSS**
- ✅ **Totalmente funcional** e implantado
- 🎨 Interface moderna e responsiva
- 🔄 Drag-and-drop no Kanban
- 📱 Design mobile-first
- 🎯 Componentes reutilizáveis

### **Backend - .NET 8 + Clean Architecture**
- 🏗️ **Em desenvolvimento** (80% concluído)
- 📦 **Domain Layer** - Entidades e regras de negócio
- 🔧 **Application Layer** - DTOs e interfaces de serviços
- 🗄️ **Infrastructure Layer** - Repositórios e serviços externos
- 🌐 **WebApi Layer** - Controllers e middleware

### **Banco de Dados**
- 🐘 **PostgreSQL** com Entity Framework Core
- 📊 Estrutura otimizada para performance
- 🔄 Migrações automáticas
- 📈 Índices e relacionamentos configurados

## 🚀 **Tecnologias Utilizadas**

### **Frontend**
- React 18
- TailwindCSS
- React Router DOM
- Lucide React (ícones)
- React Beautiful DnD (drag-and-drop)
- Recharts (gráficos)

### **Backend**
- .NET 8
- Entity Framework Core
- PostgreSQL (Npgsql)
- JWT Authentication
- BCrypt (criptografia)
- AWS S3 SDK
- MailKit

### **DevOps**
- Docker & Docker Compose
- GitLab CI/CD
- Scripts de backup e deploy

## 📁 **Estrutura do Projeto**

```
acervo-edu/
├── frontend/                 # React Application
│   ├── src/
│   │   ├── components/      # Componentes reutilizáveis
│   │   ├── pages/          # Páginas da aplicação
│   │   ├── contexts/       # Contextos React
│   │   ├── services/       # Serviços e API
│   │   └── utils/          # Utilitários
│   ├── public/             # Arquivos públicos
│   └── package.json        # Dependências
│
├── backend/                 # .NET 8 Clean Architecture
│   ├── AcervoEducacional.Domain/        # Entidades e regras
│   ├── AcervoEducacional.Application/   # DTOs e interfaces
│   ├── AcervoEducacional.Infrastructure/# Repositórios e serviços
│   └── AcervoEducacional.WebApi/       # Controllers e API
│
├── docs/                    # Documentação
├── scripts/                 # Scripts de deploy e backup
├── docker-compose.yml       # Configuração Docker
└── README.md               # Este arquivo
```

## 🎯 **Funcionalidades Implementadas**

### ✅ **Frontend (100% Funcional)**

#### 📊 **Dashboard**
- Cards de métricas coloridos
- Gráficos de distribuição de cursos
- Atividades recentes
- Cursos em desenvolvimento

#### 📋 **Kanban**
- 3 colunas fixas conforme especificação
- Drag-and-drop funcional entre colunas
- Cards detalhados com informações completas
- Busca e filtros avançados
- Contadores por coluna em tempo real

#### 📚 **Gestão de Cursos**
- Tabela completa com paginação
- Filtros múltiplos (status, origem, ambiente, acesso)
- Ações individuais e em lote
- Modal de detalhes completo
- Badges coloridos por categoria

#### 👥 **Gestão de Usuários**
- Cards de estatísticas
- Avatars com iniciais coloridas
- Filtros por tipo, status e departamento
- Ações de ativação/desativação

#### 📊 **Logs e Relatórios**
- Gráficos de atividades
- Tabela completa de logs
- Modal de relatórios avançados
- Exportação em múltiplos formatos

#### ⚙️ **Configurações**
- 6 abas organizadas (Sistema, Senior, Email, AWS, Segurança, Notificações)
- Cards de status das integrações
- Configurações avançadas de segurança

### 🔄 **Backend (80% Implementado)**

#### 🏗️ **Infrastructure Layer**
- ✅ **DbContext** completo com todas as entidades
- ✅ **BaseRepository<T>** genérico com CRUD
- ✅ **UsuarioRepository** com filtros avançados
- ✅ **CursoRepository** com dados Kanban
- ✅ **ArquivoRepository** com categorização
- ✅ **UnitOfWork** pattern implementado

#### 📦 **Domain Layer**
- ✅ **Entidades** completas (Usuario, Curso, Arquivo, etc.)
- ✅ **Enums** organizados por contexto
- ✅ **Interfaces** de repositórios

#### 🔧 **Application Layer**
- ✅ **DTOs** organizados por contexto
- ✅ **Interfaces** de serviços
- ✅ **Responses** padronizados

## 🛠️ **Instalação e Execução**

### **Pré-requisitos**
- Node.js 18+
- .NET 8 SDK
- PostgreSQL 14+
- Docker (opcional)

### **Frontend**
```bash
cd frontend
npm install
npm run dev
```

### **Backend**
```bash
cd backend
dotnet restore
dotnet run --project AcervoEducacional.WebApi
```

### **Docker (Recomendado)**
```bash
docker-compose up -d
```

## 🔧 **Configuração**

### **Variáveis de Ambiente**
Copie `.env.example` para `.env` e configure:

```env
# Database
DATABASE_URL=postgresql://user:password@localhost:5432/acervo_educacional

# JWT
JWT_SECRET=sua_chave_secreta_aqui
JWT_EXPIRATION_HOURS=24

# AWS S3
AWS_ACCESS_KEY_ID=sua_access_key
AWS_SECRET_ACCESS_KEY=sua_secret_key
AWS_REGION=us-east-1
AWS_BUCKET_NAME=acervo-educacional

# Email
SMTP_HOST=smtp.gmail.com
SMTP_PORT=587
SMTP_USERNAME=seu_email@gmail.com
SMTP_PASSWORD=sua_senha

# Senior Integration
SENIOR_API_URL=https://api.senior.com.br
SENIOR_API_KEY=sua_chave_senior
```

## 📈 **Roadmap**

### **Próximas Implementações**
- [ ] **WebApi Layer** completa
- [ ] **Serviços de Application** 
- [ ] **Autenticação JWT** real
- [ ] **Integração AWS S3**
- [ ] **Serviço de Email**
- [ ] **Integração Senior**
- [ ] **Testes unitários**
- [ ] **Deploy em produção**

## 🤝 **Contribuição**

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## 📄 **Licença**

Este projeto está sob a licença MIT. Veja o arquivo `LICENSE` para mais detalhes.

## 👨‍💻 **Desenvolvido por**

**Sistema Acervo Educacional** - Desenvolvido com ❤️ para gestão educacional moderna.

---

**🌟 Acesse a demonstração ao vivo:** https://fldtrzta.manus.space

