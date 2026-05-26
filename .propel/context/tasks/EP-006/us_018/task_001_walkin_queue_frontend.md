# Task - task_001_walkin_queue_frontend

## Requirement Reference

- **User Story:** US_018
- **Story Location:** .propel/context/tasks/EP-006/us_018/us_018.md
- **Acceptance Criteria:**
  - AC-1: Walk-in patient quick registration — streamlined form (name, phone, reason, urgency), queue entry within 15 seconds, queue number assigned
  - AC-2: Real-time queue display — patients in priority order with queue number, name, reason, urgency, estimated wait, time-in-queue; updates via WebSocket
  - AC-3: Queue priority management — change urgency level, patient repositioned, wait times recalculate
  - AC-4: Patient called from queue and slot assigned — "Call Next" or select specific patient, status changes to "In Service"
  - AC-5: Estimated wait time calculation — based on average service time, queue depth, available providers
- **Edge Cases:**
  - Walk-in patient leaves without being seen → mark as LWBS, free queue position
  - All providers busy → queue pauses, wait times increase, show provider schedule gaps
  - Queue grows beyond 15 patients → alert manager with capacity recommendation

---

## Design References

| Reference Type | Value |
|----------------|-------|
| **UI Impact** | Yes |
| **Figma URL** | N/A |
| **Wireframe Status** | N/A |
| **Wireframe Type** | N/A |
| **Wireframe Path/URL** | N/A |
| **Screen Spec** | SCR-QUEUE-001 |
| **UXR Requirements** | Kanban-style board (Waiting → In Service → Completed); drag-and-drop for priority reorder; urgency color coding (red=urgent, orange=high, yellow=medium, blue=low); large timer display showing current wait |
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
| Frontend | Angular | 17 | TR-001 — Standalone components for queue management module |
| Frontend | Angular Material | 17.x | TR-001 — Drag-drop CDK, form fields, dialogs |
| Frontend | RxJS | 7.x | TR-001 — WebSocket stream for real-time queue updates |
| Frontend | TypeScript | 5.x | TR-001 — Type-safe queue entry models |

---

## Task Overview

Implement the staff-facing walk-in queue management interface as a kanban-style board with three columns (Waiting, In Service, Completed). The UI features a quick registration form completing in under 15 seconds, real-time WebSocket updates for queue state, drag-and-drop priority reordering, urgency color coding, estimated wait time display, and "Call Next" functionality for assigning patients to providers.

## Dependent Tasks

- US_001/task_001 — Frontend scaffolding (Angular project, routing, shared module)

## Impacted Components

- `src/app/features/queue/` — New queue management feature module
- `src/app/features/queue/queue-board/` — Kanban board component
- `src/app/features/queue/walkin-registration/` — Quick registration form component
- `src/app/features/queue/services/queue.service.ts` — Queue API and WebSocket service
- `src/app/features/queue/models/queue.model.ts` — Queue entry interfaces

## Implementation Plan

1. Create queue feature module with lazy-loaded route configuration
2. Define TypeScript interfaces for QueueEntry, QueueStatus, UrgencyLevel, and WaitTimeEstimate
3. Implement QueueService with REST API methods and WebSocket connection for real-time updates
4. Build WalkinRegistrationComponent with streamlined form (name, phone, reason, urgency) optimized for 15-second completion
5. Build QueueBoardComponent as kanban board with Waiting/In Service/Completed columns
6. Add drag-and-drop priority reordering using Angular CDK DragDrop with urgency color coding
7. Implement "Call Next" button and patient selection with status transition to "In Service"
8. Add estimated wait time display with large timer and queue overflow alert (>15 patients)

## Current Project State

```text
src/
├── app/
│   └── features/
│       └── queue/                          ← NEW
│           ├── queue-board/
│           │   ├── queue-board.component.ts
│           │   ├── queue-board.component.html
│           │   └── queue-board.component.scss
│           ├── walkin-registration/
│           │   ├── walkin-registration.component.ts
│           │   ├── walkin-registration.component.html
│           │   └── walkin-registration.component.scss
│           ├── services/
│           │   └── queue.service.ts
│           └── models/
│               └── queue.model.ts
```

## Expected Changes

| Action | File Path | Description |
|--------|-----------|-------------|
| CREATE | src/app/features/queue/models/queue.model.ts | QueueEntry, UrgencyLevel, WaitTimeEstimate interfaces |
| CREATE | src/app/features/queue/services/queue.service.ts | REST API + WebSocket service for queue operations |
| CREATE | src/app/features/queue/walkin-registration/walkin-registration.component.ts | Quick registration form with urgency selector |
| CREATE | src/app/features/queue/queue-board/queue-board.component.ts | Kanban board with drag-drop, call-next, LWBS marking |
| CREATE | src/app/features/queue/queue-board/queue-board.component.html | Three-column kanban layout with urgency color coding |
| CREATE | src/app/features/queue/queue-board/queue-board.component.scss | Kanban styles, urgency colors, timer display |
| MODIFY | src/app/app.routes.ts | Add lazy-loaded route for queue feature |

## External References

- [Angular CDK Drag and Drop](https://material.angular.io/cdk/drag-drop/overview)
- [Angular Material 17 Components](https://material.angular.io/components/categories)
- [RxJS WebSocket](https://rxjs.dev/api/webSocket/webSocket)

## Build Commands

- [Refer to applicable technology stack build commands](.propel/build/)

## Implementation Validation Strategy

- [ ] Unit tests pass for QueueService (REST calls, WebSocket message handling)
- [ ] Unit tests pass for QueueBoardComponent (drag-drop reorder, call-next, LWBS marking)
- [ ] Integration tests pass (route lazy-loading, real-time WebSocket updates)

## Implementation Checklist

- [ ] Create queue feature module with lazy-loaded route and queue entry interfaces — maps to AC-1
- [ ] Build WalkinRegistrationComponent with streamlined form optimized for 15-second completion — maps to AC-1
- [ ] Implement QueueService with REST API and WebSocket connection for real-time queue updates — maps to AC-2
- [ ] Build QueueBoardComponent kanban board with Waiting/In Service/Completed columns and urgency color coding — maps to AC-2
- [ ] Add drag-and-drop priority reordering with automatic wait time recalculation display — maps to AC-3
- [ ] Implement "Call Next" button and specific patient selection with In Service transition — maps to AC-4
- [ ] Display estimated wait times per patient with large timer and queue overflow alert at 15+ patients — maps to AC-5
- [ ] Handle LWBS marking and queue position release for patients who leave — maps to edge cases
