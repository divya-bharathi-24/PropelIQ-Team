# Task - task_001_core_domain_schemas

## Requirement Reference

- **User Story:** us_004
- **Story Location:** `.propel/context/tasks/EP-DATA/us_004/us_004.md`
- **Acceptance Criteria:**
  - AC-1: Schema-per-service isolation implemented — PostgreSQL 16 with separate schemas (auth, patient, appointment, provider) with no cross-schema foreign keys
  - AC-2: Auth domain tables created — Users, Roles, UserRoles, RefreshTokens, LoginAttempts with indexes on email (unique), role lookups, and token expiry
  - AC-3: Patient domain tables created — Patients, PatientContacts, InsuranceDetails, MedicalHistory with PII columns marked for encryption and composite indexes on (last_name, date_of_birth)
  - AC-4: Appointment domain tables created — Appointments, TimeSlots, ProviderSchedules, SlotSwapRequests with indexes on (provider_id, date), (patient_id, status), and no-overlap constraint per provider
  - AC-5: EF Core 8 configuration with Npgsql — DbContext per service resolves correctly, connection pooling (min 2, max 10), idempotent migrations via CLI
- **Edge Cases:**
  - Migration conflict on parallel development → timestamp-prefixed naming; CI rejects duplicate migration names
  - Connection pool exhaustion → circuit breaker returns 503 after 5-second timeout
  - Schema drift between environments → CI compares pending migrations and blocks deployment if unapplied migrations exist

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
| Database | PostgreSQL | 16.x | TR-004 — Spec constraint; schema-per-service on single instance |
| ORM | Entity Framework Core | 8.0 | TR-004 — Code-first migrations; Npgsql provider |
| ORM Provider | Npgsql | 8.x | DR-001 — PostgreSQL-specific EF Core provider |
| Backend Framework | ASP.NET Core Web API | 8.0 | TR-002 — Host for DbContext registration and DI |

---

## Task Overview

Design and create PostgreSQL database schemas for the four core domains (Auth, Patient, Appointment, Provider) using EF Core 8 code-first migrations with Npgsql. Each domain uses its own schema within a shared PostgreSQL 16 instance, enforcing bounded context isolation with no cross-schema foreign keys. Connection pooling is configured for free-tier resource constraints.

## Dependent Tasks

- task_001_backend_api_scaffolding (US_002) — Backend service projects must exist before database contexts can be configured

## Impacted Components

- New: `src/Services/Auth/HealthPlatform.Auth.Api/Data/AuthDbContext.cs` — Auth EF Core context
- New: `src/Services/Auth/HealthPlatform.Auth.Api/Data/Entities/` — Auth domain entities
- New: `src/Services/Auth/HealthPlatform.Auth.Api/Data/Migrations/` — Auth schema migrations
- New: `src/Services/Appointment/HealthPlatform.Appointment.Api/Data/AppointmentDbContext.cs` — Appointment EF Core context
- New: `src/Services/Appointment/HealthPlatform.Appointment.Api/Data/Entities/` — Appointment domain entities
- New: `src/Services/Appointment/HealthPlatform.Appointment.Api/Data/Migrations/` — Appointment schema migrations
- New: `src/Shared/HealthPlatform.Shared/Data/PatientDbContext.cs` — Patient EF Core context (shared library)
- New: `src/Shared/HealthPlatform.Shared/Data/Entities/` — Patient domain entities

## Implementation Plan

1. Create Auth schema entities (User, Role, UserRole, RefreshToken, LoginAttempt) with EF Core configurations including unique email index, role lookup indexes, and token expiry index.
2. Create Patient schema entities (Patient, PatientContact, InsuranceDetail, MedicalHistory) with PII column markers and composite index on (last_name, date_of_birth).
3. Create Appointment schema entities (Appointment, TimeSlot, ProviderSchedule, SlotSwapRequest) with provider-date indexes, patient-status indexes, and no-overlap check constraint.
4. Create Provider schema within Appointment context for schedule management.
5. Configure each DbContext with schema isolation (`HasDefaultSchema`), Npgsql provider, and connection pooling (min 2, max 10).
6. Generate timestamped initial EF Core migrations for all 4 schemas.
7. Verify idempotent migration application via `dotnet ef database update`.
8. Add connection pool exhaustion handling with 5-second timeout circuit breaker.

## Current Project State

```text
src/
├── HealthPlatform.sln
├── Gateway/
│   └── HealthPlatform.Gateway/
├── Services/
│   ├── Auth/HealthPlatform.Auth.Api/
│   │   ├── Controllers/
│   │   ├── Services/
│   │   ├── Repositories/
│   │   └── Program.cs
│   ├── Appointment/HealthPlatform.Appointment.Api/
│   │   ├── Controllers/
│   │   ├── Services/
│   │   ├── Repositories/
│   │   └── Program.cs
│   └── ... (5 more services)
└── README.md
```

## Expected Changes

| Action | File Path | Description |
|--------|-----------|-------------|
| CREATE | `src/Services/Auth/HealthPlatform.Auth.Api/Data/AuthDbContext.cs` | Auth DbContext with `auth` schema, connection pooling config |
| CREATE | `src/Services/Auth/HealthPlatform.Auth.Api/Data/Entities/User.cs` | User entity with unique email index |
| CREATE | `src/Services/Auth/HealthPlatform.Auth.Api/Data/Entities/Role.cs` | Role entity with lookup index |
| CREATE | `src/Services/Auth/HealthPlatform.Auth.Api/Data/Entities/UserRole.cs` | UserRole join entity |
| CREATE | `src/Services/Auth/HealthPlatform.Auth.Api/Data/Entities/RefreshToken.cs` | RefreshToken with expiry index |
| CREATE | `src/Services/Auth/HealthPlatform.Auth.Api/Data/Entities/LoginAttempt.cs` | LoginAttempt tracking entity |
| CREATE | `src/Services/Auth/HealthPlatform.Auth.Api/Data/Configurations/` | EF Core entity configurations for Auth schema |
| CREATE | `src/Services/Appointment/HealthPlatform.Appointment.Api/Data/AppointmentDbContext.cs` | Appointment DbContext with `appointment` schema |
| CREATE | `src/Services/Appointment/HealthPlatform.Appointment.Api/Data/Entities/Appointment.cs` | Appointment entity with status lifecycle |
| CREATE | `src/Services/Appointment/HealthPlatform.Appointment.Api/Data/Entities/TimeSlot.cs` | TimeSlot entity with no-overlap constraint |
| CREATE | `src/Services/Appointment/HealthPlatform.Appointment.Api/Data/Entities/ProviderSchedule.cs` | Provider schedule entity |
| CREATE | `src/Services/Appointment/HealthPlatform.Appointment.Api/Data/Entities/SlotSwapRequest.cs` | Slot swap waitlist entity |
| CREATE | `src/Shared/HealthPlatform.Shared/Data/PatientDbContext.cs` | Patient DbContext with `patient` schema |
| CREATE | `src/Shared/HealthPlatform.Shared/Data/Entities/Patient.cs` | Patient entity with PII markers, composite index |
| CREATE | `src/Shared/HealthPlatform.Shared/Data/Entities/PatientContact.cs` | Patient contact entity |
| CREATE | `src/Shared/HealthPlatform.Shared/Data/Entities/InsuranceDetail.cs` | Insurance details entity |
| CREATE | `src/Shared/HealthPlatform.Shared/Data/Entities/MedicalHistory.cs` | Medical history entity |

## External References

- [EF Core 8.0 — PostgreSQL Provider (Npgsql)](https://www.npgsql.org/efcore/)
- [EF Core Schema Configuration](https://learn.microsoft.com/en-us/ef/core/modeling/relational/default-schema)
- [EF Core Migrations](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli)
- [PostgreSQL 16 Documentation](https://www.postgresql.org/docs/16/)
- [EF Core Connection Pooling with Npgsql](https://www.npgsql.org/doc/connection-string-parameters.html)

## Build Commands

- Refer to [.propel/build/](.propel/build/) for applicable technology stack build commands

## Implementation Validation Strategy

- [ ] Unit tests pass — entity configurations produce expected schema
- [ ] Integration tests pass — migrations apply idempotently to PostgreSQL 16
- [ ] All 4 schemas created with correct table structures and indexes
- [ ] No cross-schema foreign keys exist

## Implementation Checklist

- [x] Create schema-per-service isolation (auth, patient, appointment, provider schemas) with no cross-schema foreign keys → AC-1
- [x] Create Auth domain tables (Users, Roles, UserRoles, RefreshTokens, LoginAttempts) with unique email and role lookup indexes → AC-2
- [x] Create Patient domain tables (Patients, PatientContacts, InsuranceDetails, MedicalHistory) with PII markers and composite index → AC-3
- [x] Create Appointment domain tables (Appointments, TimeSlots, ProviderSchedules, SlotSwapRequests) with indexes and no-overlap constraint → AC-4
- [x] Configure EF Core 8 DbContext per service with Npgsql and connection pooling (min 2, max 10) → AC-5
- [x] Generate timestamped initial EF Core migration scripts → AC-5
- [x] Verify idempotent migration application via CLI → AC-5
- [x] Add connection pool exhaustion circuit breaker (5-second timeout, 503 response) → AC-5 (edge case)
