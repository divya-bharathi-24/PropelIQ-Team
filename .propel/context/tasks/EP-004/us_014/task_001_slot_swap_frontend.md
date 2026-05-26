# Task - task_001_slot_swap_frontend

## Requirement Reference

- **User Story:** us_014
- **Story Location:** `.propel/context/tasks/EP-004/us_014/us_014.md`
- **Acceptance Criteria:**
  - AC-1: Swap request submission — select occupied preferred slot, click "Request Swap"; request created with "pending" status; patient added to waitlist with queue position shown
  - AC-2: Automatic notification when slot becomes available — first patient in waitlist receives push notification and email within 60 seconds; 15-minute acceptance window
  - AC-3: Swap acceptance completes atomic exchange — accept within 15-minute window; existing slot released, preferred slot reserved; confirmation to both parties
  - AC-4: Swap expiry and queue advancement — 15-minute window expires; status to "expired"; next patient notified; slot remains available
  - AC-5: Swap request limit per patient — max 3 concurrent requests; 4th prevented with limit message
- **Edge Cases:**
  - Notification delivery fails → retry via alternative channel; advance queue after 5 minutes
  - Swap for slot <2 hours away → reject with lead time message
  - Both parties cancel simultaneously → both slots released; no swap occurs

---

## Design References

| Reference Type | Value |
|----------------|-------|
| **UI Impact** | Yes |
| **Figma URL** | N/A |
| **Wireframe Status** | N/A |
| **Wireframe Type** | N/A |
| **Wireframe Path/URL** | N/A |
| **Screen Spec** | SCR-SWAP-001 |
| **UXR Requirements** | Swap request as slide-over panel from appointment detail; countdown timer during acceptance window; queue position badge on appointment card |
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
| Frontend Framework | Angular | 17 | TR-001 — Standalone components; real-time countdown and queue updates |
| Frontend UI Library | Angular Material | 17.x | TR-001 — Side-nav, badges, progress bar for countdown |
| Frontend State | RxJS | 7.x | TR-001 — Timer observables for countdown; real-time queue updates |
| Language | TypeScript | 5.x | TR-001 — Type-safe development |

---

## Task Overview

Build the preferred slot swap interface including slide-over panel for swap requests from appointment detail, queue position badge display on appointment cards, 15-minute countdown timer during acceptance window, swap request limit enforcement (max 3), and swap expiry UI with queue advancement indication.

## Dependent Tasks

- task_001_frontend_scaffolding (US_001) — Angular 17 project must exist
- task_001_appointment_mgmt_frontend (US_012) — Appointment card/detail must exist for slide-over integration

## Impacted Components

- New: `src/app/features/appointments/components/swap-panel/swap-panel.component.ts` — Slide-over swap request panel
- New: `src/app/features/appointments/components/swap-panel/swap-panel.component.html` — Swap panel template
- New: `src/app/features/appointments/components/swap-countdown/swap-countdown.component.ts` — 15-minute countdown timer
- New: `src/app/features/appointments/services/swap.service.ts` — Swap request HTTP service
- Modify: `src/app/features/appointments/components/appointment-card/appointment-card.component.ts` — Add queue position badge

## Implementation Plan

1. Create slide-over panel component for swap requests — triggered from appointment detail view.
2. Display preferred slot selection (currently occupied slots) with "Request Swap" button.
3. Show queue position badge on appointment cards with active swap requests.
4. Build 15-minute countdown timer component using RxJS interval/timer operators.
5. Implement swap acceptance UI — accept/decline buttons within countdown window.
6. Display swap request limit message when 3 concurrent requests exist.

## Current Project State

```text
src/app/features/appointments/
├── pages/
│   └── appointment-list/
├── components/
│   ├── appointment-card/
│   ├── reschedule-dialog/
│   └── cancel-dialog/
├── services/
│   └── appointment-mgmt.service.ts
└── appointments.routes.ts
```

## Expected Changes

| Action | File Path | Description |
|--------|-----------|-------------|
| CREATE | `src/app/features/appointments/components/swap-panel/swap-panel.component.ts` | Slide-over panel for swap requests |
| CREATE | `src/app/features/appointments/components/swap-panel/swap-panel.component.html` | Swap panel template with slot selection |
| CREATE | `src/app/features/appointments/components/swap-panel/swap-panel.component.scss` | Swap panel styles |
| CREATE | `src/app/features/appointments/components/swap-countdown/swap-countdown.component.ts` | 15-minute countdown with RxJS timer |
| CREATE | `src/app/features/appointments/components/swap-countdown/swap-countdown.component.html` | Countdown display template |
| CREATE | `src/app/features/appointments/services/swap.service.ts` | Swap request, accept, cancel HTTP service |
| MODIFY | `src/app/features/appointments/components/appointment-card/appointment-card.component.ts` | Add queue position badge |
| MODIFY | `src/app/features/appointments/components/appointment-card/appointment-card.component.html` | Render queue position badge |

## External References

- [Angular Material 17 Sidenav](https://material.angular.io/components/sidenav/overview)
- [Angular Material 17 Badge](https://material.angular.io/components/badge/overview)
- [RxJS Timer/Interval](https://rxjs.dev/api/index/function/timer)

## Build Commands

- Refer to [.propel/build/](.propel/build/) for applicable technology stack build commands

## Implementation Validation Strategy

- [x] Unit tests pass — countdown logic, queue badge rendering, limit enforcement
- [x] Integration tests pass — swap panel opens from appointment; countdown ticks correctly

## Implementation Checklist

- [x] Create slide-over swap request panel from appointment detail with preferred slot selection → AC-1
- [x] Display queue position badge on appointment cards with active swap requests → AC-1
- [x] Implement swap availability notification UI with accept/decline buttons → AC-2, AC-3
- [x] Build 15-minute countdown timer with RxJS and expiry handling → AC-3, AC-4
- [x] Display swap request limit message (max 3 concurrent) → AC-5
- [x] Handle swap expiry with queue advancement feedback → AC-4
