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

## Run Unit and Integration Tests
1) Open a terminal & navigate to the project's root directory.
2) Execute the following command:

```bat
scripts\test.bat
```

## Quick Start
1) Open a terminal & navigate to the project's root directory.
2) Execute the following command:

```bat
scripts\up.bat -d
```

3) Access the system at the following URLs:
- React Frontend: [http://localhost:3000](http://localhost:3000)
- C# API Backend: [http://localhost:8080/swagger](http://localhost:8080/swagger)

The PostgreSQL server is also running locally on port 5432.

3) When you are done, execute the following command:

```bat
scripts\down.bat
```

## API Endpoints
- `POST /auth/register` - create user and return JWT
- `POST /auth/login` - authenticate and return JWT
- `GET /user/info` - protected endpoint, requires `Authorization: Bearer <token>`

## Auth and Security (Assessment Scope)
- Password hashing: PBKDF2 + random salt
- JWT: HMAC SHA-256, 6-hour expiry
- CORS: restricted to known local frontend origins (`localhost:3000`, `localhost:5173`)
- Validation: DataAnnotations (`EmailAddress`, `MinLength`, `Required`, etc.)
- Validation errors: consistent JSON format (`code`, `message`, `errors`)

## Notes
- API applies EF Core migrations automatically on startup.
- `.env` is included for assessment convenience.
