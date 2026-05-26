# Task - task_001_insurance_precheck_frontend

## Requirement Reference

- **User Story:** us_013
- **Story Location:** `.propel/context/tasks/EP-003/us_013/us_013.md`
- **Acceptance Criteria:**
  - AC-1: Insurance details capture during booking — form with insurance provider, policy number, group number, member ID with format validation
  - AC-2: Eligibility check returns coverage status — within 5 seconds: "Active - Covered", "Active - Partial Coverage", "Inactive", or "Unable to Verify" with copay estimate
  - AC-3: Coverage information displayed before booking confirmation — status, estimated copay, limitations displayed prominently before "Confirm Booking"
- **Edge Cases:**
  - Verification timeout → proceed with "verification pending"; retry asynchronously
  - Multiple insurance plans → primary and secondary entry; check primary first
  - Insurance expired between verification and appointment → re-verify 24h before; notify if changed

---

## Design References

| Reference Type | Value |
|----------------|-------|
| **UI Impact** | Yes |
| **Figma URL** | N/A |
| **Wireframe Status** | N/A |
| **Wireframe Type** | N/A |
| **Wireframe Path/URL** | N/A |
| **Screen Spec** | SCR-INS-001 |
| **UXR Requirements** | Stepped sub-flow within booking wizard; color-coded badge (green=covered, yellow=partial, red=inactive); tooltip for "Unable to Verify" |
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
| Frontend Framework | Angular | 17 | TR-001 — Standalone components; reactive forms for insurance wizard |
| Frontend UI Library | Angular Material | 17.x | TR-001 — Stepper, badges, tooltips for insurance flow |
| Frontend State | RxJS | 7.x | TR-001 — Async eligibility check response handling |
| Language | TypeScript | 5.x | TR-001 — Type-safe development |

---

## Task Overview

Build the insurance pre-check interface as a stepped sub-flow within the booking wizard. Includes insurance details capture form with format validation, eligibility check result display with color-coded badges, coverage information in booking summary, and support for primary/secondary insurance entry.

## Dependent Tasks

- task_001_frontend_scaffolding (US_001) — Angular 17 project must exist
- task_001_slot_booking_frontend (US_011) — Booking flow must exist for wizard integration

## Impacted Components

- New: `src/app/features/booking/components/insurance-step/insurance-step.component.ts` — Insurance form step
- New: `src/app/features/booking/components/insurance-step/insurance-step.component.html` — Insurance form template
- New: `src/app/features/booking/components/coverage-display/coverage-display.component.ts` — Coverage result badge
- New: `src/app/features/booking/services/insurance.service.ts` — Insurance check HTTP service
- Modify: `src/app/features/booking/pages/booking/booking.component.ts` — Integrate insurance step into booking wizard

## Implementation Plan

1. Create insurance step component with reactive form (provider, policy number, group number, member ID) and format validation per common insurance ID patterns.
2. Implement eligibility check trigger on form completion — display loading state during 5-second check.
3. Create coverage display component with color-coded badge (green=Active-Covered, yellow=Active-Partial, red=Inactive) and tooltip for "Unable to Verify".
4. Integrate coverage status, copay estimate, and limitations into booking summary before "Confirm Booking" button.
5. Add primary/secondary insurance entry toggle.

## Current Project State

```text
src/app/features/booking/
├── pages/
│   └── booking/ (booking.component.ts/html/scss)
├── components/
│   ├── slot-grid/
│   ├── slot-filter/
│   └── booking-confirmation/
├── services/
│   ├── booking.service.ts
│   └── slot-websocket.service.ts
└── booking.routes.ts
```

## Expected Changes

| Action | File Path | Description |
|--------|-----------|-------------|
| CREATE | `src/app/features/booking/components/insurance-step/insurance-step.component.ts` | Insurance form with format validation |
| CREATE | `src/app/features/booking/components/insurance-step/insurance-step.component.html` | Stepped form template |
| CREATE | `src/app/features/booking/components/insurance-step/insurance-step.component.scss` | Insurance form styles |
| CREATE | `src/app/features/booking/components/coverage-display/coverage-display.component.ts` | Color-coded coverage badge with tooltip |
| CREATE | `src/app/features/booking/services/insurance.service.ts` | Insurance eligibility check HTTP service |
| MODIFY | `src/app/features/booking/pages/booking/booking.component.ts` | Integrate insurance step and coverage display |
| MODIFY | `src/app/features/booking/pages/booking/booking.component.html` | Add insurance step to booking wizard |

## External References

- [Angular Material 17 Stepper](https://material.angular.io/components/stepper/overview)
- [Angular Material 17 Badge](https://material.angular.io/components/badge/overview)
- [Angular Material 17 Tooltip](https://material.angular.io/components/tooltip/overview)

## Build Commands

- Refer to [.propel/build/](.propel/build/) for applicable technology stack build commands

## Implementation Validation Strategy

- [x] Unit tests pass — form validation, coverage badge rendering, primary/secondary toggle
- [x] Integration tests pass — insurance step integrates with booking wizard flow

## Implementation Checklist

- [x] Create insurance form with provider, policy, group, member ID and format validation → AC-1
- [x] Implement eligibility check trigger with loading state (5-second timeout) → AC-2
- [x] Create coverage display with color-coded badge (green/yellow/red) and "Unable to Verify" tooltip → AC-2
- [x] Display coverage status and copay estimate in booking summary before "Confirm Booking" → AC-3
- [x] Add primary/secondary insurance entry support → Edge case
