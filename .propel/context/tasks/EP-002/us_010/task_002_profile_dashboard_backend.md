# Task - task_002_profile_dashboard_backend

## Requirement Reference

- **User Story:** us_010
- **Story Location:** `.propel/context/tasks/EP-002/us_010/us_010.md`
- **Acceptance Criteria:**
  - AC-1: Patient profile view and edit — CRUD API for personal information, insurance details, emergency contacts with field validation
  - AC-2: Profile photo upload — validate 2MB/JPEG/PNG, resize to 200x200px, store securely, serve within 5 seconds
  - AC-3: Dashboard displays upcoming appointments — API returns next 5 appointments sorted by date
  - AC-4: Dashboard quick actions panel — routing endpoints for feature navigation
  - AC-5: Recent activity feed — API returns 10 most recent activities in chronological order
- **Edge Cases:**
  - Invalid phone format → return validation error with previous value preserved
  - Dashboard data partially unavailable → circuit breaker per data source; return partial response
  - Photo upload exceeds 2MB → reject with 413 status

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
| Backend Framework | ASP.NET Core Web API | 8.0 | TR-002 — Minimal API for profile and dashboard endpoints |
| Database | PostgreSQL | 16.x | TR-004 — Patient schema for profile data |
| ORM | Entity Framework Core | 8.0 | TR-004 — Data access for patient CRUD |
| Cache | Upstash Redis | Free tier | TR-005 — Dashboard data caching for performance |

---

## Task Overview

Implement the patient profile CRUD API and dashboard data aggregation API. Profile management includes photo upload with server-side validation and image resizing. The dashboard API aggregates data from multiple sources (appointments, activity log) with circuit breaker isolation per data source to enable partial responses when individual services are unavailable.

## Dependent Tasks

- task_001_backend_api_scaffolding (US_002) — Backend service projects must exist
- task_001_core_domain_schemas (US_004) — Patient schema must exist

## Impacted Components

- New: `src/Services/Auth/HealthPlatform.Auth.Api/Controllers/PatientProfileController.cs` — Profile CRUD endpoints
- New: `src/Services/Auth/HealthPlatform.Auth.Api/Services/PatientProfileService.cs` — Profile business logic
- New: `src/Services/Auth/HealthPlatform.Auth.Api/Services/PhotoUploadService.cs` — Photo validation and resize
- New: `src/Services/Appointment/HealthPlatform.Appointment.Api/Controllers/DashboardController.cs` — Dashboard data endpoint
- New: `src/Services/Appointment/HealthPlatform.Appointment.Api/Services/DashboardService.cs` — Dashboard aggregation

## Implementation Plan

1. Create patient profile CRUD endpoints (GET, PUT) with field validation (phone format, email format).
2. Implement profile photo upload endpoint — validate file type (JPEG/PNG) and size (2MB max), resize to 200x200px, store path in database.
3. Create dashboard data aggregation endpoint returning upcoming appointments (next 5), activity feed (last 10), and quick action availability.
4. Implement circuit breaker isolation per dashboard data source — return partial response with null sections for failed sources.
5. Cache dashboard data in Upstash Redis with 60-second TTL.
6. Implement phone format validation with proper error response preserving previous value.

## Current Project State

```text
src/Services/
├── Auth/HealthPlatform.Auth.Api/
│   ├── Controllers/ (Registration, Auth, StaffAccount, Audit)
│   ├── Services/ (Registration, Token, Session, RateLimit, StaffAccount, AuthAudit)
│   └── Data/ (AuthDbContext, entities)
├── Appointment/HealthPlatform.Appointment.Api/
│   ├── Controllers/
│   ├── Services/
│   └── Data/ (AppointmentDbContext, entities)
└── Shared/HealthPlatform.Shared/
    └── Data/ (PatientDbContext, entities)
```

## Expected Changes

| Action | File Path | Description |
|--------|-----------|-------------|
| CREATE | `src/Services/Auth/HealthPlatform.Auth.Api/Controllers/PatientProfileController.cs` | Profile GET/PUT with photo upload |
| CREATE | `src/Services/Auth/HealthPlatform.Auth.Api/Services/PatientProfileService.cs` | Profile CRUD business logic |
| CREATE | `src/Services/Auth/HealthPlatform.Auth.Api/Services/PhotoUploadService.cs` | Photo validation (2MB, JPEG/PNG), 200x200 resize |
| CREATE | `src/Services/Auth/HealthPlatform.Auth.Api/Models/PatientProfileDto.cs` | Profile data transfer object |
| CREATE | `src/Services/Auth/HealthPlatform.Auth.Api/Validators/PatientProfileValidator.cs` | Phone format and field validation |
| CREATE | `src/Services/Appointment/HealthPlatform.Appointment.Api/Controllers/DashboardController.cs` | Dashboard data aggregation endpoint |
| CREATE | `src/Services/Appointment/HealthPlatform.Appointment.Api/Services/DashboardService.cs` | Dashboard aggregation with circuit breaker |

## External References

- [ASP.NET Core 8.0 File Upload](https://learn.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-8.0)
- [ImageSharp for .NET](https://docs.sixlabors.com/articles/imagesharp/)
- [Polly Circuit Breaker](https://www.thepollyproject.org/)
- [FluentValidation](https://docs.fluentvalidation.net/en/latest/)

## Build Commands

- Refer to [.propel/build/](.propel/build/) for applicable technology stack build commands

## Implementation Validation Strategy

- [x] Unit tests pass — photo validation, profile CRUD, dashboard aggregation with partial failures
- [x] Integration tests pass — profile update persists; dashboard returns partial data on source failure

## Implementation Checklist

- [x] Create patient profile CRUD API (GET/PUT) with field validation (phone, email format) → AC-1
- [x] Implement photo upload with validation (2MB, JPEG/PNG), 200x200px resize, secure storage → AC-2
- [x] Create dashboard API returning next 5 upcoming appointments sorted by date → AC-3
- [x] Implement quick action availability check endpoints → AC-4
- [x] Create activity feed API returning 10 most recent activities in chronological order → AC-5
- [x] Add circuit breaker per dashboard data source for partial response on failures → Edge case
