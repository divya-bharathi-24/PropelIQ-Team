# Task - task_002_patient_360_backend

## Requirement Reference

- **User Story:** US_023
- **Story Location:** .propel/context/tasks/EP-009/us_023/us_023.md
- **Acceptance Criteria:**
  - AC-1: Unified dashboard aggregates cross-service data — fetch from Appointment, Intake, Document, ClinicalIntelligence, Notification services within 3 seconds
  - AC-2: AI-generated health timeline — Gemini generates chronological health narrative with significant events and trend patterns
  - AC-3: Clinical alerts and contraindication warnings — AI identifies drug interactions, allergy conflicts, overdue screenings, abnormal trends
  - AC-4: Real-time data freshness indicators — each section tracks last-updated timestamp and source (cache vs live)
  - AC-5: Data section graceful degradation — available sections render, unavailable show placeholder without full page failure
- **Edge Cases:**
  - AI timeline exceeds rate limit → deliver asynchronously, cache for 24 hours
  - Provider views while another edits → WebSocket notification of concurrent edit

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
| **AIR Requirements** | AIR-007, AIR-009 |
| **AI Pattern** | Human-in-the-Loop (HITL) |
| **Prompt Template Path** | prompts/patient-360/ |
| **Guardrails Config** | config/ai-guardrails/patient-360.json |
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
| Backend | ASP.NET Core Web API | 8.0 | TR-002 — Minimal API for aggregation endpoints |
| Backend | Google Gemini API | Free tier (gemini-1.5-flash) | AIR-007, AIR-009 — Health timeline and clinical alerts |
| Backend | Polly | 8.x | TR-006 — Circuit breaker per downstream service |
| Backend | Entity Framework Core | 8.0 | TR-004 — PatientView360 read model persistence |
| Cache | Upstash Redis | Free tier | TR-005 — CQRS read model cache for sub-200ms reads |
| Database | PostgreSQL | 16.x | TR-004 — PatientView360 entity, timeline cache |

---

## Task Overview

Implement the ClinicalIntelligence service aggregation endpoints for the 360-degree patient view. This includes cross-service data aggregation with Polly circuit breakers per downstream service, Gemini-powered health timeline generation and clinical alerts, CQRS read model in Redis for sub-200ms reads, freshness tracking per data source, and graceful degradation when individual services are unavailable.

## Dependent Tasks

- US_002/task_001 — Backend API scaffolding (solution structure, YARP gateway)
- US_004/task_001 — Core domain schemas (patient, appointment tables)
- US_005/task_001 — Clinical/audit schemas (document, extracted data tables)
- US_006/task_001 — Data operations compliance (Redis caching infrastructure)

## Impacted Components

- `src/Services/ClinicalIntelligenceService/` — New microservice project
- `src/Services/ClinicalIntelligenceService/Services/PatientAggregationService.cs` — Cross-service data aggregation
- `src/Services/ClinicalIntelligenceService/Services/TimelineGenerationService.cs` — Gemini health timeline
- `src/Services/ClinicalIntelligenceService/Services/ClinicalAlertService.cs` — Contraindication detection
- `src/Services/ClinicalIntelligenceService/Endpoints/PatientViewEndpoints.cs` — API endpoints

## Implementation Plan

1. Create ClinicalIntelligenceService microservice with DI, EF Core, Redis, Polly registration
2. Define PatientView360 domain entity and CQRS read model schema
3. Build PatientAggregationService calling downstream services with individual Polly circuit breakers
4. Implement TimelineGenerationService with Gemini structured output for chronological health narrative
5. Build ClinicalAlertService detecting drug interactions, allergy conflicts, and overdue screenings
6. Add Redis CQRS read model caching with freshness timestamps per section
7. Implement concurrent edit detection via WebSocket notification
8. Add graceful degradation returning partial data when individual services are unavailable

## Current Project State

```text
src/
├── Services/
│   └── ClinicalIntelligenceService/        ← NEW
│       ├── Endpoints/
│       │   └── PatientViewEndpoints.cs
│       ├── Services/
│       │   ├── PatientAggregationService.cs
│       │   ├── TimelineGenerationService.cs
│       │   └── ClinicalAlertService.cs
│       ├── Models/
│       │   └── PatientView360.cs
│       ├── Prompts/
│       │   ├── timeline.json
│       │   └── clinical-alerts.json
│       ├── Data/
│       │   └── ClinicalIntelligenceDbContext.cs
│       └── Program.cs
```

## Expected Changes

| Action | File Path | Description |
|--------|-----------|-------------|
| CREATE | src/Services/ClinicalIntelligenceService/Program.cs | Service entry with DI for EF Core, Redis, Polly, Gemini |
| CREATE | src/Services/ClinicalIntelligenceService/Services/PatientAggregationService.cs | Cross-service aggregation with per-service circuit breakers |
| CREATE | src/Services/ClinicalIntelligenceService/Services/TimelineGenerationService.cs | Gemini structured output for health timeline with 24h cache |
| CREATE | src/Services/ClinicalIntelligenceService/Services/ClinicalAlertService.cs | Drug interaction, allergy conflict, overdue screening detection |
| CREATE | src/Services/ClinicalIntelligenceService/Endpoints/PatientViewEndpoints.cs | GET /patient/{id}/360, GET /patient/{id}/timeline, GET /patient/{id}/alerts |
| CREATE | src/Services/ClinicalIntelligenceService/Prompts/timeline.json | Gemini prompt for chronological health narrative |
| MODIFY | src/ApiGateway/yarp.json | Add route cluster for ClinicalIntelligenceService |

## External References

- [Google Gemini Structured Output](https://ai.google.dev/gemini-api/docs/structured-output)
- [Polly Circuit Breaker](https://www.thepollyproject.org/2019/02/13/introducing-polly-v7/)
- [Upstash Redis .NET](https://docs.upstash.com/redis/sdks/dotnet)

## Build Commands

- [Refer to applicable technology stack build commands](.propel/build/)

## Implementation Validation Strategy

- [ ] Unit tests pass for PatientAggregationService (concurrent calls, circuit breaker fallback, partial data)
- [ ] Unit tests pass for TimelineGenerationService (Gemini mock, cache, rate limit handling)
- [ ] Integration tests pass (end-to-end aggregation, Redis caching, graceful degradation)

## Implementation Checklist

- [ ] Create ClinicalIntelligenceService project with DI for EF Core, Redis, Polly, and Gemini — maps to AC-1
- [ ] Build PatientAggregationService with forkJoin-style concurrent calls and per-service Polly circuit breakers — maps to AC-1, AC-5
- [ ] Implement TimelineGenerationService with Gemini structured output for chronological health narrative (24h cache) — maps to AC-2
- [ ] Build ClinicalAlertService detecting drug interactions, allergy conflicts, and overdue screenings — maps to AC-3
- [ ] Add Redis CQRS read model caching with per-section freshness timestamps — maps to AC-4
- [ ] Implement graceful degradation returning partial data with section-level unavailability markers — maps to AC-5
- [ ] Add concurrent edit detection via WebSocket notification for multi-provider access — maps to edge cases
- [ ] Configure YARP gateway route and implement patient view aggregation endpoints — maps to AC-1
