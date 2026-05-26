# Task - task_001_audit_logging_frontend

## Requirement Reference

- **User Story:** US_028
- **Story Location:** .propel/context/tasks/EP-011/us_028/us_028.md
- **Acceptance Criteria:**
  - AC-2: Audit log search and filtering — search by date range, actor, action type, resource, patient ID; results within 3 seconds, paginated at 50/page, sortable, no modify/delete
  - AC-3: Audit log export for compliance reporting — CSV or PDF export with selected records, tamper-evident hash displayed
- **Edge Cases:**
  - Compliance officer exports data with PII they cannot access → display only authorized records

---

## Design References

| Reference Type | Value |
|----------------|-------|
| **UI Impact** | Yes |
| **Figma URL** | N/A |
| **Wireframe Status** | N/A |
| **Wireframe Type** | N/A |
| **Wireframe Path/URL** | N/A |
| **Screen Spec** | SCR-AUDIT-001 |
| **UXR Requirements** | Full-width data table with advanced filter panel (collapsible sidebar); timeline visualization option for event sequences; export button with format selection; read-only interface (no edit/delete actions) |
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
| Frontend | Angular | 17 | TR-001 — Standalone components for audit log viewer |
| Frontend | Angular Material | 17.x | TR-001 — Data table, sidenav, date pickers, select, export button |
| Frontend | RxJS | 7.x | TR-001 — Reactive search, filter, sort streams |
| Frontend | TypeScript | 5.x | TR-001 — Type-safe audit log models |

---

## Task Overview

Implement the compliance audit log viewer as a full-width data table with collapsible advanced filter sidebar, server-side search/filter/sort/pagination (50 per page), timeline visualization option for event sequences, and CSV/PDF export with tamper-evident hash display. The interface is strictly read-only with no edit or delete capabilities.

## Dependent Tasks

- US_001/task_001 — Frontend scaffolding (Angular project, routing, shared module)

## Impacted Components

- `src/app/features/audit/` — New audit log feature module
- `src/app/features/audit/log-viewer/` — Full-width audit log table component
- `src/app/features/audit/filter-panel/` — Collapsible advanced filter sidebar
- `src/app/features/audit/timeline/` — Timeline visualization component
- `src/app/features/audit/services/audit.service.ts` — Audit log API service

## Implementation Plan

1. Define TypeScript interfaces for AuditLogEntry, AuditFilter, ExportFormat, and TimelineEvent
2. Implement AuditService with search, filter, sort, pagination, and export methods
3. Build LogViewerComponent as full-width read-only data table with sortable columns and 50/page pagination
4. Build FilterPanelComponent as collapsible sidebar with date range, actor, action type, resource, patient ID filters
5. Build TimelineComponent as alternative view for event sequence visualization

## Current Project State

```text
src/
├── app/
│   └── features/
│       └── audit/                          ← NEW
│           ├── log-viewer/
│           │   ├── log-viewer.component.ts
│           │   ├── log-viewer.component.html
│           │   └── log-viewer.component.scss
│           ├── filter-panel/
│           │   └── filter-panel.component.ts
│           ├── timeline/
│           │   └── audit-timeline.component.ts
│           ├── services/
│           │   └── audit.service.ts
│           └── models/
│               └── audit.model.ts
```

## Expected Changes

| Action | File Path | Description |
|--------|-----------|-------------|
| CREATE | src/app/features/audit/models/audit.model.ts | AuditLogEntry, AuditFilter, ExportFormat interfaces |
| CREATE | src/app/features/audit/services/audit.service.ts | HTTP service for search, filter, sort, pagination, export |
| CREATE | src/app/features/audit/log-viewer/log-viewer.component.ts | Full-width read-only data table with sortable columns, 50/page |
| CREATE | src/app/features/audit/filter-panel/filter-panel.component.ts | Collapsible sidebar with date range, actor, action type, resource, patient ID |
| CREATE | src/app/features/audit/timeline/audit-timeline.component.ts | Timeline visualization for event sequence display |
| MODIFY | src/app/app.routes.ts | Add lazy-loaded route for audit feature |

## External References

- [Angular Material Table with Sorting](https://material.angular.io/components/sort/overview)
- [Angular Material Sidenav](https://material.angular.io/components/sidenav/overview)
- [Angular Material Datepicker](https://material.angular.io/components/datepicker/overview)

## Build Commands

- [Refer to applicable technology stack build commands](.propel/build/)

## Implementation Validation Strategy

- [ ] Unit tests pass for LogViewerComponent (read-only enforcement, sorting, pagination)
- [ ] Unit tests pass for FilterPanelComponent (filter application, date range validation)
- [ ] Integration tests pass (search within 3 seconds, export trigger, timeline view)

## Implementation Checklist

- [ ] Create audit feature module with lazy-loaded route and audit log interfaces — maps to AC-2
- [ ] Build LogViewerComponent as full-width read-only table with sortable columns and 50/page pagination — maps to AC-2
- [ ] Build FilterPanelComponent with date range, actor, action type, resource, patient ID filters — maps to AC-2
- [ ] Implement export functionality with CSV/PDF format selection and tamper-evident hash display — maps to AC-3
- [ ] Build TimelineComponent as alternative view for event sequence visualization — maps to AC-2
