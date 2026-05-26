# Task - task_002_conflict_detection_backend

## Requirement Reference

- **User Story:** US_024
- **Story Location:** .propel/context/tasks/EP-009/us_024/us_024.md
- **Acceptance Criteria:**
  - AC-1: Automated conflict detection on data ingestion — compare new data vs existing, identify duplicates (fuzzy >85%), contradictions, logical inconsistencies
  - AC-2: AI-powered resolution suggestions — Gemini analyzes conflicts, generates recommendation with confidence, rationale, action
  - AC-3: Conflict dashboard for providers — sorted by severity, paginated, with patient context
  - AC-4: One-click resolution with audit trail — atomic resolution, conflicting record archived, audit logged
  - AC-5: Conflict prevention on entry — warning before save without blocking
- **Edge Cases:**
  - Same conflict from multiple processes → deduplicate using conflict signature hash
  - AI confidence below 0.5 → mark "Requires Manual Review", no auto-suggestion
  - Legally protected data → role-based visibility restriction

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
| **AIR Requirements** | AIR-005 |
| **AI Pattern** | Human-in-the-Loop (HITL) |
| **Prompt Template Path** | prompts/conflict-detection/ |
| **Guardrails Config** | config/ai-guardrails/conflict-detection.json |
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
| Backend | ASP.NET Core Web API | 8.0 | TR-002 — Minimal API for conflict endpoints |
| Backend | Google Gemini API | Free tier (gemini-1.5-flash) | AIR-005 — Semantic conflict analysis and resolution suggestions |
| Backend | Polly | 8.x | TR-006 — Circuit breaker for Gemini calls |
| Backend | Entity Framework Core | 8.0 | TR-004 — Conflict record persistence |
| Database | PostgreSQL | 16.x | TR-004 — DataConflict entity, conflict signatures |

---

## Task Overview

Implement the conflict detection backend within the ClinicalIntelligence service. This includes automated conflict detection triggered on data ingestion (fuzzy matching >85%, contradiction detection, logical inconsistency checks), Gemini AI-powered resolution suggestions with confidence scoring, atomic resolution with audit trail, conflict deduplication via signature hash, and real-time conflict prevention checks during data entry.

## Dependent Tasks

- US_002/task_001 — Backend API scaffolding (solution structure, YARP gateway)
- US_004/task_001 — Core domain schemas (patient tables)
- US_005/task_001 — Clinical/audit schemas (extracted data, document tables)

## Impacted Components

- `src/Services/ClinicalIntelligenceService/Services/ConflictDetectionService.cs` — New conflict detection logic
- `src/Services/ClinicalIntelligenceService/Services/ConflictResolutionService.cs` — New resolution with Gemini
- `src/Services/ClinicalIntelligenceService/Endpoints/ConflictEndpoints.cs` — New conflict API endpoints
- `src/Services/ClinicalIntelligenceService/Models/DataConflict.cs` — Conflict entity
- `src/Services/ClinicalIntelligenceService/Prompts/conflict-resolution.json` — Gemini prompt

## Implementation Plan

1. Define DataConflict entity with signature hash, severity, resolution status, and source references
2. Create EF Core migration adding data_conflicts table with unique constraint on signature hash
3. Implement ConflictDetectionService with fuzzy matching (>85%), contradiction detection, and logical inconsistency checks
4. Build ConflictResolutionService with Gemini AI for resolution suggestions (confidence, rationale, action)
5. Add conflict deduplication using SHA-256 signature hash of conflicting field+values
6. Build ConflictEndpoints: GET /conflicts, GET /conflicts/{id}, POST /conflicts/{id}/resolve, POST /conflicts/check
7. Implement atomic resolution with conflict archival, audit trail, and dashboard removal
8. Add real-time conflict prevention check endpoint for data entry validation

## Current Project State

```text
src/
├── Services/
│   └── ClinicalIntelligenceService/
│       ├── Endpoints/
│       │   ├── PatientViewEndpoints.cs
│       │   └── ConflictEndpoints.cs        ← NEW
│       ├── Services/
│       │   ├── PatientAggregationService.cs
│       │   ├── TimelineGenerationService.cs
│       │   ├── ClinicalAlertService.cs
│       │   ├── ConflictDetectionService.cs ← NEW
│       │   └── ConflictResolutionService.cs ← NEW
│       ├── Models/
│       │   ├── PatientView360.cs
│       │   └── DataConflict.cs             ← NEW
│       └── Prompts/
│           └── conflict-resolution.json    ← NEW
```

## Expected Changes

| Action | File Path | Description |
|--------|-----------|-------------|
| CREATE | src/Services/ClinicalIntelligenceService/Services/ConflictDetectionService.cs | Fuzzy matching, contradiction detection, logical inconsistency checks |
| CREATE | src/Services/ClinicalIntelligenceService/Services/ConflictResolutionService.cs | Gemini AI resolution suggestions with confidence scoring |
| CREATE | src/Services/ClinicalIntelligenceService/Endpoints/ConflictEndpoints.cs | GET /conflicts, POST /conflicts/{id}/resolve, POST /conflicts/check |
| CREATE | src/Services/ClinicalIntelligenceService/Models/DataConflict.cs | Conflict entity with signature hash, severity enum, source refs |
| CREATE | src/Services/ClinicalIntelligenceService/Prompts/conflict-resolution.json | Gemini prompt for resolution suggestion with structured output |
| MODIFY | src/Services/ClinicalIntelligenceService/Data/ClinicalIntelligenceDbContext.cs | Add DataConflict DbSet and migration |

## External References

- [Google Gemini Structured Output](https://ai.google.dev/gemini-api/docs/structured-output)
- [Fuzzy String Matching (Levenshtein)](https://en.wikipedia.org/wiki/Levenshtein_distance)
- [Polly Circuit Breaker](https://www.thepollyproject.org/2019/02/13/introducing-polly-v7/)

## Build Commands

- [Refer to applicable technology stack build commands](.propel/build/)

## Implementation Validation Strategy

- [ ] Unit tests pass for ConflictDetectionService (fuzzy match, contradiction, inconsistency)
- [ ] Unit tests pass for ConflictResolutionService (Gemini mock, confidence thresholds, audit)
- [ ] Integration tests pass (full detection-resolution cycle, deduplication, prevention check)

## Implementation Checklist

- [ ] Define DataConflict entity with signature hash and create EF Core migration — maps to AC-1
- [ ] Implement ConflictDetectionService with fuzzy matching (>85%), contradiction, and inconsistency detection — maps to AC-1
- [ ] Build ConflictResolutionService with Gemini AI suggestions (confidence, rationale, recommended action) — maps to AC-2
- [ ] Add conflict deduplication using SHA-256 signature hash preventing duplicate conflict records — maps to edge cases
- [ ] Build conflict dashboard endpoint returning severity-sorted, paginated conflicts — maps to AC-3
- [ ] Implement atomic resolution with conflict archival, audit trail logging, and dashboard removal — maps to AC-4
- [ ] Add real-time conflict prevention check endpoint for data entry warning without blocking — maps to AC-5
- [ ] Handle low-confidence AI suggestions (< 0.5) by marking "Requires Manual Review" — maps to edge cases
