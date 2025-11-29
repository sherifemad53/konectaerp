# User Management Service API Documentation

This service handles user management, roles, and permissions for the ERP system.

## Endpoints

### Users (`api/Users`)

All endpoints require **Admin** authorization unless otherwise specified.

| Method | Endpoint               | Description                                              | Query Parameters                                                     |
| ------ | ---------------------- | -------------------------------------------------------- | -------------------------------------------------------------------- |
| GET    | `/`                    | Get paginated list of users                              | `page`, `pageSize`, `search`, `role`, etc. (via UserQueryParameters) |
| GET    | `/all-users`           | Get all users without pagination                         | -                                                                    |
| GET    | `/{id}`                | Get user by ID                                           | -                                                                    |
| POST   | `/`                    | Create a new user                                        | -                                                                    |
| PUT    | `/{id}`                | Update user details                                      | -                                                                    |
| PATCH  | `/{id}/role`           | Change user's role                                       | -                                                                    |
| GET    | `/{id}/roles`          | Get all roles assigned to a user                         | -                                                                    |
| PUT    | `/{id}/roles`          | Set/update user's roles                                  | -                                                                    |
| PATCH  | `/{id}/status`         | Update user status (active/inactive)                     | -                                                                    |
| DELETE | `/{id}`                | Soft delete a user                                       | -                                                                    |
| POST   | `/{id}/restore`        | Restore a soft-deleted user                              | -                                                                    |
| GET    | `/summary`             | Get user statistics summary                              | -                                                                    |
| GET    | `/{id}/authorizations` | Get user authorization profile (service-to-service only) | -                                                                    |

**Note:** The `/authorizations` endpoint is accessible without user authentication but requires a valid service token in the `X-Service-Token` header.

### Roles (`api/Roles`)

All endpoints require **Admin** authorization.

| Method | Endpoint            | Description             |
| ------ | ------------------- | ----------------------- |
| GET    | `/`                 | Get all roles           |
| GET    | `/{id}`             | Get role by ID          |
| POST   | `/`                 | Create a new role       |
| PUT    | `/{id}`             | Update role details     |
| DELETE | `/{id}`             | Delete a role           |
| PUT    | `/{id}/permissions` | Update role permissions |

### Permissions (`api/Permissions`)

All endpoints require **Admin** authorization.

| Method | Endpoint | Description               |
| ------ | -------- | ------------------------- |
| GET    | `/`      | Get all permissions       |
| GET    | `/{id}`  | Get permission by ID      |
| POST   | `/`      | Create a new permission   |
| PUT    | `/{id}`  | Update permission details |
| DELETE | `/{id}`  | Delete a permission       |

## Authorization

- **Admin Only**: Most endpoints require the `AdminOnly` policy
- **Service-to-Service**: The user authorizations endpoint uses a shared secret token for inter-service communication
