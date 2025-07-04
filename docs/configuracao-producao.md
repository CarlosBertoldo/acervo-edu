# 🚀 **GUIA DE CONFIGURAÇÃO PARA PRODUÇÃO**
## **Sistema Acervo Educacional Ferreira Costa**

**Data:** 02/07/2025  
**Versão:** v1.0.0  
**Ambiente:** Produção  

---

## 🎯 **VISÃO GERAL**

Este guia detalha como configurar o Sistema Acervo Educacional para produção com todas as correções de segurança implementadas, mantendo compatibilidade com desenvolvimento local.

---

## 🔧 **CONFIGURAÇÕES DE SEGURANÇA**

### **1. Flags de Segurança**

Configure as seguintes flags no `appsettings.Production.json`:

```json
{
  "Security": {
    "EnableBolaProtection": true,
    "EnableStrictCors": true,
    "UseEnvironmentCredentials": true,
    "ProductionMode": true
  }
}
```

### **2. Variáveis de Ambiente Obrigatórias**

```bash
# Segurança JWT
JWT_SECRET_KEY=sua-chave-jwt-super-segura-para-producao-com-pelo-menos-32-caracteres

# AWS S3
AWS_ACCESS_KEY_ID=sua-access-key-aws-producao
AWS_SECRET_ACCESS_KEY=sua-secret-key-aws-producao

# Email SMTP
EMAIL_PASSWORD=sua-senha-email-producao

# Banco de Dados
DATABASE_CONNECTION_STRING=Host=seu-host-producao;Database=acervo_educacional;Username=seu-usuario;Password=sua-senha

# CORS - Domínios Permitidos
CORS_ALLOWED_ORIGINS=https://acervo.ferreiracosta.com,https://app.ferreiracosta.com
```

---

## 🛡️ **FUNCIONALIDADES DE SEGURANÇA ATIVADAS**

### **1. Proteção BOLA (Broken Object Level Authorization)**
- **Status:** ✅ Ativada em produção
- **Função:** Impede usuários de acessar dados de outros usuários
- **Endpoints protegidos:**
  - `/api/v1/usuario/{id}`
  - `/api/v1/arquivo/{id}`
  - `/api/v1/curso/{id}`

### **2. Credenciais Externalizadas**
- **Status:** ✅ Obrigatório em produção
- **Função:** Remove credenciais sensíveis do código
- **Fallback:** Não disponível em modo produção

### **3. CORS Restrito**
- **Status:** ✅ Apenas domínios específicos
- **Função:** Previne ataques cross-origin
- **Domínios permitidos:** Configuráveis via variável de ambiente

### **4. Rate Limiting Rigoroso**
- **Status:** ✅ Limites reduzidos para produção
- **Login:** 3 tentativas/minuto (vs 5 em dev)
- **Registro:** 2 tentativas/minuto (vs 3 em dev)
- **Geral:** 60 req/min (vs 100 em dev)

---

## 📋 **CHECKLIST DE DEPLOY**

### **Pré-Deploy**
- [ ] Configurar todas as variáveis de ambiente
- [ ] Validar conexão com banco de produção
- [ ] Testar credenciais AWS S3
- [ ] Verificar configuração SMTP
- [ ] Configurar domínios no CORS

### **Deploy**
- [ ] Usar `appsettings.Production.json`
- [ ] Definir `ASPNETCORE_ENVIRONMENT=Production`
- [ ] Aplicar migrations do banco
- [ ] Verificar logs de inicialização
- [ ] Testar endpoints críticos

### **Pós-Deploy**
- [ ] Verificar logs de segurança
- [ ] Testar proteção BOLA
- [ ] Validar CORS restrito
- [ ] Monitorar rate limiting
- [ ] Verificar health checks

---

## 🔍 **LOGS DE SEGURANÇA**

### **Logs Esperados na Inicialização**

```
🔒 MODO PRODUÇÃO ATIVADO - Credenciais obrigatórias de variáveis de ambiente
🔒 BOLA Protection ATIVADA - Middleware de autorização contextual habilitado
🔒 CORS RESTRITO ativado para domínios: https://acervo.ferreiracosta.com, https://app.ferreiracosta.com
JWT Secret carregada de variável de ambiente
AWS Access Key carregada de variável de ambiente
Database Connection String carregada de variável de ambiente
```

### **Logs de Violação de Segurança**

```json
{
  "level": "Warning",
  "message": "VIOLAÇÃO DE SEGURANÇA BOLA",
  "data": {
    "Timestamp": "2025-07-02T22:30:00Z",
    "UserId": 123,
    "ResourceType": "usuario",
    "ResourceId": 456,
    "Path": "/api/v1/usuario/456",
    "Method": "GET",
    "IpAddress": "192.168.1.100",
    "UserAgent": "Mozilla/5.0..."
  }
}
```

---

## 🚨 **TROUBLESHOOTING**

### **Erro: JWT_SECRET_KEY não encontrada**
```
Causa: Variável de ambiente não configurada
Solução: Definir JWT_SECRET_KEY nas variáveis de ambiente
```

### **Erro: CORS bloqueando requisições**
```
Causa: Domínio não está na lista permitida
Solução: Adicionar domínio em CORS_ALLOWED_ORIGINS
```

### **Erro: Acesso negado (403)**
```
Causa: Proteção BOLA ativa
Solução: Verificar se usuário tem permissão para acessar o recurso
```

### **Erro: Too Many Requests (429)**
```
Causa: Rate limiting ativo
Solução: Aguardar ou verificar se não é um ataque
```

---

## 🔄 **DESENVOLVIMENTO vs PRODUÇÃO**

| Configuração | Desenvolvimento | Produção |
|--------------|-----------------|----------|
| **BOLA Protection** | ❌ Desativada | ✅ Ativada |
| **Credenciais** | appsettings.json | Variáveis de ambiente |
| **CORS** | Permissivo (qualquer origem) | Restrito (domínios específicos) |
| **Rate Limiting** | Permissivo | Rigoroso |
| **Logs** | Console + arquivo | Estruturados + monitoramento |

---

## 📊 **MONITORAMENTO**

### **Métricas Importantes**
- Tentativas de violação BOLA
- Requisições bloqueadas por CORS
- Rate limiting ativado
- Falhas de autenticação
- Tempo de resposta dos endpoints

### **Alertas Recomendados**
- Mais de 10 violações BOLA por hora
- Mais de 100 requisições bloqueadas por CORS por hora
- Taxa de erro > 5%
- Tempo de resposta > 2 segundos

---

## 🔐 **SEGURANÇA ADICIONAL**

### **Recomendações**
1. **WAF (Web Application Firewall)** na frente da aplicação
2. **HTTPS obrigatório** com certificados válidos
3. **Backup automático** do banco de dados
4. **Monitoramento 24/7** com alertas
5. **Auditoria regular** dos logs de segurança

### **Próximas Implementações**
- [ ] 2FA para usuários administrativos
- [ ] CAPTCHA em formulários críticos
- [ ] Detecção de comportamento anômalo
- [ ] Criptografia de dados sensíveis

---

## 📞 **SUPORTE**

**Em caso de problemas:**
1. Verificar logs da aplicação
2. Validar variáveis de ambiente
3. Testar conectividade com dependências
4. Contatar equipe de desenvolvimento

**Contatos:**
- **Desenvolvimento:** [dev@ferreiracosta.com]
- **DevOps:** [devops@ferreiracosta.com]
- **Segurança:** [security@ferreiracosta.com]

---

*Este guia garante que o Sistema Acervo Educacional seja deployado com todas as correções de segurança ativas, mantendo a flexibilidade para desenvolvimento local.*

