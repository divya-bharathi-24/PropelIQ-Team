# Task - task_002_user_management_backend

## Requirement Reference

- **User Story:** US_027
- **Story Location:** .propel/context/tasks/EP-011/us_027/us_027.md
- **Acceptance Criteria:**
  - AC-1: User list with search, filter, pagination — all users with columns, searchable by name/email, filterable by role/status, 25/page, < 2s
  - AC-2: Create new user with role assignment — account created, welcome email via SendGrid, status pending_activation, audit logged
  - AC-3: Modify user role — effect on next token refresh (15 min), logged with admin ID, previous/new role, reason, user notified
  - AC-4: Deactivate user — status deactivated, all sessions terminated (refresh tokens revoked), data retained per HIPAA
  - AC-5: Bulk user operations — bulk deactivate, role change, CSV export, send notification, each individually audit-logged
- **Edge Cases:**
  - Admin deactivates own account → reject with message
  - Last admin deactivation → reject with message
  - Email exists (deactivated) → return reactivation option

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
| Backend | ASP.NET Core Web API | 8.0 | TR-002 — Minimal API for admin user management endpoints |
| Backend | Entity Framework Core | 8.0 | TR-004 — User entity CRUD operations |
| Backend | SendGrid | Free tier | NFR-012 — Welcome email and notification delivery |
| Database | PostgreSQL | 16.x | TR-004 — User accounts, role assignments, audit trail |
| Cache | Upstash Redis | Free tier | TR-005 — Session/token revocation broadcast |

---

## Task Overview

Implement the admin user management backend within the AdminService. This includes user CRUD with server-side search/filter/pagination, user creation with SendGrid welcome email and temporary password, role modification with audit trail and next-refresh enforcement, account deactivation with immediate session termination (Redis token revocation), bulk operations with individual audit logging, and HIPAA-compliant data retention for deactivated accounts.

## Dependent Tasks

- US_002/task_001 — Backend API scaffolding (solution structure, YARP gateway, JWT)
- US_004/task_001 — Core domain schemas (user, auth tables)

## Impacted Components

- `src/Services/AdminService/` — New microservice project
- `src/Services/AdminService/Endpoints/UserManagementEndpoints.cs` — User management API
- `src/Services/AdminService/Services/UserManagementService.cs` — User CRUD logic
- `src/Services/AdminService/Services/SessionRevocationService.cs` — Token/session termination
- `src/Services/AdminService/Models/` — Admin domain entities

## Implementation Plan

1. Create AdminService microservice project with DI, EF Core, SendGrid, Redis registration
2. Implement UserManagementService with CRUD, search, filter, pagination, and guard validations
3. Build user creation flow with temporary password generation, SendGrid welcome email, and pending_activation status
4. Implement role modification with reason field, audit logging (admin ID, previous/new role), and role claim update
5. Build deactivation flow with session termination via Redis refresh token revocation
6. Implement bulk operations (deactivate, role change, CSV export) with individual audit logging
7. Add self-deactivation guard and last-admin guard at service level

## Current Project State

```text
src/
├── Services/
│   └── AdminService/                       ← NEW
│       ├── Endpoints/
│       │   └── UserManagementEndpoints.cs
│       ├── Services/
│       │   ├── UserManagementService.cs
│       │   └── SessionRevocationService.cs
│       ├── Models/
│       │   └── UserManagement.cs
│       ├── Data/
│       │   └── AdminDbContext.cs
│       └── Program.cs
```

## Expected Changes

| Action | File Path | Description |
|--------|-----------|-------------|
| CREATE | src/Services/AdminService/Program.cs | Service entry with DI for EF Core, SendGrid, Redis |
| CREATE | src/Services/AdminService/Endpoints/UserManagementEndpoints.cs | GET /admin/users, POST /admin/users, PUT /admin/users/{id}/role, PUT /admin/users/{id}/deactivate, POST /admin/users/bulk |
| CREATE | src/Services/AdminService/Services/UserManagementService.cs | User CRUD, search, filter, pagination, guard validations |
| CREATE | src/Services/AdminService/Services/SessionRevocationService.cs | Redis-based refresh token revocation and session termination |
| CREATE | src/Services/AdminService/Models/UserManagement.cs | Admin DTOs and operational models |
| MODIFY | src/ApiGateway/yarp.json | Add route cluster for AdminService |

## External References

- [SendGrid .NET SDK](https://docs.sendgrid.com/for-developers/sending-email/quickstart-dotnet)
- [ASP.NET Core 8 Minimal APIs](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis?view=aspnetcore-8.0)
- [Upstash Redis .NET](https://docs.upstash.com/redis/sdks/dotnet)

## Build Commands

- [Refer to applicable technology stack build commands](.propel/build/)

## Implementation Validation Strategy

- [ ] Unit tests pass for UserManagementService (CRUD, guards, pagination)
- [ ] Unit tests pass for SessionRevocationService (token revocation, session cleanup)
- [ ] Integration tests pass (creation with email, role change audit, deactivation flow, bulk ops)

## Implementation Checklist

- [ ] Create AdminService project with EF Core, SendGrid, and Redis DI registration — maps to AC-1
- [ ] Implement user list endpoint with server-side search, filter by role/status, pagination at 25/page — maps to AC-1
- [ ] Build user creation with temporary password, SendGrid welcome email, pending_activation status, audit log — maps to AC-2
- [ ] Implement role modification with required reason, audit (admin ID, prev/new role), notification to affected user — maps to AC-3
- [ ] Build deactivation with Redis refresh token revocation, immediate session termination, HIPAA data retention — maps to AC-4
- [ ] Implement bulk operations (deactivate, role change, CSV export, notification) with individual audit logging — maps to AC-5
- [ ] Add self-deactivation guard and last-admin guard returning specific rejection messages — maps to edge cases
