# Task - task_002_patient_registration_backend

## Requirement Reference

- **User Story:** us_007
- **Story Location:** `.propel/context/tasks/EP-001/us_007/us_007.md`
- **Acceptance Criteria:**
  - AC-1: Registration form captures required patient data — form validation for first name, last name, email, phone, DOB, password with complexity rules
  - AC-2: Email verification sent on registration — SendGrid email within 30 seconds with time-limited token (24-hour expiry); account status "pending_verification"
  - AC-3: Account activation on email confirmation — verification link click within 24 hours; status to "active"; patient role assigned
  - AC-4: Duplicate email prevention — generic "registration failed" message without revealing email existence
  - AC-5: Password hashing with bcrypt — cost factor 12; plaintext never logged or stored
- **Edge Cases:**
  - Verification email not received → resend endpoint with 60-second cooldown; max 3 per hour per email
  - Disposable email domains → allow but flag for review
  - Browser closed during registration → partial data not persisted

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
| Backend Framework | ASP.NET Core Web API | 8.0 | TR-002 — Minimal API pattern for registration endpoints |
| Authentication | ASP.NET Core Identity + bcrypt | 8.0 | NFR-008 — Password hashing with bcrypt cost factor 12 |
| Email Service | SendGrid | Free tier | NFR-012 — Transactional email for verification; 100/day free |
| Database | PostgreSQL | 16.x | TR-004 — Auth schema for user registration data |
| ORM | Entity Framework Core | 8.0 | TR-004 — Data access for user creation and token management |

---

## Task Overview

Implement the patient self-registration backend API including registration endpoint with input validation, SendGrid email verification with time-limited tokens, account activation endpoint, duplicate email detection with generic error responses, and bcrypt password hashing. Includes rate-limited verification email resend functionality.

## Dependent Tasks

- task_001_backend_api_scaffolding (US_002) — Auth service project must exist
- task_001_core_domain_schemas (US_004) — Auth schema (Users, Roles) must exist

## Impacted Components

- New: `src/Services/Auth/HealthPlatform.Auth.Api/Controllers/RegistrationController.cs` — Registration API endpoints
- New: `src/Services/Auth/HealthPlatform.Auth.Api/Services/RegistrationService.cs` — Registration business logic
- New: `src/Services/Auth/HealthPlatform.Auth.Api/Services/EmailVerificationService.cs` — Token generation and verification
- New: `src/Services/Auth/HealthPlatform.Auth.Api/Services/SendGridEmailService.cs` — SendGrid email delivery
- New: `src/Services/Auth/HealthPlatform.Auth.Api/Models/RegisterRequest.cs` — Registration request DTO
- New: `src/Services/Auth/HealthPlatform.Auth.Api/Validators/RegisterRequestValidator.cs` — FluentValidation rules

## Implementation Plan

1. Create registration endpoint accepting name, email, phone, DOB, password with FluentValidation input validation.
2. Implement bcrypt password hashing with cost factor 12 — ensure plaintext is never logged or stored.
3. Implement duplicate email detection — check existing users, return generic "registration failed" regardless of whether email exists.
4. Implement SendGrid email verification — generate cryptographic token (24-hour expiry), send verification email within 30 seconds.
5. Create account activation endpoint — validate token, update status to "active", assign patient role.
6. Add resend verification endpoint with rate limiting (60-second cooldown, 3 per hour per email address).

## Current Project State

```text
src/Services/Auth/HealthPlatform.Auth.Api/
├── Controllers/
├── Services/
├── Repositories/
├── Data/
│   ├── AuthDbContext.cs
│   ├── Entities/
│   │   ├── User.cs
│   │   ├── Role.cs
│   │   ├── UserRole.cs
│   │   ├── RefreshToken.cs
│   │   └── LoginAttempt.cs
│   └── Migrations/
├── Program.cs
└── appsettings.json
```

## Expected Changes

| Action | File Path | Description |
|--------|-----------|-------------|
| CREATE | `src/Services/Auth/HealthPlatform.Auth.Api/Controllers/RegistrationController.cs` | Register, verify, activate, resend endpoints |
| CREATE | `src/Services/Auth/HealthPlatform.Auth.Api/Services/RegistrationService.cs` | Registration logic with duplicate check and bcrypt hashing |
| CREATE | `src/Services/Auth/HealthPlatform.Auth.Api/Services/EmailVerificationService.cs` | Token generation (24h expiry), validation, and revocation |
| CREATE | `src/Services/Auth/HealthPlatform.Auth.Api/Services/SendGridEmailService.cs` | SendGrid API integration for verification emails |
| CREATE | `src/Services/Auth/HealthPlatform.Auth.Api/Models/RegisterRequest.cs` | Registration request DTO with validation attributes |
| CREATE | `src/Services/Auth/HealthPlatform.Auth.Api/Models/RegisterResponse.cs` | Registration response DTO |
| CREATE | `src/Services/Auth/HealthPlatform.Auth.Api/Validators/RegisterRequestValidator.cs` | FluentValidation for password complexity and email format |

## External References

- [ASP.NET Core 8.0 Minimal APIs](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/overview?view=aspnetcore-8.0)
- [BCrypt.Net-Next NuGet](https://github.com/BcryptNet/bcrypt.net)
- [SendGrid C# SDK](https://docs.sendgrid.com/for-developers/sending-email/quickstart-csharp)
- [FluentValidation for ASP.NET Core](https://docs.fluentvalidation.net/en/latest/aspnet.html)

## Build Commands

- Refer to [.propel/build/](.propel/build/) for applicable technology stack build commands

## Implementation Validation Strategy

- [x] Unit tests pass — bcrypt hashing, token generation/validation, duplicate detection
- [x] Integration tests pass — registration flow creates user with correct status

## Implementation Checklist

- [x] Create registration API endpoint with FluentValidation input validation → AC-1
- [x] Implement SendGrid email verification with 24-hour token expiry, sent within 30 seconds → AC-2
- [x] Create account activation endpoint (token validation, status to "active", patient role assigned) → AC-3
- [x] Implement duplicate email detection with generic "registration failed" response → AC-4
- [x] Implement bcrypt password hashing with cost factor 12 (plaintext never logged or stored) → AC-5
- [x] Add resend verification endpoint with 60-second cooldown and 3/hour rate limit → Edge case
