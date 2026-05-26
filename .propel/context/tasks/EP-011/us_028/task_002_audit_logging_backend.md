# Task - task_002_audit_logging_backend

## Requirement Reference

- **User Story:** US_028
- **Story Location:** .propel/context/tasks/EP-011/us_028/us_028.md
- **Acceptance Criteria:**
  - AC-1: All significant actions generate audit records — immutable record with event_id, actor_id, actor_role, action_type, resource_type, resource_id, timestamp UTC, IP, user agent, success/failure, change details (before/after)
  - AC-2: Audit log search and filtering — search within 3 seconds for up to 1M records, paginated 50/page, sortable, no modify/delete
  - AC-3: Audit log export — CSV/PDF with SHA-256 tamper-evident hash, export timestamp, exporter ID; export itself audit-logged
  - AC-4: Patient data access logging (break-the-glass) — non-primary provider access grants immediately, high-priority event logged, compliance alert, 24h justification required
  - AC-5: Audit log retention and archival — never auto-delete; >12 months compressed to cold storage; active storage for recent 12 months
- **Edge Cases:**
  - Audit service temporarily unavailable → buffer in Redis (max 10K), flush on recovery; if Redis unavailable, log to local file
  - High-volume batch operations → batch writes in groups of 100 with atomicity
  - Export with unauthorized PII → row-level security enforcement

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
| Backend | ASP.NET Core Web API | 8.0 | TR-002 — Minimal API for audit endpoints |
| Backend | Entity Framework Core | 8.0 | TR-004 — Audit log persistence with append-only constraints |
| Backend | Hangfire | 1.8.x | TR-007 — Archival job and buffer flush job |
| Backend | Serilog | 3.x | TR-009 — Structured logging integration for audit events |
| Database | PostgreSQL | 16.x | TR-004 — Immutable audit_log table with partitioning |
| Cache | Upstash Redis | Free tier | TR-005 — Event buffer when DB unavailable |

---

## Task Overview

Implement the comprehensive audit logging backend within the AdminService. This includes an immutable append-only audit log pipeline capturing all significant platform actions, high-performance search with filtering across up to 1M records, CSV/PDF export with SHA-256 tamper-evident hash, break-the-glass emergency access logging with compliance alerts, event buffering in Redis when the database is unavailable, batch write optimization, and automated archival of records older than 12 months to cold storage.

## Dependent Tasks

- US_002/task_001 — Backend API scaffolding (solution structure, YARP gateway)
- US_005/task_001 — Clinical/audit schemas (audit log table)

## Impacted Components

- `src/Services/AdminService/Services/AuditLogService.cs` — New audit log pipeline
- `src/Services/AdminService/Services/AuditExportService.cs` — New CSV/PDF export with hash
- `src/Services/AdminService/Services/BreakTheGlassService.cs` — New emergency access logic
- `src/Services/AdminService/Endpoints/AuditEndpoints.cs` — New audit API endpoints
- `src/Services/AdminService/Jobs/AuditArchivalJob.cs` — New archival Hangfire job
- `src/Services/AdminService/Jobs/AuditBufferFlushJob.cs` — New Redis buffer flush job
- `src/Services/AdminService/Middleware/AuditMiddleware.cs` — New action capture middleware

## Implementation Plan

1. Define AuditLogEntry entity as append-only (no UPDATE/DELETE EF Core operations) with all required fields
2. Create EF Core migration with BRIN index on timestamp for fast range queries on 1M+ records
3. Implement AuditMiddleware capturing significant actions across all service endpoints
4. Build AuditLogService with batch writes (groups of 100), Redis buffer fallback, and local file fallback
5. Implement search/filter endpoint with date range, actor, action type, resource, patient ID; optimized for 3s response
6. Build AuditExportService generating CSV/PDF with SHA-256 tamper-evident hash and row-level security
7. Implement BreakTheGlassService with immediate access grant, high-priority audit event, compliance alert, 24h justification
8. Create AuditArchivalJob (Hangfire, daily) compressing records >12 months to cold storage partition

## Current Project State

```text
src/
├── Services/
│   └── AdminService/
│       ├── Endpoints/
│       │   ├── UserManagementEndpoints.cs
│       │   └── AuditEndpoints.cs           ← NEW
│       ├── Services/
│       │   ├── UserManagementService.cs
│       │   ├── SessionRevocationService.cs
│       │   ├── AuditLogService.cs          ← NEW
│       │   ├── AuditExportService.cs       ← NEW
│       │   └── BreakTheGlassService.cs     ← NEW
│       ├── Jobs/
│       │   ├── AuditArchivalJob.cs         ← NEW
│       │   └── AuditBufferFlushJob.cs      ← NEW
│       ├── Middleware/
│       │   └── AuditMiddleware.cs          ← NEW
│       └── Models/
│           └── AuditLogEntry.cs            ← NEW
```

## Expected Changes

| Action | File Path | Description |
|--------|-----------|-------------|
| CREATE | src/Services/AdminService/Services/AuditLogService.cs | Append-only audit pipeline with batch writes and Redis buffer fallback |
| CREATE | src/Services/AdminService/Services/AuditExportService.cs | CSV/PDF export with SHA-256 hash, export timestamp, exporter ID |
| CREATE | src/Services/AdminService/Services/BreakTheGlassService.cs | Emergency access with audit event, compliance alert, 24h justification |
| CREATE | src/Services/AdminService/Endpoints/AuditEndpoints.cs | GET /audit/logs, POST /audit/export, POST /audit/break-the-glass |
| CREATE | src/Services/AdminService/Jobs/AuditArchivalJob.cs | Daily archival of records >12 months to cold storage partition |
| CREATE | src/Services/AdminService/Jobs/AuditBufferFlushJob.cs | Redis buffer flush to database on recovery |
| CREATE | src/Services/AdminService/Middleware/AuditMiddleware.cs | Action capture middleware for significant operations |
| MODIFY | src/Services/AdminService/Data/AdminDbContext.cs | Add AuditLogEntry DbSet with append-only config and BRIN index |

## External References

- [PostgreSQL BRIN Index](https://www.postgresql.org/docs/16/brin-intro.html)
- [ASP.NET Core Middleware](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-8.0)
- [SHA-256 in .NET](https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.sha256)

## Build Commands

- [Refer to applicable technology stack build commands](.propel/build/)

## Implementation Validation Strategy

- [ ] Unit tests pass for AuditLogService (append-only, batch writes, buffer fallback)
- [ ] Unit tests pass for AuditExportService (CSV/PDF generation, hash computation, row-level security)
- [ ] Integration tests pass (search performance on 1M records, break-the-glass flow, archival)

## Implementation Checklist

- [ ] Define append-only AuditLogEntry entity and create EF Core migration with BRIN index — maps to AC-1
- [ ] Implement AuditMiddleware capturing significant actions with all required fields (event_id through change details) — maps to AC-1
- [ ] Build search/filter endpoint optimized for 3s response on 1M records, paginated at 50/page, sortable — maps to AC-2
- [ ] Implement AuditExportService generating CSV/PDF with SHA-256 hash, export itself audit-logged — maps to AC-3
- [ ] Build BreakTheGlassService with immediate access, high-priority event, compliance alert, 24h justification — maps to AC-4
- [ ] Create AuditArchivalJob compressing records >12 months to cold storage, never auto-deleting — maps to AC-5
- [ ] Add Redis event buffer (max 10K) with flush job and local file fallback when Redis unavailable — maps to edge cases
- [ ] Implement batch writes in groups of 100 with atomicity for high-volume operations — maps to edge cases
