# Task - task_001_slot_booking_frontend

## Requirement Reference

- **User Story:** us_011
- **Story Location:** `.propel/context/tasks/EP-003/us_011/us_011.md`
- **Acceptance Criteria:**
  - AC-1: Slot availability displayed in real-time — available slots within 2 seconds, grouped by date, provider/time/duration/capacity, refreshing every 30 seconds via WebSocket
  - AC-2: Filter by provider, specialty, and date — results update within 1 second; empty results show "No available slots" with suggested alternative dates
  - AC-3: Appointment booking with instant confirmation — select slot, confirm, confirmation displayed within 3 seconds
  - AC-4: Optimistic locking prevents double-booking — second user sees "slot no longer available" within 2 seconds; slot list refreshes automatically
- **Edge Cases:**
  - Redis cache stale → WebSocket pushes invalidation; UI shows "verifying availability"
  - Patient books then navigates away → booking persists
  - Provider schedule changed while viewing → "schedule updated" error; force refresh

---

## Design References

| Reference Type | Value |
|----------------|-------|
| **UI Impact** | Yes |
| **Figma URL** | N/A |
| **Wireframe Status** | N/A |
| **Wireframe Type** | N/A |
| **Wireframe Path/URL** | N/A |
| **Screen Spec** | SCR-BOOK-001 |
| **UXR Requirements** | Calendar view with day/week toggle; slot chips color-coded (green=available, yellow=limited, grey=booked); mobile-first with bottom sheet for booking confirmation |
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
| Frontend Framework | Angular | 17 | TR-001 — Standalone components; reactive updates for real-time slots |
| Frontend UI Library | Angular Material | 17.x | TR-001 — Date picker, chips, bottom sheet components |
| Frontend State | RxJS | 7.x | TR-001 — WebSocket subscription and reactive filter updates |
| Language | TypeScript | 5.x | TR-001 — Type-safe development |

---

## Task Overview

Build the appointment booking page with real-time slot availability display via WebSocket, provider/specialty/date filters, day/week calendar toggle, color-coded slot chips, mobile-first booking confirmation bottom sheet, and automatic slot list refresh on concurrent booking conflicts. The UI provides instant visual feedback for slot availability changes.

## Dependent Tasks

- task_001_frontend_scaffolding (US_001) — Angular 17 project must exist

## Impacted Components

- New: `src/app/features/booking/pages/booking/booking.component.ts` — Booking page with calendar and filters
- New: `src/app/features/booking/pages/booking/booking.component.html` — Booking template
- New: `src/app/features/booking/components/slot-grid/slot-grid.component.ts` — Slot display grid with chips
- New: `src/app/features/booking/components/booking-confirmation/booking-confirmation.component.ts` — Bottom sheet confirmation
- New: `src/app/features/booking/components/slot-filter/slot-filter.component.ts` — Provider/specialty/date filter panel
- New: `src/app/features/booking/services/booking.service.ts` — Booking API service
- New: `src/app/features/booking/services/slot-websocket.service.ts` — WebSocket subscription for real-time updates

## Implementation Plan

1. Create booking page with day/week calendar toggle and filter panel (provider dropdown, specialty dropdown, date range picker).
2. Build slot grid component with color-coded chips (green=available, yellow=limited, grey=booked), grouped by date.
3. Implement WebSocket service subscribing to slot availability channel with 30-second refresh fallback.
4. Create booking confirmation bottom sheet (mobile-first) with slot details and confirm button.
5. Handle "slot no longer available" response — display message, auto-refresh slot list within 2 seconds.
6. Implement "verifying availability" loading state during booking attempt.
7. Display "No available slots" with suggested alternative dates on empty filter results.

## Current Project State

```text
src/app/
├── app.component.ts
├── app.routes.ts
├── core/
├── shared/
└── features/
    ├── auth/ (login, registration)
    ├── staff/ (create-patient)
    ├── admin/ (audit-log)
    └── patient/ (profile, dashboard)
```

## Expected Changes

| Action | File Path | Description |
|--------|-----------|-------------|
| CREATE | `src/app/features/booking/pages/booking/booking.component.ts` | Booking page with calendar toggle and filter integration |
| CREATE | `src/app/features/booking/pages/booking/booking.component.html` | Booking template with slot grid and filter panel |
| CREATE | `src/app/features/booking/pages/booking/booking.component.scss` | Booking page styles |
| CREATE | `src/app/features/booking/components/slot-grid/slot-grid.component.ts` | Color-coded slot chips grouped by date |
| CREATE | `src/app/features/booking/components/slot-filter/slot-filter.component.ts` | Provider/specialty/date filter panel |
| CREATE | `src/app/features/booking/components/booking-confirmation/booking-confirmation.component.ts` | Bottom sheet booking confirmation |
| CREATE | `src/app/features/booking/services/booking.service.ts` | Booking HTTP service |
| CREATE | `src/app/features/booking/services/slot-websocket.service.ts` | WebSocket subscription for real-time slot updates |
| CREATE | `src/app/features/booking/booking.routes.ts` | Booking feature lazy-loaded routes |
| MODIFY | `src/app/app.routes.ts` | Add lazy-loaded route for booking feature |

## External References

- [Angular Material 17 Datepicker](https://material.angular.io/components/datepicker/overview)
- [Angular Material 17 Chips](https://material.angular.io/components/chips/overview)
- [Angular Material 17 Bottom Sheet](https://material.angular.io/components/bottom-sheet/overview)
- [RxJS WebSocket](https://rxjs.dev/api/webSocket/webSocket)

## Build Commands

- Refer to [.propel/build/](.propel/build/) for applicable technology stack build commands

## Implementation Validation Strategy

- [x] Unit tests pass — filter logic, slot display logic, WebSocket reconnection
- [x] Integration tests pass — booking flow from slot selection to confirmation

## Implementation Checklist

- [x] Create booking page with day/week calendar toggle and WebSocket-driven real-time slot display → AC-1
- [x] Implement provider/specialty/date filter panel with results updating within 1 second → AC-2
- [x] Create slot grid with color-coded chips (green/yellow/grey) grouped by date → AC-1
- [x] Create booking confirmation bottom sheet (mobile-first) with instant confirmation display → AC-3
- [x] Handle "slot no longer available" with message display and automatic slot list refresh → AC-4
- [x] Add "verifying availability" loading state during booking → AC-4 (edge case)
- [x] Display "No available slots" with suggested alternative dates on empty results → AC-2 (edge case)
