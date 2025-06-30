#!/bin/bash

# Script de Deploy - Sistema Acervo Educacional
# Automatiza o processo de deploy em produção

set -e

# Configurações
PROJECT_NAME="acervo-educacional"
DOCKER_COMPOSE_FILE="docker-compose.yml"
ENV_FILE=".env"
BACKUP_DIR="./backups"

# Cores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
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

info() {
    echo -e "${BLUE}[$(date '+%Y-%m-%d %H:%M:%S')] INFO:${NC} $1"
}

# Função para mostrar ajuda
show_help() {
    echo "Script de Deploy - Sistema Acervo Educacional"
    echo ""
    echo "Uso: $0 [OPÇÃO]"
    echo ""
    echo "Opções:"
    echo "  install     Instalação inicial completa"
    echo "  update      Atualização do sistema"
    echo "  restart     Reiniciar serviços"
    echo "  stop        Parar todos os serviços"
    echo "  backup      Realizar backup manual"
    echo "  logs        Visualizar logs dos serviços"
    echo "  status      Verificar status dos serviços"
    echo "  cleanup     Limpeza de recursos não utilizados"
    echo "  help        Mostrar esta ajuda"
    echo ""
    echo "Exemplos:"
    echo "  $0 install    # Primeira instalação"
    echo "  $0 update     # Atualizar sistema existente"
    echo "  $0 logs api   # Ver logs do serviço API"
}

# Verificar pré-requisitos
check_prerequisites() {
    log "Verificando pré-requisitos..."
    
    # Verificar Docker
    if ! command -v docker >/dev/null 2>&1; then
        error "Docker não está instalado. Instale o Docker primeiro."
        exit 1
    fi
    
    # Verificar Docker Compose
    if ! command -v docker-compose >/dev/null 2>&1; then
        error "Docker Compose não está instalado. Instale o Docker Compose primeiro."
        exit 1
    fi
    
    # Verificar arquivo .env
    if [ ! -f "$ENV_FILE" ]; then
        warning "Arquivo .env não encontrado. Copiando do exemplo..."
        if [ -f ".env.example" ]; then
            cp .env.example "$ENV_FILE"
            warning "Configure as variáveis em $ENV_FILE antes de continuar."
            exit 1
        else
            error "Arquivo .env.example não encontrado."
            exit 1
        fi
    fi
    
    log "Pré-requisitos verificados com sucesso."
}

# Instalação inicial
install() {
    log "Iniciando instalação do Sistema Acervo Educacional..."
    
    check_prerequisites
    
    # Criar diretórios necessários
    log "Criando estrutura de diretórios..."
    mkdir -p "$BACKUP_DIR"
    mkdir -p "./logs"
    mkdir -p "./uploads"
    mkdir -p "./nginx/ssl"
    
    # Tornar scripts executáveis
    chmod +x ./scripts/*.sh
    
    # Baixar imagens Docker
    log "Baixando imagens Docker..."
    docker-compose pull
    
    # Construir imagens customizadas
    log "Construindo imagens customizadas..."
    docker-compose build
    
    # Inicializar banco de dados
    log "Inicializando banco de dados..."
    docker-compose up -d postgres redis
    
    # Aguardar banco ficar disponível
    log "Aguardando banco de dados ficar disponível..."
    sleep 30
    
    # Executar migrations
    log "Executando migrations do banco..."
    docker-compose run --rm backend dotnet ef database update || true
    
    # Iniciar todos os serviços
    log "Iniciando todos os serviços..."
    docker-compose up -d
    
    # Aguardar serviços ficarem prontos
    log "Aguardando serviços ficarem prontos..."
    sleep 60
    
    # Verificar status
    check_health
    
    log "Instalação concluída com sucesso!"
    log "Frontend: http://localhost:3000"
    log "Backend API: http://localhost:5000"
    log "Swagger: http://localhost:5000/swagger"
    log "Hangfire: http://localhost:5000/hangfire"
}

# Atualização do sistema
update() {
    log "Iniciando atualização do sistema..."
    
    # Backup antes da atualização
    backup
    
    # Parar serviços
    log "Parando serviços..."
    docker-compose down
    
    # Atualizar imagens
    log "Atualizando imagens..."
    docker-compose pull
    docker-compose build
    
    # Executar migrations
    log "Executando migrations..."
    docker-compose run --rm backend dotnet ef database update || true
    
    # Reiniciar serviços
    log "Reiniciando serviços..."
    docker-compose up -d
    
    # Verificar status
    sleep 30
    check_health
    
    log "Atualização concluída com sucesso!"
}

# Reiniciar serviços
restart() {
    log "Reiniciando serviços..."
    docker-compose restart
    sleep 30
    check_health
    log "Serviços reiniciados com sucesso!"
}

# Parar serviços
stop() {
    log "Parando todos os serviços..."
    docker-compose down
    log "Serviços parados com sucesso!"
}

# Backup manual
backup() {
    log "Iniciando backup manual..."
    
    if docker-compose ps postgres | grep -q "Up"; then
        docker-compose exec postgres /backup.sh
        log "Backup concluído com sucesso!"
    else
        error "Serviço PostgreSQL não está rodando."
        exit 1
    fi
}

# Visualizar logs
show_logs() {
    local service=${1:-""}
    
    if [ -n "$service" ]; then
        log "Mostrando logs do serviço: $service"
        docker-compose logs -f "$service"
    else
        log "Mostrando logs de todos os serviços..."
        docker-compose logs -f
    fi
}

# Verificar status dos serviços
status() {
    log "Status dos serviços:"
    docker-compose ps
    echo ""
    
    log "Uso de recursos:"
    docker stats --no-stream --format "table {{.Container}}\t{{.CPUPerc}}\t{{.MemUsage}}\t{{.NetIO}}\t{{.BlockIO}}"
}

# Verificar saúde dos serviços
check_health() {
    log "Verificando saúde dos serviços..."
    
    # Verificar PostgreSQL
    if docker-compose exec -T postgres pg_isready -U acervo_user -d acervo_educacional >/dev/null 2>&1; then
        log "✅ PostgreSQL: Saudável"
    else
        error "❌ PostgreSQL: Não responsivo"
    fi
    
    # Verificar Redis
    if docker-compose exec -T redis redis-cli ping >/dev/null 2>&1; then
        log "✅ Redis: Saudável"
    else
        error "❌ Redis: Não responsivo"
    fi
    
    # Verificar Backend
    if curl -f http://localhost:5000/health >/dev/null 2>&1; then
        log "✅ Backend API: Saudável"
    else
        error "❌ Backend API: Não responsivo"
    fi
    
    # Verificar Frontend
    if curl -f http://localhost:3000 >/dev/null 2>&1; then
        log "✅ Frontend: Saudável"
    else
        error "❌ Frontend: Não responsivo"
    fi
}

# Limpeza de recursos
cleanup() {
    log "Iniciando limpeza de recursos..."
    
    # Remover containers parados
    docker container prune -f
    
    # Remover imagens não utilizadas
    docker image prune -f
    
    # Remover volumes órfãos
    docker volume prune -f
    
    # Remover redes não utilizadas
    docker network prune -f
    
    log "Limpeza concluída!"
}

# Função principal
main() {
    case "${1:-help}" in
        install)
            install
            ;;
        update)
            update
            ;;
        restart)
            restart
            ;;
        stop)
            stop
            ;;
        backup)
            backup
            ;;
        logs)
            show_logs "$2"
            ;;
        status)
            status
            ;;
        health)
            check_health
            ;;
        cleanup)
            cleanup
            ;;
        help|--help|-h)
            show_help
            ;;
        *)
            error "Opção inválida: $1"
            show_help
            exit 1
            ;;
    esac
}

# Verificar se está sendo executado como root (para algumas operações)
if [ "$EUID" -eq 0 ] && [ "${1:-}" != "help" ]; then
    warning "Executando como root. Considere usar um usuário não-root para maior segurança."
fi

# Executar função principal
main "$@"

