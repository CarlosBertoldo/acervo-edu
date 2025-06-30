#!/bin/bash

# Script de Backup Automático - Sistema Acervo Educacional
# Executa backup diário do PostgreSQL e arquivos

set -e

# Configurações
BACKUP_DIR="/backup"
DB_HOST="postgres"
DB_NAME="acervo_educacional"
DB_USER="acervo_user"
DATE=$(date +%Y%m%d_%H%M%S)
RETENTION_DAYS=30

# Cores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Função de log
log() {
    echo -e "${GREEN}[$(date '+%Y-%m-%d %H:%M:%S')]${NC} $1"
}

error() {
    echo -e "${RED}[$(date '+%Y-%m-%d %H:%M:%S')] ERROR:${NC} $1" >&2
}

warning() {
    echo -e "${YELLOW}[$(date '+%Y-%m-%d %H:%M:%S')] WARNING:${NC} $1"
}

# Criar diretórios de backup
mkdir -p "$BACKUP_DIR/database"
mkdir -p "$BACKUP_DIR/logs"

log "Iniciando backup do Sistema Acervo Educacional..."

# Backup do banco de dados
log "Realizando backup do banco de dados PostgreSQL..."
BACKUP_FILE="$BACKUP_DIR/database/acervo_educacional_$DATE.sql"

if pg_dump -h "$DB_HOST" -U "$DB_USER" -d "$DB_NAME" --verbose --clean --no-owner --no-privileges > "$BACKUP_FILE"; then
    log "Backup do banco de dados concluído: $BACKUP_FILE"
    
    # Comprimir backup
    if gzip "$BACKUP_FILE"; then
        log "Backup comprimido: $BACKUP_FILE.gz"
        BACKUP_FILE="$BACKUP_FILE.gz"
    else
        warning "Falha ao comprimir backup, mantendo arquivo original"
    fi
else
    error "Falha no backup do banco de dados"
    exit 1
fi

# Backup de logs (se existirem)
if [ -d "/app/logs" ]; then
    log "Realizando backup dos logs..."
    LOG_BACKUP="$BACKUP_DIR/logs/logs_$DATE.tar.gz"
    
    if tar -czf "$LOG_BACKUP" -C /app logs/; then
        log "Backup dos logs concluído: $LOG_BACKUP"
    else
        warning "Falha no backup dos logs"
    fi
fi

# Limpeza de backups antigos
log "Removendo backups antigos (mais de $RETENTION_DAYS dias)..."

# Remover backups de banco antigos
find "$BACKUP_DIR/database" -name "*.sql.gz" -type f -mtime +$RETENTION_DAYS -delete 2>/dev/null || true
find "$BACKUP_DIR/database" -name "*.sql" -type f -mtime +$RETENTION_DAYS -delete 2>/dev/null || true

# Remover backups de logs antigos
find "$BACKUP_DIR/logs" -name "*.tar.gz" -type f -mtime +$RETENTION_DAYS -delete 2>/dev/null || true

# Verificar espaço em disco
DISK_USAGE=$(df "$BACKUP_DIR" | awk 'NR==2 {print $5}' | sed 's/%//')
if [ "$DISK_USAGE" -gt 85 ]; then
    warning "Uso de disco alto: ${DISK_USAGE}%. Considere aumentar o espaço ou reduzir a retenção."
fi

# Estatísticas do backup
BACKUP_SIZE=$(du -h "$BACKUP_FILE" | cut -f1)
TOTAL_BACKUPS=$(find "$BACKUP_DIR/database" -name "*.sql*" -type f | wc -l)

log "Backup concluído com sucesso!"
log "Tamanho do backup: $BACKUP_SIZE"
log "Total de backups mantidos: $TOTAL_BACKUPS"

# Opcional: Enviar backup para S3 (se configurado)
if [ -n "$AWS_ACCESS_KEY" ] && [ -n "$AWS_SECRET_KEY" ] && [ -n "$AWS_BACKUP_BUCKET" ]; then
    log "Enviando backup para AWS S3..."
    
    # Configurar AWS CLI (se disponível)
    if command -v aws >/dev/null 2>&1; then
        aws configure set aws_access_key_id "$AWS_ACCESS_KEY"
        aws configure set aws_secret_access_key "$AWS_SECRET_KEY"
        aws configure set default.region "$AWS_REGION"
        
        S3_KEY="backups/database/$(basename "$BACKUP_FILE")"
        
        if aws s3 cp "$BACKUP_FILE" "s3://$AWS_BACKUP_BUCKET/$S3_KEY"; then
            log "Backup enviado para S3: s3://$AWS_BACKUP_BUCKET/$S3_KEY"
        else
            warning "Falha ao enviar backup para S3"
        fi
    else
        warning "AWS CLI não disponível, pulando upload para S3"
    fi
fi

# Opcional: Notificação por webhook (se configurado)
if [ -n "$WEBHOOK_URL" ]; then
    log "Enviando notificação de backup..."
    
    PAYLOAD=$(cat <<EOF
{
    "text": "✅ Backup do Acervo Educacional concluído",
    "attachments": [
        {
            "color": "good",
            "fields": [
                {
                    "title": "Data/Hora",
                    "value": "$(date '+%d/%m/%Y %H:%M:%S')",
                    "short": true
                },
                {
                    "title": "Tamanho",
                    "value": "$BACKUP_SIZE",
                    "short": true
                },
                {
                    "title": "Arquivo",
                    "value": "$(basename "$BACKUP_FILE")",
                    "short": false
                }
            ]
        }
    ]
}
EOF
)

    if curl -X POST -H 'Content-type: application/json' --data "$PAYLOAD" "$WEBHOOK_URL" >/dev/null 2>&1; then
        log "Notificação enviada com sucesso"
    else
        warning "Falha ao enviar notificação"
    fi
fi

log "Script de backup finalizado."
exit 0

