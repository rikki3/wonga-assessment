# Wonga Assessment
Full-stack authentication assessment built with React, ASP.NET Core, PostgreSQL, and Docker.

## What Is Implemented
- Register page (`firstName`, `lastName`, `email`, `password`)
- Login page (`email`, `password`)
- Protected user details page (`firstName`, `lastName`, `email`)
- JWT authentication and authorization
- Unit tests and integration tests

## Tech Stack
- Frontend: React + Vite
- Backend: ASP.NET Core 8 Web API
- Database: PostgreSQL 16 (EF Core + Npgsql)
- Containers: Docker Compose

## Quick Start (Recommended)
From project root:

```bat
scripts\up.bat -d
```

App URLs:
- Frontend: http://localhost:3000
- API Swagger: http://localhost:8080/swagger
- Postgres: `localhost:5432`

Stop everything:

```bat
scripts\down.bat
```

## Run Tests
From project root:

```bat
scripts\test.bat
```

## API Endpoints
Base URL: `http://localhost:8080`

- `POST /auth/register` - create user and return JWT
- `POST /auth/login` - authenticate and return JWT
- `GET /user/info` - protected endpoint, requires `Authorization: Bearer <token>`

Example register payload:

```json
{
  "email": "user@example.com",
  "password": "StrongPass123!",
  "firstName": "Jane",
  "lastName": "Doe"
}
```

## Auth and Security (Assessment Scope)
- Password hashing: PBKDF2 + random salt
- JWT: HMAC SHA-256, 6-hour expiry
- CORS: restricted to known local frontend origins (`localhost:3000`, `localhost:5173`)
- Validation: DataAnnotations (`EmailAddress`, `MinLength`, `Required`, etc.)
- Validation errors: consistent JSON format (`code`, `message`, `errors`)

## Project Structure
```text
frontend/                  React app
backend/Wonga.Api/         ASP.NET Core API
backend/Wonga.Api.Tests/   Unit + integration tests
docker-compose.yml         API + DB + web containers
scripts/                   test/up/down helper scripts
```

## Notes
- API applies EF Core migrations automatically on startup.
- `.env` is included for assessment convenience.
