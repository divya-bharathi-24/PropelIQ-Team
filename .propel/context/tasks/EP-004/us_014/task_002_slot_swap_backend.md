# Task - task_002_slot_swap_backend

## Requirement Reference

- **User Story:** us_014
- **Story Location:** `.propel/context/tasks/EP-004/us_014/us_014.md`
- **Acceptance Criteria:**
  - AC-1: Swap request submission — create request with "pending" status; add to FIFO waitlist; return queue position
  - AC-2: Automatic notification when slot becomes available — first patient notified via push + email within 60 seconds; 15-minute acceptance window
  - AC-3: Swap acceptance completes atomic exchange — existing slot released, preferred slot reserved atomically; both appointments updated; confirmation to both parties
  - AC-4: Swap expiry and queue advancement — 15-minute expiry → status "expired"; next patient notified; slot remains available during transitions
  - AC-5: Swap request limit per patient — max 3 concurrent requests; 4th prevented
- **Edge Cases:**
  - Notification delivery fails → retry via alternative channel; advance queue after 5 minutes
  - Both patients cancel simultaneously → both slots released; no swap
  - Swap for slot <2 hours away → reject with lead time message

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
| Backend Framework | ASP.NET Core Web API | 8.0 | TR-002 — Minimal API for swap endpoints |
| Database | PostgreSQL | 16.x | TR-004 — Appointment schema with SlotSwapRequests table |
| ORM | Entity Framework Core | 8.0 | TR-004 — Atomic transactions for swap execution |
| Background Jobs | Hangfire | 1.8.x | TR-007 — Slot monitoring, expiry timers, notification scheduling |
| Email Service | SendGrid | Free tier | NFR-012 — Swap notification emails |
| Cache | Upstash Redis | Free tier | TR-005 — Queue position tracking and real-time swap state |

---

## Task Overview

Implement the preferred slot swap backend including FIFO waitlist queue management, Hangfire-based slot availability monitoring, atomic appointment swap execution, 15-minute acceptance window with expiry and queue advancement, notification delivery via push/email, and concurrent request limit enforcement. Ensures NFR-004 compliance (swap within 60 seconds of slot becoming available).

## Dependent Tasks

- task_002_slot_booking_backend (US_011) — Booking service for slot reservation reuse
- task_001_core_domain_schemas (US_004) — SlotSwapRequests table must exist

## Impacted Components

- New: `src/Services/Appointment/HealthPlatform.Appointment.Api/Controllers/SwapController.cs` — Swap request/accept/cancel endpoints
- New: `src/Services/Appointment/HealthPlatform.Appointment.Api/Services/SwapService.cs` — Swap business logic
- New: `src/Services/Appointment/HealthPlatform.Appointment.Api/Services/SwapQueueService.cs` — FIFO waitlist management
- New: `src/Services/Appointment/HealthPlatform.Appointment.Api/Jobs/SlotMonitorJob.cs` — Monitor preferred slot availability
- New: `src/Services/Appointment/HealthPlatform.Appointment.Api/Jobs/SwapExpiryJob.cs` — 15-minute expiry and queue advancement
- New: `src/Services/Notification/HealthPlatform.Notification.Api/Services/SwapNotificationService.cs` — Swap notification delivery

## Implementation Plan

1. Create swap request endpoint — validate concurrent limit (max 3), validate lead time (≥2 hours), create "pending" request, add to FIFO waitlist.
2. Implement FIFO waitlist queue service using PostgreSQL ordered by registration timestamp.
3. Create Hangfire slot monitoring job — detect preferred slot availability on cancellation/reschedule events; notify first-in-queue within 60 seconds.
4. Implement swap acceptance endpoint — atomic transaction releasing existing slot and reserving preferred slot; update both appointments.
5. Create 15-minute expiry Hangfire job — on timeout, set status "expired", notify next patient in queue.
6. Implement notification delivery (push + email) with retry via alternative channel on failure.
7. Handle concurrent request limit enforcement — reject 4th request with clear error message.
8. Validate 2-hour lead time requirement — reject swap requests for slots less than 2 hours away.

## Current Project State

```text
src/Services/Appointment/HealthPlatform.Appointment.Api/
├── Controllers/
│   ├── SlotController.cs
│   ├── BookingController.cs
│   ├── AppointmentController.cs
│   ├── InsuranceController.cs
│   └── DashboardController.cs
├── Services/
│   ├── SlotAvailabilityService.cs
│   ├── BookingService.cs
│   ├── AppointmentManagementService.cs
│   ├── ScheduleConstraintService.cs
│   └── InsuranceCheckService.cs
├── Hubs/SlotHub.cs
├── Jobs/ (InsuranceReVerifyJob, PdfGenerationJob)
└── Data/ (AppointmentDbContext, entities including SlotSwapRequest)
```

## Expected Changes

| Action | File Path | Description |
|--------|-----------|-------------|
| CREATE | `src/Services/Appointment/HealthPlatform.Appointment.Api/Controllers/SwapController.cs` | Swap request, accept, cancel, status endpoints |
| CREATE | `src/Services/Appointment/HealthPlatform.Appointment.Api/Services/SwapService.cs` | Atomic swap execution and validation |
| CREATE | `src/Services/Appointment/HealthPlatform.Appointment.Api/Services/SwapQueueService.cs` | FIFO waitlist with queue position tracking |
| CREATE | `src/Services/Appointment/HealthPlatform.Appointment.Api/Jobs/SlotMonitorJob.cs` | Preferred slot availability detection |
| CREATE | `src/Services/Appointment/HealthPlatform.Appointment.Api/Jobs/SwapExpiryJob.cs` | 15-minute window expiry and queue advancement |
| CREATE | `src/Services/Notification/HealthPlatform.Notification.Api/Services/SwapNotificationService.cs` | Push + email notification for swap events |
| CREATE | `src/Services/Appointment/HealthPlatform.Appointment.Api/Models/SwapRequest.cs` | Swap request DTO |
| CREATE | `src/Services/Appointment/HealthPlatform.Appointment.Api/Models/SwapAcceptRequest.cs` | Swap acceptance DTO |

## External References

- [Hangfire Background Jobs](https://docs.hangfire.io/en/latest/)
- [EF Core Transactions](https://learn.microsoft.com/en-us/ef/core/saving/transactions)
- [SendGrid C# SDK](https://docs.sendgrid.com/for-developers/sending-email/quickstart-csharp)

## Build Commands

- Refer to [.propel/build/](.propel/build/) for applicable technology stack build commands

## Implementation Validation Strategy

- [x] Unit tests pass — queue ordering, concurrent limit, lead time validation, atomic swap
- [x] Integration tests pass — swap executes within 60s of slot availability; expiry advances queue

## Implementation Checklist

- [x] Create swap request endpoint with FIFO waitlist queue and queue position return → AC-1
- [x] Implement Hangfire slot availability monitoring with notification within 60 seconds → AC-2
- [x] Implement atomic swap acceptance (release old slot, reserve new, update both appointments) → AC-3
- [x] Create 15-minute expiry job with status update and queue advancement → AC-4
- [x] Enforce max 3 concurrent swap requests per patient → AC-5
- [x] Implement notification delivery with retry via alternative channel on failure → Edge case
- [x] Reject swap requests for slots less than 2 hours away → Edge case
- [x] Handle simultaneous cancellation by both parties (release both slots, no swap) → Edge case
