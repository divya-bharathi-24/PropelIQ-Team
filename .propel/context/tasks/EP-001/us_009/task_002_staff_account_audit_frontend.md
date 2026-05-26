# Task - task_002_staff_account_audit_frontend

## Requirement Reference

- **User Story:** us_009
- **Story Location:** `.propel/context/tasks/EP-001/us_009/us_009.md`
- **Acceptance Criteria:**
  - AC-1: Staff can create patient accounts — patient creation form with required fields (name, phone, DOB); auto-generated password displayed with copy button
  - AC-2: Temporary password with forced change — display temporary password once with copy button; forced password change UI on first login
  - AC-4: Auth audit log queryable by admin — audit log table with sortable columns, pagination; search by user ID, date range, event type
- **Edge Cases:**
  - Staff creates duplicate phone → display flag for potential merge
  - Audit log growing → pagination handles large datasets efficiently

---

## Design References

| Reference Type | Value |
|----------------|-------|
| **UI Impact** | Yes |
| **Figma URL** | N/A |
| **Wireframe Status** | N/A |
| **Wireframe Type** | N/A |
| **Wireframe Path/URL** | N/A |
| **Screen Spec** | SCR-STAFF-001 |
| **UXR Requirements** | Minimal required fields with clear labeling; auto-generated password displayed once with copy button; audit log table with sortable columns |
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
| Frontend Framework | Angular | 17 | TR-001 — Standalone components; reactive forms for staff operations |
| Frontend UI Library | Angular Material | 17.x | TR-001 — Data table, form controls, sortable columns |
| Frontend State | RxJS | 7.x | TR-001 — Reactive data flow for audit log queries |
| Language | TypeScript | 5.x | TR-001 — Type-safe development |

---

## Task Overview

Build the staff patient account creation form and admin authentication audit log interface. The creation form uses minimal required fields with the auto-generated temporary password displayed once with a copy-to-clipboard button. The audit log presents a paginated, sortable, searchable table of all authentication events restricted to admin users.

## Dependent Tasks

- task_001_frontend_scaffolding (US_001) — Angular 17 project must exist
- task_001_staff_account_auth_backend (US_009) — Staff account and audit API endpoints must exist

## Impacted Components

- New: `src/app/features/staff/pages/create-patient/create-patient.component.ts` — Staff patient creation form
- New: `src/app/features/staff/pages/create-patient/create-patient.component.html` — Creation form template
- New: `src/app/features/admin/pages/audit-log/audit-log.component.ts` — Auth audit log table
- New: `src/app/features/admin/pages/audit-log/audit-log.component.html` — Audit log template
- New: `src/app/features/auth/pages/force-password-change/force-password-change.component.ts` — Forced password change page
- New: `src/app/features/staff/services/staff-account.service.ts` — Staff account API service

## Implementation Plan

1. Create staff patient creation form with minimal required fields (name, phone, DOB) using reactive forms.
2. Display auto-generated temporary password in a read-only field with copy-to-clipboard button after successful creation.
3. Create forced password change page for users flagged with temp password on first login.
4. Create admin audit log table with sortable columns (event type, user, timestamp, status).
5. Implement audit log search/filter panel (user ID, date range, event type selector) with server-side pagination (50/page).

## Current Project State

```text
src/app/
├── app.component.ts
├── app.routes.ts
├── core/
│   ├── interceptors/auth.interceptor.ts
│   ├── guards/
│   └── services/token-storage.service.ts
├── shared/
└── features/
    └── auth/
        ├── pages/ (login, registration, verify-email, activate)
        └── services/auth.service.ts
```

## Expected Changes

| Action | File Path | Description |
|--------|-----------|-------------|
| CREATE | `src/app/features/staff/pages/create-patient/create-patient.component.ts` | Staff patient creation form with temp password display |
| CREATE | `src/app/features/staff/pages/create-patient/create-patient.component.html` | Form template with copy-to-clipboard |
| CREATE | `src/app/features/staff/pages/create-patient/create-patient.component.scss` | Creation form styles |
| CREATE | `src/app/features/staff/services/staff-account.service.ts` | Staff account creation HTTP service |
| CREATE | `src/app/features/staff/staff.routes.ts` | Staff feature lazy-loaded routes |
| CREATE | `src/app/features/admin/pages/audit-log/audit-log.component.ts` | Sortable, filterable audit log table |
| CREATE | `src/app/features/admin/pages/audit-log/audit-log.component.html` | Audit log template with search panel |
| CREATE | `src/app/features/admin/pages/audit-log/audit-log.component.scss` | Audit log styles |
| CREATE | `src/app/features/admin/services/audit.service.ts` | Audit log query HTTP service |
| CREATE | `src/app/features/admin/admin.routes.ts` | Admin feature lazy-loaded routes |
| CREATE | `src/app/features/auth/pages/force-password-change/force-password-change.component.ts` | Forced password change on first login |
| MODIFY | `src/app/app.routes.ts` | Add lazy-loaded routes for staff and admin features |

## External References

- [Angular Material 17 Data Table](https://material.angular.io/components/table/overview)
- [Angular Material 17 Paginator](https://material.angular.io/components/paginator/overview)
- [Angular Material 17 Sort Header](https://material.angular.io/components/sort/overview)
- [Clipboard API](https://developer.mozilla.org/en-US/docs/Web/API/Clipboard_API)

## Build Commands

- Refer to [.propel/build/](.propel/build/) for applicable technology stack build commands

## Implementation Validation Strategy

- [x] Unit tests pass — form validation, clipboard functionality, pagination logic
- [x] Integration tests pass — staff creates patient and sees temp password; admin views audit log

## Implementation Checklist

- [x] Create staff patient creation form with minimal required fields (name, phone, DOB) → AC-1
- [x] Display auto-generated temporary password with copy-to-clipboard button → AC-1, AC-2
- [x] Create forced password change page for first login with temp password → AC-2
- [x] Create admin audit log table with sortable columns and server-side pagination (50/page) → AC-4
- [x] Implement audit log search/filter panel (user ID, date range, event type) → AC-4
