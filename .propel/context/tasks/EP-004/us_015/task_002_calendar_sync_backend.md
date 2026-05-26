# Task - task_002_calendar_sync_backend

## Requirement Reference

- **User Story:** us_015
- **Story Location:** `.propel/context/tasks/EP-004/us_015/us_015.md`
- **Acceptance Criteria:**
  - AC-1: ICS feed URL generated per patient — unique, unguessable feed URL containing all future confirmed appointments for external calendar subscription
  - AC-2: Google Calendar integration via OAuth — OAuth 2.0 consent flow; appointments pushed to "Healthcare" calendar within 2 minutes; create/update/delete sync
  - AC-3: Appointment changes reflected in external calendar — updated within 5 minutes on book/reschedule/cancel; correct time/provider/status; cancelled removed
  - AC-4: Calendar disconnect and data cleanup — OAuth token revoked; ICS feed URL invalidated; confirmation response
- **Edge Cases:**
  - OAuth token expires → attempt silent refresh; if fails, mark "disconnected" and notify patient
  - 100+ appointments → ICS feed paginates: future + last 5 completed; full history on-demand
  - External service unavailable → queue operations with exponential backoff (max 6 retries/1 hour)

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
| Backend Framework | ASP.NET Core Web API | 8.0 | TR-002 — Minimal API for calendar sync endpoints |
| Database | PostgreSQL | 16.x | TR-004 — Store OAuth tokens and ICS feed URLs |
| Background Jobs | Hangfire | 1.8.x | TR-007 — Calendar sync queue with retry; event-driven push |
| Email Service | SendGrid | Free tier | NFR-012 — Disconnect notifications |

---

## Task Overview

Implement calendar sync backend including cryptographically unique ICS feed URL generation, Google Calendar OAuth 2.0 integration with appointment push/update/delete sync, event-driven calendar updates on appointment changes (within 5 minutes), and disconnect with OAuth token revocation and feed invalidation. Includes exponential backoff retry queue for external service unavailability.

## Dependent Tasks

- task_001_backend_api_scaffolding (US_002) — Backend service projects must exist
- task_002_appointment_mgmt_backend (US_012) — Appointment management must exist for change event integration

## Impacted Components

- New: `src/Services/Appointment/HealthPlatform.Appointment.Api/Controllers/CalendarController.cs` — Calendar sync endpoints
- New: `src/Services/Appointment/HealthPlatform.Appointment.Api/Services/CalendarSyncService.cs` — Calendar sync logic
- New: `src/Services/Appointment/HealthPlatform.Appointment.Api/Services/IcsFeedService.cs` — ICS feed generation
- New: `src/Services/Appointment/HealthPlatform.Appointment.Api/Services/GoogleCalendarService.cs` — Google Calendar API integration
- New: `src/Services/Appointment/HealthPlatform.Appointment.Api/Jobs/CalendarSyncJob.cs` — Hangfire sync queue with retry
- New: `src/Services/Appointment/HealthPlatform.Appointment.Api/Data/Entities/CalendarConnection.cs` — OAuth token storage entity

## Implementation Plan

1. Create calendar sync enable endpoint — generate cryptographically unique ICS feed URL per patient.
2. Implement ICS feed endpoint serving future confirmed appointments in iCalendar format.
3. Implement Google Calendar OAuth 2.0 flow — authorization URL, callback handler, token storage.
4. Create appointment push service — push appointments to dedicated "Healthcare" calendar via Google Calendar API within 2 minutes.
5. Implement event-driven sync — on appointment book/reschedule/cancel, queue Hangfire job to update/remove calendar entry within 5 minutes.
6. Create disconnect endpoint — revoke OAuth token, invalidate ICS feed URL, return confirmation.
7. Implement exponential backoff retry (max 6 retries over 1 hour) for external service unavailability.

## Current Project State

```text
src/Services/Appointment/HealthPlatform.Appointment.Api/
├── Controllers/
│   ├── SlotController.cs
│   ├── BookingController.cs
│   ├── AppointmentController.cs
│   ├── InsuranceController.cs
│   ├── SwapController.cs
│   └── DashboardController.cs
├── Services/
│   ├── SlotAvailabilityService.cs
│   ├── BookingService.cs
│   ├── AppointmentManagementService.cs
│   ├── SwapService.cs
│   └── InsuranceCheckService.cs
├── Jobs/ (SlotMonitorJob, SwapExpiryJob, InsuranceReVerifyJob)
└── Data/ (AppointmentDbContext, entities)
```

## Expected Changes

| Action | File Path | Description |
|--------|-----------|-------------|
| CREATE | `src/Services/Appointment/HealthPlatform.Appointment.Api/Controllers/CalendarController.cs` | Enable sync, ICS feed, OAuth callback, disconnect endpoints |
| CREATE | `src/Services/Appointment/HealthPlatform.Appointment.Api/Services/CalendarSyncService.cs` | Sync orchestration and token management |
| CREATE | `src/Services/Appointment/HealthPlatform.Appointment.Api/Services/IcsFeedService.cs` | ICS format generation for appointment feed |
| CREATE | `src/Services/Appointment/HealthPlatform.Appointment.Api/Services/GoogleCalendarService.cs` | Google Calendar API push/update/delete |
| CREATE | `src/Services/Appointment/HealthPlatform.Appointment.Api/Jobs/CalendarSyncJob.cs` | Event-driven sync with exponential backoff retry |
| CREATE | `src/Services/Appointment/HealthPlatform.Appointment.Api/Data/Entities/CalendarConnection.cs` | OAuth token and feed URL storage |
| CREATE | `src/Services/Appointment/HealthPlatform.Appointment.Api/Models/CalendarSyncResponse.cs` | Sync status response DTO |

## External References

- [Google Calendar API v3](https://developers.google.com/calendar/api/v3/reference)
- [Google OAuth 2.0 for Server-side Apps](https://developers.google.com/identity/protocols/oauth2/web-server)
- [iCalendar RFC 5545](https://datatracker.ietf.org/doc/html/rfc5545)
- [Ical.Net NuGet Package](https://github.com/rianjs/ical.net)
- [Polly Retry Policies](https://www.thepollyproject.org/)

## Build Commands

- Refer to [.propel/build/](.propel/build/) for applicable technology stack build commands

## Implementation Validation Strategy

- [x] Unit tests pass — ICS generation, OAuth flow, sync event handling, retry logic
- [x] Integration tests pass — ICS feed returns valid calendar; Google Calendar push succeeds

## Implementation Checklist

- [x] Generate cryptographically unique ICS feed URL per patient with appointment data → AC-1
- [x] Implement Google Calendar OAuth 2.0 flow (authorization, callback, token storage) → AC-2
- [x] Push appointments to "Healthcare" calendar within 2 minutes via Google Calendar API → AC-2
- [x] Create event-driven sync on appointment changes (book/reschedule/cancel) within 5 minutes → AC-3
- [x] Implement disconnect with OAuth token revocation and ICS feed URL invalidation → AC-4
- [x] Add exponential backoff retry queue (max 6 retries over 1 hour) for service unavailability → Edge case
- [x] Implement silent OAuth token refresh with "disconnected" fallback on failure → Edge case
