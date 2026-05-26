# Task - task_001_document_processing_frontend

## Requirement Reference

- **User Story:** US_022
- **Story Location:** .propel/context/tasks/EP-008/us_022/us_022.md
- **Acceptance Criteria:**
  - AC-2: AI-powered document summarization — structured summary displayed with key findings, dates, medications, recommendations
  - AC-4: Processing status visible to patient and provider — status indicator shows Uploaded → Processing → Text Extracted → Summarized → Complete with estimated time
- **Edge Cases:**
  - OCR fails on poor quality scan → display "manual review required" status
  - Gemini returns low-confidence summary → display "low confidence" warning badge prominently

---

## Design References

| Reference Type | Value |
|----------------|-------|
| **UI Impact** | Yes |
| **Figma URL** | N/A |
| **Wireframe Status** | N/A |
| **Wireframe Type** | N/A |
| **Wireframe Path/URL** | N/A |
| **Screen Spec** | SCR-DOC-002 |
| **UXR Requirements** | Processing status as stepper component; AI summary in collapsible card with "AI Generated" badge; extracted data as chips with accept/reject actions for provider |
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
| Frontend | Angular | 17 | TR-001 — Standalone components for document processing UI |
| Frontend | Angular Material | 17.x | TR-001 — Stepper, expansion panel, chip components |
| Frontend | RxJS | 7.x | TR-001 — Polling for processing status updates |
| Frontend | TypeScript | 5.x | TR-001 — Type-safe processing status models |

---

## Task Overview

Implement the frontend document processing status display and AI summary presentation. The document detail page shows a stepper component tracking pipeline stages (Uploaded → Processing → Text Extracted → Summarized → Complete), a collapsible AI-generated summary card with confidence badge, and extracted data chips with accept/reject actions for provider verification.

## Dependent Tasks

- US_001/task_001 — Frontend scaffolding (Angular project, routing, shared module)
- US_021/task_001 — Document upload frontend (document feature module, gallery base)

## Impacted Components

- `src/app/features/documents/processing/` — New processing status component
- `src/app/features/documents/summary/` — New AI summary card component
- `src/app/features/documents/extracted-data/` — New extracted data chips component
- `src/app/features/documents/services/document.service.ts` — Extend with processing status methods

## Implementation Plan

1. Define TypeScript interfaces for ProcessingStatus, AiSummary, ExtractedDataElement, ConfidenceLevel
2. Build ProcessingStatusComponent as Material Stepper showing pipeline stages with estimated time
3. Build AiSummaryCardComponent as collapsible card with "AI Generated" badge and confidence indicator
4. Build ExtractedDataChipsComponent with accept/reject actions for provider verification

## Current Project State

```text
src/
├── app/
│   └── features/
│       └── documents/
│           ├── upload/
│           ├── metadata/
│           ├── gallery/
│           ├── processing/                 ← NEW
│           │   ├── processing-status.component.ts
│           │   └── processing-status.component.html
│           ├── summary/                    ← NEW
│           │   ├── ai-summary-card.component.ts
│           │   └── ai-summary-card.component.html
│           ├── extracted-data/             ← NEW
│           │   ├── extracted-data-chips.component.ts
│           │   └── extracted-data-chips.component.html
│           ├── services/
│           │   └── document.service.ts
│           └── models/
│               └── document.model.ts
```

## Expected Changes

| Action | File Path | Description |
|--------|-----------|-------------|
| CREATE | src/app/features/documents/processing/processing-status.component.ts | Stepper showing Uploaded → Processing → Extracted → Summarized → Complete |
| CREATE | src/app/features/documents/summary/ai-summary-card.component.ts | Collapsible card with AI badge, key findings, medications, recommendations |
| CREATE | src/app/features/documents/extracted-data/extracted-data-chips.component.ts | Chips with accept/reject for provider verification of extracted data |
| MODIFY | src/app/features/documents/services/document.service.ts | Add getProcessingStatus(), getSummary(), acceptExtraction(), rejectExtraction() |
| MODIFY | src/app/features/documents/models/document.model.ts | Add ProcessingStatus, AiSummary, ExtractedDataElement interfaces |

## External References

- [Angular Material Stepper](https://material.angular.io/components/stepper/overview)
- [Angular Material Chips](https://material.angular.io/components/chips/overview)
- [Angular Material Expansion Panel](https://material.angular.io/components/expansion/overview)

## Build Commands

- [Refer to applicable technology stack build commands](.propel/build/)

## Implementation Validation Strategy

- [ ] Unit tests pass for ProcessingStatusComponent (stage transitions, estimated time display)
- [ ] Unit tests pass for AiSummaryCardComponent (summary rendering, confidence badge)
- [ ] Integration tests pass (status polling, accept/reject actions)

## Implementation Checklist

- [ ] Define ProcessingStatus, AiSummary, and ExtractedDataElement TypeScript interfaces — maps to AC-4
- [ ] Build ProcessingStatusComponent as stepper showing pipeline stages with estimated time remaining — maps to AC-4
- [ ] Build AiSummaryCardComponent as collapsible card with "AI Generated" badge and structured summary fields — maps to AC-2
- [ ] Build ExtractedDataChipsComponent with accept/reject actions for provider verification — maps to AC-2
