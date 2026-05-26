# Task - task_001_profile_dashboard_frontend

## Requirement Reference

- **User Story:** us_010
- **Story Location:** `.propel/context/tasks/EP-002/us_010/us_010.md`
- **Acceptance Criteria:**
  - AC-1: Patient profile view and edit — personal info, insurance details, emergency contacts with inline editing (save/cancel)
  - AC-2: Profile photo upload — max 2MB, JPEG/PNG only, resized to 200x200px, displayed as avatar within 5 seconds
  - AC-3: Dashboard displays upcoming appointments — next 5 as cards (provider, specialty, date/time, status), sorted by nearest date
  - AC-4: Dashboard quick actions panel — "Book Appointment", "Upload Document", "View Medical History", "Contact Support" with single-click navigation
  - AC-5: Recent activity feed — 10 most recent activities in chronological order with type icon, description, relative timestamp
- **Edge Cases:**
  - No upcoming appointments → "No upcoming appointments" with "Book Now" CTA
  - Invalid phone format → inline validation; previous value preserved until successful save
  - Dashboard data partially unavailable → "Unable to load" placeholder per section; do not block entire dashboard

---

## Design References

| Reference Type | Value |
|----------------|-------|
| **UI Impact** | Yes |
| **Figma URL** | N/A |
| **Wireframe Status** | N/A |
| **Wireframe Type** | N/A |
| **Wireframe Path/URL** | N/A |
| **Screen Spec** | SCR-DASH-001, SCR-PROF-001 |
| **UXR Requirements** | Card-based layout (1 col mobile, 2 col tablet, 3 col desktop); profile grouped sections with collapsible panels |
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
| Frontend Framework | Angular | 17 | TR-001 — Standalone components; reactive forms for profile editing |
| Frontend UI Library | Angular Material | 17.x | TR-001 — Card components, expansion panels, responsive grid |
| Frontend State | RxJS | 7.x | TR-001 — Reactive data streams for dashboard aggregation |
| Language | TypeScript | 5.x | TR-001 — Type-safe development |

---

## Task Overview

Build the patient profile management page with inline editing and photo upload, plus the personalized dashboard showing upcoming appointments (card-based), quick action panel, and recent activity feed. The dashboard uses a responsive card grid layout (1/2/3 columns) with graceful degradation when individual data sources are unavailable.

## Dependent Tasks

- task_001_frontend_scaffolding (US_001) — Angular 17 project must exist

## Impacted Components

- New: `src/app/features/patient/pages/profile/profile.component.ts` — Patient profile page
- New: `src/app/features/patient/pages/profile/profile.component.html` — Profile template with collapsible panels
- New: `src/app/features/patient/pages/dashboard/dashboard.component.ts` — Patient dashboard page
- New: `src/app/features/patient/pages/dashboard/dashboard.component.html` — Dashboard template
- New: `src/app/features/patient/components/appointment-card/appointment-card.component.ts` — Appointment card component
- New: `src/app/features/patient/components/activity-feed/activity-feed.component.ts` — Activity feed component
- New: `src/app/features/patient/components/quick-actions/quick-actions.component.ts` — Quick actions panel
- New: `src/app/features/patient/services/patient.service.ts` — Patient profile/dashboard API service

## Implementation Plan

1. Create profile page with grouped sections (personal info, insurance, emergency contacts) using Angular Material expansion panels.
2. Implement inline editing with save/cancel actions — preserve previous values until successful save.
3. Implement profile photo upload with client-side validation (2MB, JPEG/PNG) and 200x200px resize preview.
4. Create dashboard with responsive card grid (1 col mobile, 2 col tablet, 3 col desktop) using CSS Grid/Flexbox.
5. Build appointment card component displaying next 5 upcoming appointments sorted by date.
6. Build quick actions panel with 4 navigation buttons.
7. Build activity feed showing 10 most recent items with icons and relative timestamps.
8. Implement section-level error handling with "Unable to load" placeholders for failed API calls.

## Current Project State

```text
src/app/
├── app.component.ts
├── app.routes.ts
├── core/
│   ├── interceptors/auth.interceptor.ts
│   └── services/token-storage.service.ts
├── shared/
└── features/
    ├── auth/ (login, registration)
    ├── staff/ (create-patient)
    └── admin/ (audit-log)
```

## Expected Changes

| Action | File Path | Description |
|--------|-----------|-------------|
| CREATE | `src/app/features/patient/pages/profile/profile.component.ts` | Profile page with inline editing and collapsible sections |
| CREATE | `src/app/features/patient/pages/profile/profile.component.html` | Profile template with expansion panels |
| CREATE | `src/app/features/patient/pages/profile/profile.component.scss` | Profile styles |
| CREATE | `src/app/features/patient/pages/dashboard/dashboard.component.ts` | Dashboard with responsive card grid |
| CREATE | `src/app/features/patient/pages/dashboard/dashboard.component.html` | Dashboard template |
| CREATE | `src/app/features/patient/pages/dashboard/dashboard.component.scss` | Dashboard responsive styles |
| CREATE | `src/app/features/patient/components/appointment-card/appointment-card.component.ts` | Appointment card with status badge |
| CREATE | `src/app/features/patient/components/activity-feed/activity-feed.component.ts` | Activity feed with relative timestamps |
| CREATE | `src/app/features/patient/components/quick-actions/quick-actions.component.ts` | Quick action button panel |
| CREATE | `src/app/features/patient/services/patient.service.ts` | Patient API service (profile, dashboard data) |
| CREATE | `src/app/features/patient/patient.routes.ts` | Patient feature lazy-loaded routes |
| MODIFY | `src/app/app.routes.ts` | Add lazy-loaded route for patient feature |

## External References

- [Angular Material 17 Expansion Panel](https://material.angular.io/components/expansion/overview)
- [Angular Material 17 Card](https://material.angular.io/components/card/overview)
- [Angular 17 Reactive Forms](https://angular.io/guide/reactive-forms)
- [CSS Grid Layout](https://developer.mozilla.org/en-US/docs/Web/CSS/CSS_grid_layout)

## Build Commands

- Refer to [.propel/build/](.propel/build/) for applicable technology stack build commands

## Implementation Validation Strategy

- [x] Unit tests pass — inline edit save/cancel, photo validation, responsive grid breakpoints
- [x] Integration tests pass — dashboard renders cards, handles partial failures gracefully

## Implementation Checklist

- [x] Create profile page with inline editing (personal info, insurance, emergency contacts) in collapsible panels → AC-1
- [x] Implement profile photo upload with validation (2MB, JPEG/PNG) and 200x200px resize → AC-2
- [x] Create dashboard with appointment cards (next 5, sorted by date, provider/specialty/time/status) → AC-3
- [x] Create quick actions panel (Book, Upload, Medical History, Contact Support) with single-click navigation → AC-4
- [x] Create activity feed (10 most recent, chronological, type icon, relative timestamp) → AC-5
- [x] Implement responsive card grid layout (1 col mobile, 2 col tablet, 3 col desktop) → UXR
- [x] Add "No upcoming appointments" state with "Book Now" CTA → Edge case
- [x] Implement section-level error placeholders ("Unable to load") for partial data failures → Edge case
