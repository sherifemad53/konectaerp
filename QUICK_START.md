# Konecta ERP - Quick Start Guide

## Prerequisites

- Podman or Docker installed
- Bash shell (Linux/Mac/Git Bash)

## Start the System

```bash
./startup.sh
```

Wait for the services to start (about 1-2 minutes). You'll see:
- ✅ PostgreSQL databases starting
- ✅ Authentication Service starting
- ✅ User Management Service starting
- ✅ Frontend starting
- ✅ Health checks

## Access the Application

1. Open your browser: http://localhost:4200
2. Login with:
   - Email: `admin@konecta.com`
   - Password: `admin123`

## Available Services

| Service | URL | Description |
|---------|-----|-------------|
| Frontend | http://localhost:4200 | Main application |
| Auth API | http://localhost:5001 | Authentication |
| User Mgmt API | http://localhost:5002 | User management |
| Auth Swagger | http://localhost:5001/swagger | API documentation |
| User Mgmt Swagger | http://localhost:5002/swagger | API documentation |

## Stop the System

```bash
# Stop services (keep data)
./stop.sh

# Stop and delete all data
./stop.sh --volumes
```

## What Was Updated

### Backend
- ✅ AuthenticationService (Port 5001)
- ✅ UserManagementService (Port 5002)
- ✅ Separate PostgreSQL databases
- ✅ JWT integration
- ✅ Role-based authorization

### Frontend
- ✅ User management interface
- ✅ Role-based menu visibility
- ✅ CRUD operations for users
- ✅ Role assignment UI
- ✅ Admin-only access

### Startup Script
- ✅ Independent of docker-compose
- ✅ Supports Podman and Docker
- ✅ Automatic health checks
- ✅ Separate databases per service
- ✅ Color-coded output

## Quick Commands

```bash
# Start system
./startup.sh

# Stop system (keep data)
./stop.sh

# Stop and remove all data
./stop.sh --volumes

# View running containers
podman ps  # or docker ps

# View logs
podman logs konecta-auth-service
podman logs konecta-user-management-service

# Restart a service
podman restart konecta-auth-service
```

## Testing the Integration

### 1. Login as Admin
```bash
curl -X POST http://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@konecta.com",
    "password": "admin123"
  }'
```

### 2. Use the JWT token in User Management requests
```bash
curl -X GET http://localhost:5002/api/users \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

### 3. Or use the UI
- Navigate to http://localhost:4200
- Login as admin
- Click "User Management" in sidebar
- Create, edit, and manage users

## Troubleshooting

### Port Already in Use
```bash
# Find what's using the port
lsof -i :4200  # or any port

# Kill the process
kill -9 <PID>
```

### Containers Not Starting
```bash
# Check logs
podman logs <container-name>

# Remove and restart
podman rm <container-name>
./startup.sh
```

### Database Issues
```bash
# Check database status
podman logs konecta-postgres-auth
podman logs konecta-postgres-user

# Access database
podman exec -it konecta-postgres-auth psql -U postgres -d konecta_auth
```

## Next Steps

1. Explore the user management interface
2. Create new users with different roles
3. Test the role-based access control
4. Check out the API documentation in Swagger
5. Review the code structure

## Support

Check the documentation in the `docs/` directory for more details.

