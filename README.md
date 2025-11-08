# Konecta ERP System

A microservices-based ERP system built with Angular frontend and .NET backend services.

## System Overview

The Konecta ERP system consists of:

- **Authentication Service**: Handles user authentication and JWT token generation
- **User Management Service**: Manages users and roles with comprehensive CRUD operations
- **Frontend**: Angular-based UI with role-based access control
- **Databases**: Separate PostgreSQL databases for each service

## Quick Start

### Prerequisites

- Podman or Docker installed
- Bash shell

### Start the System

```bash
./startup.sh
```

This script will:
1. Create the Docker/Podman network
2. Build and start PostgreSQL databases (separate for each service)
3. Build and start Authentication Service
4. Build and start User Management Service
5. Build and start Frontend
6. Perform health checks

### Stop the System

```bash
# Stop containers only (preserve database data)
./stop.sh

# Stop containers and remove volumes (delete all data)
./stop.sh --volumes
```

## Services

| Service | Port | Description |
|---------|------|-------------|
| Frontend | 4200 | Angular UI |
| Authentication Service | 5001 | JWT generation, login, registration |
| User Management Service | 5002 | User and role management |
| PostgreSQL Auth | 5433 | Authentication database |
| PostgreSQL User | 5434 | User management database |

## Access Points

- **Frontend**: http://localhost:4200
- **Auth API**: http://localhost:5001
- **User Management API**: http://localhost:5002
- **Auth Swagger**: http://localhost:5001/swagger
- **User Mgmt Swagger**: http://localhost:5002/swagger
- **Health Checks**: http://localhost:5001/health, http://localhost:5002/health

## Default Credentials

- **Email**: admin@konecta.com
- **Password**: admin123
- **Role**: Admin

## Features

### Authentication
- User registration and login
- JWT-based authentication
- Password hashing (SHA256)
- Token refresh support

### User Management
- Full CRUD operations for users
- Role assignment (Admin, Manager, Employee)
- Admin-only access control
- User status management (Active/Inactive)

### Role-Based Access Control
- **Admin**: Full system access
- **Manager**: Department management
- **Employee**: Basic access

## Development

### Directory Structure

```
konectaerp/
├── backend/
│   ├── AuthenticationService/    # Auth service with JWT
│   └── UserManagementService/   # User management service
├── frontend/                    # Angular frontend
├── database/                     # Database setup
├── docs/                         # Documentation
├── startup.sh                    # Start script
└── stop.sh                       # Stop script
```

### Using Docker Compose (Alternative)

```bash
# Start all services
docker-compose up -d

# View logs
docker-compose logs -f

# Stop services
docker-compose down
```

## Management Commands

### View Containers
```bash
podman ps  # or docker ps
```

### View Logs
```bash
podman logs konecta-auth-service
podman logs konecta-user-management-service
podman logs konecta-frontend
```

### Access Database
```bash
# Auth database
podman exec -it konecta-postgres-auth psql -U postgres -d konecta_auth

# User database
podman exec -it konecta-postgres-user psql -U postgres -d konecta_user
```

## Documentation

- [Team Guide](docs/TEAM_GUIDE.md) - Development guidelines
- [Docker Setup](docs/DOCKER_SETUP.md) - Docker configuration details
- [User Management Setup](USER_MANAGEMENT_SERVICE_SETUP.md) - User management details

## Technology Stack

### Backend
- .NET 8.0
- Entity Framework Core
- PostgreSQL
- JWT Authentication

### Frontend
- Angular 20
- RxJS
- JWT Token Management

### Infrastructure
- Podman/Docker
- PostgreSQL 16
- Nginx (for frontend)

## Troubleshooting

### Services Not Starting
```bash
# Check logs
podman logs <service-name>

# Restart a service
podman restart <container-name>
```

### Database Connection Issues
```bash
# Check database status
podman logs konecta-postgres-auth
podman logs konecta-postgres-user

# Test connection
podman exec konecta-postgres-auth pg_isready -U postgres
```

### Permission Issues
```bash
# Make scripts executable
chmod +x startup.sh stop.sh
```

## License

This project is proprietary software for Konecta.
