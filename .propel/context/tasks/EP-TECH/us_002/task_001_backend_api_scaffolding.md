# Task - task_001_backend_api_scaffolding

## Requirement Reference

- **User Story:** us_002
- **Story Location:** `.propel/context/tasks/EP-TECH/us_002/us_002.md`
- **Acceptance Criteria:**
  - AC-1: .NET 8 solution with 7 microservice projects created (Auth, Appointment, Intake, Document, ClinicalIntelligence, Notification, Admin) each with Controller/Service/Repository layers and DI configured
  - AC-2: YARP API Gateway configured as single entry point — routes all requests to downstream services, validates JWT Bearer tokens, returns 401 for unauthenticated calls
  - AC-3: REST API with OpenAPI 3.0 documentation via Swashbuckle accessible at `/swagger`
  - AC-4: TLS 1.2+ enforced on all endpoints — HTTP connections rejected or redirected to HTTPS
  - AC-5: JWT Bearer authentication configured — valid tokens authenticated, claims extracted, role-based access enforced at gateway
- **Edge Cases:**
  - Microservice fails to start → YARP health check marks it unhealthy and returns 503 for that service's routes within 60 seconds
  - JWT token expired mid-request → return 401 with `token_expired` error code, not a generic 403
  - OpenAPI spec generation fails silently → CI pipeline validates swagger.json exists and is valid JSON on every build

---

## Design References

| Reference Type | Value |
|----------------|-------|
| **UI Impact** | No |
| **Figma URL** | N/A |
| **Wireframe Status** | N/A |
| **Wireframe Type** | N/A |
| **Wireframe Path/URL** | N/A |
| **Screen Spec** | N/A |
| **UXR Requirements** | N/A |
| **Design Tokens** | N/A |

---

## AI References

| Reference Type | Value |
|----------------|-------|
| **AI Impact** | No |
| **AIR Requirements** | N/A |
| **AI Pattern** | N/A |
| **Prompt Template Path** | N/A |
| **Guardrails Config** | N/A |
| **Model Provider** | N/A |

---

## Mobile References

| Reference Type | Value |
|----------------|-------|
| **Mobile Impact** | No |
| **Platform Target** | N/A |
| **Min OS Version** | N/A |
| **Mobile Framework** | N/A |

---

## Applicable Technology Stack

| Layer | Technology | Version | Justification |
|-------|------------|---------|---------------|
| Backend Framework | ASP.NET Core Web API | 8.0 | TR-002 — Spec constraint; minimal API pattern with DI and 3-tier layered architecture |
| API Gateway | YARP | 2.1 | TR-003 — Centralized RBAC, JWT validation, rate limiting; native .NET 8 integration |
| ORM | Entity Framework Core | 8.0 | TR-004 — Code-first migrations; LINQ query composition; Npgsql provider |
| Database | PostgreSQL | 16.x | TR-004 — Spec constraint; schema-per-service isolation on single instance |
| Authentication | ASP.NET Core Identity + JWT | 8.0 | NFR-009 — RBAC enforcement; bcrypt password hashing; refresh token rotation |
| API Documentation | Swashbuckle (Swagger) | 6.x | TR-003 — Auto-generated OpenAPI 3.0 spec from controller metadata |
| Logging | Serilog | 3.x | TR-009 — Structured JSON logging with correlation IDs |

---

## Task Overview

Scaffold the complete .NET 8 backend solution containing 7 domain-aligned microservice projects and a YARP-based API Gateway. Each microservice follows a 3-tier layered architecture (Controller/Service/Repository) with dependency injection. The API Gateway serves as the single client entry point, enforcing JWT Bearer authentication, RBAC, and TLS 1.2+. All services expose OpenAPI 3.0 documentation via Swashbuckle.

## Dependent Tasks

- None — this is a foundational task

## Impacted Components

- New: `HealthPlatform.sln` — root solution file
- New: `src/Gateway/HealthPlatform.Gateway` — YARP API Gateway project
- New: `src/Services/Auth/HealthPlatform.Auth.Api` — Auth microservice
- New: `src/Services/Appointment/HealthPlatform.Appointment.Api` — Appointment microservice
- New: `src/Services/Intake/HealthPlatform.Intake.Api` — Intake microservice
- New: `src/Services/Document/HealthPlatform.Document.Api` — Document microservice
- New: `src/Services/ClinicalIntelligence/HealthPlatform.ClinicalIntelligence.Api` — Clinical Intelligence microservice
- New: `src/Services/Notification/HealthPlatform.Notification.Api` — Notification microservice
- New: `src/Services/Admin/HealthPlatform.Admin.Api` — Admin microservice

## Implementation Plan

1. Create the root .NET 8 solution file (`HealthPlatform.sln`) and folder structure under `src/`.
2. Scaffold the YARP API Gateway project with reverse proxy route configuration pointing to all 7 downstream services.
3. Scaffold each of the 7 microservice projects using the minimal API pattern, each with `Controllers/`, `Services/`, `Repositories/` folders and DI registration in `Program.cs`.
4. Configure JWT Bearer authentication in the Gateway project — RS256 token validation, role claim extraction, 401 for unauthenticated requests.
5. Configure HTTPS enforcement and TLS 1.2+ on all project Kestrel configurations.
6. Add Swashbuckle to each microservice for OpenAPI 3.0 spec generation at `/swagger`.
7. Add health check endpoints (`/health`, `/ready`) to all services and configure YARP health probes for downstream service monitoring.
8. Add Serilog structured logging with correlation ID middleware to all projects.

## Current Project State

```text
d:\PropelIQ-Team\
├── .propel/
│   ├── context/
│   │   ├── docs/
│   │   │   ├── spec.md
│   │   │   ├── design.md
│   │   │   └── model.md
│   │   └── tasks/
│   │       └── EP-TECH/
│   │           └── us_001/
│   │               └── task_001_frontend_scaffolding.md
│   └── templates/
└── README.md
```

## Expected Changes

| Action | File Path | Description |
|--------|-----------|-------------|
| CREATE | `src/HealthPlatform.sln` | Root solution file referencing all 8 projects |
| CREATE | `src/Gateway/HealthPlatform.Gateway/HealthPlatform.Gateway.csproj` | YARP API Gateway project file |
| CREATE | `src/Gateway/HealthPlatform.Gateway/Program.cs` | Gateway startup with YARP, JWT validation, HTTPS enforcement |
| CREATE | `src/Gateway/HealthPlatform.Gateway/appsettings.json` | YARP route configuration and JWT settings |
| CREATE | `src/Services/Auth/HealthPlatform.Auth.Api/HealthPlatform.Auth.Api.csproj` | Auth service project file |
| CREATE | `src/Services/Auth/HealthPlatform.Auth.Api/Program.cs` | Auth service startup with DI, Swagger, Serilog, health checks |
| CREATE | `src/Services/Auth/HealthPlatform.Auth.Api/Controllers/` | Auth API controllers directory |
| CREATE | `src/Services/Auth/HealthPlatform.Auth.Api/Services/` | Auth business logic services directory |
| CREATE | `src/Services/Auth/HealthPlatform.Auth.Api/Repositories/` | Auth data access repositories directory |
| CREATE | `src/Services/Appointment/HealthPlatform.Appointment.Api/HealthPlatform.Appointment.Api.csproj` | Appointment service project file |
| CREATE | `src/Services/Appointment/HealthPlatform.Appointment.Api/Program.cs` | Appointment service startup |
| CREATE | `src/Services/Intake/HealthPlatform.Intake.Api/HealthPlatform.Intake.Api.csproj` | Intake service project file |
| CREATE | `src/Services/Intake/HealthPlatform.Intake.Api/Program.cs` | Intake service startup |
| CREATE | `src/Services/Document/HealthPlatform.Document.Api/HealthPlatform.Document.Api.csproj` | Document service project file |
| CREATE | `src/Services/Document/HealthPlatform.Document.Api/Program.cs` | Document service startup |
| CREATE | `src/Services/ClinicalIntelligence/HealthPlatform.ClinicalIntelligence.Api/HealthPlatform.ClinicalIntelligence.Api.csproj` | Clinical Intelligence service project file |
| CREATE | `src/Services/ClinicalIntelligence/HealthPlatform.ClinicalIntelligence.Api/Program.cs` | Clinical Intelligence service startup |
| CREATE | `src/Services/Notification/HealthPlatform.Notification.Api/HealthPlatform.Notification.Api.csproj` | Notification service project file |
| CREATE | `src/Services/Notification/HealthPlatform.Notification.Api/Program.cs` | Notification service startup |
| CREATE | `src/Services/Admin/HealthPlatform.Admin.Api/HealthPlatform.Admin.Api.csproj` | Admin service project file |
| CREATE | `src/Services/Admin/HealthPlatform.Admin.Api/Program.cs` | Admin service startup |

## External References

- [ASP.NET Core 8.0 Minimal APIs](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/overview?view=aspnetcore-8.0)
- [YARP 2.1 Documentation](https://microsoft.github.io/reverse-proxy/)
- [Swashbuckle ASP.NET Core 6.x](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)
- [ASP.NET Core JWT Bearer Authentication](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/?view=aspnetcore-8.0)
- [Serilog ASP.NET Core 3.x](https://github.com/serilog/serilog-aspnetcore)
- [ASP.NET Core Health Checks](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-8.0)

## Build Commands

- Refer to [.propel/build/](.propel/build/) for applicable technology stack build commands

## Implementation Validation Strategy

- [ ] Unit tests pass — service DI registration resolves all dependencies
- [ ] Integration tests pass — YARP routes traffic to each downstream service
- [ ] All 7 microservices start and respond to health check endpoints
- [ ] API Gateway rejects unauthenticated requests with 401
- [ ] Swagger UI accessible at `/swagger` for each microservice

## Implementation Checklist

- [x] Create .NET 8 solution with 7 minimal API projects (Auth, Appointment, Intake, Document, ClinicalIntelligence, Notification, Admin) with Controller/Service/Repository layers and DI → AC-1
- [x] Configure YARP reverse proxy with route definitions, JWT Bearer validation, and 401 for unauthenticated calls → AC-2
- [x] Add Swashbuckle OpenAPI 3.0 documentation to each microservice at `/swagger` → AC-3
- [x] Enforce HTTPS/TLS 1.2+ with HTTP-to-HTTPS redirect on all endpoints → AC-4
- [x] Configure JWT Bearer authentication with RS256 signing and role claim extraction → AC-5
- [x] Add health check endpoints (`/health`, `/ready`) with YARP health probes → AC-1 (edge case: unhealthy service returns 503)
- [x] Configure expired token handling to return 401 with `token_expired` error code → AC-5 (edge case)
- [x] Add Serilog structured JSON logging with correlation ID middleware → TR-009
