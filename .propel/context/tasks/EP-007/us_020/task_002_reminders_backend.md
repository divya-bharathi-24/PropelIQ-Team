# Task - task_002_reminders_backend

## Requirement Reference

- **User Story:** US_020
- **Story Location:** .propel/context/tasks/EP-007/us_020/us_020.md
- **Acceptance Criteria:**
  - AC-1: Reminder schedule with configurable intervals — 72h, 24h, 2h defaults; per patient preferred channel
  - AC-2: Multi-channel delivery (email, SMS, push) — SendGrid, SMS provider, Firebase Cloud Messaging
  - AC-3: Reminder contains actionable response options — Confirm, Reschedule, Cancel links that perform actions
  - AC-4: Confirmation response tracked — appointment status updated to patient_confirmed, no-show risk re-evaluated
  - AC-5: Escalated reminders for high-risk appointments — additional at 48h and 6h, staff follow-up at 12h if no confirmation
- **Edge Cases:**
  - Patient has no email/phone → push only; if no push, create staff task for manual call
  - SendGrid daily limit reached (100/day) → queue remaining, prioritize by appointment proximity
  - Reminder sent but appointment already cancelled → check status before sending, send cancellation notice instead

---

## Design References

| Reference Type | Value |
|----------------|-------|
| **UI Impact** | No |
| **Figma URL** | N/A |
| **Wireframe Status** | N/A |
| **Wireframe Type** | N/A |
| **Wireframe Path/URL** | N/A |
| **Screen Spec** | N/A |
| **UXR Requirements** | N/A |
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
| Backend | ASP.NET Core Web API | 8.0 | TR-002 — Minimal API for reminder endpoints |
| Backend | Hangfire | 1.8.x | TR-007 — Scheduled reminder jobs at configurable intervals |
| Backend | SendGrid | Free tier | NFR-012 — Email delivery (100/day free) |
| Backend | Entity Framework Core | 8.0 | TR-004 — Reminder preferences and delivery event logging |
| Database | PostgreSQL | 16.x | TR-004 — Notification events and preferences |

---

## Task Overview

Implement the notification service backend for multi-channel appointment reminders. This includes Hangfire-scheduled reminder jobs at configurable intervals (72h, 24h, 2h), multi-channel delivery via SendGrid (email), SMS provider, and Firebase Cloud Messaging (push), actionable response processing for confirm/reschedule/cancel, confirmation tracking with no-show risk re-evaluation trigger, and escalated reminder logic for high-risk appointments.

## Dependent Tasks

- US_002/task_001 — Backend API scaffolding (solution structure, YARP gateway)
- US_004/task_001 — Core domain schemas (appointment, patient tables)

## Impacted Components

- `src/Services/NotificationService/` — New microservice project
- `src/Services/NotificationService/Services/ReminderScheduler.cs` — Reminder scheduling logic
- `src/Services/NotificationService/Services/ChannelDispatcher.cs` — Multi-channel delivery
- `src/Services/NotificationService/Services/SendGridEmailSender.cs` — SendGrid email implementation
- `src/Services/NotificationService/Jobs/ReminderJob.cs` — Hangfire reminder job
- `src/Services/NotificationService/Endpoints/ReminderEndpoints.cs` — API endpoints
- `src/Services/NotificationService/Models/` — Domain entities

## Implementation Plan

1. Create NotificationService microservice project with DI, EF Core, Hangfire, SendGrid registration
2. Define domain entities (ReminderPreference, NotificationEvent, ReminderSchedule) and EF Core DbContext
3. Create EF Core migration for notification schema (preferences, events, schedules tables)
4. Implement ReminderScheduler creating Hangfire jobs at 72h/24h/2h before appointment time
5. Build ChannelDispatcher routing to email/SMS/push based on patient preference
6. Implement SendGridEmailSender with responsive HTML templates and actionable links
7. Add confirmation processing endpoint updating appointment status and triggering risk re-evaluation
8. Implement escalated reminder logic for high-risk appointments (48h, 6h additional, 12h staff task)

## Current Project State

```text
src/
├── Services/
│   └── NotificationService/               ← NEW
│       ├── Endpoints/
│       │   └── ReminderEndpoints.cs
│       ├── Services/
│       │   ├── ReminderScheduler.cs
│       │   ├── ChannelDispatcher.cs
│       │   └── SendGridEmailSender.cs
│       ├── Jobs/
│       │   └── ReminderJob.cs
│       ├── Models/
│       │   ├── ReminderPreference.cs
│       │   └── NotificationEvent.cs
│       ├── Templates/
│       │   └── reminder-email.html
│       ├── Data/
│       │   └── NotificationDbContext.cs
│       └── Program.cs
```

## Expected Changes

| Action | File Path | Description |
|--------|-----------|-------------|
| CREATE | src/Services/NotificationService/Program.cs | Service entry with DI for EF Core, Hangfire, SendGrid |
| CREATE | src/Services/NotificationService/Services/ReminderScheduler.cs | Schedule Hangfire jobs at 72h/24h/2h intervals per appointment |
| CREATE | src/Services/NotificationService/Services/ChannelDispatcher.cs | Route delivery to email/SMS/push based on patient preference |
| CREATE | src/Services/NotificationService/Services/SendGridEmailSender.cs | SendGrid email with responsive HTML and actionable links |
| CREATE | src/Services/NotificationService/Jobs/ReminderJob.cs | Hangfire job checking appointment status then dispatching reminder |
| CREATE | src/Services/NotificationService/Endpoints/ReminderEndpoints.cs | GET /reminders/{appointmentId}, POST /reminders/confirm |
| CREATE | src/Services/NotificationService/Templates/reminder-email.html | Responsive HTML email template with Confirm/Reschedule/Cancel buttons |
| MODIFY | src/ApiGateway/yarp.json | Add route cluster for NotificationService |

## External References

- [SendGrid .NET SDK](https://docs.sendgrid.com/for-developers/sending-email/quickstart-dotnet)
- [Hangfire Scheduled Jobs](https://docs.hangfire.io/en/latest/background-methods/calling-methods-with-delay.html)
- [ASP.NET Core 8 Minimal APIs](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis?view=aspnetcore-8.0)

## Build Commands

- [Refer to applicable technology stack build commands](.propel/build/)

## Implementation Validation Strategy

- [ ] Unit tests pass for ReminderScheduler (interval calculation, escalation logic)
- [ ] Unit tests pass for ChannelDispatcher (routing, fallback for missing channels)
- [ ] Integration tests pass (SendGrid email delivery, confirmation processing, Hangfire scheduling)

## Implementation Checklist

- [ ] Create NotificationService project with EF Core, Hangfire, and SendGrid DI registration — maps to AC-1
- [ ] Define domain entities and create EF Core migration for notification schema — maps to AC-1
- [ ] Implement ReminderScheduler creating Hangfire jobs at 72h/24h/2h default intervals — maps to AC-1
- [ ] Build ChannelDispatcher routing to email (SendGrid), SMS, and push (FCM) based on patient preference — maps to AC-2
- [ ] Implement SendGrid email sender with responsive HTML template containing Confirm/Reschedule/Cancel links — maps to AC-3
- [ ] Add confirmation processing endpoint updating appointment to patient_confirmed and triggering risk re-evaluation — maps to AC-4
- [ ] Implement escalated reminder logic for high-risk: additional at 48h/6h, staff follow-up task at 12h — maps to AC-5
- [ ] Add SendGrid rate limit handling queuing overflow emails prioritized by appointment proximity — maps to edge cases
