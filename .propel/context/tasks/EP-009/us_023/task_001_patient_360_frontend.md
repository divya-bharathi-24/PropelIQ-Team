# Task - task_001_patient_360_frontend

## Requirement Reference

- **User Story:** US_023
- **Story Location:** .propel/context/tasks/EP-009/us_023/us_023.md
- **Acceptance Criteria:**
  - AC-1: Unified patient dashboard aggregates cross-service data — rendered within 3 seconds showing demographics, appointments, intake, documents, medications, allergies, risk scores
  - AC-2: AI-generated health timeline — interactive timeline with expandable detail nodes
  - AC-3: Clinical alerts and contraindication warnings — displayed prominently at top with red/orange severity
  - AC-4: Real-time data freshness indicators — "Last updated" timestamp per section with green/yellow/red indicator and manual refresh
  - AC-5: Data section graceful degradation — unavailable sections show placeholder with retry button
- **Edge Cases:**
  - Patient has no data in a section → show "No data yet" with contextual CTA
  - AI timeline exceeds rate limit → show "Generating timeline..." placeholder, deliver async
  - Provider views while another edits → display "Record being edited by Dr. X" banner

---

## Design References

| Reference Type | Value |
|----------------|-------|
| **UI Impact** | Yes |
| **Figma URL** | N/A |
| **Wireframe Status** | N/A |
| **Wireframe Type** | N/A |
| **Wireframe Path/URL** | N/A |
| **Screen Spec** | SCR-360-001 |
| **UXR Requirements** | Tabbed layout with Overview/Timeline/Documents/Medications/Risks tabs; alerts banner pinned at top; responsive masonry grid for data cards; print-friendly view for clinical summaries |
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
| Frontend | Angular | 17 | TR-001 — Standalone components for patient view module |
| Frontend | Angular Material | 17.x | TR-001 — Tabs, cards, expansion panels, alert banners |
| Frontend | RxJS | 7.x | TR-001 — Concurrent data loading with forkJoin, freshness polling |
| Frontend | TypeScript | 5.x | TR-001 — Type-safe patient view models |

---

## Task Overview

Implement the 360-degree patient view as a tabbed layout (Overview, Timeline, Documents, Medications, Risks) with concurrent multi-service data loading, clinical alerts banner pinned at top, responsive masonry grid for data cards, real-time freshness indicators per data section, graceful degradation for unavailable services, and print-friendly clinical summary view. Data loads within 3 seconds using parallel API calls.

## Dependent Tasks

- US_001/task_001 — Frontend scaffolding (Angular project, routing, shared module)

## Impacted Components

- `src/app/features/patient-view/` — New patient 360 feature module
- `src/app/features/patient-view/overview/` — Overview tab with masonry grid
- `src/app/features/patient-view/timeline/` — AI health timeline component
- `src/app/features/patient-view/alerts/` — Clinical alerts banner
- `src/app/features/patient-view/freshness/` — Data freshness indicator
- `src/app/features/patient-view/services/patient-view.service.ts` — Multi-service aggregation

## Implementation Plan

1. Define TypeScript interfaces for PatientView360, TimelineEvent, ClinicalAlert, DataSection, FreshnessStatus
2. Implement PatientViewService with forkJoin for concurrent multi-service data loading
3. Build PatientViewComponent as tabbed layout with Overview/Timeline/Documents/Medications/Risks tabs
4. Build OverviewComponent with responsive masonry grid for data cards and contextual CTAs for empty sections
5. Build TimelineComponent as interactive health timeline with expandable detail nodes
6. Build ClinicalAlertsComponent as top-pinned banner with red/orange severity badges
7. Add FreshnessIndicatorComponent per section (green < 5min, yellow 5-60min, red > 60min) with refresh button
8. Implement graceful degradation with "Data temporarily unavailable" placeholder and retry button per section

## Current Project State

```text
src/
├── app/
│   └── features/
│       └── patient-view/                   ← NEW
│           ├── patient-view.component.ts
│           ├── overview/
│           │   └── overview.component.ts
│           ├── timeline/
│           │   └── timeline.component.ts
│           ├── alerts/
│           │   └── clinical-alerts.component.ts
│           ├── freshness/
│           │   └── freshness-indicator.component.ts
│           ├── services/
│           │   └── patient-view.service.ts
│           └── models/
│               └── patient-view.model.ts
```

## Expected Changes

| Action | File Path | Description |
|--------|-----------|-------------|
| CREATE | src/app/features/patient-view/models/patient-view.model.ts | PatientView360, TimelineEvent, ClinicalAlert, FreshnessStatus interfaces |
| CREATE | src/app/features/patient-view/services/patient-view.service.ts | Multi-service aggregation with forkJoin and error isolation |
| CREATE | src/app/features/patient-view/patient-view.component.ts | Tabbed layout host with tab routing |
| CREATE | src/app/features/patient-view/overview/overview.component.ts | Masonry grid with data cards and contextual CTAs |
| CREATE | src/app/features/patient-view/timeline/timeline.component.ts | Interactive health timeline with expandable nodes |
| CREATE | src/app/features/patient-view/alerts/clinical-alerts.component.ts | Top-pinned alerts banner with severity badges |
| CREATE | src/app/features/patient-view/freshness/freshness-indicator.component.ts | Per-section freshness indicator with refresh button |
| MODIFY | src/app/app.routes.ts | Add lazy-loaded route for patient-view feature |

## External References

- [Angular Material Tabs](https://material.angular.io/components/tabs/overview)
- [Angular Material Card](https://material.angular.io/components/card/overview)
- [RxJS forkJoin](https://rxjs.dev/api/index/function/forkJoin)

## Build Commands

- [Refer to applicable technology stack build commands](.propel/build/)

## Implementation Validation Strategy

- [ ] Unit tests pass for PatientViewService (concurrent loading, error isolation, freshness)
- [ ] Unit tests pass for OverviewComponent (masonry grid, empty states, contextual CTAs)
- [ ] Integration tests pass (tab routing, graceful degradation, timeline rendering)

## Implementation Checklist

- [ ] Create patient-view feature module with tabbed layout and lazy-loaded route — maps to AC-1
- [ ] Implement PatientViewService with forkJoin for concurrent multi-service data loading within 3 seconds — maps to AC-1
- [ ] Build OverviewComponent with responsive masonry grid for data cards (demographics, appointments, medications) — maps to AC-1
- [ ] Build TimelineComponent as interactive health timeline with expandable detail nodes — maps to AC-2
- [ ] Build ClinicalAlertsComponent as top-pinned banner showing drug interactions, allergy conflicts, overdue screenings — maps to AC-3
- [ ] Add FreshnessIndicatorComponent per section (green/yellow/red) with manual refresh button — maps to AC-4
- [ ] Implement graceful degradation with "Data temporarily unavailable" placeholder and retry per section — maps to AC-5
- [ ] Add print-friendly view for clinical summaries and empty state CTAs for new patients — maps to edge cases
