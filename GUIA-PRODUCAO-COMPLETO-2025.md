# 🚀 **GUIA COMPLETO DE PRODUÇÃO**
## **Sistema Acervo Educacional Ferreira Costa - 2025**

---

## 📋 **ÍNDICE**

1. [Pré-requisitos e Infraestrutura](#pré-requisitos)
2. [Configuração de Ambiente](#configuração-ambiente)
3. [Deploy do Backend](#deploy-backend)
4. [Deploy do Frontend](#deploy-frontend)
5. [Configurações de Segurança](#segurança)
6. [Monitoramento e Logs](#monitoramento)
7. [Backup e Recuperação](#backup)
8. [Checklist de Go-Live](#checklist)
9. [Troubleshooting](#troubleshooting)
10. [Manutenção Contínua](#manutenção)

---

## 🎯 **PRÉ-REQUISITOS E INFRAESTRUTURA** {#pré-requisitos}

### **💻 Infraestrutura Mínima Recomendada**

#### **🖥️ Servidor de Aplicação**
```
Especificações Mínimas:
├── CPU: 4 vCPUs (Intel/AMD x64)
├── RAM: 8 GB DDR4
├── Storage: 100 GB SSD
├── Rede: 1 Gbps
├── OS: Ubuntu 22.04 LTS ou Windows Server 2022
└── Uptime: 99.9% SLA
```

#### **🗄️ Banco de Dados**
```
PostgreSQL 15+:
├── CPU: 2 vCPUs dedicados
├── RAM: 4 GB DDR4
├── Storage: 50 GB SSD (IOPS 3000+)
├── Backup: Automático diário
└── Replicação: Master-Slave (recomendado)
```

#### **☁️ Serviços Cloud Necessários**
```
Azure/AWS Services:
├── 🗄️ Azure Database for PostgreSQL / AWS RDS
├── 📁 Azure Blob Storage / AWS S3
├── 🔐 Azure Key Vault / AWS Secrets Manager
├── 📊 Azure Application Insights / AWS CloudWatch
├── 🌐 Azure CDN / AWS CloudFront
├── 🔒 Azure AD / AWS Cognito (SSO)
└── 📧 SendGrid / AWS SES (Email)
```

### **🔧 Ferramentas de Desenvolvimento**

#### **Backend (.NET)**
```bash
# .NET 8 SDK
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt-get update
sudo apt-get install -y dotnet-sdk-8.0

# Entity Framework Tools
dotnet tool install --global dotnet-ef
```

#### **Frontend (Node.js)**
```bash
# Node.js 20 LTS
curl -fsSL https://deb.nodesource.com/setup_20.x | sudo -E bash -
sudo apt-get install -y nodejs

# Package Manager
npm install -g pnpm@latest
```

#### **Ferramentas de Deploy**
```bash
# Docker
sudo apt-get update
sudo apt-get install -y docker.io docker-compose-v2
sudo systemctl enable docker

# Nginx
sudo apt-get install -y nginx
sudo systemctl enable nginx
```

---

## ⚙️ **CONFIGURAÇÃO DE AMBIENTE** {#configuração-ambiente}

### **🔐 Variáveis de Ambiente de Produção**

#### **Backend (.env)**
```bash
# Ambiente
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://0.0.0.0:5000

# Segurança
SECURITY_ENABLE_BOLA_PROTECTION=true
SECURITY_ENABLE_STRICT_CORS=true
SECURITY_PRODUCTION_MODE=true

# Database
DATABASE_CONNECTION_STRING="Host=prod-db.postgres.database.azure.com;Database=acervo_educacional;Username=acervo_admin;Password=${DB_PASSWORD};SSL Mode=Require;"

# JWT
JWT_SECRET_KEY="${JWT_SECRET_256_BITS}"
JWT_ISSUER="https://acervo.ferreiracosta.com"
JWT_AUDIENCE="https://acervo.ferreiracosta.com"
JWT_EXPIRY_HOURS=8

# AWS S3
AWS_ACCESS_KEY="${AWS_ACCESS_KEY}"
AWS_SECRET_KEY="${AWS_SECRET_KEY}"
AWS_BUCKET_NAME="acervo-educacional-prod"
AWS_REGION="us-east-1"

# Email
EMAIL_SMTP_HOST="smtp.sendgrid.net"
EMAIL_SMTP_PORT=587
EMAIL_USERNAME="apikey"
EMAIL_PASSWORD="${SENDGRID_API_KEY}"
EMAIL_FROM="noreply@ferreiracosta.com"

# CORS
CORS_ALLOWED_ORIGINS="https://acervo.ferreiracosta.com,https://app.ferreiracosta.com"

# Hangfire
HANGFIRE_DASHBOARD_PATH="/admin/jobs"
HANGFIRE_DASHBOARD_USERNAME="admin"
HANGFIRE_DASHBOARD_PASSWORD="${HANGFIRE_PASSWORD}"

# Monitoring
APPLICATION_INSIGHTS_CONNECTION_STRING="${AI_CONNECTION_STRING}"
SENTRY_DSN="${SENTRY_DSN}"
```

#### **Frontend (.env.production)**
```bash
# API
VITE_API_BASE_URL=https://api.acervo.ferreiracosta.com
VITE_API_VERSION=v1

# Features
VITE_ENABLE_ANALYTICS=true
VITE_ENABLE_ERROR_TRACKING=true

# CDN
VITE_CDN_BASE_URL=https://cdn.ferreiracosta.com

# Auth
VITE_JWT_STORAGE_KEY=acervo_token
VITE_REFRESH_TOKEN_KEY=acervo_refresh

# Environment
VITE_ENVIRONMENT=production
VITE_DEBUG_MODE=false
```

### **🗄️ Configuração do Banco de Dados**

#### **PostgreSQL Setup**
```sql
-- Criar database
CREATE DATABASE acervo_educacional
    WITH ENCODING 'UTF8'
    LC_COLLATE = 'pt_BR.UTF-8'
    LC_CTYPE = 'pt_BR.UTF-8'
    TEMPLATE template0;

-- Criar usuário
CREATE USER acervo_admin WITH PASSWORD 'senha_super_segura_123!';
GRANT ALL PRIVILEGES ON DATABASE acervo_educacional TO acervo_admin;

-- Configurações de performance
ALTER SYSTEM SET shared_buffers = '256MB';
ALTER SYSTEM SET effective_cache_size = '1GB';
ALTER SYSTEM SET maintenance_work_mem = '64MB';
ALTER SYSTEM SET checkpoint_completion_target = 0.9;
ALTER SYSTEM SET wal_buffers = '16MB';
ALTER SYSTEM SET default_statistics_target = 100;

-- Reload configurações
SELECT pg_reload_conf();
```

#### **Migrations e Seed Data**
```bash
# Aplicar migrations
cd backend/AcervoEducacional.WebApi
dotnet ef database update --connection "${DATABASE_CONNECTION_STRING}"

# Seed data inicial
dotnet run --seed-data
```

---

## 🔧 **DEPLOY DO BACKEND** {#deploy-backend}

### **🐳 Containerização com Docker**

#### **Dockerfile**
```dockerfile
# backend/Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
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

# Publish application
FROM build AS publish
RUN dotnet publish "AcervoEducacional.WebApi.csproj" -c Release -o /app/publish

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
  CMD curl -f http://localhost:5000/health || exit 1

ENTRYPOINT ["dotnet", "AcervoEducacional.WebApi.dll"]
```

#### **docker-compose.yml**
```yaml
version: '3.8'

services:
  acervo-api:
    build:
      context: ./backend
      dockerfile: Dockerfile
    container_name: acervo-api
    restart: unless-stopped
    ports:
      - "5000:5000"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=http://0.0.0.0:5000
    env_file:
      - .env
    depends_on:
      - postgres
    networks:
      - acervo-network
    volumes:
      - ./logs:/app/logs
      - ./uploads:/app/uploads

  postgres:
    image: postgres:15-alpine
    container_name: acervo-postgres
    restart: unless-stopped
    environment:
      POSTGRES_DB: acervo_educacional
      POSTGRES_USER: acervo_admin
      POSTGRES_PASSWORD: ${DB_PASSWORD}
    volumes:
      - postgres_data:/var/lib/postgresql/data
      - ./backups:/backups
    ports:
      - "5432:5432"
    networks:
      - acervo-network

  nginx:
    image: nginx:alpine
    container_name: acervo-nginx
    restart: unless-stopped
    ports:
      - "80:80"
      - "443:443"
    volumes:
      - ./nginx/nginx.conf:/etc/nginx/nginx.conf
      - ./nginx/ssl:/etc/nginx/ssl
      - ./frontend/dist:/usr/share/nginx/html
    depends_on:
      - acervo-api
    networks:
      - acervo-network

volumes:
  postgres_data:

networks:
  acervo-network:
    driver: bridge
```

### **🚀 Deploy Commands**
```bash
# Build e deploy
docker-compose build --no-cache
docker-compose up -d

# Verificar status
docker-compose ps
docker-compose logs -f acervo-api

# Health check
curl -f http://localhost:5000/health
```

---

## 🌐 **DEPLOY DO FRONTEND** {#deploy-frontend}

### **📦 Build de Produção**

#### **Configuração Vite**
```javascript
// vite.config.js
import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import { resolve } from 'path'

export default defineConfig({
  plugins: [react()],
  base: '/',
  build: {
    outDir: 'dist',
    sourcemap: false,
    minify: 'terser',
    rollupOptions: {
      output: {
        manualChunks: {
          vendor: ['react', 'react-dom'],
          ui: ['@radix-ui/react-dialog', '@radix-ui/react-dropdown-menu'],
          router: ['react-router-dom'],
          forms: ['react-hook-form', '@hookform/resolvers'],
          charts: ['recharts'],
          utils: ['date-fns', 'clsx', 'tailwind-merge']
        }
      }
    },
    terserOptions: {
      compress: {
        drop_console: true,
        drop_debugger: true
      }
    }
  },
  resolve: {
    alias: {
      '@': resolve(__dirname, './src')
    }
  },
  server: {
    host: '0.0.0.0',
    port: 5173
  }
})
```

#### **Build Commands**
```bash
# Install dependencies
cd frontend
pnpm install --frozen-lockfile

# Build for production
pnpm run build

# Preview build
pnpm run preview

# Analyze bundle
npx vite-bundle-analyzer dist
```

### **🌐 Configuração Nginx**

#### **nginx.conf**
```nginx
events {
    worker_connections 1024;
}

http {
    include       /etc/nginx/mime.types;
    default_type  application/octet-stream;
    
    # Logging
    log_format main '$remote_addr - $remote_user [$time_local] "$request" '
                   '$status $body_bytes_sent "$http_referer" '
                   '"$http_user_agent" "$http_x_forwarded_for"';
    
    access_log /var/log/nginx/access.log main;
    error_log /var/log/nginx/error.log warn;
    
    # Performance
    sendfile on;
    tcp_nopush on;
    tcp_nodelay on;
    keepalive_timeout 65;
    types_hash_max_size 2048;
    client_max_body_size 100M;
    
    # Gzip compression
    gzip on;
    gzip_vary on;
    gzip_min_length 1024;
    gzip_proxied any;
    gzip_comp_level 6;
    gzip_types
        text/plain
        text/css
        text/xml
        text/javascript
        application/json
        application/javascript
        application/xml+rss
        application/atom+xml
        image/svg+xml;
    
    # Rate limiting
    limit_req_zone $binary_remote_addr zone=api:10m rate=10r/s;
    limit_req_zone $binary_remote_addr zone=login:10m rate=5r/m;
    
    # SSL Configuration
    ssl_protocols TLSv1.2 TLSv1.3;
    ssl_ciphers ECDHE-RSA-AES256-GCM-SHA512:DHE-RSA-AES256-GCM-SHA512:ECDHE-RSA-AES256-GCM-SHA384:DHE-RSA-AES256-GCM-SHA384;
    ssl_prefer_server_ciphers off;
    ssl_session_cache shared:SSL:10m;
    ssl_session_timeout 10m;
    
    # Security headers
    add_header X-Frame-Options DENY always;
    add_header X-Content-Type-Options nosniff always;
    add_header X-XSS-Protection "1; mode=block" always;
    add_header Referrer-Policy "strict-origin-when-cross-origin" always;
    add_header Content-Security-Policy "default-src 'self'; script-src 'self' 'unsafe-inline' 'unsafe-eval'; style-src 'self' 'unsafe-inline'; img-src 'self' data: https:; font-src 'self' data:; connect-src 'self' https://api.acervo.ferreiracosta.com;" always;
    
    # Frontend (React)
    server {
        listen 80;
        listen [::]:80;
        server_name acervo.ferreiracosta.com;
        
        # Redirect HTTP to HTTPS
        return 301 https://$server_name$request_uri;
    }
    
    server {
        listen 443 ssl http2;
        listen [::]:443 ssl http2;
        server_name acervo.ferreiracosta.com;
        
        # SSL certificates
        ssl_certificate /etc/nginx/ssl/acervo.ferreiracosta.com.crt;
        ssl_certificate_key /etc/nginx/ssl/acervo.ferreiracosta.com.key;
        
        # Document root
        root /usr/share/nginx/html;
        index index.html;
        
        # Frontend routes (SPA)
        location / {
            try_files $uri $uri/ /index.html;
            
            # Cache static assets
            location ~* \.(js|css|png|jpg|jpeg|gif|ico|svg|woff|woff2|ttf|eot)$ {
                expires 1y;
                add_header Cache-Control "public, immutable";
                add_header Vary Accept-Encoding;
            }
        }
        
        # API proxy
        location /api/ {
            limit_req zone=api burst=20 nodelay;
            
            proxy_pass http://acervo-api:5000;
            proxy_http_version 1.1;
            proxy_set_header Upgrade $http_upgrade;
            proxy_set_header Connection 'upgrade';
            proxy_set_header Host $host;
            proxy_set_header X-Real-IP $remote_addr;
            proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
            proxy_set_header X-Forwarded-Proto $scheme;
            proxy_cache_bypass $http_upgrade;
            
            # Timeouts
            proxy_connect_timeout 60s;
            proxy_send_timeout 60s;
            proxy_read_timeout 60s;
        }
        
        # Auth endpoints (stricter rate limiting)
        location /api/v1/auth/ {
            limit_req zone=login burst=5 nodelay;
            
            proxy_pass http://acervo-api:5000;
            proxy_http_version 1.1;
            proxy_set_header Host $host;
            proxy_set_header X-Real-IP $remote_addr;
            proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
            proxy_set_header X-Forwarded-Proto $scheme;
        }
        
        # Health checks
        location /health {
            proxy_pass http://acervo-api:5000;
            access_log off;
        }
        
        # Hangfire dashboard (admin only)
        location /admin/jobs {
            auth_basic "Restricted Area";
            auth_basic_user_file /etc/nginx/.htpasswd;
            
            proxy_pass http://acervo-api:5000;
            proxy_set_header Host $host;
            proxy_set_header X-Real-IP $remote_addr;
            proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
            proxy_set_header X-Forwarded-Proto $scheme;
        }
    }
}
```

---

## 🔒 **CONFIGURAÇÕES DE SEGURANÇA** {#segurança}

### **🔐 SSL/TLS Certificate**
```bash
# Usando Let's Encrypt
sudo apt-get install certbot python3-certbot-nginx

# Obter certificado
sudo certbot --nginx -d acervo.ferreiracosta.com -d api.acervo.ferreiracosta.com

# Auto-renewal
sudo crontab -e
# Adicionar: 0 12 * * * /usr/bin/certbot renew --quiet
```

### **🛡️ Firewall Configuration**
```bash
# UFW Firewall
sudo ufw enable
sudo ufw default deny incoming
sudo ufw default allow outgoing

# Allow necessary ports
sudo ufw allow 22/tcp    # SSH
sudo ufw allow 80/tcp    # HTTP
sudo ufw allow 443/tcp   # HTTPS
sudo ufw allow 5432/tcp  # PostgreSQL (internal only)

# Check status
sudo ufw status verbose
```

### **🔑 Secrets Management**

#### **Azure Key Vault Setup**
```bash
# Install Azure CLI
curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash

# Login
az login

# Create Key Vault
az keyvault create \
  --name "acervo-keyvault" \
  --resource-group "acervo-rg" \
  --location "East US"

# Add secrets
az keyvault secret set --vault-name "acervo-keyvault" --name "DatabasePassword" --value "senha_super_segura"
az keyvault secret set --vault-name "acervo-keyvault" --name "JwtSecretKey" --value "chave_jwt_256_bits"
az keyvault secret set --vault-name "acervo-keyvault" --name "AwsAccessKey" --value "aws_access_key"
```

#### **Application Configuration**
```csharp
// Program.cs - Key Vault integration
builder.Configuration.AddAzureKeyVault(
    new Uri($"https://acervo-keyvault.vault.azure.net/"),
    new DefaultAzureCredential());
```

---

## 📊 **MONITORAMENTO E LOGS** {#monitoramento}

### **📈 Application Insights Setup**
```csharp
// Program.cs
builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.ConnectionString = builder.Configuration.GetConnectionString("ApplicationInsights");
});

// Custom telemetry
builder.Services.AddSingleton<ITelemetryInitializer, CustomTelemetryInitializer>();
```

### **📋 Structured Logging**
```csharp
// Program.cs - Serilog configuration
builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithEnvironmentUserName()
        .WriteTo.Console()
        .WriteTo.File("logs/acervo-.log", rollingInterval: RollingInterval.Day)
        .WriteTo.ApplicationInsights(TelemetryConfiguration.CreateDefault(), TelemetryConverter.Traces);
});
```

### **🚨 Alertas e Notificações**
```json
{
  "AlertRules": [
    {
      "Name": "High Error Rate",
      "Condition": "requests/failed > 10 in 5 minutes",
      "Action": "Send email to admin@ferreiracosta.com"
    },
    {
      "Name": "High Response Time",
      "Condition": "requests/duration > 2000ms in 5 minutes",
      "Action": "Send Slack notification"
    },
    {
      "Name": "Database Connection Issues",
      "Condition": "dependencies/failed where target contains 'postgres' > 5 in 2 minutes",
      "Action": "Send SMS to on-call engineer"
    }
  ]
}
```

### **📊 Dashboards**
```json
{
  "Dashboards": [
    {
      "Name": "Application Overview",
      "Widgets": [
        "Request Rate",
        "Response Time",
        "Error Rate",
        "Active Users",
        "Database Performance",
        "Memory Usage",
        "CPU Usage"
      ]
    },
    {
      "Name": "Business Metrics",
      "Widgets": [
        "Daily Active Users",
        "Course Completions",
        "File Uploads",
        "User Registrations",
        "API Usage by Endpoint"
      ]
    }
  ]
}
```

---

## 💾 **BACKUP E RECUPERAÇÃO** {#backup}

### **🗄️ Database Backup**
```bash
#!/bin/bash
# backup-database.sh

DATE=$(date +%Y%m%d_%H%M%S)
BACKUP_DIR="/backups/database"
DB_NAME="acervo_educacional"
DB_USER="acervo_admin"
DB_HOST="localhost"

# Create backup directory
mkdir -p $BACKUP_DIR

# Full backup
pg_dump -h $DB_HOST -U $DB_USER -d $DB_NAME -F c -b -v -f "$BACKUP_DIR/acervo_full_$DATE.backup"

# Compress backup
gzip "$BACKUP_DIR/acervo_full_$DATE.backup"

# Upload to cloud storage
aws s3 cp "$BACKUP_DIR/acervo_full_$DATE.backup.gz" s3://acervo-backups/database/

# Clean old backups (keep last 30 days)
find $BACKUP_DIR -name "*.backup.gz" -mtime +30 -delete

echo "Backup completed: acervo_full_$DATE.backup.gz"
```

### **📁 File System Backup**
```bash
#!/bin/bash
# backup-files.sh

DATE=$(date +%Y%m%d_%H%M%S)
BACKUP_DIR="/backups/files"
SOURCE_DIR="/app/uploads"

# Create backup directory
mkdir -p $BACKUP_DIR

# Incremental backup using rsync
rsync -av --delete $SOURCE_DIR/ $BACKUP_DIR/current/

# Create daily snapshot
cp -al $BACKUP_DIR/current/ $BACKUP_DIR/daily_$DATE/

# Upload to cloud storage
aws s3 sync $BACKUP_DIR/daily_$DATE/ s3://acervo-backups/files/$DATE/

# Clean old snapshots (keep last 7 days)
find $BACKUP_DIR -name "daily_*" -mtime +7 -exec rm -rf {} \;

echo "File backup completed: daily_$DATE"
```

### **🔄 Automated Backup Schedule**
```bash
# Crontab configuration
sudo crontab -e

# Database backup - daily at 2 AM
0 2 * * * /opt/scripts/backup-database.sh >> /var/log/backup-db.log 2>&1

# File backup - every 6 hours
0 */6 * * * /opt/scripts/backup-files.sh >> /var/log/backup-files.log 2>&1

# Health check backup - weekly on Sunday at 3 AM
0 3 * * 0 /opt/scripts/test-restore.sh >> /var/log/test-restore.log 2>&1
```

### **🚑 Disaster Recovery Plan**
```bash
#!/bin/bash
# disaster-recovery.sh

echo "Starting disaster recovery process..."

# 1. Restore database
echo "Restoring database..."
LATEST_DB_BACKUP=$(aws s3 ls s3://acervo-backups/database/ | sort | tail -n 1 | awk '{print $4}')
aws s3 cp s3://acervo-backups/database/$LATEST_DB_BACKUP /tmp/
gunzip /tmp/$LATEST_DB_BACKUP
pg_restore -h localhost -U acervo_admin -d acervo_educacional_new /tmp/${LATEST_DB_BACKUP%.gz}

# 2. Restore files
echo "Restoring files..."
LATEST_FILE_BACKUP=$(aws s3 ls s3://acervo-backups/files/ | sort | tail -n 1 | awk '{print $2}')
aws s3 sync s3://acervo-backups/files/$LATEST_FILE_BACKUP /app/uploads/

# 3. Update configuration
echo "Updating configuration..."
# Update database connection string to point to restored database

# 4. Restart services
echo "Restarting services..."
docker-compose restart

# 5. Verify system health
echo "Verifying system health..."
curl -f http://localhost:5000/health

echo "Disaster recovery completed!"
```

---

## ✅ **CHECKLIST DE GO-LIVE** {#checklist}

### **🔧 Pré-Deploy**
```
□ Infraestrutura provisionada e configurada
□ Certificados SSL instalados e válidos
□ Banco de dados criado e migrations aplicadas
□ Variáveis de ambiente configuradas
□ Secrets armazenados no Key Vault
□ Backup automático configurado
□ Monitoramento e alertas configurados
□ Load balancer configurado (se aplicável)
□ CDN configurado para assets estáticos
□ DNS configurado e propagado
```

### **🧪 Testes de Produção**
```
□ Smoke tests executados com sucesso
□ Testes de carga realizados
□ Testes de segurança (penetration testing)
□ Testes de backup e restore
□ Testes de failover
□ Validação de performance
□ Testes de integração com sistemas externos
□ Validação de logs e monitoramento
□ Testes de SSL/TLS
□ Validação de CORS e headers de segurança
```

### **👥 Preparação da Equipe**
```
□ Documentação de produção atualizada
□ Runbooks criados para operações comuns
□ Equipe de suporte treinada
□ Plano de comunicação definido
□ Contatos de emergência atualizados
□ Procedimentos de rollback documentados
□ Escalation matrix definida
□ Horário de go-live comunicado
□ Stakeholders notificados
□ Plano de contingência aprovado
```

### **🚀 Go-Live**
```
□ Backup final do ambiente atual
□ Deploy executado conforme procedimento
□ Smoke tests pós-deploy executados
□ Monitoramento ativo durante go-live
□ Equipe de suporte em standby
□ Comunicação de sucesso enviada
□ Documentação pós-go-live atualizada
□ Lessons learned documentadas
□ Métricas de baseline coletadas
□ Plano de otimização pós-go-live criado
```

---

## 🔧 **TROUBLESHOOTING** {#troubleshooting}

### **🚨 Problemas Comuns**

#### **Backend não inicia**
```bash
# Verificar logs
docker-compose logs acervo-api

# Problemas comuns:
# 1. Variáveis de ambiente incorretas
docker-compose exec acervo-api env | grep -E "(DATABASE|JWT|AWS)"

# 2. Banco de dados inacessível
docker-compose exec acervo-api dotnet ef database update --dry-run

# 3. Porta em uso
sudo netstat -tulpn | grep :5000
sudo lsof -i :5000

# 4. Permissões de arquivo
sudo chown -R 1000:1000 /app/uploads
sudo chmod -R 755 /app/uploads
```

#### **Frontend não carrega**
```bash
# Verificar Nginx
sudo nginx -t
sudo systemctl status nginx
sudo tail -f /var/log/nginx/error.log

# Verificar build
ls -la /usr/share/nginx/html/
cat /usr/share/nginx/html/index.html

# Verificar proxy para API
curl -I http://localhost/api/health
```

#### **Banco de dados lento**
```sql
-- Verificar conexões ativas
SELECT count(*) FROM pg_stat_activity;

-- Verificar queries lentas
SELECT query, mean_exec_time, calls 
FROM pg_stat_statements 
ORDER BY mean_exec_time DESC 
LIMIT 10;

-- Verificar locks
SELECT * FROM pg_locks WHERE NOT granted;

-- Reindex se necessário
REINDEX DATABASE acervo_educacional;
```

#### **Problemas de memória**
```bash
# Verificar uso de memória
free -h
docker stats

# Verificar logs de OOM
dmesg | grep -i "killed process"
journalctl -u docker.service | grep -i "oom"

# Ajustar limites do container
# docker-compose.yml
services:
  acervo-api:
    deploy:
      resources:
        limits:
          memory: 2G
        reservations:
          memory: 1G
```

### **📊 Comandos de Diagnóstico**
```bash
# Health check completo
curl -s http://localhost:5000/health | jq .

# Verificar conectividade do banco
docker-compose exec acervo-api dotnet ef database update --dry-run

# Verificar logs estruturados
docker-compose logs acervo-api | grep -E "(ERROR|WARN|FATAL)"

# Verificar performance
curl -w "@curl-format.txt" -o /dev/null -s http://localhost:5000/api/v1/dashboard/stats

# Verificar SSL
openssl s_client -connect acervo.ferreiracosta.com:443 -servername acervo.ferreiracosta.com

# Verificar DNS
nslookup acervo.ferreiracosta.com
dig acervo.ferreiracosta.com
```

---

## 🔄 **MANUTENÇÃO CONTÍNUA** {#manutenção}

### **📅 Tarefas Diárias**
```bash
# Verificar health checks
curl -f http://localhost:5000/health

# Verificar logs de erro
docker-compose logs acervo-api | grep -E "(ERROR|FATAL)" | tail -20

# Verificar uso de disco
df -h
docker system df

# Verificar backups
ls -la /backups/database/ | tail -5
aws s3 ls s3://acervo-backups/database/ | tail -5
```

### **📅 Tarefas Semanais**
```bash
# Atualizar dependências de segurança
sudo apt update && sudo apt upgrade -y

# Limpar logs antigos
sudo journalctl --vacuum-time=7d
find /var/log -name "*.log" -mtime +7 -delete

# Verificar certificados SSL
openssl x509 -in /etc/nginx/ssl/acervo.ferreiracosta.com.crt -text -noout | grep "Not After"

# Revisar métricas de performance
# Acessar dashboard do Application Insights

# Verificar integridade dos backups
/opt/scripts/test-restore.sh
```

### **📅 Tarefas Mensais**
```bash
# Atualizar imagens Docker
docker-compose pull
docker-compose up -d

# Revisar logs de segurança
sudo grep -i "failed\|error\|denied" /var/log/auth.log | tail -50

# Otimizar banco de dados
docker-compose exec postgres psql -U acervo_admin -d acervo_educacional -c "VACUUM ANALYZE;"

# Revisar e atualizar documentação
# Verificar se procedimentos estão atualizados

# Revisar alertas e métricas
# Ajustar thresholds se necessário
```

### **🔄 Processo de Atualização**
```bash
#!/bin/bash
# update-application.sh

echo "Starting application update process..."

# 1. Backup antes da atualização
/opt/scripts/backup-database.sh
/opt/scripts/backup-files.sh

# 2. Pull latest code
cd /opt/acervo-edu
git fetch origin
git checkout main
git pull origin main

# 3. Build new images
docker-compose build --no-cache

# 4. Run database migrations
docker-compose run --rm acervo-api dotnet ef database update

# 5. Deploy new version
docker-compose up -d

# 6. Verify deployment
sleep 30
curl -f http://localhost:5000/health

# 7. Run smoke tests
/opt/scripts/smoke-tests.sh

echo "Application update completed successfully!"
```

### **📊 Métricas de Monitoramento**
```json
{
  "KPIs": {
    "Performance": {
      "ResponseTime": "< 500ms (95th percentile)",
      "Throughput": "> 100 requests/second",
      "ErrorRate": "< 1%",
      "Uptime": "> 99.9%"
    },
    "Business": {
      "DailyActiveUsers": "Track trend",
      "CourseCompletions": "Track trend",
      "FileUploads": "Track volume",
      "UserSatisfaction": "> 4.5/5"
    },
    "Infrastructure": {
      "CPUUsage": "< 70%",
      "MemoryUsage": "< 80%",
      "DiskUsage": "< 85%",
      "DatabaseConnections": "< 80% of max"
    }
  }
}
```

---

## 🎯 **CONCLUSÃO**

Este guia fornece uma base sólida para colocar o Sistema Acervo Educacional Ferreira Costa em produção com segurança e confiabilidade. 

### **📋 Próximos Passos Recomendados:**

1. **Revisar e adaptar** as configurações para o ambiente específico da Ferreira Costa
2. **Executar testes** em ambiente de staging antes do go-live
3. **Treinar a equipe** nos procedimentos operacionais
4. **Implementar monitoramento** proativo
5. **Estabelecer rotinas** de manutenção e backup

### **🚨 Pontos Críticos de Atenção:**

- **Segurança:** Ativar todas as flags de produção
- **Performance:** Monitorar métricas continuamente
- **Backup:** Testar procedimentos de restore regularmente
- **Monitoramento:** Configurar alertas apropriados
- **Documentação:** Manter procedimentos atualizados

**Sistema pronto para produção com este guia completo!** 🚀

---

**Guia de Produção - Sistema Acervo Educacional Ferreira Costa**  
**Versão 1.0 - 02/07/2025**  
**Status: PRONTO PARA IMPLEMENTAÇÃO** ✅

