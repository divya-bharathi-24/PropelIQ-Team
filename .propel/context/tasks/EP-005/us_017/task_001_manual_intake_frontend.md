# Task - task_001_manual_intake_frontend

## Requirement Reference

- **User Story:** US_017
- **Story Location:** .propel/context/tasks/EP-005/us_017/us_017.md
- **Acceptance Criteria:**
  - AC-1: Multi-section intake form with validation — sections for Chief Complaint, Medical History, Medications, Allergies, Family History, Lifestyle with required field indicators and section-level progress
  - AC-2: Form supports save and resume — auto-save every 30 seconds to local storage and server-side draft, resume on any device
  - AC-3: Conditional fields based on responses — dynamic follow-up fields appear without page reload using Angular reactive forms
  - AC-4: Submission creates same structured output as AI intake — identical data format for downstream provider views
- **Edge Cases:**
  - Patient switches from AI to manual mid-intake → pre-populate form with data captured in AI conversation
  - Form submission fails due to network error → retry with exponential backoff, show "submission pending" status
  - Extremely long free-text entries (>5000 chars) → enforce character limit with counter, truncation warning at 4500

---

## Design References

| Reference Type | Value |
|----------------|-------|
| **UI Impact** | Yes |
| **Figma URL** | N/A |
| **Wireframe Status** | N/A |
| **Wireframe Type** | N/A |
| **Wireframe Path/URL** | N/A |
| **Screen Spec** | SCR-INTAKE-002 |
| **UXR Requirements** | Step wizard with numbered section tabs; progress bar across top; auto-save indicator (subtle green checkmark); mobile-optimized with large touch targets for checkbox groups |
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
| Frontend | Angular | 17 | TR-001 — Standalone components with reactive forms |
| Frontend | Angular Material | 17.x | TR-001 — Stepper, form fields, checkbox components |
| Frontend | RxJS | 7.x | TR-001 — Auto-save observable interval, form value changes |
| Frontend | TypeScript | 5.x | TR-001 — Type-safe form models and validation |

---

## Task Overview

Implement the manual intake form as a multi-section step wizard using Angular 17 reactive forms. The form captures Chief Complaint, Medical History, Current Medications, Allergies, Family History, and Lifestyle data with conditional field rendering, auto-save every 30 seconds to both local storage and server-side draft, and produces the same structured output format as the AI intake for downstream interoperability.

## Dependent Tasks

- US_001/task_001 — Frontend scaffolding (Angular project, routing, shared module)
- US_016/task_001 — AI intake frontend (shared intake module, session data bridge for AI-to-manual switch)

## Impacted Components

- `src/app/features/intake/manual-intake/` — New manual intake wizard component
- `src/app/features/intake/services/intake.service.ts` — Extend with draft save/load methods
- `src/app/features/intake/models/intake.model.ts` — Extend with form section interfaces
- `src/app/features/intake/shared/` — Shared intake utilities (auto-save, character counter)

## Implementation Plan

1. Define TypeScript interfaces for each form section (ChiefComplaint, MedicalHistory, Medications, Allergies, FamilyHistory, Lifestyle)
2. Build ManualIntakeComponent as step wizard using Angular Material Stepper with section-level progress tracking
3. Implement reactive form groups per section with required field validators and conditional field logic
4. Add auto-save service using RxJS interval (30s) — persist to localStorage and server-side draft endpoint
5. Implement auto-save indicator (green checkmark) and resume logic from localStorage or server draft
6. Add character counter for free-text fields with 5000 char limit and warning at 4500
7. Handle AI-to-manual switch: pre-populate form with IntakeService session data
8. Ensure submission output matches AI intake structured format for downstream compatibility

## Current Project State

```text
src/
├── app/
│   └── features/
│       └── intake/
│           ├── ai-intake/
│           ├── manual-intake/              ← NEW
│           │   ├── manual-intake.component.ts
│           │   ├── manual-intake.component.html
│           │   ├── manual-intake.component.scss
│           │   └── sections/
│           │       ├── chief-complaint.component.ts
│           │       ├── medical-history.component.ts
│           │       ├── medications.component.ts
│           │       ├── allergies.component.ts
│           │       ├── family-history.component.ts
│           │       └── lifestyle.component.ts
│           ├── shared/
│           │   └── auto-save.service.ts
│           ├── services/
│           │   └── intake.service.ts
│           └── models/
│               └── intake.model.ts
```

## Expected Changes

| Action | File Path | Description |
|--------|-----------|-------------|
| CREATE | src/app/features/intake/manual-intake/manual-intake.component.ts | Step wizard component with Angular Material Stepper |
| CREATE | src/app/features/intake/manual-intake/manual-intake.component.html | Step wizard template with section tabs and progress bar |
| CREATE | src/app/features/intake/manual-intake/manual-intake.component.scss | Stepper styles, mobile touch targets, auto-save indicator |
| CREATE | src/app/features/intake/manual-intake/sections/chief-complaint.component.ts | Chief complaint section with reactive form and validation |
| CREATE | src/app/features/intake/shared/auto-save.service.ts | Auto-save service with 30s interval, localStorage + server sync |
| MODIFY | src/app/features/intake/services/intake.service.ts | Add saveDraft(), loadDraft(), submitManualIntake() methods |
| MODIFY | src/app/features/intake/models/intake.model.ts | Add form section interfaces and conditional field types |

## External References

- [Angular Material Stepper](https://material.angular.io/components/stepper/overview)
- [Angular Reactive Forms](https://angular.io/guide/reactive-forms)
- [RxJS interval](https://rxjs.dev/api/index/function/interval)

## Build Commands

- [Refer to applicable technology stack build commands](.propel/build/)

## Implementation Validation Strategy

- [ ] Unit tests pass for ManualIntakeComponent (section navigation, validation, conditional fields)
- [ ] Unit tests pass for auto-save service (interval trigger, localStorage, server sync)
- [ ] Integration tests pass (AI-to-manual data bridge, submission format compatibility)

## Implementation Checklist

- [ ] Create ManualIntakeComponent with Angular Material Stepper and section-level progress tracking — maps to AC-1
- [ ] Implement reactive form groups per section with required field validators and inline error messages — maps to AC-1
- [ ] Add conditional field rendering using reactive form valueChanges (e.g., pregnancy follow-ups) — maps to AC-3
- [ ] Build auto-save service with RxJS interval (30s) persisting to localStorage and server-side draft — maps to AC-2
- [ ] Implement resume logic loading from server draft (priority) or localStorage fallback — maps to AC-2
- [ ] Add character counter for free-text fields with 5000 limit and warning at 4500 — maps to edge cases
- [ ] Handle AI-to-manual switch by pre-populating form from AI session data — maps to edge cases
- [ ] Ensure submission produces identical structured output format as AI intake — maps to AC-4
