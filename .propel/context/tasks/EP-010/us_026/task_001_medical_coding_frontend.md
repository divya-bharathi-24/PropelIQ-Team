# Task - task_001_medical_coding_frontend

## Requirement Reference

- **User Story:** US_026
- **Story Location:** .propel/context/tasks/EP-010/us_026/us_026.md
- **Acceptance Criteria:**
  - AC-2: Code suggestions displayed with context — code number, description, confidence, supporting text excerpt highlighted, alternative codes ranked
  - AC-3: Provider review and approval workflow — accept, reject (with reason), modify (searchable code DB), add manual codes
  - AC-4: Code validation against payer rules — display warnings for invalid combinations, missing modifiers, unsupported diagnosis-procedure pairs
  - AC-5: Coding accuracy feedback loop — track AI accuracy metrics per provider/specialty
- **Edge Cases:**
  - Clinical notes too brief → display "Insufficient documentation" warning with low-confidence suggestions
  - Rare condition with no ICD-10 match → suggest closest parent code with qualifier

---

## Design References

| Reference Type | Value |
|----------------|-------|
| **UI Impact** | Yes |
| **Figma URL** | N/A |
| **Wireframe Status** | N/A |
| **Wireframe Type** | N/A |
| **Wireframe Path/URL** | N/A |
| **Screen Spec** | SCR-CODE-001 |
| **UXR Requirements** | Two-column layout (clinical notes left with highlighted excerpts, code suggestions right); drag codes from suggestion to accepted; confidence as percentage badge; searchable code lookup with autocomplete; validation warnings as inline alerts |
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
| Frontend | Angular | 17 | TR-001 — Standalone components for coding review module |
| Frontend | Angular Material | 17.x | TR-001 — Drag-drop CDK, autocomplete, badge, alert components |
| Frontend | RxJS | 7.x | TR-001 — Reactive code search, drag-drop streams |
| Frontend | TypeScript | 5.x | TR-001 — Type-safe medical code models |

---

## Task Overview

Implement the medical coding review interface with a two-column layout (clinical notes with highlighted excerpts on the left, AI code suggestions on the right), drag-to-accept functionality, searchable ICD-10/CPT code lookup with autocomplete, CMS validation rule warnings as inline alerts, confidence percentage badges, and provider accuracy feedback tracking. Supports accept, reject, modify, and manual code addition workflows.

## Dependent Tasks

- US_001/task_001 — Frontend scaffolding (Angular project, routing, shared module)

## Impacted Components

- `src/app/features/coding/` — New medical coding feature module
- `src/app/features/coding/review/` — Two-column coding review component
- `src/app/features/coding/code-search/` — Searchable code lookup with autocomplete
- `src/app/features/coding/validation/` — Inline validation warnings component
- `src/app/features/coding/services/coding.service.ts` — Coding API service

## Implementation Plan

1. Define TypeScript interfaces for MedicalCode, CodeSuggestion, CodingDecision, ValidationWarning, and CodeType enum
2. Implement CodingService with suggestion retrieval, decision submission, code search, and validation methods
3. Build CodingReviewComponent with two-column layout: clinical notes (left) with text highlighting, suggestions (right)
4. Add drag-from-suggestion-to-accepted functionality using Angular CDK DragDrop
5. Build CodeSearchComponent with searchable ICD-10/CPT autocomplete for manual code addition
6. Build ValidationWarningsComponent showing CMS rule violations as inline alerts
7. Add confidence percentage badges and accuracy metrics display per provider

## Current Project State

```text
src/
├── app/
│   └── features/
│       └── coding/                         ← NEW
│           ├── review/
│           │   ├── coding-review.component.ts
│           │   ├── coding-review.component.html
│           │   └── coding-review.component.scss
│           ├── code-search/
│           │   └── code-search.component.ts
│           ├── validation/
│           │   └── validation-warnings.component.ts
│           ├── services/
│           │   └── coding.service.ts
│           └── models/
│               └── coding.model.ts
```

## Expected Changes

| Action | File Path | Description |
|--------|-----------|-------------|
| CREATE | src/app/features/coding/models/coding.model.ts | MedicalCode, CodeSuggestion, CodingDecision, ValidationWarning interfaces |
| CREATE | src/app/features/coding/services/coding.service.ts | HTTP service for suggestions, decisions, search, validation |
| CREATE | src/app/features/coding/review/coding-review.component.ts | Two-column layout with notes highlighting and drag-to-accept |
| CREATE | src/app/features/coding/review/coding-review.component.scss | Two-column styles, highlighting, drag-drop zones |
| CREATE | src/app/features/coding/code-search/code-search.component.ts | Searchable ICD-10/CPT autocomplete with code descriptions |
| CREATE | src/app/features/coding/validation/validation-warnings.component.ts | Inline CMS validation warnings with severity indicators |
| MODIFY | src/app/app.routes.ts | Add lazy-loaded route for coding feature |

## External References

- [Angular CDK Drag and Drop](https://material.angular.io/cdk/drag-drop/overview)
- [Angular Material Autocomplete](https://material.angular.io/components/autocomplete/overview)
- [Angular Material Badge](https://material.angular.io/components/badge/overview)

## Build Commands

- [Refer to applicable technology stack build commands](.propel/build/)

## Implementation Validation Strategy

- [ ] Unit tests pass for CodingReviewComponent (drag-drop, accept/reject/modify actions)
- [ ] Unit tests pass for CodeSearchComponent (autocomplete, code selection)
- [ ] Integration tests pass (validation warnings, feedback tracking, route integration)

## Implementation Checklist

- [ ] Create coding feature module with lazy-loaded route and medical code TypeScript interfaces — maps to AC-2
- [ ] Build CodingReviewComponent with two-column layout showing clinical notes (highlighting) and AI suggestions — maps to AC-2
- [ ] Implement drag-to-accept from suggestion list with accept, reject (reason), modify actions — maps to AC-3
- [ ] Build CodeSearchComponent with searchable ICD-10/CPT autocomplete for manual code addition — maps to AC-3
- [ ] Build ValidationWarningsComponent showing CMS rule violations as inline alerts — maps to AC-4
- [ ] Add confidence percentage badges and AI accuracy metrics display per provider/specialty — maps to AC-2, AC-5
- [ ] Handle insufficient documentation warning with low-confidence display and parent code fallback — maps to edge cases
