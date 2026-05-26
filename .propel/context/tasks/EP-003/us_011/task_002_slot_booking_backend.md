# Task - task_002_slot_booking_backend

## Requirement Reference

- **User Story:** us_011
- **Story Location:** `.propel/context/tasks/EP-003/us_011/us_011.md`
- **Acceptance Criteria:**
  - AC-1: Slot availability displayed in real-time — slots served within 2 seconds from Redis cache, grouped by date, refreshing via WebSocket push
  - AC-2: Filter by provider, specialty, and date — query logic returns matching slots within 1 second
  - AC-3: Appointment booking with instant confirmation — slot reserved atomically, appointment created with "confirmed" status, response within 3 seconds
  - AC-4: Optimistic locking prevents double-booking — first request succeeds, second gets "slot no longer available" within 2 seconds, auto-refresh
  - AC-5: Booking respects provider schedule constraints — only working hours, 10-min buffer, max daily limit
- **Edge Cases:**
  - Redis stale data → WebSocket pushes invalidation event
  - Patient books then navigates away → booking persists; email sent regardless
  - Provider schedule changed while viewing → "schedule updated" error; force refresh

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
| Backend Framework | ASP.NET Core Web API | 8.0 | TR-002 — Minimal API for slot and booking endpoints |
| Database | PostgreSQL | 16.x | TR-004 — Appointment schema with optimistic locking |
| ORM | Entity Framework Core | 8.0 | TR-004 — Concurrency tokens for optimistic locking |
| Cache | Upstash Redis | Free tier | TR-005 — Slot availability caching with 30-second TTL |
| Real-time | ASP.NET Core SignalR | 8.0 | NFR-006 — WebSocket hub for real-time slot availability updates |

---

## Task Overview

Implement the slot availability API with Redis-cached responses, appointment booking endpoint with atomic slot reservation and optimistic locking, SignalR WebSocket hub for real-time slot availability push notifications, and provider schedule constraint enforcement. Ensures double-booking prevention through EF Core concurrency tokens and provides sub-2-second slot availability responses.

## Dependent Tasks

- task_001_backend_api_scaffolding (US_002) — Backend service projects must exist
- task_001_core_domain_schemas (US_004) — Appointment schema must exist
- task_001_data_operations_compliance (US_006) — Redis caching layer must be configured

## Impacted Components

- New: `src/Services/Appointment/HealthPlatform.Appointment.Api/Controllers/SlotController.cs` — Slot availability and filter endpoints
- New: `src/Services/Appointment/HealthPlatform.Appointment.Api/Controllers/BookingController.cs` — Booking endpoints
- New: `src/Services/Appointment/HealthPlatform.Appointment.Api/Services/SlotAvailabilityService.cs` — Slot query with Redis cache
- New: `src/Services/Appointment/HealthPlatform.Appointment.Api/Services/BookingService.cs` — Atomic booking with optimistic locking
- New: `src/Services/Appointment/HealthPlatform.Appointment.Api/Hubs/SlotHub.cs` — SignalR WebSocket hub
- New: `src/Services/Appointment/HealthPlatform.Appointment.Api/Services/ScheduleConstraintService.cs` — Provider schedule validation

## Implementation Plan

1. Create slot availability API serving data from Redis cache (30-second TTL) with PostgreSQL fallback.
2. Implement provider/specialty/date filter query logic with composite indexes for sub-1-second response.
3. Create booking endpoint with atomic slot reservation — use EF Core concurrency tokens (RowVersion) for optimistic locking.
4. Implement double-booking prevention — catch DbUpdateConcurrencyException, return "slot no longer available".
5. Create SignalR hub for real-time slot availability push — broadcast on booking/cancellation events.
6. Implement provider schedule constraint enforcement — validate working hours, 10-minute buffer, max daily appointments.
7. Configure Redis cache invalidation on booking — invalidate provider-specific slot keys on any mutation.
8. Handle stale data scenario — WebSocket invalidation event triggers client refresh.

## Current Project State

```text
src/Services/Appointment/HealthPlatform.Appointment.Api/
├── Controllers/
│   └── DashboardController.cs
├── Services/
│   └── DashboardService.cs
├── Caching/
│   └── SlotCacheInvalidator.cs
├── Data/
│   ├── AppointmentDbContext.cs
│   └── Entities/ (Appointment, TimeSlot, ProviderSchedule, SlotSwapRequest)
└── Program.cs
```

## Expected Changes

| Action | File Path | Description |
|--------|-----------|-------------|
| CREATE | `src/Services/Appointment/HealthPlatform.Appointment.Api/Controllers/SlotController.cs` | Slot availability with filter API |
| CREATE | `src/Services/Appointment/HealthPlatform.Appointment.Api/Controllers/BookingController.cs` | Booking endpoint with atomic reservation |
| CREATE | `src/Services/Appointment/HealthPlatform.Appointment.Api/Services/SlotAvailabilityService.cs` | Redis-cached slot queries with PostgreSQL fallback |
| CREATE | `src/Services/Appointment/HealthPlatform.Appointment.Api/Services/BookingService.cs` | Atomic booking with optimistic locking |
| CREATE | `src/Services/Appointment/HealthPlatform.Appointment.Api/Services/ScheduleConstraintService.cs` | Working hours, buffer, max daily validation |
| CREATE | `src/Services/Appointment/HealthPlatform.Appointment.Api/Hubs/SlotHub.cs` | SignalR hub for real-time slot updates |
| CREATE | `src/Services/Appointment/HealthPlatform.Appointment.Api/Models/BookingRequest.cs` | Booking request DTO |
| CREATE | `src/Services/Appointment/HealthPlatform.Appointment.Api/Models/SlotFilterRequest.cs` | Filter parameters DTO |

## External References

- [ASP.NET Core SignalR](https://learn.microsoft.com/en-us/aspnet/core/signalr/introduction?view=aspnetcore-8.0)
- [EF Core Optimistic Concurrency](https://learn.microsoft.com/en-us/ef/core/saving/concurrency?tabs=data-annotations)
- [Upstash Redis Cache-Aside Pattern](https://upstash.com/docs/redis/overall/getstarted)

## Build Commands

- Refer to [.propel/build/](.propel/build/) for applicable technology stack build commands

## Implementation Validation Strategy

- [x] Unit tests pass — optimistic locking, schedule constraints, cache invalidation
- [x] Integration tests pass — concurrent booking attempts; first succeeds, second gets conflict

## Implementation Checklist

- [x] Create slot availability API with Redis cache (30-second TTL) serving slots within 2 seconds → AC-1
- [x] Implement provider/specialty/date filter query with sub-1-second response → AC-2
- [x] Create booking endpoint with atomic slot reservation and "confirmed" status → AC-3
- [x] Implement optimistic locking with concurrency tokens for double-booking prevention → AC-4
- [x] Create SignalR WebSocket hub broadcasting slot changes on booking/cancellation → AC-1
- [x] Implement provider schedule constraint validation (working hours, 10-min buffer, max daily) → AC-5
- [x] Configure Redis cache invalidation on booking mutations → AC-1
- [x] Return "slot no longer available" on concurrency conflict with auto-refresh signal → AC-4
