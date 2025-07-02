# 🔐 AuthService - Documentação Completa

## 📋 Visão Geral

O **AuthService** é o serviço responsável por toda a autenticação e autorização do Sistema Acervo Educacional da Ferreira Costa. Foi implementado seguindo as melhores práticas de segurança e inclui funcionalidades avançadas de proteção.

## 🏗️ Arquitetura Implementada

### Services Implementados
- **AuthService** - Autenticação principal
- **SecurityService** - Funcionalidades de segurança avançadas  
- **EmailService** - Envio de emails com templates

### Controller Implementado
- **AuthController** - Endpoints REST para frontend

## 🔧 Funcionalidades Implementadas

### 🔑 Autenticação Core
- ✅ **Login com JWT** - Geração de tokens seguros
- ✅ **Refresh Token** - Renovação automática de tokens
- ✅ **Logout** - Invalidação de sessões
- ✅ **Validação de Token** - Verificação de validade

### 🛡️ Segurança Avançada
- ✅ **BCrypt Hashing** - Hash seguro de senhas (cost 12)
- ✅ **Rate Limiting** - Proteção contra ataques de força bruta
- ✅ **IP Blocking** - Bloqueio automático de IPs suspeitos
- ✅ **Detecção de Atividade Suspeita** - Múltiplos IPs, User-Agents
- ✅ **Validação de Força de Senha** - Regex e blacklist
- ✅ **Logs de Auditoria** - Registro detalhado de atividades

### 🔄 Recuperação de Senha
- ✅ **Solicitação de Reset** - Geração de token seguro
- ✅ **Envio de Email** - Templates HTML profissionais
- ✅ **Reset com Token** - Validação e alteração segura
- ✅ **Expiração de Tokens** - Tokens válidos por 2 horas

### 📧 Sistema de Email
- ✅ **Templates HTML** - Design com identidade Ferreira Costa
- ✅ **Email de Recuperação** - Link seguro para reset
- ✅ **Email de Boas-vindas** - Para novos usuários
- ✅ **Configuração SMTP** - Suporte a Gmail, Outlook, etc.

## 🌐 Endpoints da API

### POST /api/v1/auth/login
**Descrição:** Realizar login no sistema
```json
{
  "email": "usuario@ferreiracosta.com",
  "senha": "MinhaSenh@123"
}
```

**Resposta:**
```json
{
  "success": true,
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIs...",
    "refreshToken": "abc123def456...",
    "expiresIn": 3600,
    "usuario": {
      "id": 1,
      "nome": "João Silva",
      "email": "joao@ferreiracosta.com"
    }
  }
}
```

### POST /api/v1/auth/refresh
**Descrição:** Renovar token de acesso
```json
{
  "refreshToken": "abc123def456..."
}
```

### POST /api/v1/auth/logout
**Descrição:** Realizar logout (requer autenticação)
**Headers:** `Authorization: Bearer {token}`

### POST /api/v1/auth/forgot-password
**Descrição:** Solicitar recuperação de senha
```json
{
  "email": "usuario@ferreiracosta.com"
}
```

### POST /api/v1/auth/reset-password
**Descrição:** Redefinir senha com token
```json
{
  "token": "reset_token_here",
  "novaSenha": "NovaSenha@123"
}
```

### POST /api/v1/auth/change-password
**Descrição:** Alterar senha (requer autenticação)
```json
{
  "senhaAtual": "SenhaAtual@123",
  "novaSenha": "NovaSenha@123"
}
```

### GET /api/v1/auth/validate
**Descrição:** Validar token atual
**Headers:** `Authorization: Bearer {token}`

### GET /api/v1/auth/me
**Descrição:** Obter dados do usuário logado
**Headers:** `Authorization: Bearer {token}`

## 🔒 Recursos de Segurança

### Rate Limiting
- **Login:** 5 tentativas por 15 minutos por IP
- **Reset Password:** 3 tentativas por hora por email
- **Change Password:** 3 tentativas por 10 minutos por usuário

### Proteções Implementadas
- **Bloqueio de Conta:** Após 5 tentativas falhadas
- **

