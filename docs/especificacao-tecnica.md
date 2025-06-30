# Sistema Acervo Educacional - Especificação Técnica

**Autor:** Manus AI  
**Data:** 26 de junho de 2025  
**Versão:** 1.0

## 1. Visão Geral do Sistema

O Sistema Acervo Educacional é uma plataforma web completa para gerenciamento de cursos e materiais educacionais, desenvolvida com arquitetura moderna e recursos avançados de proteção de conteúdo. O sistema utiliza uma interface visual Kanban para organização dos cursos em diferentes estágios de desenvolvimento e oferece funcionalidades robustas para administração de arquivos educacionais.

### 1.1 Objetivos Principais

O sistema tem como principais objetivos proporcionar uma interface intuitiva para gestão de cursos educacionais, garantir a proteção e controle de acesso aos materiais didáticos, facilitar a colaboração entre equipes de desenvolvimento de conteúdo e integrar-se de forma transparente com sistemas externos como o Senior.

### 1.2 Público-Alvo

O sistema é destinado a administradores educacionais, desenvolvedores de conteúdo, coordenadores de curso e demais profissionais envolvidos na criação e gestão de materiais educacionais em instituições de ensino ou empresas de treinamento corporativo.

## 2. Arquitetura do Sistema

### 2.1 Visão Geral da Arquitetura

O sistema segue uma arquitetura de três camadas (3-tier) com separação clara entre apresentação, lógica de negócio e dados. A comunicação entre frontend e backend é realizada através de APIs RESTful, garantindo flexibilidade e escalabilidade.

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Frontend      │    │    Backend      │    │   Banco de      │
│   React +       │◄──►│   .NET 8 +      │◄──►│   Dados         │
│   TailwindCSS   │    │   JWT Auth      │    │   PostgreSQL    │
└─────────────────┘    └─────────────────┘    └─────────────────┘
         │                       │                       │
         │                       │                       │
         ▼                       ▼                       ▼
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   AWS S3        │    │   Sistema       │    │   Sistema       │
│   Armazenamento │    │   Senior        │    │   de Logs       │
│   de Arquivos   │    │   (Integração)  │    │   e Auditoria   │
└─────────────────┘    └─────────────────┘    └─────────────────┘
```

### 2.2 Tecnologias Utilizadas

**Frontend:**
- React 18+ com TypeScript para desenvolvimento de interface moderna e tipada
- TailwindCSS para estilização responsiva e consistente
- Video.js para player de vídeo customizado com recursos de proteção
- Axios para comunicação HTTP com o backend
- React Router para navegação entre páginas
- React Hook Form para gerenciamento de formulários

**Backend:**
- .NET 8 com ASP.NET Core para APIs RESTful de alta performance
- Entity Framework Core para ORM e acesso a dados
- JWT (JSON Web Tokens) para autenticação e autorização
- AutoMapper para mapeamento entre DTOs e entidades
- Hangfire para agendamento de tarefas (sincronização com Senior)
- Serilog para logging estruturado

**Banco de Dados:**
- PostgreSQL como banco principal para armazenamento de dados
- Redis para cache de sessões e dados temporários
- Migrations automáticas para versionamento do schema

**Infraestrutura:**
- AWS S3 para armazenamento seguro de arquivos
- GitLab CI/CD para integração e deploy contínuo
- Docker para containerização da aplicação
- Nginx como proxy reverso e servidor web

## 3. Requisitos Funcionais

### 3.1 Gestão de Usuários e Autenticação

O sistema deve implementar um robusto sistema de autenticação baseado em JWT, onde todos os usuários são inicialmente criados com privilégios administrativos. A autenticação deve incluir funcionalidades de login seguro, recuperação de senha via email, gerenciamento de sessões e controle de acesso baseado em tokens com expiração configurável.

### 3.2 Interface Kanban para Gestão de Cursos

A interface principal do sistema deve apresentar um layout Kanban com três colunas fixas representando os diferentes estágios dos cursos. A coluna "Backlog" contém cursos desejados que ainda não foram iniciados, a coluna "Em Desenvolvimento" apresenta cursos com produção em andamento, e a coluna "Veiculado" exibe cursos finalizados e prontos para distribuição.

Cada curso é representado por um card visual contendo informações essenciais como descrição da academia, código único do curso, nome completo, status atual, tipo de ambiente de execução, tipo de acesso permitido, data de início de operação, origem dos dados (manual ou Senior) e informações sobre o criador quando a origem for manual.

### 3.3 Gerenciamento de Arquivos Educacionais

Cada curso deve permitir o gerenciamento de arquivos educacionais organizados em categorias específicas. As categorias incluem Briefing de Desenvolvimento para documentos de planejamento, Briefing de Execução para guias operacionais, apresentações em PowerPoint, Caderno de Exercícios para atividades práticas, Plano de Aula para estruturação pedagógica, Vídeos educacionais, Podcasts de áudio e uma categoria genérica para Outros Arquivos.

### 3.4 Sistema de Proteção de Conteúdo

O sistema deve implementar recursos avançados de proteção para os arquivos, especialmente vídeos. Isso inclui a capacidade de gerar links de compartilhamento com diferentes níveis de acesso (público, restrito ou com data de expiração), criação de códigos embed com restrição de domínio, ocultação do link de origem real dos arquivos e aplicação de diversos bloqueios de segurança.

Os bloqueios de segurança devem impedir que usuários pulem ou avancem vídeos de forma não autorizada, bloquear downloads quando tecnicamente possível, desabilitar o botão direito do mouse, impedir o uso de teclas de atalho para controle de mídia e bloquear tentativas de captura de tela via printscreen.

### 3.5 Integração com Sistema Senior

O sistema deve manter sincronização automática com o sistema Senior através da leitura de views específicas no banco de dados. Esta sincronização deve ser executada periodicamente via agendamento cron, garantindo que os dados dos cursos originários do Senior permaneçam atualizados. Cursos provenientes do Senior devem ter seus dados principais bloqueados para edição manual, preservando a integridade da sincronização.

## 4. Requisitos Não Funcionais

### 4.1 Performance e Escalabilidade

O sistema deve ser capaz de suportar múltiplos usuários simultâneos sem degradação significativa de performance. O tempo de resposta para operações básicas não deve exceder 2 segundos, e o carregamento de arquivos de mídia deve ser otimizado através de técnicas de streaming e cache.

### 4.2 Segurança

Todas as comunicações entre cliente e servidor devem utilizar HTTPS. Os tokens JWT devem ter tempo de expiração configurável e renovação automática. O acesso aos arquivos no S3 deve ser controlado através de URLs assinadas com expiração. Logs de auditoria devem registrar todas as ações sensíveis do sistema.

### 4.3 Disponibilidade e Confiabilidade

O sistema deve manter alta disponibilidade através de backups automáticos diários do banco de dados e versionamento de arquivos no S3. Deve implementar tratamento robusto de erros e recuperação automática de falhas temporárias.

### 4.4 Usabilidade

A interface deve ser responsiva e funcionar adequadamente em dispositivos desktop, tablet e mobile. O design deve seguir princípios de UX/UI modernos, proporcionando uma experiência intuitiva e eficiente para os usuários.

## 5. Modelo de Dados

### 5.1 Entidades Principais

**Usuário (User)**
- Id (Guid): Identificador único do usuário
- Email (string): Email para login e comunicação
- PasswordHash (string): Hash da senha para autenticação segura
- Nome (string): Nome completo do usuário
- IsActive (bool): Status ativo/inativo do usuário
- CreatedAt (DateTime): Data de criação do registro
- UpdatedAt (DateTime): Data da última atualização

**Curso (Course)**
- Id (Guid): Identificador único do curso
- CodigoCurso (string): Código único identificador do curso
- NomeCurso (string): Nome completo do curso
- DescricaoAcademia (string): Descrição da academia responsável
- Status (enum): Status atual (Backlog, EmDesenvolvimento, Veiculado)
- TipoAmbiente (string): Tipo de ambiente de execução
- TipoAcesso (string): Tipo de acesso permitido
- DataInicioOperacao (DateTime?): Data de início das operações
- Origem (enum): Origem dos dados (Manual, Senior)
- CriadoPor (Guid?): Referência ao usuário criador (quando origem manual)
- ComentariosInternos (string): Campo para comentários da equipe
- CreatedAt (DateTime): Data de criação
- UpdatedAt (DateTime): Data da última atualização
- SyncedAt (DateTime?): Data da última sincronização com Senior

**Arquivo (File)**
- Id (Guid): Identificador único do arquivo
- CursoId (Guid): Referência ao curso proprietário
- Nome (string): Nome original do arquivo
- NomeArmazenamento (string): Nome do arquivo no S3
- Categoria (enum): Categoria do arquivo (Briefing, PPT, Video, etc.)
- TipoMime (string): Tipo MIME do arquivo
- Tamanho (long): Tamanho em bytes
- UrlS3 (string): URL do arquivo no S3
- IsPublico (bool): Indica se o arquivo é público
- DataExpiracao (DateTime?): Data de expiração do acesso
- DominiosPermitidos (string): Lista de domínios permitidos para embed
- BloqueiosAtivos (string): JSON com configurações de bloqueio
- CreatedAt (DateTime): Data de upload
- UpdatedAt (DateTime): Data da última atualização

**LogAtividade (ActivityLog)**
- Id (Guid): Identificador único do log
- UsuarioId (Guid): Usuário que executou a ação
- CursoId (Guid?): Curso relacionado à ação
- ArquivoId (Guid?): Arquivo relacionado à ação
- TipoAcao (enum): Tipo da ação executada
- Descricao (string): Descrição detalhada da ação
- DadosAnteriores (string): JSON com dados antes da alteração
- DadosNovos (string): JSON com dados após a alteração
- EnderecoIP (string): IP do usuário
- UserAgent (string): Informações do navegador
- CreatedAt (DateTime): Data e hora da ação

### 5.2 Relacionamentos

Os relacionamentos entre as entidades seguem um modelo bem estruturado onde um usuário pode criar múltiplos cursos, cada curso pode conter múltiplos arquivos organizados por categoria, e todas as ações no sistema geram logs de auditoria para rastreabilidade completa.

## 6. Especificação de APIs

### 6.1 Autenticação

**POST /api/auth/login**
- Descrição: Autentica usuário e retorna token JWT
- Parâmetros: { email: string, password: string }
- Resposta: { token: string, user: UserDto, expiresAt: DateTime }

**POST /api/auth/refresh**
- Descrição: Renova token JWT válido
- Parâmetros: { refreshToken: string }
- Resposta: { token: string, expiresAt: DateTime }

**POST /api/auth/forgot-password**
- Descrição: Inicia processo de recuperação de senha
- Parâmetros: { email: string }
- Resposta: { message: string }

### 6.2 Gestão de Cursos

**GET /api/courses**
- Descrição: Lista cursos com filtros e paginação
- Parâmetros: { page: int, size: int, status?: string, origem?: string, search?: string }
- Resposta: { courses: CourseDto[], totalCount: int, pageInfo: PageInfoDto }

**POST /api/courses**
- Descrição: Cria novo curso (apenas origem manual)
- Parâmetros: CreateCourseDto
- Resposta: CourseDto

**PUT /api/courses/{id}**
- Descrição: Atualiza curso existente
- Parâmetros: UpdateCourseDto
- Resposta: CourseDto

**PUT /api/courses/{id}/status**
- Descrição: Altera status do curso (movimentação no Kanban)
- Parâmetros: { status: string }
- Resposta: CourseDto

### 6.3 Gestão de Arquivos

**GET /api/courses/{courseId}/files**
- Descrição: Lista arquivos de um curso por categoria
- Resposta: { [categoria]: FileDto[] }

**POST /api/courses/{courseId}/files**
- Descrição: Faz upload de novo arquivo
- Parâmetros: FormData com arquivo e metadados
- Resposta: FileDto

**GET /api/files/{id}/download**
- Descrição: Gera URL temporária para download
- Resposta: { downloadUrl: string, expiresAt: DateTime }

**POST /api/files/{id}/share**
- Descrição: Gera link de compartilhamento
- Parâmetros: { isPublic: bool, expiresAt?: DateTime, allowedDomains?: string[] }
- Resposta: { shareUrl: string, embedCode?: string }

## 7. Considerações de Implementação

### 7.1 Estrutura de Diretórios

O projeto deve ser organizado em uma estrutura clara que separe frontend e backend, facilitando o desenvolvimento e manutenção. A estrutura sugerida inclui diretórios específicos para cada componente do sistema, documentação, scripts de deploy e configurações de ambiente.

### 7.2 Configurações de Ambiente

O sistema deve suportar múltiplos ambientes (desenvolvimento, teste, produção) através de arquivos de configuração específicos. Variáveis sensíveis como strings de conexão e chaves de API devem ser gerenciadas através de variáveis de ambiente ou serviços de gerenciamento de segredos.

### 7.3 Estratégia de Deploy

O deploy deve ser automatizado através do GitLab CI/CD, incluindo etapas de build, teste, containerização e deploy para os ambientes apropriados. O sistema deve suportar deploy blue-green para minimizar downtime durante atualizações.

### 7.4 Monitoramento e Observabilidade

Deve ser implementado um sistema robusto de logging e monitoramento que permita rastrear performance, erros e uso do sistema. Métricas importantes devem ser coletadas e disponibilizadas através de dashboards para acompanhamento operacional.

Este documento serve como base para o desenvolvimento do Sistema Acervo Educacional e deve ser atualizado conforme o projeto evolui e novos requisitos são identificados.

