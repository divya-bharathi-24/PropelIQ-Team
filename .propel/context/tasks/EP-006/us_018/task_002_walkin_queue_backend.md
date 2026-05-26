# Task - task_002_walkin_queue_backend

## Requirement Reference

- **User Story:** US_018
- **Story Location:** .propel/context/tasks/EP-006/us_018/us_018.md
- **Acceptance Criteria:**
  - AC-1: Walk-in patient quick registration — create queue entry within 15 seconds, assign queue number
  - AC-2: Real-time queue display — WebSocket broadcast of queue state in priority order
  - AC-3: Queue priority management — urgency change triggers repositioning and wait time recalculation
  - AC-4: Patient called from queue and slot assigned — dynamic slot creation in provider schedule, check-in time recorded
  - AC-5: Estimated wait time calculation — based on rolling 7-day average service time, queue depth, available providers
- **Edge Cases:**
  - Walk-in patient leaves → LWBS status, queue position freed, event logged
  - All providers busy → queue advancement pauses, wait times recalculated
  - Queue > 15 patients → manager alert triggered

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
| Backend | ASP.NET Core Web API | 8.0 | TR-002 — Minimal API for queue management endpoints |
| Backend | SignalR | 8.0 | NFR-006 — Real-time WebSocket for queue state broadcast |
| Backend | Entity Framework Core | 8.0 | TR-004 — Queue entry and walk-in patient persistence |
| Database | PostgreSQL | 16.x | TR-004 — Queue entries and provider schedule storage |

---

## Task Overview

Implement the backend queue management service within the AppointmentService for walk-in patient registration, real-time queue broadcasting via SignalR, priority-based queue ordering, wait time estimation using rolling 7-day averages, and dynamic slot creation when patients are called from the queue. The service manages the full queue lifecycle from registration through service completion or LWBS marking.

## Dependent Tasks

- US_002/task_001 — Backend API scaffolding (solution structure, YARP gateway)
- US_004/task_001 — Core domain schemas (patient, appointment, provider tables)

## Impacted Components

- `src/Services/AppointmentService/Endpoints/QueueEndpoints.cs` — New queue management endpoints
- `src/Services/AppointmentService/Services/QueueManager.cs` — Queue logic and priority ordering
- `src/Services/AppointmentService/Services/WaitTimeCalculator.cs` — Wait time estimation algorithm
- `src/Services/AppointmentService/Hubs/QueueHub.cs` — SignalR hub for real-time queue updates
- `src/Services/AppointmentService/Models/QueueEntry.cs` — Queue entry domain entity

## Implementation Plan

1. Define QueueEntry entity (queueId, patientName, phone, reason, urgency, queueNumber, status, checkInTime, serviceStartTime)
2. Create EF Core migration adding walkin_queue table with urgency enum and auto-incrementing queue numbers
3. Implement QueueManager with add, reorder, callNext, selectPatient, markLWBS, and complete operations
4. Build WaitTimeCalculator using rolling 7-day average service times per provider and current queue depth
5. Create SignalR QueueHub for real-time broadcast of queue state changes to all connected staff clients
6. Build QueueEndpoints: POST /queue/register, PUT /queue/{id}/urgency, POST /queue/call-next, PUT /queue/{id}/lwbs
7. Implement dynamic slot creation in provider schedule when patient is called from queue
8. Add queue overflow alert trigger when queue depth exceeds 15 patients

## Current Project State

```text
src/
├── Services/
│   └── AppointmentService/
│       ├── Endpoints/
│       │   ├── SlotEndpoints.cs
│       │   └── QueueEndpoints.cs           ← NEW
│       ├── Services/
│       │   ├── SlotService.cs
│       │   ├── QueueManager.cs             ← NEW
│       │   └── WaitTimeCalculator.cs       ← NEW
│       ├── Hubs/
│       │   ├── SlotHub.cs
│       │   └── QueueHub.cs                 ← NEW
│       ├── Models/
│       │   └── QueueEntry.cs               ← NEW
│       └── Data/
│           └── AppointmentDbContext.cs
```

## Expected Changes

| Action | File Path | Description |
|--------|-----------|-------------|
| CREATE | src/Services/AppointmentService/Endpoints/QueueEndpoints.cs | POST /queue/register, PUT /queue/{id}/urgency, POST /queue/call-next, PUT /queue/{id}/lwbs |
| CREATE | src/Services/AppointmentService/Services/QueueManager.cs | Queue lifecycle: add, reorder by priority, call-next FIFO within priority, mark LWBS |
| CREATE | src/Services/AppointmentService/Services/WaitTimeCalculator.cs | Rolling 7-day average service time calculation with queue depth and provider availability |
| CREATE | src/Services/AppointmentService/Hubs/QueueHub.cs | SignalR hub broadcasting queue state changes to connected staff clients |
| CREATE | src/Services/AppointmentService/Models/QueueEntry.cs | Queue entry entity with urgency enum, status lifecycle, timestamps |
| MODIFY | src/Services/AppointmentService/Data/AppointmentDbContext.cs | Add QueueEntry DbSet and walkin_queue migration |
| MODIFY | src/ApiGateway/yarp.json | Add route for queue endpoints and SignalR hub |

## External References

- [ASP.NET Core SignalR](https://learn.microsoft.com/en-us/aspnet/core/signalr/introduction?view=aspnetcore-8.0)
- [ASP.NET Core 8 Minimal APIs](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis?view=aspnetcore-8.0)
- [EF Core 8 Migrations](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli)

## Build Commands

- [Refer to applicable technology stack build commands](.propel/build/)

## Implementation Validation Strategy

- [ ] Unit tests pass for QueueManager (priority ordering, FIFO within priority, LWBS)
- [ ] Unit tests pass for WaitTimeCalculator (rolling average, edge cases with no history)
- [ ] Integration tests pass (SignalR hub broadcast, dynamic slot creation, YARP routing)

## Implementation Checklist

- [ ] Define QueueEntry entity and create EF Core migration for walkin_queue table — maps to AC-1
- [ ] Implement queue registration endpoint creating entry within 15 seconds with auto-assigned queue number — maps to AC-1
- [ ] Build SignalR QueueHub broadcasting queue state in priority order to all connected staff — maps to AC-2
- [ ] Implement priority reordering logic (Urgent > High > Medium > Low, FIFO within same priority) — maps to AC-3
- [ ] Build "Call Next" and select-specific-patient endpoints with dynamic slot creation in provider schedule — maps to AC-4
- [ ] Implement WaitTimeCalculator using rolling 7-day average service time per provider and queue depth — maps to AC-5
- [ ] Add LWBS marking endpoint freeing queue position and logging event — maps to edge cases
- [ ] Add queue overflow alert trigger when depth exceeds 15 patients — maps to edge cases
