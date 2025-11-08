#!/bin/bash

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$SCRIPT_DIR"

echo "=== Konecta ERP Shutdown ==="

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m'

log() {
    echo -e "${GREEN}[$(date +'%T')] $1${NC}"
}

warn() {
    echo -e "${YELLOW}[$(date +'%T')] WARNING: $1${NC}"
}

error() {
    echo -e "${RED}[$(date +'%T')] ERROR: $1${NC}"
}

# Check if podman or docker is available
if command -v podman &> /dev/null; then
    CONTAINER_CMD="podman"
elif command -v docker &> /dev/null; then
    CONTAINER_CMD="docker"
else
    error "Neither podman nor docker is installed"
    exit 1
fi

# Stop containers
log "Stopping containers..."
$CONTAINER_CMD stop konecta-postgres-auth konecta-postgres-user konecta-auth-service konecta-user-management-service konecta-frontend 2>/dev/null || warn "Some containers not running"

# Remove containers
log "Removing containers..."
$CONTAINER_CMD rm konecta-postgres-auth konecta-postgres-user konecta-auth-service konecta-user-management-service konecta-frontend 2>/dev/null || warn "Some containers not found"

# Option to remove volumes
if [ "$1" == "--volumes" ] || [ "$1" == "-v" ]; then
    log "Removing volumes..."
    $CONTAINER_CMD volume rm konecta_postgres_auth_data konecta_postgres_user_data 2>/dev/null || warn "Some volumes not found"
fi

# Remove network
log "Removing network..."
$CONTAINER_CMD network rm konecta-network 2>/dev/null || warn "Network not found"

log "=== Shutdown Complete ==="
echo ""
echo "All services have been stopped and removed."
if [ "$1" != "--volumes" ] && [ "$1" != "-v" ]; then
    echo "Database data has been preserved."
    echo "To remove volumes as well, run: $0 --volumes"
fi
echo ""

