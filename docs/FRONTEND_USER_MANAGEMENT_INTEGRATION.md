# Frontend User Management Integration

## Overview
The frontend has been updated to integrate with the UserManagementService, providing a complete user management interface for administrators.

## Changes Made

### 1. **Updated User Service** (`core/services/user.service.ts`)
   - Added interfaces for User, CreateUser, UpdateUser, Role DTOs
   - Implemented API methods to communicate with UserManagementService (port 5002)
   - Methods include:
     - `getAllUsers()` - Fetch all users
     - `getUserById()` - Fetch single user
     - `createUser()` - Create new user
     - `updateUser()` - Update user
     - `deleteUser()` - Delete user
     - `assignRoles()` - Assign roles to user
     - `removeRole()` - Remove role from user
     - `getAllRoles()` - Fetch all roles
     - `getRoleById()` - Fetch single role

### 2. **Created User Management Component** (`shared/components/user-management.component.ts`)
   - Standalone component with full CRUD functionality
   - Features:
     - View all users in a table format
     - Create new users with role assignment
     - Edit existing users
     - Delete users (with confirmation)
     - Assign/remove roles from users
     - Role-based UI (only admins can see management options)
   - Modal dialogs for user creation/edit and role assignment
   - Real-time updates after operations

### 3. **Updated Routing** (`app.routes.ts`)
   - Added `/users` route
   - Protected with `authGuard` and `roleGuard`
   - Restricted to Admin role only

### 4. **Updated Sidebar** (`shared/sidebar/sidebar.component.ts`)
   - Added "User Management" menu item
   - Icon: 👤
   - Only visible to Admin users

### 5. **Updated HTTP Configuration** (`app.config.ts`)
   - Fixed interceptor configuration
   - Added `withInterceptorsFromDi()` for proper interceptor support

## User Interface

### User Management Page Features

#### User List Table
- Displays: Name, Email, Username, Roles, Status
- Shows role badges for each user
- Color-coded status (Active/Inactive)
- Action buttons for Edit, Roles, and Delete (Admin only)

#### Create/Edit User Modal
- Fields:
  - First Name *
  - Last Name *
  - Email *
  - Username *
  - Active checkbox
  - Role selection (multiple checkboxes)
- Form validation
- Saves to backend on submit

#### Role Assignment Modal
- Shows all available roles
- Allows toggling roles for a user
- Displays role names and descriptions
- Updates user's role list in real-time

## API Integration

### Service Endpoints Used

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/users` | Get all users |
| GET | `/api/users/{id}` | Get user by ID |
| POST | `/api/users` | Create new user |
| PUT | `/api/users/{id}` | Update user |
| DELETE | `/api/users/{id}` | Delete user |
| POST | `/api/users/{id}/roles` | Assign roles |
| DELETE | `/api/users/{id}/roles/{roleId}` | Remove role |
| GET | `/api/roles` | Get all roles |

### Authentication
- All requests include JWT token in Authorization header
- Token retrieved from localStorage via AuthService
- Interceptor automatically adds token to all HTTP requests

## Role-Based Access

### Admin Access
- ✅ View all users
- ✅ Create new users
- ✅ Edit any user
- ✅ Delete users
- ✅ Assign roles
- ✅ Remove roles

### Other Roles (Manager, Employee)
- Cannot access `/users` route (redirected)
- Menu item hidden in sidebar

## Component Features

### User Creation
```typescript
{
  username: string,
  email: string,
  firstName: string,
  lastName: string,
  isActive: boolean,
  roleIds: string[]
}
```

### User Update
- Pre-fills form with existing user data
- Allows changing all fields including roles
- Maintains user ID

### Role Assignment
- Shows current roles assigned
- Allows adding/removing roles
- Saves changes to backend

## Styling

The component uses inline styles for a clean, modern UI:
- Modal dialogs for forms
- Table layout for user list
- Color-coded status indicators
- Responsive design
- Role badges for visual clarity
- Action buttons with hover effects

## Usage Example

### As Admin User:
1. Login to the application
2. Navigate to "User Management" in the sidebar
3. View all users in the table
4. Click "Add User" to create a new user
5. Fill in the form and select roles
6. Click "Save" to create the user
7. Use "Edit" to modify a user
8. Use "Roles" to change user's role assignments
9. Use "Delete" to remove a user (with confirmation)

## Error Handling

- API errors are logged to console
- User sees error message if operation fails
- List refreshes after successful operations
- Confirmation dialog before destructive operations

## Future Enhancements

1. Add pagination for large user lists
2. Add search/filter functionality
3. Add bulk operations
4. Add export functionality
5. Add audit trail display
6. Add user activity log
7. Add password reset functionality
8. Add email notifications for role changes

## Testing

To test the integration:
1. Start the backend services: `docker-compose up -d`
2. Start the frontend: `npm start` in frontend directory
3. Login as admin (admin@konecta.com / admin123)
4. Navigate to User Management
5. Create, edit, and manage users

## Dependencies

- @angular/forms (for FormsModule)
- @angular/common (for CommonModule)
- HTTP client for API calls
- JWT token management via AuthService

## Notes

- The component is standalone to reduce boilerplate
- Modal implementation uses native DOM event handling
- Role IDs are mapped to role names for display
- Current user's role is checked for admin access
- All data is fetched from UserManagementService API

