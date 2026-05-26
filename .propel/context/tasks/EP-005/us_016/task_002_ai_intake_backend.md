# Task - task_002_ai_intake_backend

## Requirement Reference

- **User Story:** US_016
- **Story Location:** .propel/context/tasks/EP-005/us_016/us_016.md
- **Acceptance Criteria:**
  - AC-1: Conversational intake initiated before appointment — personalized first question based on appointment type
  - AC-2: AI generates contextual follow-up questions — Gemini processes response within 3 seconds, max 10 questions
  - AC-3: Structured data extraction from conversational responses — AI extracts symptoms, duration, severity, medications, allergies with confidence scores
  - AC-4: Intake completion persisted and available to provider — structured data and raw transcript stored, AI-extracted fields highlighted
  - AC-5: Gemini API rate limiting and fallback — queue within rate limits, offer manual intake if queue > 30s
- **Edge Cases:**
  - Patient provides medically alarming response → return emergency message, flag for urgent staff review
  - Gemini API returns malformed/empty response → retry once, then return next predetermined question
  - Patient abandons intake mid-conversation → save progress, expire incomplete intakes after appointment passes

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
| **AI Impact** | Yes |
| **AIR Requirements** | AIR-001 |
| **AI Pattern** | Human-in-the-Loop (HITL) |
| **Prompt Template Path** | prompts/intake/ |
| **Guardrails Config** | config/ai-guardrails/intake-safety.json |
| **Model Provider** | Google Gemini API (gemini-1.5-flash) |

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
| Backend | ASP.NET Core Web API | 8.0 | TR-002 — Minimal API pattern for intake endpoints |
| Backend | Entity Framework Core | 8.0 | TR-004 — Code-first intake schema persistence |
| Backend | Google Gemini API | Free tier (gemini-1.5-flash) | AIR-001 — Conversational intake with structured output mode |
| Backend | Polly | 8.x | TR-006 — Circuit breaker (3-failure, 60s recovery) for Gemini calls |
| Backend | Hangfire | 1.8.x | TR-007 — Background job for intake expiration cleanup |
| Database | PostgreSQL | 16.x | TR-004 — Intake record persistence with JSONB for conversation data |

---

## Task Overview

Implement the backend intake service for AI-powered conversational intake. This includes Gemini API integration with structured output mode for generating contextual follow-up questions, extracting structured clinical data from free-text responses, managing intake session lifecycle, and implementing rate limiting with fallback to manual intake. The service uses Polly circuit breaker for Gemini API resilience and Hangfire for intake expiration cleanup.

## Dependent Tasks

- US_002/task_001 — Backend API scaffolding (solution structure, YARP gateway, JWT)
- US_004/task_001 — Core domain schemas (patient, appointment tables for intake foreign keys)

## Impacted Components

- `src/Services/IntakeService/` — New microservice project
- `src/Services/IntakeService/Endpoints/IntakeEndpoints.cs` — Minimal API endpoints
- `src/Services/IntakeService/Services/GeminiIntakeService.cs` — Gemini API integration
- `src/Services/IntakeService/Services/IntakeSessionManager.cs` — Session lifecycle management
- `src/Services/IntakeService/Models/` — Request/response DTOs and domain entities
- `src/Services/IntakeService/Prompts/` — Versioned prompt templates
- `src/ApiGateway/yarp.json` — Route configuration for intake service

## Implementation Plan

1. Create IntakeService microservice project within the solution with DI, EF Core, and Polly registration
2. Define intake domain entities (IntakeSession, ConversationMessage, ExtractedIntakeData) and EF Core DbContext
3. Create EF Core migration for intake schema (intake_sessions, conversation_messages, extracted_data tables)
4. Implement versioned prompt templates for conversational intake (initial question, follow-up generation, data extraction)
5. Build GeminiIntakeService with structured output mode — question generation and data extraction endpoints
6. Implement IntakeSessionManager for session lifecycle (create, resume, complete, expire)
7. Configure Polly circuit breaker (3-failure threshold, 60s recovery) with fallback to predetermined questions
8. Add rate limiting middleware (12 RPM target to leave headroom) and YARP gateway route

## Current Project State

```text
src/
├── ApiGateway/
│   └── yarp.json
├── Services/
│   ├── AuthService/
│   ├── AppointmentService/
│   └── IntakeService/                 ← NEW
│       ├── Endpoints/
│       │   └── IntakeEndpoints.cs
│       ├── Services/
│       │   ├── GeminiIntakeService.cs
│       │   └── IntakeSessionManager.cs
│       ├── Models/
│       ├── Prompts/
│       ├── Data/
│       │   └── IntakeDbContext.cs
│       └── Program.cs
```

## Expected Changes

| Action | File Path | Description |
|--------|-----------|-------------|
| CREATE | src/Services/IntakeService/Program.cs | Service entry point with DI, EF Core, Polly, Hangfire registration |
| CREATE | src/Services/IntakeService/Endpoints/IntakeEndpoints.cs | Minimal API endpoints: POST /intake/start, POST /intake/respond, GET /intake/{id}/status |
| CREATE | src/Services/IntakeService/Services/GeminiIntakeService.cs | Gemini API client with structured output mode for question generation and data extraction |
| CREATE | src/Services/IntakeService/Services/IntakeSessionManager.cs | Session lifecycle: create, resume, complete, expire with auto-save |
| CREATE | src/Services/IntakeService/Models/IntakeSession.cs | Domain entity for intake session with conversation history |
| CREATE | src/Services/IntakeService/Prompts/intake-initial.json | Versioned prompt template for first question by appointment type |
| CREATE | src/Services/IntakeService/Prompts/intake-followup.json | Versioned prompt template for contextual follow-up generation |
| MODIFY | src/ApiGateway/yarp.json | Add route cluster for IntakeService endpoints |

## External References

- [Google Gemini API Structured Output](https://ai.google.dev/gemini-api/docs/structured-output)
- [Polly Circuit Breaker Documentation](https://www.thepollyproject.org/2019/02/13/introducing-polly-v7/)
- [ASP.NET Core 8 Minimal APIs](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis?view=aspnetcore-8.0)

## Build Commands

- [Refer to applicable technology stack build commands](.propel/build/)

## Implementation Validation Strategy

- [ ] Unit tests pass for GeminiIntakeService (mock Gemini responses, circuit breaker behavior)
- [ ] Unit tests pass for IntakeSessionManager (session lifecycle, resume, expiration)
- [ ] Integration tests pass (end-to-end intake flow, rate limiting, YARP routing)

## Implementation Checklist

- [ ] Create IntakeService project with DI registration for EF Core, Polly, and Hangfire — maps to AC-1
- [ ] Define intake domain entities and EF Core migration for intake schema — maps to AC-4
- [ ] Implement versioned prompt templates for initial question and contextual follow-ups — maps to AC-1, AC-2
- [ ] Build GeminiIntakeService with structured output mode for question generation (max 10 questions) — maps to AC-2
- [ ] Implement structured data extraction from conversation (symptoms, duration, severity, medications, allergies with confidence scores) — maps to AC-3
- [ ] Build IntakeSessionManager with create, resume, complete, and expire lifecycle including auto-save — maps to AC-4
- [ ] Configure Polly circuit breaker (3-failure, 60s recovery) with fallback to predetermined questions — maps to AC-5
- [ ] Add rate limiting (12 RPM) with queue management and manual intake fallback signal when wait > 30s — maps to AC-5
