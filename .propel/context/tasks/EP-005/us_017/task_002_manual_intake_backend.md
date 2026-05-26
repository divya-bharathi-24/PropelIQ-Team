# Task - task_002_manual_intake_backend

## Requirement Reference

- **User Story:** US_017
- **Story Location:** .propel/context/tasks/EP-005/us_017/us_017.md
- **Acceptance Criteria:**
  - AC-2: Form supports save and resume — server-side draft persistence, resume from any device
  - AC-4: Submission creates same structured output as AI intake — identical downstream data format
- **Edge Cases:**
  - Patient switches from AI to manual mid-intake → serve AI-captured data for form pre-population
  - Form submission fails due to network error → idempotent submission endpoint prevents duplicates
  - Extremely long free-text entries (>5000 chars) → server-side validation enforces limit

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
| Backend | ASP.NET Core Web API | 8.0 | TR-002 — Minimal API for draft and submission endpoints |
| Backend | Entity Framework Core | 8.0 | TR-004 — Intake draft and submission persistence |
| Database | PostgreSQL | 16.x | TR-004 — JSONB storage for flexible draft data |

---

## Task Overview

Extend the IntakeService with endpoints for manual intake draft persistence, submission processing, and AI-to-manual data bridging. The backend stores form drafts as JSONB, enables cross-device resume, validates submissions server-side, and transforms manual intake data into the same structured format as AI intake for downstream provider views.

## Dependent Tasks

- US_002/task_001 — Backend API scaffolding (solution structure, YARP gateway)
- US_004/task_001 — Core domain schemas (patient, appointment foreign keys)
- US_016/task_002 — AI intake backend (IntakeService project, shared intake schema)

## Impacted Components

- `src/Services/IntakeService/Endpoints/ManualIntakeEndpoints.cs` — New manual intake API endpoints
- `src/Services/IntakeService/Services/ManualIntakeService.cs` — Draft and submission logic
- `src/Services/IntakeService/Models/ManualIntakeDraft.cs` — Draft entity with JSONB payload
- `src/Services/IntakeService/Validators/ManualIntakeValidator.cs` — Server-side validation

## Implementation Plan

1. Define ManualIntakeDraft entity with JSONB payload column for flexible section storage
2. Create EF Core migration adding manual_intake_drafts table
3. Implement ManualIntakeService with saveDraft, loadDraft, submitIntake, and getAiSessionData methods
4. Build ManualIntakeEndpoints: PUT /intake/draft, GET /intake/draft/{appointmentId}, POST /intake/manual/submit
5. Add server-side validation (required fields, character limits, data format) via FluentValidation
6. Implement idempotent submission with idempotency key to prevent duplicate entries on retry

## Current Project State

```text
src/
├── Services/
│   └── IntakeService/
│       ├── Endpoints/
│       │   ├── IntakeEndpoints.cs
│       │   └── ManualIntakeEndpoints.cs    ← NEW
│       ├── Services/
│       │   ├── GeminiIntakeService.cs
│       │   └── ManualIntakeService.cs      ← NEW
│       ├── Models/
│       │   ├── IntakeSession.cs
│       │   └── ManualIntakeDraft.cs        ← NEW
│       ├── Validators/
│       │   └── ManualIntakeValidator.cs    ← NEW
│       └── Data/
│           └── IntakeDbContext.cs
```

## Expected Changes

| Action | File Path | Description |
|--------|-----------|-------------|
| CREATE | src/Services/IntakeService/Endpoints/ManualIntakeEndpoints.cs | PUT /intake/draft, GET /intake/draft/{appointmentId}, POST /intake/manual/submit |
| CREATE | src/Services/IntakeService/Services/ManualIntakeService.cs | Draft save/load, submission processing, AI-to-manual data bridge |
| CREATE | src/Services/IntakeService/Models/ManualIntakeDraft.cs | Draft entity with JSONB payload for flexible section storage |
| CREATE | src/Services/IntakeService/Validators/ManualIntakeValidator.cs | FluentValidation rules for character limits and required fields |
| MODIFY | src/Services/IntakeService/Data/IntakeDbContext.cs | Add ManualIntakeDraft DbSet and migration |

## External References

- [ASP.NET Core 8 Minimal APIs](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis?view=aspnetcore-8.0)
- [EF Core JSONB with PostgreSQL](https://www.npgsql.org/efcore/mapping/json.html)
- [FluentValidation for .NET](https://docs.fluentvalidation.net/en/latest/)

## Build Commands

- [Refer to applicable technology stack build commands](.propel/build/)

## Implementation Validation Strategy

- [ ] Unit tests pass for ManualIntakeService (draft CRUD, submission transformation, idempotency)
- [ ] Unit tests pass for ManualIntakeValidator (required fields, character limits)
- [ ] Integration tests pass (draft save/resume, AI-to-manual bridge, submission format parity)

## Implementation Checklist

- [ ] Define ManualIntakeDraft entity with JSONB payload and create EF Core migration — maps to AC-2
- [ ] Implement draft save/load endpoints (PUT /intake/draft, GET /intake/draft/{appointmentId}) — maps to AC-2
- [ ] Build AI-to-manual data bridge endpoint returning AI session data for form pre-population — maps to edge cases
- [ ] Add server-side validation with FluentValidation (required fields, 5000-char limit) — maps to edge cases
- [ ] Implement idempotent submission endpoint with idempotency key to prevent duplicates — maps to edge cases
- [ ] Transform manual submission into identical structured format as AI intake output — maps to AC-4
