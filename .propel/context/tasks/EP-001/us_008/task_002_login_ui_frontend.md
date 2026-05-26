# Task - task_002_login_ui_frontend

## Requirement Reference

- **User Story:** us_008
- **Story Location:** `.propel/context/tasks/EP-001/us_008/us_008.md`
- **Acceptance Criteria:**
  - AC-1: Role hierarchy with 4 tiers — login authenticates and receives JWT with role claims
  - AC-2: JWT access token — frontend stores and sends token in Authorization header
  - AC-3: Refresh token rotation — frontend HTTP interceptor handles expired tokens by calling refresh endpoint
  - AC-5: Login endpoint with rate limiting — display account lockout message with retry timer on 429 response
- **Edge Cases:**
  - Refresh token reuse detected → handle forced re-authentication redirect
  - Clock skew → handled by backend; frontend respects 401 responses

---

## Design References

| Reference Type | Value |
|----------------|-------|
| **UI Impact** | Yes |
| **Figma URL** | N/A |
| **Wireframe Status** | N/A |
| **Wireframe Type** | N/A |
| **Wireframe Path/URL** | N/A |
| **Screen Spec** | SCR-LOGIN-001 |
| **UXR Requirements** | Login form with email/password, "Remember me" checkbox, forgot password link, loading spinner during auth |
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
| Frontend Framework | Angular | 17 | TR-001 — Standalone components; reactive forms for login |
| Frontend UI Library | Angular Material | 17.x | TR-001 — WCAG 2.1 AA form controls |
| Frontend State | RxJS | 7.x | TR-001 — Reactive auth state management |
| Language | TypeScript | 5.x | TR-001 — Type-safe development |

---

## Task Overview

Build the login page as a standalone Angular 17 component with email/password form, "Remember me" checkbox, forgot password link, and loading spinner. Implement JWT token storage, HTTP interceptor for automatic Bearer token injection and refresh token rotation, account lockout display with retry timer, and forced re-authentication handling.

## Dependent Tasks

- task_001_frontend_scaffolding (US_001) — Angular 17 project must exist
- task_001_rbac_session_backend (US_008) — Auth API endpoints must exist

## Impacted Components

- New: `src/app/features/auth/pages/login/login.component.ts` — Login page standalone component
- New: `src/app/features/auth/pages/login/login.component.html` — Login form template
- New: `src/app/features/auth/pages/login/login.component.scss` — Login page styles
- New: `src/app/core/interceptors/auth.interceptor.ts` — JWT injection and refresh interceptor
- New: `src/app/core/services/token-storage.service.ts` — Secure token storage
- Modify: `src/app/features/auth/services/auth.service.ts` — Add login, refresh, logout methods

## Implementation Plan

1. Create the login standalone component with email/password reactive form, "Remember me" checkbox, and forgot password link.
2. Implement loading spinner during authentication API call.
3. Create JWT token storage service — secure storage with "Remember me" persistence toggle (localStorage vs. sessionStorage).
4. Implement HTTP interceptor for automatic Bearer token injection on all API requests.
5. Add token refresh logic in interceptor — on 401 response, attempt refresh; on refresh failure, redirect to login.
6. Display account lockout message with retry countdown timer on 429 response.

## Current Project State

```text
src/app/
├── app.component.ts
├── app.config.ts
├── app.routes.ts
├── core/
│   ├── interceptors/
│   └── guards/
├── shared/
│   ├── components/
│   └── services/
└── features/
    └── auth/
        ├── auth.routes.ts
        ├── pages/
        │   ├── registration/
        │   ├── verify-email/
        │   └── activate/
        └── services/
            └── auth.service.ts
```

## Expected Changes

| Action | File Path | Description |
|--------|-----------|-------------|
| CREATE | `src/app/features/auth/pages/login/login.component.ts` | Login form with email/password, remember me, loading spinner |
| CREATE | `src/app/features/auth/pages/login/login.component.html` | Login template with lockout message and retry timer |
| CREATE | `src/app/features/auth/pages/login/login.component.scss` | Login page styles |
| CREATE | `src/app/core/interceptors/auth.interceptor.ts` | Bearer token injection, 401 refresh, 429 lockout handling |
| CREATE | `src/app/core/services/token-storage.service.ts` | Secure JWT/refresh token storage with remember-me toggle |
| MODIFY | `src/app/features/auth/services/auth.service.ts` | Add login(), refresh(), logout() methods |
| MODIFY | `src/app/features/auth/auth.routes.ts` | Add login route |
| MODIFY | `src/app/app.config.ts` | Register auth interceptor |

## External References

- [Angular 17 HTTP Interceptors](https://angular.io/guide/http-interceptor-use-cases)
- [Angular Material 17 Form Field](https://material.angular.io/components/form-field/overview)
- [Angular 17 Standalone Components](https://angular.io/guide/standalone-components)

## Build Commands

- Refer to [.propel/build/](.propel/build/) for applicable technology stack build commands

## Implementation Validation Strategy

- [x] Unit tests pass — form validation, interceptor token injection, refresh logic
- [x] Integration tests pass — login displays lockout on 429; refresh renews tokens

## Implementation Checklist

- [x] Create login page with email/password fields, "Remember me" checkbox, and forgot password link → AC-1, AC-5
- [x] Implement loading spinner during authentication API call → UXR
- [x] Create secure token storage service with remember-me persistence toggle → AC-2
- [x] Implement HTTP interceptor for Bearer token injection and refresh token rotation on 401 → AC-2, AC-3
- [x] Display account lockout message with retry countdown timer on 429 response → AC-5
- [x] Handle forced re-authentication redirect on refresh token rejection → Edge case
