# Task - task_001_rbac_session_backend

## Requirement Reference

- **User Story:** us_008
- **Story Location:** `.propel/context/tasks/EP-001/us_008/us_008.md`
- **Acceptance Criteria:**
  - AC-1: Role hierarchy with 4 tiers enforced — Patient, Staff, Provider, Admin; JWT contains role claims; API Gateway enforces access with higher roles inheriting lower-role permissions
  - AC-2: JWT access token with short expiry — 15-minute expiry, contains user ID/email/roles, signed with RS256 using 2048-bit RSA key
  - AC-3: Refresh token rotation — new access + refresh tokens issued; old refresh token invalidated (single-use); 7-day sliding expiry
  - AC-4: Concurrent session limit enforced — max 3 devices; 4th login terminates oldest session, revokes its refresh token, sends notification
  - AC-5: Login endpoint with rate limiting — 5 failed attempts in 15 minutes → 30-minute lock; notification to account email; 429 with retry-after header
- **Edge Cases:**
  - Refresh token reuse (replay attack) → revoke ALL tokens for user; force re-authentication; log security event
  - Clock skew between services → 30-second leeway on token expiry validation
  - Role changed while session active → existing valid until expiry; updated on next refresh

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
| Backend Framework | ASP.NET Core Web API | 8.0 | TR-002 — Minimal API for auth endpoints |
| Authentication | ASP.NET Core Identity + JWT | 8.0 | NFR-009 — RBAC enforcement; refresh token rotation |
| API Gateway | YARP | 2.1 | TR-003 — Centralized JWT validation and role enforcement |
| Cache | Upstash Redis | Free tier | TR-005 — Session token storage and revocation tracking |
| Database | PostgreSQL | 16.x | TR-004 — Auth schema for tokens and login attempts |
| Email Service | SendGrid | Free tier | NFR-012 — Account lockout notifications |

---

## Task Overview

Implement the complete RBAC and session management backend including 4-tier role hierarchy with JWT claims, RS256-signed access tokens with 15-minute expiry, refresh token rotation with single-use invalidation, concurrent session limiting (max 3 devices), and login rate limiting with 30-minute account lockout. Includes replay attack detection, clock skew tolerance, and security event logging.

## Dependent Tasks

- task_001_backend_api_scaffolding (US_002) — Backend API and gateway must exist
- task_001_core_domain_schemas (US_004) — Auth schema (Users, Roles, RefreshTokens, LoginAttempts) must exist

## Impacted Components

- New: `src/Services/Auth/HealthPlatform.Auth.Api/Controllers/AuthController.cs` — Login, refresh, logout endpoints
- New: `src/Services/Auth/HealthPlatform.Auth.Api/Services/TokenService.cs` — JWT generation and validation
- New: `src/Services/Auth/HealthPlatform.Auth.Api/Services/SessionService.cs` — Session tracking and limits
- New: `src/Services/Auth/HealthPlatform.Auth.Api/Services/RateLimitService.cs` — Login rate limiting logic
- Modify: `src/Gateway/HealthPlatform.Gateway/Program.cs` — Add role-based route authorization policies

## Implementation Plan

1. Implement JWT token generation with RS256 signing (2048-bit RSA key), 15-minute expiry, user ID/email/role claims.
2. Implement 4-tier role hierarchy (Patient < Staff < Provider < Admin) with inheritance-based authorization policies.
3. Implement refresh token rotation — generate new token pair, invalidate old refresh token, detect reuse as replay attack.
4. Configure YARP gateway with role-based authorization policies for downstream service routes.
5. Implement concurrent session tracking via Redis — max 3 sessions per user; terminate oldest on 4th login.
6. Implement login rate limiting — track failed attempts per account; lock after 5 failures in 15 minutes; 429 with retry-after.
7. Add clock skew leeway (30 seconds) on token validation across all services.
8. Implement security event logging for replay attacks, lockouts, and session terminations.

## Current Project State

```text
src/
├── Gateway/HealthPlatform.Gateway/
│   ├── Program.cs (YARP configured)
│   └── appsettings.json
├── Services/Auth/HealthPlatform.Auth.Api/
│   ├── Controllers/
│   │   └── RegistrationController.cs
│   ├── Services/
│   │   ├── RegistrationService.cs
│   │   ├── EmailVerificationService.cs
│   │   └── SendGridEmailService.cs
│   ├── Data/
│   │   ├── AuthDbContext.cs
│   │   └── Entities/ (User, Role, RefreshToken, LoginAttempt)
│   └── Program.cs
```

## Expected Changes

| Action | File Path | Description |
|--------|-----------|-------------|
| CREATE | `src/Services/Auth/HealthPlatform.Auth.Api/Controllers/AuthController.cs` | Login, refresh, logout API endpoints |
| CREATE | `src/Services/Auth/HealthPlatform.Auth.Api/Services/TokenService.cs` | JWT generation (RS256, 15-min), refresh token rotation |
| CREATE | `src/Services/Auth/HealthPlatform.Auth.Api/Services/ITokenService.cs` | Token service interface |
| CREATE | `src/Services/Auth/HealthPlatform.Auth.Api/Services/SessionService.cs` | Concurrent session tracking (max 3) using Redis |
| CREATE | `src/Services/Auth/HealthPlatform.Auth.Api/Services/RateLimitService.cs` | Login attempt tracking, 30-minute lockout |
| CREATE | `src/Services/Auth/HealthPlatform.Auth.Api/Models/LoginRequest.cs` | Login request DTO |
| CREATE | `src/Services/Auth/HealthPlatform.Auth.Api/Models/TokenResponse.cs` | JWT + refresh token response DTO |
| MODIFY | `src/Gateway/HealthPlatform.Gateway/Program.cs` | Add role-based authorization policies to YARP routes |

## External References

- [ASP.NET Core JWT Bearer Authentication](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/?view=aspnetcore-8.0)
- [RSA Key Generation for JWT](https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.rsa)
- [YARP Authorization Policies](https://microsoft.github.io/reverse-proxy/articles/authn-authz.html)
- [Upstash Redis Session Management](https://upstash.com/docs/redis/overall/getstarted)

## Build Commands

- Refer to [.propel/build/](.propel/build/) for applicable technology stack build commands

## Implementation Validation Strategy

- [x] Unit tests pass — JWT generation/validation, role hierarchy, replay detection, rate limiting
- [x] Integration tests pass — login flow produces valid tokens; refresh rotation works; lockout triggers

## Implementation Checklist

- [x] Implement 4-tier role hierarchy (Patient < Staff < Provider < Admin) with JWT role claims and gateway enforcement → AC-1
- [x] Configure JWT access token with 15-minute expiry, RS256 signing (2048-bit RSA key), user ID/email/roles → AC-2
- [x] Implement refresh token rotation with single-use invalidation and 7-day sliding expiry → AC-3
- [x] Implement concurrent session limit (max 3 devices) with oldest session termination and notification → AC-4
- [x] Implement login rate limiting (5 failures/15 min → 30-min lock, email notification, 429 with retry-after) → AC-5
- [x] Add refresh token reuse detection — revoke ALL tokens and force re-authentication → Edge case
- [x] Configure 30-second clock skew leeway on token expiry validation → Edge case
- [x] Log security events (replay attacks, lockouts, forced terminations) to audit log → AC-1
