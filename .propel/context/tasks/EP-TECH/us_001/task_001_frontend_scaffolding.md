# Task - task_001_frontend_scaffolding

## Requirement Reference

- **User Story:** us_001
- **Story Location:** .propel/context/tasks/EP-TECH/us_001/us_001.md
- **Acceptance Criteria:**
  - AC-1: Angular 17 project initialized with Standalone Components, no NgModules, TypeScript strict mode
  - AC-2: Route-based lazy loading configured for Patient, Staff, Admin dashboards; initial bundle under 300KB gzipped
  - AC-3: Angular Material 17.x integrated with WCAG 2.1 AA-compliant custom theme
  - AC-4: PWA manifest with app name, icons, theme color; service worker caches static assets and visited pages
  - AC-5: ReactiveFormsModule importable in any standalone component; RxJS operators available for reactive state
- **Edge Cases:**
  - Initial bundle exceeds 300KB gzipped due to Angular Material → tree-shake unused components and verify with `ng build --stats-json`
  - Service worker conflicts with dev environment hot reload → disable service worker in development mode via environment flag
  - Multiple developers scaffolding simultaneously → lock project init to a single CI pipeline step

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
| Frontend | Angular | 17 | TR-001 — Spec-mandated frontend framework with Standalone Components and Reactive Forms |
| Frontend | Angular Material | 17.x | TR-001 — WCAG 2.1 AA accessible component library for HIPAA-grade form controls |
| Frontend | RxJS | 7.x | TR-001 — Reactive state management bundled with Angular 17 |
| Frontend | TypeScript | 5.x | TR-001 — Strict mode enabled for type safety |
| Frontend | Angular PWA (@angular/pwa) | 17.x | TR-013 — Service worker and manifest for offline caching |
| Frontend | Angular CLI | 17.x | TR-001 — Project scaffolding and build tooling |

---

## Task Overview

Scaffold the Angular 17 frontend project from scratch with Standalone Components architecture (no NgModules), three lazy-loaded role-based route groups (Patient, Staff, Admin), Angular Material 17.x with a WCAG 2.1 AA-compliant custom theme, PWA support with service worker and manifest, and global availability of ReactiveFormsModule and RxJS. This establishes the frontend foundation upon which all feature epics build their UI components.

## Dependent Tasks

- None — this is the first task in the project (green-field scaffolding)

## Impacted Components

- **src/app/app.component.ts** — Root standalone component (CREATE)
- **src/app/app.routes.ts** — Top-level route configuration with lazy loading (CREATE)
- **src/app/app.config.ts** — Application configuration with providers (CREATE)
- **src/app/patient/** — Patient role lazy-loaded route module (CREATE)
- **src/app/staff/** — Staff role lazy-loaded route module (CREATE)
- **src/app/admin/** — Admin role lazy-loaded route module (CREATE)
- **src/styles.scss** — Global styles with Angular Material custom theme (CREATE)
- **src/manifest.webmanifest** — PWA manifest (CREATE)
- **ngsw-config.json** — Service worker caching configuration (CREATE)
- **src/environments/** — Environment-specific configuration files (CREATE)

## Implementation Plan

1. **Initialize Angular 17 project** — Run `ng new` with `--standalone --routing --style=scss --strict` flags. Verify no NgModules are generated; the `app.config.ts` uses `provideRouter()` and `provideAnimationsAsync()`.
2. **Configure TypeScript strict mode** — Confirm `tsconfig.json` has `strict: true`, `noImplicitAny: true`, `strictNullChecks: true`, `noUnusedLocals: true`.
3. **Create lazy-loaded route groups** — Define three lazy route entries in `app.routes.ts` using `loadChildren(() => import('./patient/patient.routes').then(m => m.PATIENT_ROUTES))` pattern for Patient, Staff, and Admin. Create each routes file as a standalone `Routes` array export.
4. **Install and configure Angular Material** — Run `ng add @angular/material@17`. Select a custom theme. Configure `styles.scss` using `@use '@angular/material' as mat` with custom palettes ensuring primary/accent/warn colors all meet WCAG 2.1 AA contrast ratio (≥ 4.5:1 for normal text, ≥ 3:1 for large text).
5. **Add PWA support** — Run `ng add @angular/pwa@17`. Configure `manifest.webmanifest` with app name ("Unified Patient Access Platform"), icons (192px, 512px), theme color, and background color. Configure `ngsw-config.json` with `assetGroups` for static assets (prefetch) and `dataGroups` for API responses (performance strategy with TTL).
6. **Configure environment-based service worker toggle** — In `app.config.ts`, conditionally register the service worker only when `environment.production` is true. Create `src/environments/environment.ts` (dev, SW disabled) and `src/environments/environment.prod.ts` (prod, SW enabled).
7. **Ensure ReactiveFormsModule availability** — Import `provideReactiveFormsSupport` (or `ReactiveFormsModule`) in `app.config.ts` providers. Verify any standalone component can import `ReactiveFormsModule` directly in its `imports` array.
8. **Build and verify bundle size** — Run `ng build --configuration=production --stats-json`. Verify main bundle is under 300KB gzipped. If exceeding, tree-shake unused Material components by importing only required modules (e.g., `MatButtonModule` not full `MaterialModule`).

## Current Project State

```
(empty — green-field project, no codebase exists yet)
```

## Expected Changes

| Action | File Path | Description |
|--------|-----------|-------------|
| CREATE | angular.json | Angular CLI workspace configuration with production/development build configurations |
| CREATE | tsconfig.json | TypeScript compiler options with strict mode enabled |
| CREATE | tsconfig.app.json | App-specific TS config extending base tsconfig |
| CREATE | package.json | Project dependencies (Angular 17, Material 17.x, PWA) |
| CREATE | src/main.ts | Bootstrap standalone application using `bootstrapApplication()` |
| CREATE | src/app/app.config.ts | Application providers: router, animations, reactive forms, service worker |
| CREATE | src/app/app.component.ts | Root standalone component with `<router-outlet>` |
| CREATE | src/app/app.routes.ts | Top-level routes with 3 lazy-loaded feature groups |
| CREATE | src/app/patient/patient.routes.ts | Patient role route definitions (placeholder) |
| CREATE | src/app/staff/staff.routes.ts | Staff role route definitions (placeholder) |
| CREATE | src/app/admin/admin.routes.ts | Admin role route definitions (placeholder) |
| CREATE | src/styles.scss | Angular Material custom theme with WCAG 2.1 AA palettes |
| CREATE | src/manifest.webmanifest | PWA manifest with app metadata and icons |
| CREATE | ngsw-config.json | Service worker caching strategy configuration |
| CREATE | src/environments/environment.ts | Development environment (serviceWorker: false) |
| CREATE | src/environments/environment.prod.ts | Production environment (serviceWorker: true) |

## External References

- Angular 17 CLI documentation: https://v17.angular.io/cli
- Angular 17 Standalone Components guide: https://v17.angular.io/guide/standalone-components
- Angular 17 Lazy Loading guide: https://v17.angular.io/guide/lazy-loading-ngmodules
- Angular Material 17.x Getting Started: https://v17.material.angular.io/guide/getting-started
- Angular Material Theming: https://v17.material.angular.io/guide/theming
- Angular PWA guide: https://v17.angular.io/guide/service-worker-getting-started
- WCAG 2.1 AA Quick Reference: https://www.w3.org/WAI/WCAG21/quickref/

## Build Commands

- [Refer to applicable technology stack build commands](.propel/build/)

## Implementation Validation Strategy

- [ ] Unit tests pass — `ng test --watch=false --code-coverage`
- [ ] Production build succeeds — `ng build --configuration=production`
- [ ] Bundle size verified under 300KB gzipped via `source-map-explorer` or `webpack-bundle-analyzer`
- [ ] PWA manifest passes Lighthouse PWA audit
- [ ] Angular Material theme colors meet WCAG 2.1 AA contrast ratio (verified via contrast checker)

## Implementation Checklist

- [x] Initialize Angular 17 project with `--standalone --routing --style=scss --strict` and verify no NgModules exist (AC-1)
- [x] Configure three lazy-loaded route groups (Patient, Staff, Admin) producing separate chunks in `app.routes.ts` (AC-2)
- [x] Install Angular Material 17.x and create custom WCAG 2.1 AA-compliant theme in `styles.scss` (AC-3)
- [x] Add PWA manifest with app name, icons (192px, 512px), and theme color via `@angular/pwa` (AC-4)
- [x] Configure `ngsw-config.json` service worker to cache static assets and previously visited pages (AC-4)
- [x] Ensure ReactiveFormsModule is provided in `app.config.ts` and importable in standalone components; verify RxJS available (AC-5)
- [x] Add environment-based service worker toggle — disabled in dev, enabled in production (Edge Case)
- [x] Verify production build initial bundle is under 300KB gzipped; tree-shake unused Material modules if needed (AC-2, Edge Case)
