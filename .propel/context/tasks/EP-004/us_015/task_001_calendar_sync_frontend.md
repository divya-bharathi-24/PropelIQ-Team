# Task - task_001_calendar_sync_frontend

## Requirement Reference

- **User Story:** us_015
- **Story Location:** `.propel/context/tasks/EP-004/us_015/us_015.md`
- **Acceptance Criteria:**
  - AC-1: ICS feed URL generated per patient — enable calendar sync in settings; unique URL generated for external calendar subscription
  - AC-2: Google Calendar integration via OAuth — "Connect Google Calendar" button; OAuth 2.0 consent flow; appointments pushed to "Healthcare" calendar
  - AC-4: Calendar disconnect and data cleanup — "Disconnect" button; confirmation message explaining existing entries will remain but no longer update
- **Edge Cases:**
  - OAuth token expires → display "disconnected" status; prompt reconnection
  - Patient has 100+ appointments → ICS feed shows future + last 5 completed
  - External service unavailable → show "sync delayed" status

---

## Design References

| Reference Type | Value |
|----------------|-------|
| **UI Impact** | Yes |
| **Figma URL** | N/A |
| **Wireframe Status** | N/A |
| **Wireframe Type** | N/A |
| **Wireframe Path/URL** | N/A |
| **Screen Spec** | SCR-CAL-001 |
| **UXR Requirements** | Calendar sync as tabbed section in patient profile; connected calendars as cards with status indicator (green=syncing, yellow=delayed, red=disconnected); one-click connect buttons with provider logos |
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
| Frontend Framework | Angular | 17 | TR-001 — Standalone components; OAuth redirect handling |
| Frontend UI Library | Angular Material | 17.x | TR-001 — Cards, tabs, icons for calendar provider display |
| Frontend State | RxJS | 7.x | TR-001 — Reactive sync status updates |
| Language | TypeScript | 5.x | TR-001 — Type-safe development |

---

## Task Overview

Build the calendar sync settings interface as a tabbed section within the patient profile, displaying connected calendars as cards with status indicators (green/yellow/red), one-click connect buttons with provider logos for Google Calendar and ICS feed, and disconnect functionality with cleanup confirmation message.

## Dependent Tasks

- task_001_frontend_scaffolding (US_001) — Angular 17 project must exist
- task_001_profile_dashboard_frontend (US_010) — Patient profile must exist for tab integration

## Impacted Components

- New: `src/app/features/patient/components/calendar-sync/calendar-sync.component.ts` — Calendar sync settings tab
- New: `src/app/features/patient/components/calendar-sync/calendar-sync.component.html` — Sync settings template
- New: `src/app/features/patient/components/calendar-card/calendar-card.component.ts` — Connected calendar card
- New: `src/app/features/patient/services/calendar.service.ts` — Calendar sync HTTP service
- Modify: `src/app/features/patient/pages/profile/profile.component.ts` — Add calendar sync tab

## Implementation Plan

1. Create calendar sync component as a tabbed section within the patient profile page.
2. Build calendar card component showing connected calendars with status indicators (green=syncing, yellow=delayed, red=disconnected).
3. Implement "Connect Google Calendar" button that initiates OAuth 2.0 redirect flow.
4. Handle OAuth callback — process authorization code and display connection success.
5. Implement "Disconnect" button with confirmation dialog explaining residual calendar entries.
6. Display ICS feed URL with copy-to-clipboard functionality for manual subscription.

## Current Project State

```text
src/app/features/patient/
├── pages/
│   ├── profile/ (profile.component.ts/html/scss)
│   └── dashboard/ (dashboard.component.ts/html/scss)
├── components/
│   ├── appointment-card/
│   ├── activity-feed/
│   └── quick-actions/
├── services/
│   └── patient.service.ts
└── patient.routes.ts
```

## Expected Changes

| Action | File Path | Description |
|--------|-----------|-------------|
| CREATE | `src/app/features/patient/components/calendar-sync/calendar-sync.component.ts` | Calendar sync settings with provider cards |
| CREATE | `src/app/features/patient/components/calendar-sync/calendar-sync.component.html` | Sync settings template with tabs |
| CREATE | `src/app/features/patient/components/calendar-sync/calendar-sync.component.scss` | Calendar sync styles |
| CREATE | `src/app/features/patient/components/calendar-card/calendar-card.component.ts` | Connected calendar card with status indicator |
| CREATE | `src/app/features/patient/services/calendar.service.ts` | Calendar sync API (connect, disconnect, status) |
| MODIFY | `src/app/features/patient/pages/profile/profile.component.ts` | Add calendar sync tab |
| MODIFY | `src/app/features/patient/pages/profile/profile.component.html` | Add tab for calendar sync component |

## External References

- [Angular Material 17 Tabs](https://material.angular.io/components/tabs/overview)
- [Angular Material 17 Card](https://material.angular.io/components/card/overview)
- [Google Calendar OAuth 2.0](https://developers.google.com/calendar/api/guides/auth)
- [Clipboard API](https://developer.mozilla.org/en-US/docs/Web/API/Clipboard_API)

## Build Commands

- Refer to [.propel/build/](.propel/build/) for applicable technology stack build commands

## Implementation Validation Strategy

- [x] Unit tests pass — OAuth redirect initiation, disconnect confirmation, status rendering
- [x] Integration tests pass — calendar tab renders in profile; connect/disconnect flows

## Implementation Checklist

- [x] Create calendar sync tabbed section in patient profile with ICS feed URL and copy button → AC-1
- [x] Build connected calendar cards with status indicators (green/yellow/red) → AC-1
- [x] Implement "Connect Google Calendar" button with OAuth 2.0 redirect flow → AC-2
- [x] Create "Disconnect" button with confirmation message about residual entries → AC-4
- [x] Display "sync delayed" status for external service unavailability → Edge case
