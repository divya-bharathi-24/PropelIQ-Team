# Task - task_002_appointment_mgmt_backend

## Requirement Reference

- **User Story:** us_012
- **Story Location:** `.propel/context/tasks/EP-003/us_012/us_012.md`
- **Acceptance Criteria:**
  - AC-1: Patient appointment list view — API returns appointments with status, date/time, provider, type; sortable; paginated (10/page)
  - AC-2: Appointment rescheduling — original slot released, new slot reserved atomically; reschedule count incremented; confirmation email sent
  - AC-3: Appointment cancellation with policy enforcement — >24h: immediate; <24h: warning flag; slot released to availability pool
  - AC-4: PDF confirmation generation — PDF with appointment details, patient name, QR code; generated within 3 seconds
  - AC-5: Confirmation email with ICS attachment — SendGrid email within 60 seconds with appointment details and ICS calendar file
- **Edge Cases:**
  - PDF service temporarily unavailable → queue for retry; email fallback
  - 3+ cancellations/day → soft-block requiring staff intervention
  - Reschedule <24h → allow with reliability score warning flag

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
| Backend Framework | ASP.NET Core Web API | 8.0 | TR-002 — Minimal API for appointment management endpoints |
| Database | PostgreSQL | 16.x | TR-004 — Appointment schema for status management |
| ORM | Entity Framework Core | 8.0 | TR-004 — Atomic transactions for reschedule/cancel |
| Email Service | SendGrid | Free tier | NFR-003 — Confirmation emails with ICS attachment |
| Background Jobs | Hangfire | 1.8.x | TR-007 — PDF generation queue and retry |

---

## Task Overview

Implement appointment management API including list endpoint with pagination/sort, reschedule with atomic slot swap, cancellation with 24-hour policy enforcement and daily limit, PDF confirmation generation with QR code, and SendGrid email delivery with ICS calendar attachment. Includes Hangfire queue for PDF generation retry on service failure.

## Dependent Tasks

- task_002_slot_booking_backend (US_011) — Booking service must exist for slot reservation logic reuse
- task_001_core_domain_schemas (US_004) — Appointment schema must exist

## Impacted Components

- New: `src/Services/Appointment/HealthPlatform.Appointment.Api/Controllers/AppointmentController.cs` — List, reschedule, cancel endpoints
- New: `src/Services/Appointment/HealthPlatform.Appointment.Api/Services/AppointmentManagementService.cs` — Reschedule/cancel business logic
- New: `src/Services/Appointment/HealthPlatform.Appointment.Api/Services/PdfConfirmationService.cs` — PDF generation with QR code
- New: `src/Services/Appointment/HealthPlatform.Appointment.Api/Services/IcsCalendarService.cs` — ICS file generation
- New: `src/Services/Notification/HealthPlatform.Notification.Api/Services/AppointmentEmailService.cs` — SendGrid email with ICS

## Implementation Plan

1. Create appointment list API endpoint with status filter, date sorting, and server-side pagination (10/page).
2. Implement reschedule endpoint — atomic transaction releasing original slot and reserving new slot; increment reschedule count; trigger confirmation email.
3. Implement cancellation endpoint — check 24-hour policy; release slot to availability pool; enforce 3/day soft-block with staff override.
4. Implement PDF confirmation generation — appointment details, patient name, QR code for check-in; target < 3-second generation.
5. Implement ICS calendar file generation for email attachment.
6. Create SendGrid email integration — send confirmation with ICS attachment within 60 seconds of booking/reschedule.
7. Add Hangfire job for PDF generation retry on failure.
8. Track reschedule count per appointment and daily cancellation count per patient.

## Current Project State

```text
src/Services/Appointment/HealthPlatform.Appointment.Api/
├── Controllers/
│   ├── SlotController.cs
│   ├── BookingController.cs
│   └── DashboardController.cs
├── Services/
│   ├── SlotAvailabilityService.cs
│   ├── BookingService.cs
│   ├── ScheduleConstraintService.cs
│   └── DashboardService.cs
├── Hubs/
│   └── SlotHub.cs
├── Caching/
│   └── SlotCacheInvalidator.cs
└── Data/ (AppointmentDbContext, entities)
```

## Expected Changes

| Action | File Path | Description |
|--------|-----------|-------------|
| CREATE | `src/Services/Appointment/HealthPlatform.Appointment.Api/Controllers/AppointmentController.cs` | List, reschedule, cancel, PDF download endpoints |
| CREATE | `src/Services/Appointment/HealthPlatform.Appointment.Api/Services/AppointmentManagementService.cs` | Reschedule/cancel logic with policy enforcement |
| CREATE | `src/Services/Appointment/HealthPlatform.Appointment.Api/Services/PdfConfirmationService.cs` | PDF generation with QR code (< 3s) |
| CREATE | `src/Services/Appointment/HealthPlatform.Appointment.Api/Services/IcsCalendarService.cs` | ICS calendar file generation |
| CREATE | `src/Services/Notification/HealthPlatform.Notification.Api/Services/AppointmentEmailService.cs` | SendGrid email with ICS attachment |
| CREATE | `src/Services/Appointment/HealthPlatform.Appointment.Api/Models/RescheduleRequest.cs` | Reschedule request DTO |
| CREATE | `src/Services/Appointment/HealthPlatform.Appointment.Api/Jobs/PdfGenerationJob.cs` | Hangfire PDF retry job |
| CREATE | `src/Services/Appointment/HealthPlatform.Appointment.Api/Models/AppointmentListResponse.cs` | Paginated list response DTO |

## External References

- [QuestPDF for .NET](https://www.questpdf.com/)
- [SendGrid C# SDK — Attachments](https://docs.sendgrid.com/for-developers/sending-email/quickstart-csharp)
- [iCalendar (ICS) Format RFC 5545](https://datatracker.ietf.org/doc/html/rfc5545)
- [QRCoder NuGet Package](https://github.com/codebude/QRCoder)

## Build Commands

- Refer to [.propel/build/](.propel/build/) for applicable technology stack build commands

## Implementation Validation Strategy

- [x] Unit tests pass — reschedule atomicity, cancellation policy, PDF generation, ICS format
- [x] Integration tests pass — reschedule releases old slot; cancellation enforces daily limit; email sent within 60s

## Implementation Checklist

- [x] Create appointment list API with status filter, date sort, and pagination (10/page) → AC-1
- [x] Implement reschedule endpoint with atomic slot swap and reschedule count increment → AC-2
- [x] Implement cancellation endpoint with 24-hour policy enforcement and slot release → AC-3
- [x] Implement PDF confirmation generation with appointment details and QR code within 3 seconds → AC-4
- [x] Create SendGrid email with ICS calendar attachment sent within 60 seconds → AC-5
- [x] Add daily cancellation limit (3/day) with soft-block requiring staff intervention → Edge case
- [x] Implement Hangfire PDF generation retry on service failure → Edge case
- [x] Track reschedule count per appointment → AC-2
