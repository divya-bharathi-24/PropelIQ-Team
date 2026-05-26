# Task - task_002_insurance_precheck_backend

## Requirement Reference

- **User Story:** us_013
- **Story Location:** `.propel/context/tasks/EP-003/us_013/us_013.md`
- **Acceptance Criteria:**
  - AC-1: Insurance details capture during booking — server-side validation for insurance provider, policy number, group number, member ID
  - AC-2: Eligibility check returns coverage status — within 5 seconds: "Active - Covered", "Active - Partial Coverage", "Inactive", or "Unable to Verify"; copay estimate if available
  - AC-3: Coverage information displayed before booking confirmation — API returns coverage status and limitations for frontend display
  - AC-4: Insurance details saved to patient profile — verified details saved with "last verified" timestamp; eliminates re-entry
- **Edge Cases:**
  - Verification service timeout → proceed with booking; mark "verification pending"; async retry within 1 hour
  - Primary/secondary insurance → check primary first; secondary if primary denies
  - Insurance expired between verification and appointment → re-verify 24h before; notify patient

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
| Backend Framework | ASP.NET Core Web API | 8.0 | TR-002 — Minimal API for insurance check endpoint |
| Database | PostgreSQL | 16.x | TR-004 — Patient schema for insurance details; dummy records table |
| ORM | Entity Framework Core | 8.0 | TR-004 — Data access for insurance matching and profile save |
| Background Jobs | Hangfire | 1.8.x | TR-007 — Async retry and 24-hour pre-appointment re-verification |

---

## Task Overview

Implement the insurance eligibility check API that matches patient-provided insurance against predefined dummy insurer records, returns coverage status with copay estimate, persists verified details to the patient profile with timestamp, and handles timeout/retry scenarios via Hangfire background jobs. Booking proceeds regardless of insurance result (informational only).

## Dependent Tasks

- task_001_backend_api_scaffolding (US_002) — Backend service projects must exist
- task_001_core_domain_schemas (US_004) — Patient schema with InsuranceDetails must exist

## Impacted Components

- New: `src/Services/Appointment/HealthPlatform.Appointment.Api/Controllers/InsuranceController.cs` — Insurance check endpoint
- New: `src/Services/Appointment/HealthPlatform.Appointment.Api/Services/InsuranceCheckService.cs` — Eligibility matching logic
- New: `src/Services/Appointment/HealthPlatform.Appointment.Api/Jobs/InsuranceReVerifyJob.cs` — 24h pre-appointment re-verify
- New: `src/Services/Appointment/HealthPlatform.Appointment.Api/Models/InsuranceCheckRequest.cs` — Check request DTO
- New: `src/Services/Appointment/HealthPlatform.Appointment.Api/Models/InsuranceCheckResponse.cs` — Coverage result DTO

## Implementation Plan

1. Create insurance eligibility check endpoint accepting provider, policy number, group number, member ID.
2. Implement eligibility matching against predefined dummy insurer records — return Active-Covered, Active-Partial, Inactive, or Unable to Verify.
3. Include copay estimate in response when available from matching records.
4. Save verified insurance details to patient profile with "last verified" timestamp.
5. Implement 5-second timeout with "verification pending" fallback — queue async retry via Hangfire.
6. Create Hangfire job for 24-hour pre-appointment re-verification with patient notification on status change.

## Current Project State

```text
src/Services/Appointment/HealthPlatform.Appointment.Api/
├── Controllers/
│   ├── SlotController.cs
│   ├── BookingController.cs
│   ├── AppointmentController.cs
│   └── DashboardController.cs
├── Services/
│   ├── SlotAvailabilityService.cs
│   ├── BookingService.cs
│   ├── AppointmentManagementService.cs
│   └── ScheduleConstraintService.cs
└── Data/ (AppointmentDbContext, entities)
```

## Expected Changes

| Action | File Path | Description |
|--------|-----------|-------------|
| CREATE | `src/Services/Appointment/HealthPlatform.Appointment.Api/Controllers/InsuranceController.cs` | Insurance eligibility check endpoint |
| CREATE | `src/Services/Appointment/HealthPlatform.Appointment.Api/Services/InsuranceCheckService.cs` | Dummy record matching with copay estimate |
| CREATE | `src/Services/Appointment/HealthPlatform.Appointment.Api/Jobs/InsuranceReVerifyJob.cs` | 24h pre-appointment re-verification Hangfire job |
| CREATE | `src/Services/Appointment/HealthPlatform.Appointment.Api/Models/InsuranceCheckRequest.cs` | Insurance check request DTO |
| CREATE | `src/Services/Appointment/HealthPlatform.Appointment.Api/Models/InsuranceCheckResponse.cs` | Coverage result with status and copay |

## External References

- [ASP.NET Core 8.0 Minimal APIs](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/overview?view=aspnetcore-8.0)
- [Hangfire Recurring Jobs](https://docs.hangfire.io/en/latest/background-methods/performing-recurrent-tasks.html)

## Build Commands

- Refer to [.propel/build/](.propel/build/) for applicable technology stack build commands

## Implementation Validation Strategy

- [x] Unit tests pass — eligibility matching logic, timeout handling, profile save
- [x] Integration tests pass — check matches dummy records; profile saved with timestamp

## Implementation Checklist

- [x] Create insurance eligibility check endpoint with server-side validation → AC-1
- [x] Implement dummy record matching returning coverage status within 5 seconds → AC-2
- [x] Include copay estimate in response when available → AC-2
- [x] Return coverage status and limitations for booking summary display → AC-3
- [x] Save verified insurance details to patient profile with "last verified" timestamp → AC-4
- [x] Implement 5-second timeout with "verification pending" and Hangfire async retry → Edge case
