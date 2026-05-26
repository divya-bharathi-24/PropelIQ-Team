# Task - task_002_risk_scoring_backend

## Requirement Reference

- **User Story:** US_019
- **Story Location:** .propel/context/tasks/EP-007/us_019/us_019.md
- **Acceptance Criteria:**
  - AC-1: Risk score computed for every confirmed appointment — daily batch (6 AM), score 0-100 via Gemini structured output, factors: no-show rate, lead time, day/time, weather
  - AC-2: Risk categorization with thresholds — Low (0-30), Medium (31-60), High (61-100), only Medium/High flagged
  - AC-3: Risk factors explainability — top 3 factors with relative weights stored per score
  - AC-4: Risk score triggers reminder escalation — High risk: additional reminders at 48h and 6h, staff follow-up task if no confirmation by 12h
  - AC-5: Polly circuit breaker for Gemini API failures — 3 consecutive errors opens breaker for 60s, fallback to rule-based model, scores marked "estimated"
- **Edge Cases:**
  - New patient with no history → assign medium risk (50), historical model activates after 3 appointments
  - Patient confirms → re-score within 1 hour, update category if changed
  - Batch job during maintenance → retry after 30 minutes, alert on two consecutive failures

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
| **AIR Requirements** | AIR-006 |
| **AI Pattern** | Human-in-the-Loop (HITL) |
| **Prompt Template Path** | prompts/risk-scoring/ |
| **Guardrails Config** | config/ai-guardrails/risk-scoring.json |
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
| Backend | ASP.NET Core Web API | 8.0 | TR-002 — Minimal API for risk score endpoints |
| Backend | Hangfire | 1.8.x | TR-007 — Daily batch job at 6 AM for risk scoring |
| Backend | Google Gemini API | Free tier (gemini-1.5-flash) | AIR-006 — Structured output for risk score with factors |
| Backend | Polly | 8.x | TR-006 — Circuit breaker (3-failure, 60s recovery) for Gemini |
| Backend | Entity Framework Core | 8.0 | TR-004 — Risk score persistence |
| Database | PostgreSQL | 16.x | TR-004 — Risk scores and factor storage |
| Cache | Upstash Redis | Free tier | TR-005 — Cached risk scores for dashboard read performance |

---

## Task Overview

Implement the backend no-show risk scoring service with daily Hangfire batch processing, Gemini AI structured output for multi-factor risk computation, rule-based fallback model, Polly circuit breaker protection, risk categorization with escalation triggers, and re-scoring on patient confirmation events. Risk scores are cached in Redis for sub-200ms dashboard reads.

## Dependent Tasks

- US_002/task_001 — Backend API scaffolding (solution structure, YARP gateway)
- US_004/task_001 — Core domain schemas (appointment, patient tables)
- US_006/task_001 — Data operations compliance (Redis caching infrastructure)

## Impacted Components

- `src/Services/AppointmentService/Services/RiskScoringService.cs` — New risk scoring logic
- `src/Services/AppointmentService/Services/RiskFallbackModel.cs` — Rule-based fallback model
- `src/Services/AppointmentService/Jobs/DailyRiskScoringJob.cs` — Hangfire batch job
- `src/Services/AppointmentService/Endpoints/RiskEndpoints.cs` — Risk score API endpoints
- `src/Services/AppointmentService/Models/RiskScore.cs` — Risk score entity
- `src/Services/AppointmentService/Prompts/risk-scoring.json` — Gemini prompt template

## Implementation Plan

1. Define RiskScore entity (scoreId, appointmentId, score, category, factors JSON, isEstimated, computedAt)
2. Create EF Core migration adding risk_scores table
3. Implement Gemini prompt template for risk scoring with structured output schema (score, factors with weights)
4. Build RiskScoringService with Gemini integration, Polly circuit breaker, and factor extraction
5. Implement RiskFallbackModel using historical no-show percentage only (activated on circuit breaker open)
6. Create DailyRiskScoringJob (Hangfire, 6 AM) processing all confirmed appointments > 24h future
7. Add escalation logic: High risk → create additional reminder jobs and staff follow-up task
8. Implement re-scoring endpoint triggered by patient confirmation events

## Current Project State

```text
src/
├── Services/
│   └── AppointmentService/
│       ├── Endpoints/
│       │   ├── SlotEndpoints.cs
│       │   ├── QueueEndpoints.cs
│       │   └── RiskEndpoints.cs            ← NEW
│       ├── Services/
│       │   ├── SlotService.cs
│       │   ├── QueueManager.cs
│       │   ├── RiskScoringService.cs       ← NEW
│       │   └── RiskFallbackModel.cs        ← NEW
│       ├── Jobs/
│       │   └── DailyRiskScoringJob.cs      ← NEW
│       ├── Models/
│       │   └── RiskScore.cs                ← NEW
│       └── Prompts/
│           └── risk-scoring.json           ← NEW
```

## Expected Changes

| Action | File Path | Description |
|--------|-----------|-------------|
| CREATE | src/Services/AppointmentService/Models/RiskScore.cs | Risk score entity with factors JSONB, category enum, isEstimated flag |
| CREATE | src/Services/AppointmentService/Services/RiskScoringService.cs | Gemini-powered scoring with structured output and Polly circuit breaker |
| CREATE | src/Services/AppointmentService/Services/RiskFallbackModel.cs | Rule-based fallback using historical no-show percentage |
| CREATE | src/Services/AppointmentService/Jobs/DailyRiskScoringJob.cs | Hangfire recurring job at 6 AM processing confirmed appointments |
| CREATE | src/Services/AppointmentService/Endpoints/RiskEndpoints.cs | GET /risk/{appointmentId}, POST /risk/{appointmentId}/rescore |
| CREATE | src/Services/AppointmentService/Prompts/risk-scoring.json | Versioned Gemini prompt with structured output schema |
| MODIFY | src/Services/AppointmentService/Data/AppointmentDbContext.cs | Add RiskScore DbSet and migration |
| MODIFY | src/ApiGateway/yarp.json | Add route for risk score endpoints |

## External References

- [Google Gemini Structured Output](https://ai.google.dev/gemini-api/docs/structured-output)
- [Polly Circuit Breaker](https://www.thepollyproject.org/2019/02/13/introducing-polly-v7/)
- [Hangfire Recurring Jobs](https://docs.hangfire.io/en/latest/background-methods/performing-recurrent-tasks.html)

## Build Commands

- [Refer to applicable technology stack build commands](.propel/build/)

## Implementation Validation Strategy

- [ ] Unit tests pass for RiskScoringService (score computation, factor extraction, circuit breaker behavior)
- [ ] Unit tests pass for RiskFallbackModel (historical percentage calculation, new patient default)
- [ ] Integration tests pass (Hangfire job scheduling, escalation trigger, re-score on confirmation)

## Implementation Checklist

- [ ] Define RiskScore entity with factors JSONB and create EF Core migration — maps to AC-1
- [ ] Implement Gemini prompt template with structured output for score (0-100) and top 3 factors with weights — maps to AC-1, AC-3
- [ ] Build RiskScoringService with Polly circuit breaker (3-failure, 60s) and Gemini structured output — maps to AC-1, AC-5
- [ ] Implement risk categorization (Low 0-30, Medium 31-60, High 61-100) with persistence — maps to AC-2
- [ ] Create DailyRiskScoringJob (Hangfire, 6 AM daily) for all confirmed appointments > 24h future — maps to AC-1
- [ ] Implement RiskFallbackModel using historical no-show percentage, marking scores as "estimated" — maps to AC-5
- [ ] Add escalation logic for High risk: additional reminders at 48h/6h and staff follow-up task at 12h — maps to AC-4
- [ ] Implement re-scoring endpoint triggered within 1 hour of patient confirmation — maps to edge cases
