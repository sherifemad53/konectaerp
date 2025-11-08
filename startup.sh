#!/bin/bash

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$SCRIPT_DIR"

echo "=== Konecta ERP Deployment ==="

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

log() {
    echo -e "${GREEN}[$(date +'%T')] $1${NC}"
}

warn() {
    echo -e "${YELLOW}[$(date +'%T')] WARNING: $1${NC}"
}

error() {
    echo -e "${RED}[$(date +'%T')] ERROR: $1${NC}"
    exit 1
}

info() {
    echo -e "${BLUE}[$(date +'%T')] $1${NC}"
}

# Cleanup function
cleanup() {
    log "Cleaning up..."
    podman stop konecta-postgres-auth konecta-postgres-user konecta-auth-service konecta-user-management-service konecta-frontend 2>/dev/null || true
    podman rm konecta-postgres-auth konecta-postgres-user konecta-auth-service konecta-user-management-service konecta-frontend 2>/dev/null || true
}

# Trap Ctrl+C
trap cleanup SIGINT

# Function to wait for container to be healthy
wait_for_container() {
    local container_name=$1
    local max_attempts=30
    local attempt=0
    
    log "Waiting for $container_name to be ready..."
    while [ $attempt -lt $max_attempts ]; do
        if podman exec $container_name pg_isready -U postgres > /dev/null 2>&1; then
            log "$container_name is ready"
            return 0
        fi
        attempt=$((attempt + 1))
        sleep 2
    done
    
    error "$container_name failed to start within timeout"
}

# Check if podman or docker is available
if command -v podman &> /dev/null; then
    CONTAINER_CMD="podman"
    log "Using podman"
elif command -v docker &> /dev/null; then
    CONTAINER_CMD="docker"
    log "Using docker"
else
    error "Neither podman nor docker is installed"
fi

# Create network
log "Creating network..."
$CONTAINER_CMD network create konecta-network 2>/dev/null || log "Network already exists"

# Build PostgreSQL image with custom config
log "Building PostgreSQL image with custom configuration..."
$CONTAINER_CMD build -t konecta-postgres-custom -f database/Dockerfile database/ || error "Failed to build PostgreSQL image"

# Start PostgreSQL Auth Database
log "Starting PostgreSQL Auth Database..."
$CONTAINER_CMD run -d \
  --name konecta-postgres-auth \
  --network konecta-network \
  -e POSTGRES_DB=konecta_auth \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres123 \
  -e POSTGRES_INITDB_ARGS="--encoding=UTF-8" \
  -p 5433:5432 \
  -v konecta_postgres_auth_data:/var/lib/postgresql/data \
  konecta-postgres-custom || error "Failed to start PostgreSQL Auth database"

wait_for_container konecta-postgres-auth

# Start PostgreSQL User Database
log "Starting PostgreSQL User Database..."
$CONTAINER_CMD run -d \
  --name konecta-postgres-user \
  --network konecta-network \
  -e POSTGRES_DB=konecta_user \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres123 \
  -e POSTGRES_INITDB_ARGS="--encoding=UTF-8" \
  -p 5434:5432 \
  -v konecta_postgres_user_data:/var/lib/postgresql/data \
  konecta-postgres-custom || error "Failed to start PostgreSQL User database"

wait_for_container konecta-postgres-user

# Build and start Authentication Service
log "Building Authentication Service..."
if [ -d "backend/AuthenticationService" ]; then
    cd backend/AuthenticationService
    $CONTAINER_CMD build -t authentication-service . || error "Failed to build authentication service"
    
    log "Starting Authentication Service..."
    $CONTAINER_CMD run -d \
      --name konecta-auth-service \
      --network konecta-network \
      -e ASPNETCORE_ENVIRONMENT=Development \
      -e ASPNETCORE_URLS=http://+:5001 \
      -e "ConnectionStrings__DefaultConnection=Host=konecta-postgres-auth;Port=5432;Database=konecta_auth;Username=postgres;Password=postgres123" \
      -e JwtSettings__SecretKey=YourSuperSecretKeyForJWTTokenGenerationMinimum32Characters \
      -e JwtSettings__Issuer=KonectaERP \
      -e JwtSettings__Audience=KonectaERPUsers \
      -e JwtSettings__ExpirationMinutes=60 \
      -e Consul__Enabled=false \
      -e RUN_MIGRATIONS=true \
      -p 5001:5001 \
      authentication-service || error "Failed to start authentication service"
    
    cd ../..
else
    warn "AuthenticationService directory not found. Skipping."
fi

# Build and start User Management Service
log "Building User Management Service..."
if [ -d "backend/UserManagementService" ]; then
    cd backend/UserManagementService
    $CONTAINER_CMD build -t user-management-service . || error "Failed to build user management service"
    
    log "Starting User Management Service..."
    $CONTAINER_CMD run -d \
      --name konecta-user-management-service \
      --network konecta-network \
      -e ASPNETCORE_ENVIRONMENT=Development \
      -e ASPNETCORE_URLS=http://+:5002 \
      -e "ConnectionStrings__DefaultConnection=Host=konecta-postgres-user;Port=5432;Database=konecta_user;Username=postgres;Password=postgres123" \
      -e JwtSettings__SecretKey=YourSuperSecretKeyForJWTTokenGenerationMinimum32Characters \
      -e JwtSettings__Issuer=KonectaERP \
      -e JwtSettings__Audience=KonectaERPUsers \
      -e Consul__Enabled=false \
      -e ServiceConfig__Host=user-management-service \
      -e ServiceConfig__Port=5002 \
      -p 5002:5002 \
      user-management-service || error "Failed to start user management service"
    
    cd ../..
else
    warn "UserManagementService directory not found. Skipping."
fi

# Build and start Frontend
log "Building Frontend..."
if [ -d "frontend" ] && [ -f "frontend/Dockerfile" ]; then
    cd frontend
    $CONTAINER_CMD build -t frontend .|| warn "Failed to build frontend"
    
    log "Starting Frontend..."
    $CONTAINER_CMD run -d \
      --name konecta-frontend \
      --network konecta-network \
      -e NODE_ENV=production \
      -p 4200:80 \
      frontend || warn "Failed to start frontend"
    
    cd ..
else
    warn "Frontend directory or Dockerfile not found. Skipping frontend."
fi

# Health checks
log "Performing health checks..."
sleep 15

log "Checking services..."
info "PostgreSQL Auth: $($CONTAINER_CMD inspect --format='{{.State.Status}}' konecta-postgres-auth)"
info "PostgreSQL User: $($CONTAINER_CMD inspect --format='{{.State.Status}}' konecta-postgres-user)"

if $CONTAINER_CMD ps --format "{{.Names}}" | grep -q konecta-auth-service; then
    info "Auth Service: $($CONTAINER_CMD inspect --format='{{.State.Status}}' konecta-auth-service)"
    info "Testing Auth Service health..."
    curl -f http://localhost:5001/health > /dev/null 2>&1 && log "Auth Service is healthy" || warn "Auth Service health check failed"
fi

if $CONTAINER_CMD ps --format "{{.Names}}" | grep -q konecta-user-management-service; then
    info "User Management Service: $($CONTAINER_CMD inspect --format='{{.State.Status}}' konecta-user-management-service)"
    info "Testing User Management Service health..."
    curl -f http://localhost:5002/health > /dev/null 2>&1 && log "User Management Service is healthy" || warn "User Management Service health check failed"
fi

if $CONTAINER_CMD ps --format "{{.Names}}" | grep -q konecta-frontend; then
    info "Frontend: $($CONTAINER_CMD inspect --format='{{.State.Status}}' konecta-frontend)"
fi

echo ""
log "=== Deployment Complete ==="
echo ""
echo "Services:"
echo "  PostgreSQL Auth DB:     localhost:5433"
echo "  PostgreSQL User DB:     localhost:5434"
echo "  Authentication API:     localhost:5001"
echo "  User Management API:    localhost:5002"
echo "  Frontend:               localhost:4200"
echo ""
echo "Management Commands:"
echo "  View all containers:    $CONTAINER_CMD ps"
echo "  View logs:              $CONTAINER_CMD logs <container-name>"
echo "  Stop services:         $CONTAINER_CMD stop konecta-postgres-auth konecta-postgres-user konecta-auth-service konecta-user-management-service konecta-frontend"
echo "  Remove services:       $CONTAINER_CMD rm konecta-postgres-auth konecta-postgres-user konecta-auth-service konecta-user-management-service konecta-frontend"
echo ""
echo "Quick Access:"
echo "  Frontend:              http://localhost:4200"
echo "  Auth Swagger:          http://localhost:5001/swagger"
echo "  User Mgmt Swagger:     http://localhost:5002/swagger"
echo "  Auth Health:           http://localhost:5001/health"
echo "  User Mgmt Health:      http://localhost:5002/health"
echo ""
