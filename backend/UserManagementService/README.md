# User Management Service

## Overview
The User Management Service is part of the Konecta ERP system, responsible for managing users and their roles within the organization. This service integrates with the Authentication Service using JWT for secure communication.

## Features

### User Management
- **Create Users**: Create new users with role assignments
- **Get User**: Retrieve a single user by ID with their roles
- **Get All Users**: Retrieve all users (Admin only)
- **Update Users**: Update user information and roles
- **Delete Users**: Remove users from the system (Admin only)
- **Assign Roles**: Assign additional roles to users (Admin only)
- **Remove Roles**: Remove roles from users (Admin only)

### Role Management
- **Get Roles**: Retrieve all available roles
- **Get Role**: Retrieve a single role by ID
- **Create Roles**: Create new roles (Admin only)
- **Update Roles**: Update role information (Admin only)
- **Delete Roles**: Remove roles (Admin only)

## Default Roles

The system comes with three default roles seeded at startup:

1. **Admin** - System Administrator with full access
2. **Manager** - Department Manager with elevated permissions
3. **Employee** - Regular Employee with basic access

## Database

The service uses its own PostgreSQL database (`konecta_user`) with the following schema:

- **Users**: User information table
- **Roles**: Role definitions table
- **UserRoles**: Many-to-many relationship table for user-role assignments

## API Endpoints

### Users Endpoints

| Method | Endpoint | Description | Authorization |
|--------|----------|-------------|---------------|
| GET | `/api/users/{id}` | Get user by ID | Any authenticated user |
| GET | `/api/users` | Get all users | Admin only |
| POST | `/api/users` | Create new user | Admin only |
| PUT | `/api/users/{id}` | Update user | Admin only |
| DELETE | `/api/users/{id}` | Delete user | Admin only |
| POST | `/api/users/{id}/roles` | Assign roles to user | Admin only |
| DELETE | `/api/users/{id}/roles/{roleId}` | Remove role from user | Admin only |

### Roles Endpoints

| Method | Endpoint | Description | Authorization |
|--------|----------|-------------|---------------|
| GET | `/api/roles/{id}` | Get role by ID | Any authenticated user |
| GET | `/api/roles` | Get all roles | Any authenticated user |
| POST | `/api/roles` | Create new role | Admin only |
| PUT | `/api/roles/{id}` | Update role | Admin only |
| DELETE | `/api/roles/{id}` | Delete role | Admin only |

## JWT Integration

The service validates JWT tokens issued by the Authentication Service:
- **Secret Key**: Shared secret for token validation
- **Issuer**: KonectaERP
- **Audience**: KonectaERPUsers
- **Expiration**: 60 minutes (configurable)

### Authorization Policies

- **AdminOnly**: Requires user to have "Admin" role
- **HRManager**: Requires user to have "Admin" or "HR_Manager" role

## Running the Service

### Using Docker Compose

The service is included in the main `docker-compose.yml` file:

```bash
docker-compose up user-management-service
```

### Local Development

1. Update `appsettings.json` with your database connection string
2. Run migrations: The service automatically applies migrations on startup
3. Start the service: `dotnet run`

## Database Connection

The service connects to a separate PostgreSQL database:
- **Host**: `postgres-user` (Docker) or `localhost` (local)
- **Database**: `konecta_user`
- **Port**: 5432 (Docker) or 5434 (local with docker-compose)

## Service Configuration

- **Port**: 5002
- **Environment**: Development
- **Swagger**: Available at `/swagger` (Development mode)

## Health Check

The service exposes a health check endpoint at `/health` for container monitoring.

## Example Usage

### Create a User

```bash
curl -X POST http://localhost:5002/api/users \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer <JWT_TOKEN>" \
  -d '{
    "username": "john.doe",
    "email": "john.doe@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "roleIds": ["22222222-2222-2222-2222-222222222222"]
  }'
```

### Assign Roles to a User

```bash
curl -X POST http://localhost:5002/api/users/{userId}/roles \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer <JWT_TOKEN>" \
  -d '{
    "roleIds": ["33333333-3333-3333-3333-333333333333"]
  }'
```

## Development Notes

- The service uses Entity Framework Core for database access
- Migrations are automatically applied on startup
- Default roles are seeded during database initialization
- JWT validation requires the same secret key as the Authentication Service

