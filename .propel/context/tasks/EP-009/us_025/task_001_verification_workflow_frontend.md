# Task - task_001_verification_workflow_frontend

## Requirement Reference

- **User Story:** US_025
- **Story Location:** .propel/context/tasks/EP-009/us_025/us_025.md
- **Acceptance Criteria:**
  - AC-1: Verification queue for AI-generated data — all pending items listed with patient name, data type, confidence, time since generation, sorted by confidence (lowest first)
  - AC-2: Side-by-side source and suggestion view — original source alongside AI extraction, differences highlighted, per-field confidence
  - AC-3: Approve, reject, or modify workflow — approve (becomes record), reject (discard with reason), modify (edit before approve), all logged
  - AC-4: Verification SLA tracking — >24h highlighted red and escalated, >48h auto-assigned
- **Edge Cases:**
  - Bulk verification (100+ items) → batch approve for confidence > 0.95 with individual audit records
  - Staff modifies creating new conflict → warn before persisting

---

## Design References

| Reference Type | Value |
|----------------|-------|
| **UI Impact** | Yes |
| **Figma URL** | N/A |
| **Wireframe Status** | N/A |
| **Wireframe Type** | N/A |
| **Wireframe Path/URL** | N/A |
| **Screen Spec** | SCR-VERIFY-001 |
| **UXR Requirements** | Split-pane layout (source left, suggestion right); inline editing for modify; keyboard shortcuts (Ctrl+Enter approve, Ctrl+Backspace reject); batch selection with checkboxes; confidence meter as horizontal bar |
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
| Frontend | Angular | 17 | TR-001 — Standalone components for verification workflow |
| Frontend | Angular Material | 17.x | TR-001 — Split-pane, checkboxes, progress bars, expansion panels |
| Frontend | RxJS | 7.x | TR-001 — Reactive queue updates, keyboard shortcut streams |
| Frontend | TypeScript | 5.x | TR-001 — Type-safe verification models |

---

## Task Overview

Implement the staff verification workflow interface with a queue listing sorted by confidence (lowest first), split-pane source-vs-suggestion comparison view with diff highlighting, approve/reject/modify actions with keyboard shortcuts, batch selection for high-confidence items, SLA tracking with visual escalation indicators, and inline editing for modification before approval.

## Dependent Tasks

- US_001/task_001 — Frontend scaffolding (Angular project, routing, shared module)
- US_023/task_001 — Patient 360 frontend (patient-view module, verification integration point)

## Impacted Components

- `src/app/features/verification/` — New verification workflow feature module
- `src/app/features/verification/queue/` — Verification queue list component
- `src/app/features/verification/comparison/` — Split-pane comparison view
- `src/app/features/verification/services/verification.service.ts` — Verification API service

## Implementation Plan

1. Define TypeScript interfaces for VerificationItem, VerificationAction, SlaStatus, and ComparisonData
2. Implement VerificationService with queue listing, action submission, and batch operations
3. Build VerificationQueueComponent with sortable table (confidence ascending), SLA indicators, batch checkboxes
4. Build ComparisonViewComponent with split-pane layout, diff highlighting, and inline editing
5. Add keyboard shortcuts (Ctrl+Enter approve, Ctrl+Backspace reject) via Angular HostListener
6. Implement batch approve feature for items with confidence > 0.95

## Current Project State

```text
src/
├── app/
│   └── features/
│       └── verification/                   ← NEW
│           ├── queue/
│           │   ├── verification-queue.component.ts
│           │   ├── verification-queue.component.html
│           │   └── verification-queue.component.scss
│           ├── comparison/
│           │   ├── comparison-view.component.ts
│           │   ├── comparison-view.component.html
│           │   └── comparison-view.component.scss
│           ├── services/
│           │   └── verification.service.ts
│           └── models/
│               └── verification.model.ts
```

## Expected Changes

| Action | File Path | Description |
|--------|-----------|-------------|
| CREATE | src/app/features/verification/models/verification.model.ts | VerificationItem, VerificationAction, SlaStatus interfaces |
| CREATE | src/app/features/verification/services/verification.service.ts | HTTP service for queue, actions, batch operations |
| CREATE | src/app/features/verification/queue/verification-queue.component.ts | Queue list sorted by confidence with SLA indicators and batch checkboxes |
| CREATE | src/app/features/verification/comparison/comparison-view.component.ts | Split-pane with diff highlighting, inline editing, keyboard shortcuts |
| CREATE | src/app/features/verification/comparison/comparison-view.component.scss | Split-pane styles, confidence meter, diff colors |
| MODIFY | src/app/app.routes.ts | Add lazy-loaded route for verification feature |

## External References

- [Angular Material Table with Selection](https://material.angular.io/components/table/overview)
- [Angular Material Progress Bar](https://material.angular.io/components/progress-bar/overview)
- [Angular HostListener](https://angular.io/api/core/HostListener)

## Build Commands

- [Refer to applicable technology stack build commands](.propel/build/)

## Implementation Validation Strategy

- [ ] Unit tests pass for VerificationQueueComponent (sorting, SLA indicators, batch selection)
- [ ] Unit tests pass for ComparisonViewComponent (diff rendering, inline edit, keyboard shortcuts)
- [ ] Integration tests pass (action submission, batch approve, queue refresh)

## Implementation Checklist

- [ ] Create verification feature module with lazy-loaded route and TypeScript interfaces — maps to AC-1
- [ ] Build VerificationQueueComponent sorted by confidence (lowest first) with SLA color indicators — maps to AC-1, AC-4
- [ ] Build ComparisonViewComponent with split-pane layout and diff highlighting per field — maps to AC-2
- [ ] Implement approve, reject (with reason), and modify (inline edit) actions with audit metadata — maps to AC-3
- [ ] Add keyboard shortcuts (Ctrl+Enter approve, Ctrl+Backspace reject) — maps to AC-3
- [ ] Implement batch selection with batch approve for items with confidence > 0.95 — maps to edge cases
