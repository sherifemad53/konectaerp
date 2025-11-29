# Authentication Service API Documentation

This service handles user authentication, JWT token generation, and token validation for the ERP system.

## Endpoints

### Authentication (`api/Auth`)

| Method | Endpoint           | Description                             | Authorization             |
| ------ | ------------------ | --------------------------------------- | ------------------------- |
| POST   | `/register`        | Register a new user account             | Anonymous                 |
| POST   | `/login`           | Authenticate user and receive JWT token | Anonymous                 |
| POST   | `/validate-token`  | Validate a JWT token                    | Anonymous                 |
| PUT    | `/update-password` | Update user password                    | JWT Bearer Token Required |

#### Register

**Request Body:**

```json
{
  "fullName": "string",
  "email": "string",
  "password": "string"
}
```

**Response:**

- Success: User details with confirmation message
- Error: Validation errors or duplicate email message

#### Login

**Request Body:**

```json
{
  "email": "string",
  "password": "string"
}
```

**Response:**

```json
{
  "result": {
    "accessToken": "string",
    "expiresAtUtc": "datetime",
    "keyId": "string",
    "userId": "string",
    "email": "string",
    "roles": ["string"],
    "permissions": ["string"]
  },
  "code": "200",
  "c_Message": "Login successful.",
  "s_Message": "JWT token generated successfully."
}
```

#### Validate Token

**Request Body:** JWT token string

**Response:**

- Valid token: User email, userId, and roles
- Invalid token: 401 Unauthorized

#### Update Password

**Request Body:**

```json
{
  "oldPassword": "string",
  "newPassword": "string",
  "confirmPassword": "string"
}
```

**Response:**

- Success: Confirmation message
- Error: Validation errors or incorrect old password

### Well-Known (`.well-known`)

| Method | Endpoint     | Description                                        | Authorization |
| ------ | ------------ | -------------------------------------------------- | ------------- |
| GET    | `/jwks.json` | Get JSON Web Key Set (JWKS) for token verification | Anonymous     |

#### JWKS Endpoint

Returns the public keys used to verify JWT signatures. This endpoint is used by other services to validate tokens issued by the Authentication Service.

**Response:**

```json
{
  "keys": [
    {
      "kty": "RSA",
      "use": "sig",
      "kid": "string",
      "n": "string",
      "e": "string"
    }
  ]
}
```

## Response Format

All responses follow a standard format:

```json
{
  "result": {},
  "code": "string",
  "c_Message": "string (client-facing message)",
  "s_Message": "string (system/debug message)"
}
```

## Authentication Flow

1. **Registration**: User registers with email and password
2. **Login**: User authenticates and receives JWT token with roles and permissions
3. **Authorization**: JWT token is used in subsequent requests to protected endpoints
4. **Token Validation**: Services can validate tokens using the `/validate-token` endpoint or JWKS
5. **Password Management**: Authenticated users can update their passwords
