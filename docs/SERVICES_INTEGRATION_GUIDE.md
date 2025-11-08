# Services Integration Guide

## Architecture Overview

The Konecta ERP system uses a microservices architecture with separate services for authentication and user management, each with its own database.

```
┌─────────────────┐         ┌──────────────────────┐
│   Frontend      │◄────────┤  Docker Network      │
│   (Angular)     │         │  (konecta-network)   │
└─────────────────┘         └──────────────────────┘
         │                             │
         │                             │
         ├─────────────────┐           ├──────────────────────┐
         │                 │           │                      │
         ▼                 ▼           ▼                      ▼
┌──────────────────┐  ┌──────────────────────┐   ┌──────────────────┐
│ AuthService      │  │ UserMgmtService      │   │   Databases       │
│ Port: 5001       │  │ Port: 5002           │   │                   │
├──────────────────┤  ├──────────────────────┤   │  ┌──────────────┐ │
│ - Login          │  │ - User CRUD          │   │  │ postgres-auth │ │
│ - Register       │  │ - Role Management    │   │  │ Port: 5433    │ │
│ - JWT Token Gen  │  │ - Role Assignment    │   │  └──────────────┘ │
│ - User Info      │  │ - Admin Controls     │   │                   │
└──────────────────┘  └──────────────────────┘   │  ┌──────────────┐ │
                                                    │  │ postgres-user │ │
                                                    │  │ Port: 5434    │ │
                                                    │  └──────────────┘ │
                                                    └───────────────────┘
```

## Service Communication

### JWT-Based Authentication Flow

```
1. User logs in via AuthService
   POST /api/auth/login
   → Returns JWT token + user info

2. Frontend stores JWT token

3. User Management requests include JWT in header
   Authorization: Bearer <JWT_TOKEN>

4. UserManagementService validates JWT
   - Checks signature with shared secret
   - Extracts user claims (roles, user ID)
   - Enforces role-based authorization
```

## Link Between Services

### 1. JWT Configuration
Both services share the same JWT configuration:

```json
{
  "JwtSettings": {
    "SecretKey": "YourSuperSecretKeyForJWTTokenGenerationMinimum32Characters",
    "Issuer": "KonectaERP",
    "Audience": "KonectaERPUsers",
    "ExpirationMinutes": 60
  }
}
```

### 2. Token Structure
Tokens issued by AuthService contain:
- User ID (ClaimTypes.NameIdentifier)
- Email (ClaimTypes.Name)
- Roles (ClaimTypes.Role)
- Issuer, Audience, Expiration

### 3. Authorization Policies
UserManagementService uses the roles from the JWT token:

```csharp
// Admin-only endpoints
[Authorize(Policy = "AdminOnly")]

// Definitions in Program.cs
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("HRManager", policy => policy.RequireRole("Admin", "HR_Manager"));
});
```

## Database Links

### Separate Databases, Linked by User IDs

**AuthService Database (konecta_auth)**:
- Stores authentication credentials
- Stores user identities (Email, PasswordHash, etc.)
- Manages JWT generation
- **User ID**: UUID (used as linking key)

**UserManagementService Database (konecta_user)**:
- Stores extended user profiles
- Manages user details (FirstName, LastName, etc.)
- Manages role assignments
- **User ID**: Should match AuthService UUID (conceptually linked)

### Current Design
For now, each service maintains its own user table independently. To properly link them:

**Option 1: UserManagementService queries AuthService**
- Store minimal user data in UserManagementService
- Query AuthService for authentication details when needed

**Option 2: Shared User ID Reference**
- Both services use the same UUID for users
- External ID service generates unique IDs

**Option 3: Sync users from AuthService**
- AuthService is the source of truth
- UserManagementService periodically syncs user data

## API Endpoints Summary

### Authentication Service (Port 5001)

| Endpoint | Method | Description | Auth Required |
|----------|--------|-------------|--------------|
| `/api/auth/register` | POST | Register new user | No |
| `/api/auth/login` | POST | Login and get JWT | No |
| `/api/auth/me` | GET | Get current user info | Yes |
| `/api/auth/change-password` | POST | Change password | Yes |
| `/api/auth/logout` | POST | Logout | Yes |

### User Management Service (Port 5002)

| Endpoint | Method | Description | Auth Required | Role Required |
|----------|--------|-------------|---------------|---------------|
| `/api/users` | GET | Get all users | Yes | Admin |
| `/api/users/{id}` | GET | Get user by ID | Yes | - |
| `/api/users` | POST | Create user | Yes | Admin |
| `/api/users/{id}` | PUT | Update user | Yes | Admin |
| `/api/users/{id}` | DELETE | Delete user | Yes | Admin |
| `/api/users/{id}/roles` | POST | Assign roles | Yes | Admin |
| `/api/users/{id}/roles/{roleId}` | DELETE | Remove role | Yes | Admin |
| `/api/roles` | GET | Get all roles | Yes | - |
| `/api/roles/{id}` | GET | Get role by ID | Yes | - |
| `/api/roles` | POST | Create role | Yes | Admin |
| `/api/roles/{id}` | PUT | Update role | Yes | Admin |
| `/api/roles/{id}` | DELETE | Delete role | Yes | Admin |

## Docker Compose Service Links

### Database Services
- `postgres-auth`: Database for authentication
- `postgres-user`: Database for user management

### Application Services
- `authentication-service`: Handles authentication and JWT generation
- `user-management-service`: Handles user and role management

### Dependencies
```yaml
user-management-service:
  depends_on:
    - postgres-user
    - authentication-service
```

## Role Hierarchy

```
Admin
  └─ Full CRUD on all users and roles
  └─ Can assign/remove any role
  └─ Can delete users

Manager  
  └─ Can view users
  └─ Can manage employees (future)
  └─ Limited permissions

Employee
  └─ Can view own profile
  └─ Limited access (future)
```

## Default Credentials

### Admin User (in AuthService)
- **Email**: admin@konecta.com
- **Password**: admin123
- **Role**: Admin
- **User ID**: 11111111-1111-1111-1111-111111111111

## Testing the Integration

### Step 1: Login via AuthService
```bash
curl -X POST http://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@konecta.com",
    "password": "admin123"
  }'
```

Returns:
```json
{
  "token": "eyJhbGc...",
  "user": {
    "id": "11111111-1111-1111-1111-111111111111",
    "roles": ["Admin"]
  }
}
```

### Step 2: Use JWT token with UserManagementService
```bash
curl -X GET http://localhost:5002/api/users \
  -H "Authorization: Bearer eyJhbGc..."
```

The JWT token from Step 1 is validated by UserManagementService using the shared secret, and the Admin role is checked to allow access.

## Environment Variables

### AuthService
```
ASPNETCORE_URLS=http://+:5001
ConnectionStrings__DefaultConnection=Host=postgres-auth;Port=5432;Database=konecta_auth;...
JwtSettings__SecretKey=YourSuperSecretKeyForJWTTokenGenerationMinimum32Characters
JwtSettings__Issuer=KonectaERP
JwtSettings__Audience=KonectaERPUsers
```

### UserManagementService
```
ASPNETCORE_URLS=http://+:5002
ConnectionStrings__DefaultConnection=Host=postgres-user;Port=5432;Database=konecta_user;...
JwtSettings__SecretKey=YourSuperSecretKeyForJWTTokenGenerationMinimum32Characters
JwtSettings__Issuer=KonectaERP
JwtSettings__Audience=KonectaERPUsers
```

## Network Communication

All services communicate over the Docker network:
- **Network Name**: konecta-network
- **Driver**: bridge
- **Internal DNS**: Services can reference each other by name
  - `http://postgres-auth:5432`
  - `http://postgres-user:5432`
  - `http://authentication-service:5001`
  - `http://user-management-service:5002`

## Security Considerations

1. **JWT Secret**: Must be identical in both services
2. **HTTPS**: Should be enabled in production
3. **Token Expiration**: 60 minutes (configurable)
4. **Role Validation**: Enforced at the API level
5. **Database Isolation**: Each service has its own database

## Monitoring

### Health Checks
- AuthService: http://localhost:5001/health
- UserManagementService: http://localhost:5002/health

### Consul Integration
Both services support Consul for service discovery (disabled by default):
- AuthService registers as: `authentication-service`
- UserManagementService registers as: `user-management-service`

## Deployment

### Running with Docker Compose
```bash
# Start all services
docker-compose up -d

# View logs
docker-compose logs -f

# Stop all services
docker-compose down

# Stop and remove volumes (WARNING: Deletes data)
docker-compose down -v
```

## Summary

The two services are linked through:
1. **JWT tokens** (shared secret and validation)
2. **Role-based authorization** (roles stored in JWT)
3. **Docker network** (service-to-service communication)
4. **API contracts** (well-defined REST endpoints)

Each service maintains its own database, providing data isolation while sharing authentication context through JWT tokens.

