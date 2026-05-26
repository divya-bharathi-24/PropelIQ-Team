# Task - task_001_appointment_mgmt_frontend

## Requirement Reference

- **User Story:** us_012
- **Story Location:** `.propel/context/tasks/EP-003/us_012/us_012.md`
- **Acceptance Criteria:**
  - AC-1: Patient appointment list view — all appointments with status badges (confirmed/completed/cancelled/no-show), date/time, provider, type; sortable by date; paginated (10/page)
  - AC-2: Appointment rescheduling — click "Reschedule", select new slot; original released, new reserved; confirmation email sent
  - AC-3: Appointment cancellation with policy enforcement — >24h: immediate; <24h: warning about no-show impact; slot released
  - AC-4: PDF confirmation generation — click "Download Confirmation"; PDF with details, QR code; downloadable within 3 seconds
- **Edge Cases:**
  - Reschedule <24h → allow with warning "Late reschedule may affect reliability score"
  - PDF service unavailable → "PDF will be available shortly" message
  - 3+ cancellations/day → message "Maximum cancellations reached"

---

## Design References

| Reference Type | Value |
|----------------|-------|
| **UI Impact** | Yes |
| **Figma URL** | N/A |
| **Wireframe Status** | N/A |
| **Wireframe Type** | N/A |
| **Wireframe Path/URL** | N/A |
| **Screen Spec** | SCR-APPT-001, SCR-APPT-002 |
| **UXR Requirements** | Appointment cards with status badges; swipe actions on mobile (left=cancel, right=reschedule); modal confirmation for destructive actions |
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
| Frontend Framework | Angular | 17 | TR-001 — Standalone components; reactive state for appointment management |
| Frontend UI Library | Angular Material | 17.x | TR-001 — Cards, badges, dialogs, swipe gestures |
| Frontend State | RxJS | 7.x | TR-001 — Reactive appointment list and PDF download streams |
| Language | TypeScript | 5.x | TR-001 — Type-safe development |

---

## Task Overview

Build the appointment management interface with appointment list view (status badges, sortable, paginated), reschedule flow with slot selection, cancellation with 24-hour policy enforcement and confirmation modal, and PDF confirmation download. Mobile-responsive with swipe actions for quick cancel/reschedule gestures.

## Dependent Tasks

- task_001_frontend_scaffolding (US_001) — Angular 17 project must exist
- task_001_slot_booking_frontend (US_011) — Booking UI components exist for slot selection reuse

## Impacted Components

- New: `src/app/features/appointments/pages/appointment-list/appointment-list.component.ts` — Appointment list with pagination
- New: `src/app/features/appointments/components/appointment-card/appointment-card.component.ts` — Card with status badge and swipe
- New: `src/app/features/appointments/components/reschedule-dialog/reschedule-dialog.component.ts` — Reschedule slot picker dialog
- New: `src/app/features/appointments/components/cancel-dialog/cancel-dialog.component.ts` — Cancel confirmation modal
- New: `src/app/features/appointments/services/appointment-mgmt.service.ts` — Appointment management HTTP service

## Implementation Plan

1. Create appointment list page with paginated view (10/page), sortable by date, with status badge rendering.
2. Build appointment card component with status badges (confirmed=green, completed=blue, cancelled=grey, no-show=red).
3. Implement mobile swipe actions (left=cancel, right=reschedule) using touch event handlers.
4. Create reschedule dialog reusing slot selection components — show 24-hour warning for late reschedules.
5. Create cancel confirmation dialog with 24-hour policy enforcement — warning for <24h cancellations.
6. Implement PDF download button triggering backend PDF generation endpoint with 3-second loading state.

## Current Project State

```text
src/app/features/
├── auth/ (login, registration)
├── staff/ (create-patient)
├── admin/ (audit-log)
├── patient/ (profile, dashboard)
└── booking/ (booking page, slot grid, confirmation)
```

## Expected Changes

| Action | File Path | Description |
|--------|-----------|-------------|
| CREATE | `src/app/features/appointments/pages/appointment-list/appointment-list.component.ts` | Paginated list with sort and status badges |
| CREATE | `src/app/features/appointments/pages/appointment-list/appointment-list.component.html` | Appointment list template |
| CREATE | `src/app/features/appointments/pages/appointment-list/appointment-list.component.scss` | List styles with responsive layout |
| CREATE | `src/app/features/appointments/components/appointment-card/appointment-card.component.ts` | Card with swipe actions |
| CREATE | `src/app/features/appointments/components/reschedule-dialog/reschedule-dialog.component.ts` | Slot picker dialog for rescheduling |
| CREATE | `src/app/features/appointments/components/cancel-dialog/cancel-dialog.component.ts` | Cancellation confirmation modal |
| CREATE | `src/app/features/appointments/services/appointment-mgmt.service.ts` | List, reschedule, cancel, PDF download API calls |
| CREATE | `src/app/features/appointments/appointments.routes.ts` | Appointments feature lazy-loaded routes |
| MODIFY | `src/app/app.routes.ts` | Add lazy-loaded route for appointments feature |

## External References

- [Angular Material 17 Dialog](https://material.angular.io/components/dialog/overview)
- [Angular Material 17 Paginator](https://material.angular.io/components/paginator/overview)
- [Angular Material 17 Badge](https://material.angular.io/components/badge/overview)
- [Touch Events API](https://developer.mozilla.org/en-US/docs/Web/API/Touch_events)

## Build Commands

- Refer to [.propel/build/](.propel/build/) for applicable technology stack build commands

## Implementation Validation Strategy

- [x] Unit tests pass — pagination, sort, swipe detection, policy enforcement logic
- [x] Integration tests pass — reschedule/cancel flows complete with correct state transitions

## Implementation Checklist

- [x] Create appointment list page with status badges, date sort, and pagination (10/page) → AC-1
- [x] Implement reschedule flow with slot selection dialog and 24-hour late-reschedule warning → AC-2
- [x] Create cancellation modal with 24-hour policy enforcement and no-show impact warning → AC-3
- [x] Implement PDF confirmation download button with 3-second loading state → AC-4
- [x] Add mobile swipe actions (left=cancel, right=reschedule) on appointment cards → UXR
- [x] Display "PDF will be available shortly" on service unavailability → Edge case
