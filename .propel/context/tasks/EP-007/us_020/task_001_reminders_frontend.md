# Task - task_001_reminders_frontend

## Requirement Reference

- **User Story:** US_020
- **Story Location:** .propel/context/tasks/EP-007/us_020/us_020.md
- **Acceptance Criteria:**
  - AC-2: Multi-channel delivery — patient's preferred channel configurable in profile settings
  - AC-3: Reminder contains actionable response options — "Confirm", "Reschedule", "Cancel" buttons/links
  - AC-4: Confirmation response tracked — appointment status updated, UI reflects confirmed state
- **Edge Cases:**
  - Patient has no email/phone → push notification only; display registration prompt for missing channels

---

## Design References

| Reference Type | Value |
|----------------|-------|
| **UI Impact** | Yes |
| **Figma URL** | N/A |
| **Wireframe Status** | N/A |
| **Wireframe Type** | N/A |
| **Wireframe Path/URL** | N/A |
| **Screen Spec** | SCR-REM-001 |
| **UXR Requirements** | Reminder preferences in patient settings as channel toggle switches; reminder history shown in appointment detail; email template uses responsive HTML |
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
| Frontend | Angular | 17 | TR-001 — Standalone components for notification preferences |
| Frontend | Angular Material | 17.x | TR-001 — Toggle switches, list components |
| Frontend | RxJS | 7.x | TR-001 — Reactive form value changes |
| Frontend | TypeScript | 5.x | TR-001 — Type-safe preference models |

---

## Task Overview

Implement the patient-facing reminder preferences UI and reminder history display. The preferences section in patient profile settings allows toggling notification channels (email, SMS, push) with channel toggle switches. The appointment detail view shows reminder history with delivery status. Actionable confirmation handling updates appointment status in real-time.

## Dependent Tasks

- US_001/task_001 — Frontend scaffolding (Angular project, routing, shared module)

## Impacted Components

- `src/app/features/notifications/` — New notifications feature module
- `src/app/features/notifications/reminder-preferences/` — Channel toggle preferences component
- `src/app/features/notifications/reminder-history/` — Reminder delivery history component
- `src/app/features/notifications/services/notification.service.ts` — Notification API service

## Implementation Plan

1. Define TypeScript interfaces for ReminderPreference, ReminderHistory, DeliveryStatus
2. Implement NotificationService with preference CRUD and history retrieval methods
3. Build ReminderPreferencesComponent with channel toggle switches (email, SMS, push)
4. Build ReminderHistoryComponent showing delivery timeline in appointment detail view

## Current Project State

```text
src/
├── app/
│   └── features/
│       └── notifications/                  ← NEW
│           ├── reminder-preferences/
│           │   ├── reminder-preferences.component.ts
│           │   ├── reminder-preferences.component.html
│           │   └── reminder-preferences.component.scss
│           ├── reminder-history/
│           │   ├── reminder-history.component.ts
│           │   └── reminder-history.component.html
│           ├── services/
│           │   └── notification.service.ts
│           └── models/
│               └── notification.model.ts
```

## Expected Changes

| Action | File Path | Description |
|--------|-----------|-------------|
| CREATE | src/app/features/notifications/models/notification.model.ts | ReminderPreference, ReminderHistory, DeliveryStatus interfaces |
| CREATE | src/app/features/notifications/services/notification.service.ts | HTTP service for preferences and reminder history |
| CREATE | src/app/features/notifications/reminder-preferences/reminder-preferences.component.ts | Channel toggle switches for email, SMS, push |
| CREATE | src/app/features/notifications/reminder-preferences/reminder-preferences.component.html | Toggle layout with channel descriptions |
| CREATE | src/app/features/notifications/reminder-history/reminder-history.component.ts | Reminder delivery timeline with status badges |

## External References

- [Angular Material Slide Toggle](https://material.angular.io/components/slide-toggle/overview)
- [Angular Material List](https://material.angular.io/components/list/overview)

## Build Commands

- [Refer to applicable technology stack build commands](.propel/build/)

## Implementation Validation Strategy

- [ ] Unit tests pass for ReminderPreferencesComponent (toggle state, save on change)
- [ ] Unit tests pass for ReminderHistoryComponent (timeline rendering, status display)
- [ ] Integration tests pass (preference persistence, history display in appointment detail)

## Implementation Checklist

- [ ] Define ReminderPreference and ReminderHistory TypeScript interfaces — maps to AC-2
- [ ] Implement NotificationService with preference CRUD and history retrieval — maps to AC-2, AC-4
- [ ] Build ReminderPreferencesComponent with channel toggle switches in patient profile settings — maps to AC-2
- [ ] Build ReminderHistoryComponent showing delivery timeline with confirm/reschedule/cancel action status — maps to AC-3, AC-4
