# Task - task_002_verification_workflow_backend

## Requirement Reference

- **User Story:** US_025
- **Story Location:** .propel/context/tasks/EP-009/us_025/us_025.md
- **Acceptance Criteria:**
  - AC-1: Verification queue for AI-generated data — list pending items with metadata, sorted by confidence
  - AC-2: Side-by-side source and suggestion view — serve original source data alongside AI extraction
  - AC-3: Approve, reject, or modify workflow — persist actions with staff ID and timestamp audit
  - AC-4: Verification SLA tracking — >24h escalation with notification, >48h auto-assignment
- **Edge Cases:**
  - Staff verifies then patient uploads contradicting document → re-trigger conflict detection
  - Bulk verification (100+ items) → batch approve endpoint with individual audit records
  - Staff modifies creating new conflict → run conflict detection on modified data before persisting

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
| Backend | ASP.NET Core Web API | 8.0 | TR-002 — Minimal API for verification endpoints |
| Backend | Hangfire | 1.8.x | TR-007 — SLA monitoring and auto-assignment job |
| Backend | Entity Framework Core | 8.0 | TR-004 — Verification queue and action persistence |
| Database | PostgreSQL | 16.x | TR-004 — Verification items, action history |

---

## Task Overview

Implement the verification workflow backend within the ClinicalIntelligence service. This includes a verification queue API serving pending AI-generated items sorted by confidence, source-vs-suggestion data assembly for comparison, approve/reject/modify action endpoints with immutable audit logging, SLA monitoring via Hangfire (24h escalation, 48h auto-assignment), batch approve with individual audit records, and conflict detection integration for modified data.

## Dependent Tasks

- US_002/task_001 — Backend API scaffolding (solution structure, YARP gateway)
- US_004/task_001 — Core domain schemas (patient tables)
- US_005/task_001 — Clinical/audit schemas (extracted data, audit tables)

## Impacted Components

- `src/Services/ClinicalIntelligenceService/Services/VerificationQueueService.cs` — New queue management
- `src/Services/ClinicalIntelligenceService/Endpoints/VerificationEndpoints.cs` — New verification API
- `src/Services/ClinicalIntelligenceService/Jobs/SlaMonitoringJob.cs` — New SLA check job
- `src/Services/ClinicalIntelligenceService/Models/VerificationItem.cs` — Verification entity

## Implementation Plan

1. Define VerificationItem entity (itemId, patientId, dataType, aiData, sourceData, confidence, status, createdAt, assignedTo)
2. Create EF Core migration adding verification_items table
3. Implement VerificationQueueService with listing (sorted by confidence), action persistence, and batch operations
4. Build VerificationEndpoints: GET /verification/queue, GET /verification/{id}, POST /verification/{id}/approve|reject|modify, POST /verification/batch-approve
5. Create SlaMonitoringJob (Hangfire, hourly) checking for >24h items and >48h auto-assignment
6. Add conflict detection hook for modified data before persistence

## Current Project State

```text
src/
├── Services/
│   └── ClinicalIntelligenceService/
│       ├── Endpoints/
│       │   ├── PatientViewEndpoints.cs
│       │   ├── ConflictEndpoints.cs
│       │   └── VerificationEndpoints.cs    ← NEW
│       ├── Services/
│       │   ├── PatientAggregationService.cs
│       │   ├── ConflictDetectionService.cs
│       │   ├── ConflictResolutionService.cs
│       │   └── VerificationQueueService.cs ← NEW
│       ├── Jobs/
│       │   └── SlaMonitoringJob.cs         ← NEW
│       └── Models/
│           └── VerificationItem.cs         ← NEW
```

## Expected Changes

| Action | File Path | Description |
|--------|-----------|-------------|
| CREATE | src/Services/ClinicalIntelligenceService/Services/VerificationQueueService.cs | Queue listing, action persistence, batch approve, SLA checks |
| CREATE | src/Services/ClinicalIntelligenceService/Endpoints/VerificationEndpoints.cs | GET queue, POST approve/reject/modify, POST batch-approve |
| CREATE | src/Services/ClinicalIntelligenceService/Jobs/SlaMonitoringJob.cs | Hangfire hourly job for 24h escalation and 48h auto-assignment |
| CREATE | src/Services/ClinicalIntelligenceService/Models/VerificationItem.cs | Verification entity with confidence, status, assignment tracking |
| MODIFY | src/Services/ClinicalIntelligenceService/Data/ClinicalIntelligenceDbContext.cs | Add VerificationItem DbSet and migration |

## External References

- [ASP.NET Core 8 Minimal APIs](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis?view=aspnetcore-8.0)
- [Hangfire Recurring Jobs](https://docs.hangfire.io/en/latest/background-methods/performing-recurrent-tasks.html)

## Build Commands

- [Refer to applicable technology stack build commands](.propel/build/)

## Implementation Validation Strategy

- [ ] Unit tests pass for VerificationQueueService (listing, actions, batch, SLA logic)
- [ ] Unit tests pass for SlaMonitoringJob (escalation at 24h, auto-assignment at 48h)
- [ ] Integration tests pass (full approve/reject/modify cycle, audit logging, conflict re-check)

## Implementation Checklist

- [ ] Define VerificationItem entity and create EF Core migration for verification_items table — maps to AC-1
- [ ] Implement queue listing endpoint sorted by confidence (lowest first) with patient metadata — maps to AC-1
- [ ] Build source-vs-suggestion data assembly endpoint for comparison view — maps to AC-2
- [ ] Implement approve, reject (with reason), and modify action endpoints with immutable audit logging — maps to AC-3
- [ ] Create SlaMonitoringJob (Hangfire, hourly) for 24h escalation and 48h auto-assignment — maps to AC-4
- [ ] Add batch approve endpoint for items with confidence > 0.95 with individual audit records — maps to edge cases
