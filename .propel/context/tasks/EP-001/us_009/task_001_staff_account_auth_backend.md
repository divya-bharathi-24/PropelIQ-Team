# Task - task_001_staff_account_auth_backend

## Requirement Reference

- **User Story:** us_009
- **Story Location:** `.propel/context/tasks/EP-001/us_009/us_009.md`
- **Acceptance Criteria:**
  - AC-1: Staff can create patient accounts — Staff+ role authentication required; form with name, phone, DOB; status "active" (no email verification); temporary password generated; creating staff recorded in audit log
  - AC-2: Temporary password with forced change — first login forces password change; temporary password invalidated
  - AC-3: Authentication events logged immutably — (event_type, user_id, ip_address, user_agent, timestamp, success/failure, failure_reason)
  - AC-4: Auth audit log queryable by admin — search by user ID, date range, event type; paginated (50/page) within 2 seconds; no modify/delete
- **Edge Cases:**
  - Staff creates duplicate phone → allow; flag for potential merge review
  - Audit log storage approaching capacity → alert at 80%; never drop records
  - Concurrent staff creating same patient → optimistic concurrency with retry

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
| Backend Framework | ASP.NET Core Web API | 8.0 | TR-002 — Minimal API for staff account and audit endpoints |
| Authentication | ASP.NET Core Identity | 8.0 | NFR-009 — Role-based authorization for Staff+ access |
| Database | PostgreSQL | 16.x | TR-004 — Auth schema for user records; audit schema for event log |
| ORM | Entity Framework Core | 8.0 | TR-004 — Data access with optimistic concurrency |
| Logging | Serilog | 3.x | TR-009 — Structured audit event logging |

---

## Task Overview

Implement the staff patient account creation API (Staff+ authorization, temporary password generation, forced change on first login) and the immutable authentication event logging system with admin-only queryable audit log. All auth events (login, logout, failed attempts, token refresh, password changes) are recorded with full context and are queryable by admin within 2 seconds.

## Dependent Tasks

- task_001_rbac_session_backend (US_008) — RBAC and JWT must exist before staff role enforcement

## Impacted Components

- New: `src/Services/Auth/HealthPlatform.Auth.Api/Controllers/StaffAccountController.cs` — Staff patient creation endpoint
- New: `src/Services/Auth/HealthPlatform.Auth.Api/Controllers/AuditController.cs` — Admin audit query endpoint
- New: `src/Services/Auth/HealthPlatform.Auth.Api/Services/StaffAccountService.cs` — Staff account creation logic
- New: `src/Services/Auth/HealthPlatform.Auth.Api/Services/AuthAuditService.cs` — Immutable auth event logging
- New: `src/Services/Auth/HealthPlatform.Auth.Api/Services/TemporaryPasswordService.cs` — Temp password generation
- Modify: `src/Services/Auth/HealthPlatform.Auth.Api/Controllers/AuthController.cs` — Add forced password change check on login

## Implementation Plan

1. Create staff patient account creation endpoint with Staff+ role authorization — accepts name, phone, DOB; creates user with "active" status.
2. Implement temporary password generation (cryptographically secure) with forced-change flag on account.
3. Add forced password change middleware — redirect users with forced-change flag before allowing any other action.
4. Implement immutable auth event logging service — write events to audit table with full context (event_type, user_id, ip, user_agent, timestamp, success/failure, reason).
5. Create admin audit query endpoint — search by user ID, date range, event type; return paginated results (50/page).
6. Add optimistic concurrency handling for concurrent patient creation.

## Current Project State

```text
src/Services/Auth/HealthPlatform.Auth.Api/
├── Controllers/
│   ├── RegistrationController.cs
│   └── AuthController.cs
├── Services/
│   ├── RegistrationService.cs
│   ├── TokenService.cs
│   ├── SessionService.cs
│   └── RateLimitService.cs
├── Data/
│   ├── AuthDbContext.cs
│   └── Entities/
└── Program.cs
```

## Expected Changes

| Action | File Path | Description |
|--------|-----------|-------------|
| CREATE | `src/Services/Auth/HealthPlatform.Auth.Api/Controllers/StaffAccountController.cs` | Staff patient creation endpoint with Staff+ auth |
| CREATE | `src/Services/Auth/HealthPlatform.Auth.Api/Controllers/AuditController.cs` | Admin-only audit query endpoint with pagination |
| CREATE | `src/Services/Auth/HealthPlatform.Auth.Api/Services/StaffAccountService.cs` | Account creation logic with temp password and audit log |
| CREATE | `src/Services/Auth/HealthPlatform.Auth.Api/Services/AuthAuditService.cs` | Immutable event logging (no update/delete) |
| CREATE | `src/Services/Auth/HealthPlatform.Auth.Api/Services/TemporaryPasswordService.cs` | Cryptographic temp password generation |
| CREATE | `src/Services/Auth/HealthPlatform.Auth.Api/Models/StaffCreatePatientRequest.cs` | Staff creation request DTO |
| MODIFY | `src/Services/Auth/HealthPlatform.Auth.Api/Controllers/AuthController.cs` | Add forced password change check on login |

## External References

- [ASP.NET Core 8.0 Authorization](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/roles?view=aspnetcore-8.0)
- [EF Core Optimistic Concurrency](https://learn.microsoft.com/en-us/ef/core/saving/concurrency?tabs=data-annotations)
- [Cryptographic Random Password Generation](https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.randomnumbergenerator)

## Build Commands

- Refer to [.propel/build/](.propel/build/) for applicable technology stack build commands

## Implementation Validation Strategy

- [x] Unit tests pass — account creation, temp password generation, audit logging immutability
- [x] Integration tests pass — staff creates patient; admin queries audit log within 2 seconds

## Implementation Checklist

- [x] Create staff patient account creation endpoint (Staff+ auth, name/phone/DOB, active status, audit logged) → AC-1
- [x] Implement temporary password generation with forced-change flag → AC-2
- [x] Add forced password change check on first login → AC-2
- [x] Implement immutable auth event logging (event_type, user_id, ip, user_agent, timestamp, success/failure) → AC-3
- [x] Create admin audit query endpoint (search by user/date/event, 50/page, within 2 seconds, read-only) → AC-4
- [x] Handle concurrent staff patient creation with optimistic concurrency → Edge case
