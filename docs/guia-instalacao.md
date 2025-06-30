# Guia de Instalação - Sistema Acervo Educacional

**Versão:** 1.0  
**Data:** Dezembro 2024  
**Autor:** Manus AI  
**Público-alvo:** Administradores de Sistema e DevOps

---

## Pré-requisitos

### Requisitos de Hardware

Para execução adequada do Sistema Acervo Educacional, recomendamos a seguinte configuração mínima de hardware:

**Ambiente de Desenvolvimento:**
- CPU: 4 cores (2.0 GHz ou superior)
- RAM: 8 GB
- Armazenamento: 50 GB de espaço livre
- Rede: Conexão estável com internet

**Ambiente de Produção:**
- CPU: 8 cores (2.5 GHz ou superior)
- RAM: 16 GB (32 GB recomendado)
- Armazenamento: 200 GB SSD (escalável conforme volume de dados)
- Rede: Conexão redundante com internet
- Backup: Armazenamento adicional para backups (mínimo 100 GB)

### Requisitos de Software

**Sistema Operacional:**
- Ubuntu 20.04 LTS ou superior (recomendado)
- CentOS 8 ou superior
- Red Hat Enterprise Linux 8 ou superior
- Windows Server 2019 ou superior (com WSL2)

**Dependências Obrigatórias:**
- Docker 20.10 ou superior
- Docker Compose 2.0 ou superior
- Git 2.25 ou superior
- Curl (para health checks e scripts)

**Dependências Opcionais:**
- Nginx (para proxy reverso em produção)
- Certbot (para certificados SSL automáticos)
- Fail2ban (para proteção adicional)

### Configuração de Rede

**Portas Necessárias:**
- 80 (HTTP)
- 443 (HTTPS)
- 5000 (Backend API)
- 3000 (Frontend React)
- 5432 (PostgreSQL - apenas interno)
- 6379 (Redis - apenas interno)

**Configuração de Firewall:**
```bash
# Ubuntu/Debian
sudo ufw allow 80/tcp
sudo ufw allow 443/tcp
sudo ufw allow 22/tcp
sudo ufw enable

# CentOS/RHEL
sudo firewall-cmd --permanent --add-port=80/tcp
sudo firewall-cmd --permanent --add-port=443/tcp
sudo firewall-cmd --permanent --add-port=22/tcp
sudo firewall-cmd --reload
```

## Instalação Passo a Passo

### 1. Preparação do Ambiente

#### Instalação do Docker

**Ubuntu/Debian:**
```bash
# Atualizar repositórios
sudo apt update

# Instalar dependências
sudo apt install -y apt-transport-https ca-certificates curl gnupg lsb-release

# Adicionar chave GPG oficial do Docker
curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo gpg --dearmor -o /usr/share/keyrings/docker-archive-keyring.gpg

# Adicionar repositório Docker
echo "deb [arch=amd64 signed-by=/usr/share/keyrings/docker-archive-keyring.gpg] https://download.docker.com/linux/ubuntu $(lsb_release -cs) stable" | sudo tee /etc/apt/sources.list.d/docker.list > /dev/null

# Instalar Docker
sudo apt update
sudo apt install -y docker-ce docker-ce-cli containerd.io

# Instalar Docker Compose
sudo curl -L "https://github.com/docker/compose/releases/latest/download/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
sudo chmod +x /usr/local/bin/docker-compose

# Adicionar usuário ao grupo docker
sudo usermod -aG docker $USER
```

**CentOS/RHEL:**
```bash
# Instalar dependências
sudo yum install -y yum-utils

# Adicionar repositório Docker
sudo yum-config-manager --add-repo https://download.docker.com/linux/centos/docker-ce.repo

# Instalar Docker
sudo yum install -y docker-ce docker-ce-cli containerd.io

# Iniciar e habilitar Docker
sudo systemctl start docker
sudo systemctl enable docker

# Instalar Docker Compose
sudo curl -L "https://github.com/docker/compose/releases/latest/download/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
sudo chmod +x /usr/local/bin/docker-compose

# Adicionar usuário ao grupo docker
sudo usermod -aG docker $USER
```

#### Verificação da Instalação

```bash
# Verificar versão do Docker
docker --version

# Verificar versão do Docker Compose
docker-compose --version

# Testar Docker
docker run hello-world
```

### 2. Download do Projeto

#### Clonagem do Repositório

```bash
# Navegar para diretório de instalação
cd /opt

# Clonar repositório (substitua pela URL real)
sudo git clone https://github.com/sua-organizacao/acervo-educacional.git

# Alterar proprietário
sudo chown -R $USER:$USER acervo-educacional

# Navegar para diretório do projeto
cd acervo-educacional
```

#### Verificação da Estrutura

```bash
# Verificar estrutura do projeto
ls -la

# Deve mostrar:
# - backend/
# - frontend/
# - docs/
# - scripts/
# - docker-compose.yml
# - .env.example
# - README.md
```

### 3. Configuração

#### Configuração de Variáveis de Ambiente

```bash
# Copiar arquivo de exemplo
cp .env.example .env

# Editar configurações
nano .env
```

**Configurações Essenciais:**
```bash
# Senhas do banco de dados
POSTGRES_PASSWORD=SuaSenhaSeguraPostgreSQL123!
REDIS_PASSWORD=SuaSenhaSeguraRedis123!

# Chave JWT (gerar chave aleatória de 64 caracteres)
JWT_SECRET_KEY=SuaChaveJWTSuperSecretaComPeloMenos64CaracteresParaSegurancaMaxima

# Configurações AWS S3
AWS_ACCESS_KEY=sua_access_key_aws
AWS_SECRET_KEY=sua_secret_key_aws
AWS_BUCKET_NAME=seu-bucket-acervo-educacional
AWS_REGION=us-east-1

# Configurações de Email
SMTP_HOST=smtp.gmail.com
SMTP_PORT=587
SMTP_USERNAME=seu-email@gmail.com
SMTP_PASSWORD=sua-senha-de-app

# URLs da aplicação (ajustar conforme seu domínio)
FRONTEND_URL=https://acervo.suaempresa.com
BACKEND_URL=https://api.acervo.suaempresa.com
```

#### Configuração AWS S3

```bash
# Criar bucket S3 (via AWS CLI)
aws s3 mb s3://seu-bucket-acervo-educacional

# Configurar CORS do bucket
aws s3api put-bucket-cors --bucket seu-bucket-acervo-educacional --cors-configuration file://aws-cors.json
```

**Arquivo aws-cors.json:**
```json
{
    "CORSRules": [
        {
            "AllowedOrigins": ["*"],
            "AllowedHeaders": ["*"],
            "AllowedMethods": ["GET", "PUT", "POST", "DELETE", "HEAD"],
            "MaxAgeSeconds": 3000
        }
    ]
}
```

### 4. Instalação

#### Instalação Automática

```bash
# Tornar script executável
chmod +x scripts/deploy.sh

# Executar instalação
./scripts/deploy.sh install
```

#### Instalação Manual

```bash
# Criar diretórios necessários
mkdir -p logs uploads backups

# Baixar imagens Docker
docker-compose pull

# Construir imagens customizadas
docker-compose build

# Iniciar banco de dados
docker-compose up -d postgres redis

# Aguardar banco ficar disponível
sleep 30

# Executar migrations (se necessário)
docker-compose run --rm backend dotnet ef database update

# Iniciar todos os serviços
docker-compose up -d

# Verificar status
docker-compose ps
```

### 5. Verificação da Instalação

#### Health Checks

```bash
# Verificar status dos containers
docker-compose ps

# Verificar logs
docker-compose logs

# Testar conectividade
curl -f http://localhost:5000/health
curl -f http://localhost:3000
```

#### Teste de Funcionalidades

1. **Acesso ao Frontend:**
   - Abra http://localhost:3000
   - Verifique se a página de login carrega corretamente

2. **Acesso à API:**
   - Abra http://localhost:5000/swagger
   - Verifique se a documentação da API está disponível

3. **Teste de Login:**
   - Use credenciais padrão: admin@acervo.com / Admin123!
   - Verifique se consegue acessar o dashboard

## Configuração de Produção

### SSL/TLS com Let's Encrypt

```bash
# Instalar Certbot
sudo apt install -y certbot python3-certbot-nginx

# Obter certificado
sudo certbot --nginx -d acervo.suaempresa.com -d api.acervo.suaempresa.com

# Configurar renovação automática
sudo crontab -e
# Adicionar linha:
# 0 12 * * * /usr/bin/certbot renew --quiet
```

### Nginx como Proxy Reverso

**Arquivo /etc/nginx/sites-available/acervo-educacional:**
```nginx
server {
    listen 80;
    server_name acervo.suaempresa.com;
    return 301 https://$server_name$request_uri;
}

server {
    listen 443 ssl http2;
    server_name acervo.suaempresa.com;

    ssl_certificate /etc/letsencrypt/live/acervo.suaempresa.com/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/acervo.suaempresa.com/privkey.pem;

    location / {
        proxy_pass http://localhost:3000;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}

server {
    listen 443 ssl http2;
    server_name api.acervo.suaempresa.com;

    ssl_certificate /etc/letsencrypt/live/acervo.suaempresa.com/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/acervo.suaempresa.com/privkey.pem;

    location / {
        proxy_pass http://localhost:5000;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

```bash
# Habilitar site
sudo ln -s /etc/nginx/sites-available/acervo-educacional /etc/nginx/sites-enabled/

# Testar configuração
sudo nginx -t

# Reiniciar Nginx
sudo systemctl restart nginx
```

### Monitoramento

#### Configuração de Logs

```bash
# Configurar logrotate
sudo nano /etc/logrotate.d/acervo-educacional
```

**Conteúdo do arquivo:**
```
/opt/acervo-educacional/logs/*.log {
    daily
    missingok
    rotate 30
    compress
    delaycompress
    notifempty
    create 644 root root
    postrotate
        docker-compose -f /opt/acervo-educacional/docker-compose.yml restart backend
    endscript
}
```

#### Configuração de Backup Automático

```bash
# Editar crontab
sudo crontab -e

# Adicionar linha para backup diário às 2h
0 2 * * * cd /opt/acervo-educacional && ./scripts/deploy.sh backup
```

## Troubleshooting

### Problemas Comuns

#### Container não inicia

```bash
# Verificar logs detalhados
docker-compose logs [nome-do-servico]

# Verificar recursos do sistema
docker system df
docker system prune

# Reiniciar Docker
sudo systemctl restart docker
```

#### Erro de conectividade com banco

```bash
# Verificar se PostgreSQL está rodando
docker-compose ps postgres

# Verificar logs do PostgreSQL
docker-compose logs postgres

# Testar conectividade
docker-compose exec postgres pg_isready -U acervo_user
```

#### Problemas de permissão

```bash
# Corrigir permissões de arquivos
sudo chown -R $USER:$USER /opt/acervo-educacional

# Corrigir permissões de scripts
chmod +x scripts/*.sh
```

### Comandos Úteis

```bash
# Ver status de todos os serviços
./scripts/deploy.sh status

# Ver logs em tempo real
./scripts/deploy.sh logs

# Fazer backup manual
./scripts/deploy.sh backup

# Reiniciar todos os serviços
./scripts/deploy.sh restart

# Atualizar sistema
./scripts/deploy.sh update

# Limpeza de recursos não utilizados
./scripts/deploy.sh cleanup
```

## Manutenção

### Atualizações

```bash
# Backup antes da atualização
./scripts/deploy.sh backup

# Atualizar código
git pull origin main

# Atualizar sistema
./scripts/deploy.sh update
```

### Monitoramento de Performance

```bash
# Verificar uso de recursos
docker stats

# Verificar espaço em disco
df -h

# Verificar logs de erro
docker-compose logs | grep ERROR
```

### Backup e Restauração

```bash
# Backup manual
./scripts/deploy.sh backup

# Restaurar backup (exemplo)
docker-compose down
docker-compose up -d postgres
sleep 30
docker-compose exec -T postgres psql -U acervo_user -d acervo_educacional < backup.sql
docker-compose up -d
```

---

**Guia elaborado por:** Manus AI  
**Última atualização:** Dezembro 2024  
**Versão:** 1.0

Para suporte adicional, consulte a documentação técnica ou entre em contato com a equipe de desenvolvimento.

