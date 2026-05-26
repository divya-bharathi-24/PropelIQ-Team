# Task - task_001_conflict_detection_frontend

## Requirement Reference

- **User Story:** US_024
- **Story Location:** .propel/context/tasks/EP-009/us_024/us_024.md
- **Acceptance Criteria:**
  - AC-2: AI-powered resolution suggestions — confidence score, rationale, recommended action displayed
  - AC-3: Conflict dashboard for providers — sorted by severity (Critical, High, Medium) with patient name, type, age
  - AC-4: One-click resolution with audit trail — accept AI suggestion or manual resolution
  - AC-5: Conflict prevention on entry — warning displayed when data would create conflict, does not block save
- **Edge Cases:**
  - AI resolution confidence below 0.5 → display "Requires Manual Review" without auto-suggestion
  - Conflict involves legally protected data → restrict visibility per role authorization

---

## Design References

| Reference Type | Value |
|----------------|-------|
| **UI Impact** | Yes |
| **Figma URL** | N/A |
| **Wireframe Status** | N/A |
| **Wireframe Type** | N/A |
| **Wireframe Path/URL** | N/A |
| **Screen Spec** | SCR-CONFLICT-001 |
| **UXR Requirements** | Side-by-side comparison view; diff highlighting (red=conflicting, green=matching); one-click action buttons (Accept AI, Keep Left, Keep Right, Merge, Flag); severity badges color-coded |
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
| Frontend | Angular | 17 | TR-001 — Standalone components for conflict resolution module |
| Frontend | Angular Material | 17.x | TR-001 — Data table, badges, buttons, diff display |
| Frontend | RxJS | 7.x | TR-001 — Reactive conflict list updates |
| Frontend | TypeScript | 5.x | TR-001 — Type-safe conflict models |

---

## Task Overview

Implement the conflict detection and resolution frontend with a dashboard sorted by severity, side-by-side comparison view with diff highlighting, one-click resolution actions (Accept AI Suggestion, Keep Left, Keep Right, Merge, Flag), severity color-coded badges, and inline conflict prevention warnings during data entry. The interface supports both AI-suggested resolutions and manual resolution workflows.

## Dependent Tasks

- US_001/task_001 — Frontend scaffolding (Angular project, routing, shared module)
- US_023/task_001 — Patient 360 frontend (patient-view module for conflict integration)

## Impacted Components

- `src/app/features/conflicts/` — New conflict resolution feature module
- `src/app/features/conflicts/dashboard/` — Conflict dashboard with severity sorting
- `src/app/features/conflicts/comparison/` — Side-by-side comparison view
- `src/app/features/conflicts/prevention/` — Inline conflict warning component
- `src/app/features/conflicts/services/conflict.service.ts` — Conflict API service

## Implementation Plan

1. Define TypeScript interfaces for DataConflict, ConflictResolution, ConflictSeverity, ResolutionAction
2. Implement ConflictService with dashboard listing, resolution submission, and prevention check
3. Build ConflictDashboardComponent with severity-sorted table, patient name, type, age columns
4. Build ComparisonViewComponent with side-by-side diff highlighting (red=conflicting, green=matching)
5. Add one-click resolution buttons: Accept AI Suggestion, Keep Left, Keep Right, Merge, Flag
6. Build ConflictPreventionComponent as inline warning during data entry

## Current Project State

```text
src/
├── app/
│   └── features/
│       └── conflicts/                      ← NEW
│           ├── dashboard/
│           │   ├── conflict-dashboard.component.ts
│           │   ├── conflict-dashboard.component.html
│           │   └── conflict-dashboard.component.scss
│           ├── comparison/
│           │   ├── comparison-view.component.ts
│           │   └── comparison-view.component.html
│           ├── prevention/
│           │   └── conflict-warning.component.ts
│           ├── services/
│           │   └── conflict.service.ts
│           └── models/
│               └── conflict.model.ts
```

## Expected Changes

| Action | File Path | Description |
|--------|-----------|-------------|
| CREATE | src/app/features/conflicts/models/conflict.model.ts | DataConflict, ConflictSeverity, ResolutionAction interfaces |
| CREATE | src/app/features/conflicts/services/conflict.service.ts | HTTP service for conflict listing, resolution, and prevention check |
| CREATE | src/app/features/conflicts/dashboard/conflict-dashboard.component.ts | Severity-sorted conflict table with patient name, type, age |
| CREATE | src/app/features/conflicts/comparison/comparison-view.component.ts | Side-by-side diff view with red/green highlighting |
| CREATE | src/app/features/conflicts/prevention/conflict-warning.component.ts | Inline warning during data entry with override option |
| MODIFY | src/app/app.routes.ts | Add lazy-loaded route for conflicts feature |

## External References

- [Angular Material Table](https://material.angular.io/components/table/overview)
- [Angular Material Badge](https://material.angular.io/components/badge/overview)

## Build Commands

- [Refer to applicable technology stack build commands](.propel/build/)

## Implementation Validation Strategy

- [ ] Unit tests pass for ConflictDashboardComponent (severity sorting, filtering)
- [ ] Unit tests pass for ComparisonViewComponent (diff highlighting, resolution actions)
- [ ] Integration tests pass (resolution submission, prevention check integration)

## Implementation Checklist

- [ ] Define DataConflict, ConflictSeverity, and ResolutionAction TypeScript interfaces — maps to AC-3
- [ ] Implement ConflictService with dashboard listing, resolution, and prevention check methods — maps to AC-3, AC-4, AC-5
- [ ] Build ConflictDashboardComponent with severity-sorted table (Critical → High → Medium) — maps to AC-3
- [ ] Build ComparisonViewComponent with side-by-side diff highlighting and AI suggestion display — maps to AC-2
- [ ] Add one-click resolution buttons (Accept AI, Keep Left, Keep Right, Merge, Flag) with confirmation — maps to AC-4
- [ ] Build ConflictPreventionComponent as inline warning during data entry without blocking save — maps to AC-5
