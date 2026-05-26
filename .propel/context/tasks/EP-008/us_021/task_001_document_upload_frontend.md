# Task - task_001_document_upload_frontend

## Requirement Reference

- **User Story:** US_021
- **Story Location:** .propel/context/tasks/EP-008/us_021/us_021.md
- **Acceptance Criteria:**
  - AC-1: Secure document upload with file validation — PDF, JPEG, PNG, TIFF; 10MB per file, 50MB per session; MIME type verified; specific error messages
  - AC-2: Metadata tagging during upload — document type, date, provider, description; auto-suggest category from filename
  - AC-3: Upload progress and confirmation — progress bar, cancel mid-progress, thumbnail + metadata summary on completion
- **Edge Cases:**
  - Upload interrupted by network failure → resume for files >2MB using chunked upload
  - Patient uploads same document twice → display duplicate warning from server with proceed/cancel option
  - Malicious file disguised as PDF → client-side MIME check before upload

---

## Design References

| Reference Type | Value |
|----------------|-------|
| **UI Impact** | Yes |
| **Figma URL** | N/A |
| **Wireframe Status** | N/A |
| **Wireframe Type** | N/A |
| **Wireframe Path/URL** | N/A |
| **Screen Spec** | SCR-DOC-001 |
| **UXR Requirements** | Drag-and-drop upload zone with file type icons; metadata form as side panel; uploaded documents as thumbnail grid with list view toggle; progress bar with cancel button |
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
| Frontend | Angular | 17 | TR-001 — Standalone components for document upload module |
| Frontend | Angular Material | 17.x | TR-001 — File upload, form fields, progress bar components |
| Frontend | RxJS | 7.x | TR-001 — Upload progress observable, chunked upload stream |
| Frontend | TypeScript | 5.x | TR-001 — Type-safe document and metadata models |

---

## Task Overview

Implement the patient-facing clinical document upload interface with drag-and-drop upload zone, client-side MIME type validation, metadata tagging side panel, upload progress tracking with cancel capability, chunked upload for resumability, and a document gallery with thumbnail grid and list view toggle. The interface supports PDF, JPEG, PNG, and TIFF formats within size constraints.

## Dependent Tasks

- US_001/task_001 — Frontend scaffolding (Angular project, routing, shared module)

## Impacted Components

- `src/app/features/documents/` — New document management feature module
- `src/app/features/documents/upload/` — Upload zone and progress components
- `src/app/features/documents/metadata/` — Metadata tagging side panel
- `src/app/features/documents/gallery/` — Document thumbnail grid and list view
- `src/app/features/documents/services/document.service.ts` — Document upload and API service

## Implementation Plan

1. Define TypeScript interfaces for Document, DocumentMetadata, UploadProgress, and DocumentType enum
2. Implement DocumentService with chunked upload, metadata submission, and document listing methods
3. Build UploadZoneComponent with drag-and-drop, file type validation, and size limit enforcement
4. Build MetadataFormComponent as side panel with document type selector and auto-suggest from filename
5. Add upload progress bar with percentage display and cancel button
6. Build DocumentGalleryComponent with thumbnail grid and list view toggle

## Current Project State

```text
src/
├── app/
│   └── features/
│       └── documents/                      ← NEW
│           ├── upload/
│           │   ├── upload-zone.component.ts
│           │   ├── upload-zone.component.html
│           │   └── upload-zone.component.scss
│           ├── metadata/
│           │   └── metadata-form.component.ts
│           ├── gallery/
│           │   ├── document-gallery.component.ts
│           │   └── document-gallery.component.html
│           ├── services/
│           │   └── document.service.ts
│           └── models/
│               └── document.model.ts
```

## Expected Changes

| Action | File Path | Description |
|--------|-----------|-------------|
| CREATE | src/app/features/documents/models/document.model.ts | Document, DocumentMetadata, UploadProgress, DocumentType interfaces |
| CREATE | src/app/features/documents/services/document.service.ts | Chunked upload, metadata CRUD, document listing with pagination |
| CREATE | src/app/features/documents/upload/upload-zone.component.ts | Drag-and-drop zone with MIME validation and size limit enforcement |
| CREATE | src/app/features/documents/upload/upload-zone.component.html | Drop zone template with file type icons and error messages |
| CREATE | src/app/features/documents/metadata/metadata-form.component.ts | Metadata side panel with document type, date, provider, description |
| CREATE | src/app/features/documents/gallery/document-gallery.component.ts | Thumbnail grid with list view toggle and document status indicators |
| MODIFY | src/app/app.routes.ts | Add lazy-loaded route for documents feature |

## External References

- [Angular CDK Drag and Drop](https://material.angular.io/cdk/drag-drop/overview)
- [Angular Material Progress Bar](https://material.angular.io/components/progress-bar/overview)
- [File API (MIME type detection)](https://developer.mozilla.org/en-US/docs/Web/API/File)

## Build Commands

- [Refer to applicable technology stack build commands](.propel/build/)

## Implementation Validation Strategy

- [ ] Unit tests pass for UploadZoneComponent (drag-drop, MIME validation, size limits)
- [ ] Unit tests pass for DocumentService (chunked upload, progress tracking)
- [ ] Integration tests pass (upload flow end-to-end, metadata submission, gallery display)

## Implementation Checklist

- [ ] Create documents feature module with lazy-loaded route and TypeScript interfaces — maps to AC-1
- [ ] Build UploadZoneComponent with drag-and-drop accepting PDF/JPEG/PNG/TIFF, 10MB/file and 50MB/session limits — maps to AC-1
- [ ] Implement client-side MIME type validation with specific error messages for invalid files — maps to AC-1
- [ ] Build MetadataFormComponent as side panel with document type, date, provider, description, and filename auto-suggest — maps to AC-2
- [ ] Implement chunked upload with progress bar, percentage display, and cancel button — maps to AC-3
- [ ] Add upload confirmation view showing document thumbnail, metadata summary, and processing status — maps to AC-3
