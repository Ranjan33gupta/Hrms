# HRMS API - Modular Monolith Architecture

## Architecture Overview

The HRMS API follows a Modular Monolith architecture, which organizes the codebase into cohesive business modules while still deploying as a single application.

### Key Benefits
- Clear module boundaries
- Better separation of concerns
- Improved maintainability
- Easier to understand and navigate
- Potential for future microservices migration

## Module Structure

Each module follows a Clean Architecture approach with these layers:

```
Modules/
├── Employee/
│   ├── API/                 # Controllers and API endpoints
│   ├── Application/         # Application services, DTOs
│   │   ├── DTOs/            # Data Transfer Objects
│   │   ├── Interfaces/      # Service interfaces
│   │   └── Services/        # Service implementations
│   ├── Domain/              # Domain entities and interfaces
│   │   ├── Interfaces/      # Repository interfaces
│   │   └── [Entities]       # Domain models
│   └── Infrastructure/      # Repository implementations
├── Leave/
│   ├── [Same structure as Employee]
└── Auth/
    ├── [Same structure as Employee]
```

## Shared Components

```
Shared/
├── Domain/                  # Shared domain primitives
└── Infrastructure/          # Shared infrastructure components
    ├── BaseRepository.cs    # Base repository implementation
    └── ModuleRegistration.cs # Module registration for DI
```

## API Endpoints

- Employee Module: `/api/employees`
- Leave Module: `/api/leaverequests`
- Auth Module: `/api/auth`

## Getting Started

1. Ensure PostgreSQL is running
2. Update connection string in appsettings.json if needed
3. Run the application: `dotnet run`
4. Access Swagger UI: `https://localhost:5001/swagger`

## Technology Stack

- ASP.NET Core 6.0
- Entity Framework Core
- PostgreSQL
- JWT Authentication
