# Task - task_002_document_processing_backend

## Requirement Reference

- **User Story:** US_022
- **Story Location:** .propel/context/tasks/EP-008/us_022/us_022.md
- **Acceptance Criteria:**
  - AC-1: OCR text extraction on upload completion — Tesseract extracts text within 60 seconds for up to 10 pages, page-level segmentation, status "text_extracted"
  - AC-2: AI-powered document summarization — Gemini generates structured summary (key findings, dates, medications, recommendations) stored as JSON
  - AC-3: Extracted data linked to patient record — medications cross-referenced, conditions suggested, all flagged "AI-suggested" requiring verification
  - AC-4: Processing status visible — status progression Uploaded → Processing → Text Extracted → Summarized → Complete
  - AC-5: Gemini rate limit management — throttle to 12 RPM, queue overflow in FIFO order
- **Edge Cases:**
  - OCR fails on poor quality scan → mark "manual review required", notify patient
  - Document in unsupported language → detect language, mark for manual review
  - Gemini returns low-confidence summary (< 0.6) → flag, do not auto-suggest linkages

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
| **AIR Requirements** | AIR-002, AIR-008 |
| **AI Pattern** | Human-in-the-Loop (HITL) |
| **Prompt Template Path** | prompts/document-processing/ |
| **Guardrails Config** | config/ai-guardrails/document-processing.json |
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
| Backend | ASP.NET Core Web API | 8.0 | TR-002 — Minimal API for processing status endpoints |
| Backend | Hangfire | 1.8.x | TR-007 — Background pipeline for OCR and summarization |
| Backend | Google Gemini API | Free tier (gemini-1.5-flash) | AIR-002 — Structured output for document summarization and extraction |
| Backend | Tesseract.NET | 5.x | AIR-002 — OCR text extraction from PDF/image documents |
| Backend | Polly | 8.x | TR-006 — Circuit breaker for Gemini API calls |
| Backend | Entity Framework Core | 8.0 | TR-004 — OCR results and extracted data persistence |
| Database | PostgreSQL | 16.x | TR-004 — OcrResults, extracted elements, summaries |

---

## Task Overview

Implement the document processing pipeline as a Hangfire background job chain: OCR text extraction via Tesseract.NET, AI-powered summarization via Gemini API with structured output, patient record data linkage with medications/conditions cross-referencing, and status tracking through the full pipeline. The pipeline includes rate limit management (12 RPM throttle), circuit breaker protection, and quality flags for low-confidence extractions.

## Dependent Tasks

- US_021/task_002 — Document upload backend (DocumentService project, upload endpoints)
- US_002/task_001 — Backend API scaffolding (solution structure, YARP gateway)

## Impacted Components

- `src/Services/DocumentService/Services/OcrExtractionService.cs` — New Tesseract OCR integration
- `src/Services/DocumentService/Services/AiSummarizationService.cs` — New Gemini summarization
- `src/Services/DocumentService/Services/DataLinkageService.cs` — New patient record cross-referencing
- `src/Services/DocumentService/Services/RateLimitManager.cs` — New Gemini rate limit throttle
- `src/Services/DocumentService/Jobs/DocumentProcessingPipeline.cs` — New Hangfire pipeline job
- `src/Services/DocumentService/Models/OcrResult.cs` — OCR result entity
- `src/Services/DocumentService/Prompts/` — Gemini prompt templates

## Implementation Plan

1. Implement OcrExtractionService with Tesseract.NET for PDF/image text extraction with page-level segmentation
2. Create Gemini prompt templates for document summarization with structured JSON output schema
3. Build AiSummarizationService with Gemini integration, confidence scoring, and Polly circuit breaker
4. Implement DataLinkageService cross-referencing extracted medications/conditions against patient records
5. Build RateLimitManager throttling Gemini requests to 12 RPM with FIFO overflow queue
6. Create DocumentProcessingPipeline as Hangfire job chain (OCR → Summarize → Link → Complete)
7. Add processing status endpoints for frontend polling
8. Handle edge cases: OCR failure, unsupported language, low-confidence summaries

## Current Project State

```text
src/
├── Services/
│   └── DocumentService/
│       ├── Endpoints/
│       │   ├── UploadEndpoints.cs
│       │   └── ProcessingEndpoints.cs      ← NEW
│       ├── Services/
│       │   ├── DocumentStorageService.cs
│       │   ├── FileValidationService.cs
│       │   ├── ChunkedUploadService.cs
│       │   ├── OcrExtractionService.cs     ← NEW
│       │   ├── AiSummarizationService.cs   ← NEW
│       │   ├── DataLinkageService.cs       ← NEW
│       │   └── RateLimitManager.cs         ← NEW
│       ├── Jobs/
│       │   └── DocumentProcessingPipeline.cs ← NEW
│       ├── Models/
│       │   ├── ClinicalDocument.cs
│       │   └── OcrResult.cs                ← NEW
│       └── Prompts/
│           ├── summarization.json          ← NEW
│           └── extraction.json             ← NEW
```

## Expected Changes

| Action | File Path | Description |
|--------|-----------|-------------|
| CREATE | src/Services/DocumentService/Services/OcrExtractionService.cs | Tesseract OCR for PDF/image with page-level text segmentation |
| CREATE | src/Services/DocumentService/Services/AiSummarizationService.cs | Gemini structured summary with confidence scoring and Polly circuit breaker |
| CREATE | src/Services/DocumentService/Services/DataLinkageService.cs | Cross-reference extracted data against patient medication/condition records |
| CREATE | src/Services/DocumentService/Services/RateLimitManager.cs | Throttle Gemini to 12 RPM with FIFO overflow queue |
| CREATE | src/Services/DocumentService/Jobs/DocumentProcessingPipeline.cs | Hangfire job chain: OCR → Summarize → Link → Complete |
| CREATE | src/Services/DocumentService/Endpoints/ProcessingEndpoints.cs | GET /documents/{id}/status, GET /documents/{id}/summary |
| CREATE | src/Services/DocumentService/Prompts/summarization.json | Gemini prompt for structured document summary |
| MODIFY | src/Services/DocumentService/Data/DocumentDbContext.cs | Add OcrResult, ExtractedDataElement DbSets |

## External References

- [Tesseract.NET Documentation](https://github.com/charlesw/tesseract)
- [Google Gemini Structured Output](https://ai.google.dev/gemini-api/docs/structured-output)
- [Hangfire Job Continuations](https://docs.hangfire.io/en/latest/background-methods/using-batches.html)

## Build Commands

- [Refer to applicable technology stack build commands](.propel/build/)

## Implementation Validation Strategy

- [ ] Unit tests pass for OcrExtractionService (text extraction, page segmentation, failure handling)
- [ ] Unit tests pass for AiSummarizationService (Gemini mock, confidence scoring, circuit breaker)
- [ ] Integration tests pass (full pipeline flow, rate limiting, status progression)

## Implementation Checklist

- [ ] Implement OcrExtractionService with Tesseract.NET for PDF/image text extraction within 60 seconds for 10 pages — maps to AC-1
- [ ] Create Gemini prompt templates and build AiSummarizationService with structured JSON output and confidence scoring — maps to AC-2
- [ ] Implement DataLinkageService cross-referencing extracted data against patient records, flagging as "AI-suggested" — maps to AC-3
- [ ] Build DocumentProcessingPipeline as Hangfire job chain with status tracking through each stage — maps to AC-4
- [ ] Implement RateLimitManager throttling to 12 RPM with FIFO overflow queue for batch processing — maps to AC-5
- [ ] Add processing status and summary endpoints for frontend consumption — maps to AC-4
- [ ] Handle OCR failure (mark manual review), unsupported language detection, and low-confidence flagging — maps to edge cases
- [ ] Configure Polly circuit breaker (3-failure, 60s recovery) for Gemini API calls in pipeline — maps to AC-5
