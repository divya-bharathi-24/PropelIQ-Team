# Task - task_001_ai_intake_frontend

## Requirement Reference

- **User Story:** US_016
- **Story Location:** .propel/context/tasks/EP-005/us_016/us_016.md
- **Acceptance Criteria:**
  - AC-1: Conversational intake initiated before appointment — chat-style interface with personalized first question
  - AC-2: AI generates contextual follow-up questions — response displayed within 3 seconds, max 10 questions
  - AC-5: Gemini API rate limiting and fallback — queue indicator, offer manual intake form if wait > 30s
- **Edge Cases:**
  - Patient provides medically alarming response → display emergency message from backend
  - Patient abandons intake mid-conversation → auto-save progress, resume on return within 48 hours
  - Gemini API returns malformed/empty response → display next predetermined question from backend

---

## Design References

| Reference Type | Value |
|----------------|-------|
| **UI Impact** | Yes |
| **Figma URL** | N/A |
| **Wireframe Status** | N/A |
| **Wireframe Type** | N/A |
| **Wireframe Path/URL** | N/A |
| **Screen Spec** | SCR-INTAKE-001 |
| **UXR Requirements** | Chat bubble interface with typing indicator; patient messages right-aligned, AI left-aligned; progress indicator showing estimated questions remaining; "Switch to form" button always visible |
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
| Frontend | Angular | 17 | TR-001 — Standalone components for chat module |
| Frontend | Angular Material | 17.x | TR-001 — WCAG 2.1 AA chat components |
| Frontend | RxJS | 7.x | TR-001 — Reactive stream for real-time chat messages |
| Frontend | TypeScript | 5.x | TR-001 — Type-safe chat message models |

---

## Task Overview

Implement the patient-facing AI conversational intake chat interface as an Angular 17 standalone component. The chat UI presents a bubble-style conversation with typing indicators during AI processing, a progress indicator for estimated questions remaining, and a persistent "Switch to form" button linking to the manual intake (US_017). Messages stream in real-time via HTTP responses from the backend intake service.

## Dependent Tasks

- US_001/task_001 — Frontend scaffolding must exist (Angular project, routing, shared module)

## Impacted Components

- `src/app/features/intake/` — New feature module directory
- `src/app/features/intake/ai-intake/` — New AI intake chat component
- `src/app/features/intake/services/intake.service.ts` — New service for intake API communication
- `src/app/features/intake/models/intake.model.ts` — New chat message and session interfaces
- `src/app/app.routes.ts` — Add intake route

## Implementation Plan

1. Create intake feature module directory with lazy-loaded route configuration
2. Define TypeScript interfaces for chat messages (ChatMessage, IntakeSession, IntakeResponse)
3. Implement IntakeService with methods: startSession(), sendResponse(), getSessionStatus(), switchToManual()
4. Build AiIntakeChatComponent with chat bubble layout — patient messages right-aligned, AI left-aligned
5. Add typing indicator animation component shown during AI processing (3-second expected response)
6. Implement progress indicator showing estimated questions remaining (max 10 per session)
7. Add persistent "Switch to form" button that navigates to manual intake (US_017) preserving session data
8. Handle edge cases: emergency response display, session resume on return, rate-limit queue indicator

## Current Project State

```text
src/
├── app/
│   ├── app.component.ts
│   ├── app.routes.ts
│   ├── core/
│   ├── shared/
│   └── features/
│       └── intake/                    ← NEW
│           ├── ai-intake/
│           │   ├── ai-intake.component.ts
│           │   ├── ai-intake.component.html
│           │   └── ai-intake.component.scss
│           ├── services/
│           │   └── intake.service.ts
│           └── models/
│               └── intake.model.ts
```

## Expected Changes

| Action | File Path | Description |
|--------|-----------|-------------|
| CREATE | src/app/features/intake/models/intake.model.ts | Chat message, intake session, and response type interfaces |
| CREATE | src/app/features/intake/services/intake.service.ts | HTTP service for intake API (start, respond, status, switch) |
| CREATE | src/app/features/intake/ai-intake/ai-intake.component.ts | Main chat component with message rendering and input handling |
| CREATE | src/app/features/intake/ai-intake/ai-intake.component.html | Chat bubble template with typing indicator and progress bar |
| CREATE | src/app/features/intake/ai-intake/ai-intake.component.scss | Chat bubble styles, alignment, animation for typing indicator |
| MODIFY | src/app/app.routes.ts | Add lazy-loaded route for intake feature |

## External References

- [Angular 17 Standalone Components](https://angular.io/guide/standalone-components)
- [Angular Material 17 Components](https://material.angular.io/components/categories)
- [RxJS 7.x Documentation](https://rxjs.dev/guide/overview)

## Build Commands

- [Refer to applicable technology stack build commands](.propel/build/)

## Implementation Validation Strategy

- [ ] Unit tests pass for IntakeService (mock HTTP responses)
- [ ] Unit tests pass for AiIntakeChatComponent (message rendering, typing indicator toggle)
- [ ] Integration tests pass (route lazy-loading, session persistence)

## Implementation Checklist

- [ ] Create intake feature module with lazy-loaded route in app.routes.ts — maps to AC-1
- [ ] Implement chat message interfaces and IntakeService with startSession/sendResponse methods — maps to AC-1, AC-2
- [ ] Build chat bubble UI with patient messages right-aligned and AI messages left-aligned — maps to AC-1
- [ ] Add typing indicator animation displayed during AI response processing — maps to AC-2
- [ ] Implement progress indicator showing estimated questions remaining out of max 10 — maps to AC-2
- [ ] Add persistent "Switch to form" button that preserves session data and navigates to manual intake — maps to AC-5
- [ ] Handle rate-limit queue indicator when backend signals wait > 30 seconds — maps to AC-5
- [ ] Implement session resume logic for abandoned conversations within 48-hour window — maps to edge cases
