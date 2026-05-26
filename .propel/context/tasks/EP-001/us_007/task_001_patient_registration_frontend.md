# Task - task_001_patient_registration_frontend

## Requirement Reference

- **User Story:** us_007
- **Story Location:** `.propel/context/tasks/EP-001/us_007/us_007.md`
- **Acceptance Criteria:**
  - AC-1: Registration form captures required patient data — first name, last name, email, phone, DOB, password with real-time validation (email format, password strength 8+ chars with uppercase/number/special)
  - AC-2: Email verification sent on registration — verification email sent via SendGrid within 30 seconds; account status set to "pending_verification"
  - AC-3: Account activation on email confirmation — click verification link within 24 hours, status changes to "active", redirected to login with success message
  - AC-4: Duplicate email prevention — generic "registration failed" message without revealing if email exists
- **Edge Cases:**
  - Verification email not received → resend button available after 60-second cooldown; max 3 resends per hour
  - Registration with disposable email → allow but flag for review
  - Browser closed during registration → partial data not persisted; user must restart

---

## Design References

| Reference Type | Value |
|----------------|-------|
| **UI Impact** | Yes |
| **Figma URL** | N/A |
| **Wireframe Status** | N/A |
| **Wireframe Type** | N/A |
| **Wireframe Path/URL** | N/A |
| **Screen Spec** | SCR-REG-001 |
| **UXR Requirements** | Single-column layout, progressive disclosure for optional fields, inline validation on blur |
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
| Frontend Framework | Angular | 17 | TR-001 — Standalone components; reactive forms for registration |
| Frontend UI Library | Angular Material | 17.x | TR-001 — WCAG 2.1 AA form controls with built-in validation |
| Frontend State | RxJS | 7.x | TR-001 — Reactive form validation and async operations |
| Language | TypeScript | 5.x | TR-001 — Type-safe frontend development |

---

## Task Overview

Build the patient self-registration page as a standalone Angular 17 component with reactive form validation, email verification pending state, account activation page, and generic duplicate email error handling. The form follows a single-column layout with inline validation on blur per UXR specifications.

## Dependent Tasks

- task_001_frontend_scaffolding (US_001) — Angular 17 project must exist
- task_001_core_domain_schemas (US_004) — Auth database schema must exist for registration API

## Impacted Components

- New: `src/app/features/auth/pages/registration/registration.component.ts` — Registration page standalone component
- New: `src/app/features/auth/pages/registration/registration.component.html` — Registration template
- New: `src/app/features/auth/pages/registration/registration.component.scss` — Registration styles
- New: `src/app/features/auth/pages/verify-email/verify-email.component.ts` — Email verification pending page
- New: `src/app/features/auth/pages/activate/activate.component.ts` — Account activation page
- New: `src/app/features/auth/services/auth.service.ts` — Auth API service

## Implementation Plan

1. Create the registration standalone component with Angular Reactive Forms capturing first name, last name, email, phone, DOB, and password.
2. Implement real-time inline validation on blur — email format regex, password strength (8+ chars, uppercase, number, special character).
3. Create the verify-email pending page displaying "Check your email" message with resend button (60-second cooldown, max 3/hour).
4. Create the account activation page that processes the verification token from URL and redirects to login on success.
5. Implement generic error messaging for duplicate email attempts — no email enumeration.
6. Add lazy-loaded route configuration for auth feature module.

## Current Project State

```text
src/
├── app/
│   ├── app.component.ts
│   ├── app.config.ts
│   ├── app.routes.ts
│   ├── core/
│   │   ├── interceptors/
│   │   └── guards/
│   ├── shared/
│   │   ├── components/
│   │   └── services/
│   └── features/
└── environments/
```

## Expected Changes

| Action | File Path | Description |
|--------|-----------|-------------|
| CREATE | `src/app/features/auth/pages/registration/registration.component.ts` | Registration form with reactive validation |
| CREATE | `src/app/features/auth/pages/registration/registration.component.html` | Single-column form template with inline validation |
| CREATE | `src/app/features/auth/pages/registration/registration.component.scss` | Registration page styles |
| CREATE | `src/app/features/auth/pages/verify-email/verify-email.component.ts` | Email verification pending UI with resend button |
| CREATE | `src/app/features/auth/pages/verify-email/verify-email.component.html` | Verify email template |
| CREATE | `src/app/features/auth/pages/activate/activate.component.ts` | Account activation with token processing |
| CREATE | `src/app/features/auth/pages/activate/activate.component.html` | Activation success/failure template |
| CREATE | `src/app/features/auth/services/auth.service.ts` | Auth HTTP service (register, verify, activate) |
| CREATE | `src/app/features/auth/auth.routes.ts` | Lazy-loaded auth feature routes |
| MODIFY | `src/app/app.routes.ts` | Add lazy-loaded route to auth feature |

## External References

- [Angular 17 Reactive Forms](https://angular.io/guide/reactive-forms)
- [Angular Material 17 Form Field](https://material.angular.io/components/form-field/overview)
- [Angular 17 Standalone Components](https://angular.io/guide/standalone-components)
- [Angular 17 Lazy Loading](https://angular.io/guide/lazy-loading-ngmodules)

## Build Commands

- Refer to [.propel/build/](.propel/build/) for applicable technology stack build commands

## Implementation Validation Strategy

- [x] Unit tests pass — form validation rules, resend cooldown logic
- [x] Integration tests pass — registration flow navigates correctly

## Implementation Checklist

- [x] Create registration form component with reactive form (name, email, phone, DOB, password) in single-column layout → AC-1
- [x] Implement real-time inline validation on blur (email format, password 8+ chars with uppercase/number/special) → AC-1
- [x] Create email verification pending page with "Check your email" message → AC-2
- [x] Create account activation page processing verification token with success redirect to login → AC-3
- [x] Implement generic "registration failed" error message for duplicate email (no email enumeration) → AC-4
- [x] Add resend verification button with 60-second cooldown and 3/hour limit → Edge case
