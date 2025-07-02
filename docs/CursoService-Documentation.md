# 📚 CursoService - Documentação Completa

## 📋 Visão Geral

O **CursoService** é o serviço responsável pela gestão completa de cursos no Sistema Acervo Educacional da Ferreira Costa. Implementa todas as operações CRUD, filtros avançados, sistema Kanban e integração com relatórios.

## 🏗️ Arquitetura Implementada

### Services Implementados
- **CursoService** - Gestão completa de cursos

### Controller Implementado
- **CursoController** - API REST completa para cursos

## 🔧 Funcionalidades Implementadas

### 📚 Gestão de Cursos
- ✅ **CRUD Completo** - Criar, ler, atualizar, excluir cursos
- ✅ **Filtros Avançados** - Busca por título, código, status, origem, datas
- ✅ **Paginação** - Listagem paginada com controle de tamanho
- ✅ **Ordenação** - Por título, status, data de criação
- ✅ **Validações** - Regras de negócio e validações de dados

### 📊 Sistema Kanban
- ✅ **Visualização Kanban** - Cursos organizados por status
- ✅ **Colunas Dinâmicas** - Todas as colunas de status, mesmo vazias
- ✅ **Contadores** - Total de cursos por coluna
- ✅ **Atualização de Status** - Endpoint específico para mudança de status

### 🔒 Segurança e Auditoria
- ✅ **Logs de Atividade** - Registro de todas as operações
- ✅ **Validação de Usuário** - Verificação de permissões
- ✅ **Soft Delete** - Exclusão lógica com verificações
- ✅ **Controle de Integridade** - Verificação de arquivos associados

### 📈 Relatórios e Estatísticas
- ✅ **Dashboard Stats** - Estatísticas gerais dos cursos
- ✅ **Distribuição por Status** - Contadores por status
- ✅ **Distribuição por Origem** - Contadores por origem
- ✅ **Integração com Arquivos** - Estatísticas de arquivos por categoria

## 🌐 Endpoints da API

### GET /api/v1/curso/{id}
**Descrição:** Obter curso por ID
**Autorização:** Requerida

**Resposta:**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "titulo": "Curso de Segurança do Trabalho",
    "descricao": "Curso completo sobre normas de segurança",
    "codigo": "SEG001",
    "status": "Ativo",
    "statusNome": "Ativo",
    "origem": "Interno",
    "origemNome": "Interno",
    "cargaHoraria": 40,
    "dataInicio": "2024-01-15T00:00:00Z",
    "dataFim": "2024-02-15T00:00:00Z",
    "instrutor": "João Silva",
    "observacoes": "Curso obrigatório para todos os funcionários",
    "criadoPor": 1,
    "criadoEm": "2024-01-01T10:00:00Z"
  }
}
```

### GET /api/v1/curso
**Descrição:** Listar cursos com filtros e paginação
**Autorização:** Requerida

**Parâmetros de Query:**
- `search` - Busca por título, descrição ou código
- `status` - Filtro por status (enum)
- `origem` - Filtro por origem (enum)
- `criadoApartirDe` - Data inicial de criação
- `criadoAte` - Data final de criação
- `page` - Página (padrão: 1)
- `pageSize` - Itens por página (padrão: 10)
- `orderBy` - Campo de ordenação (titulo, status, criadoem)
- `orderDirection` - Direção (asc, desc)

### GET /api/v1/curso/kanban
**Descrição:** Obter dados do Kanban de cursos
**Autorização:** Requerida

**Resposta:**
```json
{
  "success": true,
  "data": [
    {
      "status": "Rascunho",
      "statusNome": "Rascunho",
      "totalCursos": 5,
      "cursos": [
        {
          "id": 1,
          "titulo": "Curso de Excel",
          "codigo": "EXC001",
          "origem": "Externo",
          "origemNome": "Externo",
          "cargaHoraria": 20,
          "instrutor": "Maria Santos",
          "criadoEm": "2024-01-01T10:00:00Z"
        }
      ]
    }
  ]
}
```

### POST /api/v1/curso
**Descrição:** Criar novo curso
**Autorização:** Requerida

**Body:**
```json
{
  "titulo": "Novo Curso de Treinamento",
  "descricao": "Descrição do curso",
  "codigo": "TRE001",
  "status": "Rascunho",
  "origem": "Interno",
  "cargaHoraria": 30,
  "dataInicio": "2024-03-01T00:00:00Z",
  "dataFim": "2024-03-31T00:00:00Z",
  "instrutor": "Pedro Oliveira",
  "observacoes": "Observações do curso"
}
```

### PUT /api/v1/curso/{id}
**Descrição:** Atualizar curso existente
**Autorização:** Requerida

### PATCH /api/v1/curso/{id}/status
**Descrição:** Atualizar apenas o status do curso
**Autorização:** Requerida

**Body:**
```json
{
  "novoStatus": "Ativo"
}
```

### DELETE /api/v1/curso/{id}
**Descrição:** Excluir curso
**Autorização:** Requerida
**Observação:** Não permite exclusão se houver arquivos associados

### GET /api/v1/curso/dashboard/stats
**Descrição:** Obter estatísticas do dashboard
**Autorização:** Requerida

## 🔒 Validações Implementadas

### Validações de Criação/Atualização
- **Título:** Obrigatório, máximo 200 caracteres
- **Código:** Obrigatório, máximo 20 caracteres, único
- **Descrição:** Máximo 1000 caracteres
- **Instrutor:** Máximo 100 caracteres
- **Observações:** Máximo 500 caracteres
- **Carga Horária:** Deve ser maior que zero
- **Datas:** Data início não pode ser posterior à data fim

### Validações de Negócio
- **Código Único:** Verificação de duplicidade
- **Integridade Referencial:** Verificação de arquivos antes da exclusão
- **Logs de Auditoria:** Registro automático de todas as operações

## 📊 Status de Curso Suportados

1. **Rascunho** - Curso em elaboração inicial
2. **Planejamento** - Curso em fase de planejamento
3. **Em Desenvolvimento** - Curso sendo desenvolvido
4. **Revisão** - Curso em processo de revisão
5. **Aprovado** - Curso aprovado para publicação
6. **Publicado** - Curso publicado mas não ativo
7. **Ativo** - Curso ativo e disponível
8. **Pausado** - Curso temporariamente pausado
9. **Concluído** - Curso finalizado
10. **Arquivado** - Curso arquivado

## 🎯 Origens de Curso Suportadas

- **Interno** - Curso desenvolvido internamente
- **Externo** - Curso de fornecedor externo
- **Senior** - Curso integrado do sistema Senior
- **Parceiro** - Curso de parceiro comercial

## 🔄 Integração com Outros Services

### ArquivoService
- Verificação de arquivos associados antes da exclusão
- Estatísticas de arquivos por categoria no dashboard

### LogAtividadeService
- Registro automático de todas as operações:
  - Criação de curso
  - Edição de curso
  - Alteração de status
  - Exclusão de curso

### ReportService
- Fornece dados para relatórios de cursos
- Estatísticas para dashboard geral

## 🚀 Próximas Melhorias Sugeridas

1. **Cache** - Implementar cache para consultas frequentes
2. **Bulk Operations** - Operações em lote para múltiplos cursos
3. **Versionamento** - Controle de versões de cursos
4. **Templates** - Sistema de templates para criação rápida
5. **Workflow** - Fluxo de aprovação automático

**O CursoService está completo e pronto para uso em produção!**

