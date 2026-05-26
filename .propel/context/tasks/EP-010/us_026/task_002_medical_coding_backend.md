# Task - task_002_medical_coding_backend

## Requirement Reference

- **User Story:** US_026
- **Story Location:** .propel/context/tasks/EP-010/us_026/us_026.md
- **Acceptance Criteria:**
  - AC-1: AI generates code suggestions from clinical notes — Gemini analyzes notes, suggests ICD-10 and CPT codes with confidence (0-1) and rationale within 30 seconds
  - AC-2: Code suggestions displayed with context — supporting text excerpts and alternative codes ranked
  - AC-3: Provider review and approval workflow — accept, reject (with reason), modify, add manual; all logged
  - AC-4: Code validation against payer rules — CMS rules, no duplicates, required modifiers, diagnosis supports procedure
  - AC-5: Coding accuracy feedback loop — track per provider/specialty, identify rejection patterns, refine Gemini prompts
- **Edge Cases:**
  - Clinical notes too brief → return suggestions with confidence < 0.5, display documentation warning
  - Rare condition → suggest closest parent code with qualifier, flag for specialist review
  - Encounter amended → re-run suggestions on amended notes, show diff vs previous

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
| **AIR Requirements** | AIR-003, AIR-004 |
| **AI Pattern** | Human-in-the-Loop (HITL) |
| **Prompt Template Path** | prompts/medical-coding/ |
| **Guardrails Config** | config/ai-guardrails/medical-coding.json |
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
| Backend | ASP.NET Core Web API | 8.0 | TR-002 — Minimal API for coding endpoints |
| Backend | Google Gemini API | Free tier (gemini-1.5-flash) | AIR-003, AIR-004 — ICD-10/CPT suggestion with structured output |
| Backend | Polly | 8.x | TR-006 — Circuit breaker for Gemini calls |
| Backend | Hangfire | 1.8.x | TR-007 — Background job for code suggestion generation |
| Backend | Entity Framework Core | 8.0 | TR-004 — MedicalCode and feedback persistence |
| Database | PostgreSQL | 16.x | TR-004 — Medical codes, coding decisions, feedback metrics |

---

## Task Overview

Implement the medical coding backend within the ClinicalIntelligence service. This includes Gemini AI-powered ICD-10 and CPT code suggestion generation from clinical notes with structured output, CMS validation rules engine, provider review action persistence with audit trail, coding accuracy feedback tracking per provider/specialty, and prompt refinement based on rejection patterns. Processing completes within 30 seconds of note completion.

## Dependent Tasks

- US_002/task_001 — Backend API scaffolding (solution structure, YARP gateway)
- US_005/task_001 — Clinical/audit schemas (medical code tables)

## Impacted Components

- `src/Services/ClinicalIntelligenceService/Services/CodingSuggestionService.cs` — New Gemini coding integration
- `src/Services/ClinicalIntelligenceService/Services/CmsValidationService.cs` — New CMS rules validation
- `src/Services/ClinicalIntelligenceService/Services/CodingFeedbackService.cs` — New feedback tracking
- `src/Services/ClinicalIntelligenceService/Endpoints/CodingEndpoints.cs` — New coding API
- `src/Services/ClinicalIntelligenceService/Jobs/CodingSuggestionJob.cs` — Background suggestion job
- `src/Services/ClinicalIntelligenceService/Prompts/` — Gemini prompt templates

## Implementation Plan

1. Define MedicalCode entity with codeType (ICD-10/CPT), confidence, supportingEvidence, status (Suggested/Confirmed/Rejected)
2. Create EF Core migration adding medical_codes and coding_feedback tables
3. Create Gemini prompt templates for ICD-10 and CPT suggestion with evidence extraction
4. Build CodingSuggestionService with Gemini structured output, circuit breaker, and evidence snippet extraction
5. Implement CmsValidationService with rules: valid combinations, no duplicates, required modifiers, diagnosis-procedure support
6. Build CodingFeedbackService tracking per-provider accuracy metrics and rejection pattern analysis
7. Create CodingSuggestionJob (Hangfire) triggered on encounter note completion
8. Build CodingEndpoints: GET /coding/{encounterId}/suggestions, POST /coding/{encounterId}/decision, GET /coding/feedback

## Current Project State

```text
src/
├── Services/
│   └── ClinicalIntelligenceService/
│       ├── Endpoints/
│       │   ├── PatientViewEndpoints.cs
│       │   ├── ConflictEndpoints.cs
│       │   ├── VerificationEndpoints.cs
│       │   └── CodingEndpoints.cs          ← NEW
│       ├── Services/
│       │   ├── CodingSuggestionService.cs  ← NEW
│       │   ├── CmsValidationService.cs     ← NEW
│       │   └── CodingFeedbackService.cs    ← NEW
│       ├── Jobs/
│       │   └── CodingSuggestionJob.cs      ← NEW
│       ├── Models/
│       │   └── MedicalCode.cs              ← NEW
│       └── Prompts/
│           ├── icd10-suggestion.json       ← NEW
│           └── cpt-suggestion.json         ← NEW
```

## Expected Changes

| Action | File Path | Description |
|--------|-----------|-------------|
| CREATE | src/Services/ClinicalIntelligenceService/Services/CodingSuggestionService.cs | Gemini-powered ICD-10/CPT suggestion with evidence extraction |
| CREATE | src/Services/ClinicalIntelligenceService/Services/CmsValidationService.cs | CMS rules validation: combinations, duplicates, modifiers |
| CREATE | src/Services/ClinicalIntelligenceService/Services/CodingFeedbackService.cs | Per-provider accuracy tracking and rejection pattern analysis |
| CREATE | src/Services/ClinicalIntelligenceService/Endpoints/CodingEndpoints.cs | GET suggestions, POST decision, GET feedback |
| CREATE | src/Services/ClinicalIntelligenceService/Jobs/CodingSuggestionJob.cs | Hangfire job triggered on note completion |
| CREATE | src/Services/ClinicalIntelligenceService/Prompts/icd10-suggestion.json | Gemini prompt for ICD-10 suggestions with structured output |
| CREATE | src/Services/ClinicalIntelligenceService/Prompts/cpt-suggestion.json | Gemini prompt for CPT suggestions with structured output |
| MODIFY | src/Services/ClinicalIntelligenceService/Data/ClinicalIntelligenceDbContext.cs | Add MedicalCode, CodingFeedback DbSets and migration |

## External References

- [Google Gemini Structured Output](https://ai.google.dev/gemini-api/docs/structured-output)
- [ICD-10-CM Code Reference](https://www.cms.gov/medicare/coding-billing/icd-10-codes)
- [CPT Code Reference](https://www.ama-assn.org/practice-management/cpt)

## Build Commands

- [Refer to applicable technology stack build commands](.propel/build/)

## Implementation Validation Strategy

- [ ] Unit tests pass for CodingSuggestionService (Gemini mock, evidence extraction, confidence scoring)
- [ ] Unit tests pass for CmsValidationService (rule violations, valid combinations)
- [ ] Integration tests pass (full suggestion-decision cycle, feedback tracking, prompt refinement trigger)

## Implementation Checklist

- [ ] Define MedicalCode entity and create EF Core migration for medical_codes and coding_feedback tables — maps to AC-1
- [ ] Create Gemini prompt templates for ICD-10 and CPT suggestion with structured output and evidence extraction — maps to AC-1
- [ ] Build CodingSuggestionService generating suggestions within 30 seconds with confidence and rationale — maps to AC-1, AC-2
- [ ] Implement CmsValidationService with rules for combinations, duplicates, modifiers, and diagnosis-procedure support — maps to AC-4
- [ ] Build coding decision endpoints (accept, reject with reason, modify, add manual) with audit logging — maps to AC-3
- [ ] Implement CodingFeedbackService tracking per-provider accuracy and rejection patterns — maps to AC-5
- [ ] Create CodingSuggestionJob (Hangfire) triggered on encounter note completion — maps to AC-1
- [ ] Handle low-confidence suggestions (< 0.5) and encounter amendment re-processing — maps to edge cases
