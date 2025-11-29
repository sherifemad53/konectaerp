# SharedContracts Library

A shared library providing common contracts, utilities, and extensions for the Konecta ERP microservices architecture. This library ensures consistency across services for authentication, authorization, service discovery, and inter-service communication.

## Overview

SharedContracts is a multi-targeted .NET library (net8.0 and net9.0) that provides:

- **Authorization** - Permission-based and role-based access control
- **Security** - JWT authentication with remote JWKS validation
- **Service Discovery** - Consul integration for service registration
- **Events** - Shared event contracts for inter-service messaging
- **Configuration** - Common configuration models

## Installation

Add a project reference to SharedContracts in your service's `.csproj`:

```xml
<ItemGroup>
  <ProjectReference Include="../SharedContracts/SharedContracts.csproj" />
</ItemGroup>
```

## Components

### 1. Authorization

#### Permission-Based Access Control

The library provides a comprehensive permission system with predefined permissions for all domains.

**Available Permissions:**

```csharp
using SharedContracts.Authorization;

// Finance permissions
PermissionConstants.Finance.InvoicesRead
PermissionConstants.Finance.InvoicesManage
PermissionConstants.Finance.BudgetsRead
PermissionConstants.Finance.PayrollManage

// HR permissions
PermissionConstants.Hr.EmployeesRead
PermissionConstants.Hr.EmployeesManage
PermissionConstants.Hr.DepartmentsManage

// Inventory permissions
PermissionConstants.Inventory.ItemsRead
PermissionConstants.Inventory.StockManage

// User Management permissions
PermissionConstants.UserManagement.UsersManage
PermissionConstants.UserManagement.RolesManage

// Reporting permissions
PermissionConstants.Reporting.FinanceView
PermissionConstants.Reporting.ExportPdf
```

**Usage in Controllers:**

```csharp
using Microsoft.AspNetCore.Authorization;
using SharedContracts.Authorization;

[Authorize(Policy = PermissionConstants.Finance.InvoicesManage)]
public async Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceDto dto)
{
    // Only users with "finance.invoices.manage" permission can access
}
```

#### Role-Based Access Control

Predefined roles for the organization:

```csharp
using SharedContracts.Authorization;

RoleConstants.SystemAdmin        // Full system access
RoleConstants.HrAdmin            // HR administration
RoleConstants.HrStaff            // HR operations
RoleConstants.FinanceManager     // Finance management
RoleConstants.FinanceStaff       // Finance operations
RoleConstants.DepartmentManager  // Department-level access
RoleConstants.Employee           // Basic employee access
```

**Usage:**

```csharp
[Authorize(Roles = RoleConstants.SystemAdmin)]
public IActionResult AdminOnlyEndpoint()
{
    // Only System Admins can access
}
```

#### Setup in Program.cs

Add permission policies to your service:

```csharp
using SharedContracts.Authorization;

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

// Register all permission-based policies
builder.Services.AddPermissionPolicies();
```

This automatically creates authorization policies for all permissions defined in `PermissionConstants.All`.

---

### 2. Security (JWT Authentication)

Provides standardized JWT authentication with support for remote JWKS validation.

#### Setup

**In Program.cs:**

```csharp
using SharedContracts.Security;

// Add JWT authentication
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

// In middleware pipeline
app.UseAuthentication();
app.UseAuthorization();
```

**Configuration (appsettings.json):**

```json
{
  "JwtValidation": {
    "Issuer": "http://localhost:7280",
    "Audience": "konecta-erp-services",
    "JwksUri": "http://localhost:7280/.well-known/jwks.json",
    "RequireHttpsMetadata": false
  }
}
```

**Configuration Options:**

| Property               | Description                                   | Required           |
| ---------------------- | --------------------------------------------- | ------------------ |
| `Issuer`               | JWT token issuer (AuthenticationService URL)  | Yes                |
| `Audience`             | Expected audience claim                       | Yes                |
| `JwksUri`              | URL to fetch public keys for token validation | Yes\*              |
| `SecretKey`            | Shared secret for symmetric key validation    | Yes\*              |
| `RequireHttpsMetadata` | Require HTTPS for metadata endpoints          | No (default: true) |
| `AdditionalIssuers`    | Additional valid issuers                      | No                 |
| `AdditionalAudiences`  | Additional valid audiences                    | No                 |

\*Either `JwksUri` (recommended) or `SecretKey` must be provided.

#### How It Works

1. Services validate JWT tokens issued by AuthenticationService
2. Public keys are fetched from the JWKS endpoint
3. Tokens are validated for signature, issuer, audience, and expiration
4. User claims (including permissions) are extracted from the token

---

### 3. Service Discovery (Consul)

Automatic service registration and discovery using Consul.

#### Setup

**In Program.cs:**

```csharp
using SharedContracts.ServiceDiscovery;

// Add Consul service discovery
builder.Services.AddConsulServiceDiscovery(builder.Configuration);
```

**Configuration (appsettings.json):**

```json
{
  "ServiceConfig": {
    "ServiceName": "finance-service",
    "Host": "localhost",
    "Port": 7281,
    "Scheme": "http",
    "HealthCheckPath": "/system/health"
  },
  "Consul": {
    "Host": "http://localhost:8500",
    "Token": ""
  }
}
```

**Configuration Options:**

| Property                        | Description               | Required              |
| ------------------------------- | ------------------------- | --------------------- |
| `ServiceConfig:ServiceName`     | Unique service identifier | Yes                   |
| `ServiceConfig:Host`            | Service host address      | Yes                   |
| `ServiceConfig:Port`            | Service port number       | Yes                   |
| `ServiceConfig:Scheme`          | http or https             | No (default: http)    |
| `ServiceConfig:HealthCheckPath` | Health check endpoint     | No (default: /health) |
| `Consul:Host`                   | Consul agent address      | Yes                   |
| `Consul:Token`                  | Consul ACL token          | No                    |

#### Features

- Automatic service registration on startup
- Automatic deregistration on shutdown
- Health check configuration
- Service metadata management

---

### 4. Events

Shared event contracts for inter-service communication via message bus (RabbitMQ).

#### Available Events

**User Management Events:**

```csharp
using SharedContracts.Events;

// User provisioned in the system
UserProvisionedEvent(
    string UserId,
    Guid EmployeeId,
    string WorkEmail,
    string FullName,
    IReadOnlyCollection<string> Roles,
    DateTime ProvisionedAt
);

// User deactivated
UserDeactivatedEvent(string UserId, DateTime DeactivatedAt);

// User resigned
UserResignedEvent(string UserId, DateTime ResignedAt);

// User terminated
UserTerminatedEvent(string UserId, DateTime TerminatedAt);
```

**HR Events:**

```csharp
// Employee created
EmployeeCreatedEvent(
    Guid EmployeeId,
    string FullName,
    string WorkEmail,
    string PersonalEmail,
    string Position,
    Guid DepartmentId,
    string DepartmentName,
    DateTime HireDate
);

// Employee exited
EmployeeExitedEvent(Guid EmployeeId, DateTime ExitDate);

// Employee resignation approved
EmployeeResignationApprovedEvent(Guid EmployeeId, DateTime ApprovedAt);

// Employee terminated
EmployeeTerminatedEvent(Guid EmployeeId, DateTime TerminatedAt);
```

**Finance Events:**

```csharp
// Employee compensation events
EmployeeCompensationCreatedEvent(Guid CompensationId, Guid EmployeeId, ...);
EmployeeCompensationUpdatedEvent(Guid CompensationId, Guid EmployeeId, ...);
EmployeeCompensationDeletedEvent(Guid CompensationId, Guid EmployeeId);
```

#### Usage Example

**Publishing Events:**

```csharp
using SharedContracts.Events;

// In your service
var employeeCreatedEvent = new EmployeeCreatedEvent(
    EmployeeId: employee.Id,
    FullName: employee.FullName,
    WorkEmail: employee.WorkEmail,
    PersonalEmail: employee.PersonalEmail,
    Position: employee.Position,
    DepartmentId: employee.DepartmentId,
    DepartmentName: employee.Department.Name,
    HireDate: employee.HireDate
);

await _eventPublisher.PublishAsync("employee.created", employeeCreatedEvent);
```

**Consuming Events:**

```csharp
using SharedContracts.Events;

public class EmployeeEventsConsumer : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _consumer.ConsumeAsync<EmployeeCreatedEvent>(
            "employee.created",
            async (event) => {
                // Handle employee created event
                await ProcessEmployeeCreated(event);
            },
            stoppingToken
        );
    }
}
```

---

### 5. Configuration

Common configuration models used across services.

#### JwtSettings

Configuration for JWT token generation (used by AuthenticationService):

```csharp
using SharedContracts.Configuration;

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection(JwtSettings.SectionName)
);
```

**appsettings.json:**

```json
{
  "JwtSettings": {
    "SecretKey": "your-secret-key-at-least-32-characters",
    "Issuer": "http://localhost:7280",
    "Audience": "konecta-erp-services",
    "ExpirationMinutes": 60
  }
}
```

#### ServiceAuthOptions

Configuration for service-to-service authentication:

```csharp
using SharedContracts.Configuration;

builder.Services.Configure<ServiceAuthOptions>(
    builder.Configuration.GetSection(ServiceAuthOptions.SectionName)
);
```

**appsettings.json:**

```json
{
  "ServiceAuth": {
    "SharedSecret": "service-to-service-secret",
    "UserManagementBaseUrl": "http://localhost:7284"
  }
}
```

---

## Complete Integration Example

Here's how to integrate SharedContracts into a new microservice:

### 1. Add Project Reference

```xml
<ItemGroup>
  <ProjectReference Include="../SharedContracts/SharedContracts.csproj" />
</ItemGroup>
```

### 2. Configure Program.cs

```csharp
using SharedContracts.Authorization;
using SharedContracts.Security;
using SharedContracts.ServiceDiscovery;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Service Discovery
builder.Services.AddConsulServiceDiscovery(builder.Configuration);

// Authentication & Authorization
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
        .RequireAuthenticatedUser()
        .Build();
});
builder.Services.AddPermissionPolicies();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

### 3. Configure appsettings.json

```json
{
  "ServiceConfig": {
    "ServiceName": "my-service",
    "Host": "localhost",
    "Port": 7285,
    "Scheme": "http"
  },
  "Consul": {
    "Host": "http://localhost:8500"
  },
  "JwtValidation": {
    "Issuer": "http://localhost:7280",
    "Audience": "konecta-erp-services",
    "JwksUri": "http://localhost:7280/.well-known/jwks.json",
    "RequireHttpsMetadata": false
  }
}
```

### 4. Use in Controllers

```csharp
using Microsoft.AspNetCore.Authorization;
using SharedContracts.Authorization;

[ApiController]
[Route("api/[controller]")]
public class MyController : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = PermissionConstants.Finance.InvoicesRead)]
    public IActionResult GetInvoices()
    {
        // Only users with finance.invoices.read permission
        return Ok();
    }

    [HttpPost]
    [Authorize(Roles = RoleConstants.SystemAdmin)]
    public IActionResult AdminAction()
    {
        // Only System Admins
        return Ok();
    }
}
```

---

## Architecture Benefits

✅ **Consistency** - Standardized authentication, authorization, and service discovery across all services  
✅ **Maintainability** - Centralized permission and role definitions  
✅ **Type Safety** - Compile-time checking for permissions, roles, and events  
✅ **Reusability** - Common utilities and extensions shared across services  
✅ **Decoupling** - Services communicate via well-defined event contracts  
✅ **Security** - Standardized JWT validation with remote key management

---

## Dependencies

- **Consul** (v1.7.14.3) - Service discovery
- **Microsoft.AspNetCore.Authentication.JwtBearer** (v8.0.10) - JWT authentication
- **Microsoft.IdentityModel.Tokens** (v8.0.1) - Token validation

---

## Services Using SharedContracts

- AuthenticationService
- FinanceService
- HrService
- InventoryService
- UserManagementService

---

## Contributing

When adding new permissions, roles, or events:

1. Add constants to the appropriate class in SharedContracts
2. Update the `All` collection if applicable
3. Rebuild all dependent services
4. Update this README with examples
