# Task - task_001_user_management_frontend

## Requirement Reference

- **User Story:** US_027
- **Story Location:** .propel/context/tasks/EP-011/us_027/us_027.md
- **Acceptance Criteria:**
  - AC-1: User list with search, filter, and pagination — table with name, email, role, status, last login, created date; searchable, filterable, 25/page, < 2s
  - AC-2: Create new user with role assignment — form with name, email, role, department
  - AC-3: Modify user role and permissions — role change with required reason field
  - AC-4: Deactivate user account — deactivate with confirmation and required reason
  - AC-5: Bulk user operations — multi-select checkboxes, bulk deactivate/role change/export CSV/send notification
- **Edge Cases:**
  - Admin deactivates own account → display rejection message
  - Admin deactivates last admin → display rejection message
  - Email exists (reactivation) → present option to reactivate existing account

---

## Design References

| Reference Type | Value |
|----------------|-------|
| **UI Impact** | Yes |
| **Figma URL** | N/A |
| **Wireframe Status** | N/A |
| **Wireframe Type** | N/A |
| **Wireframe Path/URL** | N/A |
| **Screen Spec** | SCR-ADMIN-001 |
| **UXR Requirements** | Data table with sticky header, row actions dropdown, bulk selection bar fixed at top; create/edit as full-page form for accessibility; deactivation requires two-step confirmation with reason |
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
| Frontend | Angular | 17 | TR-001 — Standalone components for admin module |
| Frontend | Angular Material | 17.x | TR-001 — Data table, forms, dialogs, checkboxes, menus |
| Frontend | RxJS | 7.x | TR-001 — Reactive search, pagination, bulk selection |
| Frontend | TypeScript | 5.x | TR-001 — Type-safe user management models |

---

## Task Overview

Implement the admin user management interface with a data table featuring sticky header, server-side search/filter/pagination, row-level actions dropdown, bulk selection bar, full-page create/edit forms for accessibility, two-step deactivation confirmation with reason field, and bulk operations (deactivate, role change, CSV export, send notification).

## Dependent Tasks

- US_001/task_001 — Frontend scaffolding (Angular project, routing, shared module)

## Impacted Components

- `src/app/features/admin/` — New admin feature module
- `src/app/features/admin/user-list/` — User list table component
- `src/app/features/admin/user-form/` — Create/edit user form component
- `src/app/features/admin/bulk-actions/` — Bulk selection bar component
- `src/app/features/admin/services/admin.service.ts` — Admin API service

## Implementation Plan

1. Define TypeScript interfaces for User, UserRole, UserStatus, BulkAction, and UserFilter
2. Implement AdminService with CRUD, search, filter, pagination, bulk operations, and CSV export
3. Build UserListComponent with sticky-header data table, row actions dropdown, and server-side pagination
4. Build UserFormComponent as full-page form with role selector and department field
5. Build BulkActionsBar with checkbox selection, action dropdown, and confirmation dialog
6. Add two-step deactivation dialog with required reason text field
7. Implement self-deactivation and last-admin guard UI validations

## Current Project State

```text
src/
├── app/
│   └── features/
│       └── admin/                          ← NEW
│           ├── user-list/
│           │   ├── user-list.component.ts
│           │   ├── user-list.component.html
│           │   └── user-list.component.scss
│           ├── user-form/
│           │   ├── user-form.component.ts
│           │   ├── user-form.component.html
│           │   └── user-form.component.scss
│           ├── bulk-actions/
│           │   └── bulk-actions-bar.component.ts
│           ├── services/
│           │   └── admin.service.ts
│           └── models/
│               └── admin.model.ts
```

## Expected Changes

| Action | File Path | Description |
|--------|-----------|-------------|
| CREATE | src/app/features/admin/models/admin.model.ts | User, UserRole, UserFilter, BulkAction interfaces |
| CREATE | src/app/features/admin/services/admin.service.ts | HTTP service for user CRUD, search, filter, bulk ops, CSV export |
| CREATE | src/app/features/admin/user-list/user-list.component.ts | Sticky-header data table with search, filter, pagination, row actions |
| CREATE | src/app/features/admin/user-form/user-form.component.ts | Full-page create/edit form with role and department |
| CREATE | src/app/features/admin/bulk-actions/bulk-actions-bar.component.ts | Bulk selection bar with action dropdown and confirmation |
| MODIFY | src/app/app.routes.ts | Add lazy-loaded route for admin feature |

## External References

- [Angular Material Table](https://material.angular.io/components/table/overview)
- [Angular Material Paginator](https://material.angular.io/components/paginator/overview)
- [Angular Material Dialog](https://material.angular.io/components/dialog/overview)

## Build Commands

- [Refer to applicable technology stack build commands](.propel/build/)

## Implementation Validation Strategy

- [ ] Unit tests pass for UserListComponent (search, filter, pagination, row actions)
- [ ] Unit tests pass for UserFormComponent (validation, role selection)
- [ ] Integration tests pass (bulk operations, deactivation flow, CSV export)

## Implementation Checklist

- [ ] Create admin feature module with lazy-loaded route and user management interfaces — maps to AC-1
- [ ] Build UserListComponent with sticky-header table, search, filter by role/status, pagination at 25/page — maps to AC-1
- [ ] Build UserFormComponent as full-page form with name, email, role, department fields and validation — maps to AC-2
- [ ] Add role modification with required reason field and confirmation dialog — maps to AC-3
- [ ] Implement two-step deactivation dialog with required reason and self-deactivation/last-admin guards — maps to AC-4
- [ ] Build BulkActionsBar with checkbox selection for bulk deactivate, role change, CSV export, send notification — maps to AC-5
- [ ] Handle reactivation prompt when email already exists for deactivated account — maps to edge cases
