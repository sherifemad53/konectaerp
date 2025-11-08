# User Management Service Setup Summary

## Overview
The UserManagementService has been successfully built and integrated with the AuthenticationService. Each service now has its own database container and uses JWT for secure communication.

## Changes Made

### 1. **Database Architecture**
- **Separate Databases**: Each service now has its own PostgreSQL database
  - AuthenticationService → `konecta_auth` (port 5433)
  - UserManagementService → `konecta_user` (port 5434)
- **Docker Compose**: Updated to include separate database containers
  - `postgres-auth`: For authentication data
  - `postgres-user`: For user management data

### 2. **UserManagementService Models**

#### Updated User Model
```csharp
- Changed from int Id to Guid Id
- Added FirstName, LastName fields
- Added IsActive, CreatedAt, LastLoginAt timestamps
- Added UserRoles navigation property (many-to-many relationship)
```

#### Updated Role Model
```csharp
- Changed from int Id to Guid Id
- Added Description field
- Added CreatedAt timestamp
- Added UserRoles navigation property
```

#### New UserRole Model
```csharp
- UserId (Guid)
- RoleId (Guid)
- User and Role navigation properties
```

### 3. **Default Roles Seeded**
The system automatically seeds three roles at startup:
- **Admin** (ID: 22222222-2222-2222-2222-222222222222)
- **Manager** (ID: 33333333-3333-3333-3333-333333333333)
- **Employee** (ID: 44444444-4444-4444-4444-444444444444)

### 4. **Services Updated**

#### UserService
- Added `GetByIdWithRolesAsync()` method
- Added `GetAllWithRolesAsync()` method
- Updated `CreateAsync()` to accept role assignments
- Updated `UpdateAsync()` to handle role changes
- Added `AssignRolesAsync()` for assigning additional roles
- Added `RemoveRoleAsync()` for removing roles

#### RoleService
- Updated all methods to use Guid instead of int
- Added description handling

### 5. **Controllers with Authorization**

#### UsersController
- `Get(Guid id)` - Get user by ID (Authenticated users)
- `GetAll()` - Get all users (Admin only)
- `Create()` - Create new user (Admin only)
- `Update()` - Update user and roles (Admin only)
- `Delete()` - Delete user (Admin only)
- `AssignRoles()` - Assign roles to user (Admin only)
- `RemoveRole()` - Remove role from user (Admin only)

#### RolesController
- `Get(Guid id)` - Get role by ID (Authenticated users)
- `GetAll()` - Get all roles (Authenticated users)
- `Create()` - Create new role (Admin only)
- `Update()` - Update role (Admin only)
- `Delete()` - Delete role (Admin only)

### 6. **JWT Integration**
- Both services use the same JWT secret key
- UserManagementService validates tokens from AuthenticationService
- Authorization policies:
  - `AdminOnly`: Requires Admin role
  - `HRManager`: Requires Admin or HR_Manager role

### 7. **Docker Compose Updates**
- Added `user-management-service` container
- Added `postgres-user` database container
- Added `postgres-auth` database container
- Configured service dependencies
- Added health checks for all services

### 8. **DTOs Updated**
All DTOs now use Guid instead of int and include additional fields:
- `UserDto`: Includes roles list
- `CreateUserDto`: Includes role assignment
- `UpdateUserDto`: Includes role changes
- `RoleDto`: Includes description
- `CreateRoleDto`: Includes description
- `UpdateRoleDto`: Includes description

## Running the System

### Start All Services
```bash
docker-compose up -d
```

### Check Service Status
```bash
docker-compose ps
```

### View Logs
```bash
# Authentication Service
docker logs konecta-auth-service

# User Management Service
docker logs konecta-user-management-service

# Databases
docker logs konecta-postgres-auth
docker logs konecta-postgres-user
```

### Access Services
- **Authentication Service**: http://localhost:5001
- **User Management Service**: http://localhost:5002
- **Frontend**: http://localhost:4200
- **Swagger (Auth)**: http://localhost:5001/swagger
- **Swagger (User Mgmt)**: http://localhost:5002/swagger

## API Usage Examples

### 1. Login to Get JWT Token
```bash
curl -X POST http://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@konecta.com",
    "password": "admin123"
  }'
```

Response:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "user": {
    "id": "11111111-1111-1111-1111-111111111111",
    "email": "admin@konecta.com",
    "firstName": "admin",
    "lastName": "konecta",
    "roles": ["Admin"]
  }
}
```

### 2. Get All Users (Admin Only)
```bash
curl -X GET http://localhost:5002/api/users \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

### 3. Create New User (Admin Only)
```bash
curl -X POST http://localhost:5002/api/users \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -d '{
    "username": "john.doe",
    "email": "john.doe@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "roleIds": ["44444444-4444-4444-4444-444444444444"]
  }'
```

### 4. Assign Additional Roles to User (Admin Only)
```bash
curl -X POST http://localhost:5002/api/users/{userId}/roles \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -d '{
    "roleIds": ["33333333-3333-3333-3333-333333333333"]
  }'
```

### 5. Get All Roles
```bash
curl -X GET http://localhost:5002/api/roles \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

## Database Connection Details

### Authentication Service Database
- **Host**: postgres-auth (Docker) / localhost (local)
- **Port**: 5432 (Docker) / 5433 (local)
- **Database**: konecta_auth
- **User**: postgres
- **Password**: postgres123

### User Management Service Database
- **Host**: postgres-user (Docker) / localhost (local)
- **Port**: 5432 (Docker) / 5434 (local)
- **Database**: konecta_user
- **User**: postgres
- **Password**: postgres123

## Testing the System

1. **Start the services**:
   ```bash
   docker-compose up -d
   ```

2. **Wait for services to be healthy**:
   ```bash
   docker-compose ps
   # All services should show "healthy"
   ```

3. **Login as admin**:
   ```bash
   curl -X POST http://localhost:5001/api/auth/login \
     -H "Content-Type: application/json" \
     -d '{"email": "admin@konecta.com", "password": "admin123"}'
   ```

4. **Copy the JWT token** from the response

5. **Test UserManagementService endpoints** using the JWT token

## Key Features

### Role-Based Access Control
- **Admin**: Full access to all users and roles
- **Manager**: Can view and manage employees
- **Employee**: Limited access to their own data

### CRUD Operations
- ✅ Create users with role assignments
- ✅ Read user data with roles
- ✅ Update user information and roles
- ✅ Delete users (Admin only)
- ✅ Assign additional roles to users
- ✅ Remove roles from users
- ✅ View all users (Admin only)

### Database Structure
- ✅ Separate databases for each service
- ✅ Proper relationships with UserRoles junction table
- ✅ Automatic migrations on startup
- ✅ Seed data for default roles

### Security
- ✅ JWT authentication integration
- ✅ Role-based authorization policies
- ✅ Shared secret key for token validation
- ✅ Secure password storage (SHA256 in AuthService)

## Next Steps

1. Test the API endpoints using Swagger UI
2. Integrate with the frontend application
3. Add additional business logic as needed
4. Implement audit logging for user changes
5. Add pagination for user lists
6. Implement user search functionality

