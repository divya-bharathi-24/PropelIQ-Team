# Task - task_002_document_upload_backend

## Requirement Reference

- **User Story:** US_021
- **Story Location:** .propel/context/tasks/EP-008/us_021/us_021.md
- **Acceptance Criteria:**
  - AC-1: Secure document upload with file validation — MIME type verification (magic bytes), reject invalid with specific errors
  - AC-2: Metadata tagging during upload — required fields persisted with document reference
  - AC-3: Upload progress and confirmation — chunked upload support for resumability
  - AC-4: Document stored with encryption at rest — AES-256, file reference in DB, immutable audit record
- **Edge Cases:**
  - Upload interrupted → chunked upload with 24-hour chunk retention for resume
  - Patient uploads same document twice → SHA-256 hash dedup detection with warning response
  - Malicious file disguised as PDF → server-side magic bytes validation, quarantine if suspicious

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
| Backend | ASP.NET Core Web API | 8.0 | TR-002 — Minimal API for document upload endpoints |
| Backend | Entity Framework Core | 8.0 | TR-004 — Document metadata persistence |
| Database | PostgreSQL | 16.x | TR-004 — Document metadata and file reference storage |

---

## Task Overview

Implement the document service backend for secure clinical document upload with server-side MIME type validation (magic bytes), AES-256 encryption at rest, chunked upload for resumability, SHA-256 duplicate detection, metadata persistence, and immutable audit logging. The service stores encrypted file blobs with database references and supports the full upload lifecycle from chunk reception through finalization.

## Dependent Tasks

- US_002/task_001 — Backend API scaffolding (solution structure, YARP gateway, encryption middleware)
- US_005/task_001 — Clinical/audit schemas (document and audit tables)

## Impacted Components

- `src/Services/DocumentService/` — New microservice project
- `src/Services/DocumentService/Endpoints/UploadEndpoints.cs` — Upload API endpoints
- `src/Services/DocumentService/Services/DocumentStorageService.cs` — AES-256 encryption and storage
- `src/Services/DocumentService/Services/FileValidationService.cs` — MIME magic bytes validation
- `src/Services/DocumentService/Services/ChunkedUploadService.cs` — Chunked upload management
- `src/Services/DocumentService/Models/` — Domain entities

## Implementation Plan

1. Create DocumentService microservice project with DI, EF Core registration
2. Define domain entities (ClinicalDocument, DocumentChunk, DocumentMetadata) and EF Core DbContext
3. Create EF Core migration for document schema (documents, chunks, metadata tables)
4. Implement FileValidationService with magic bytes MIME verification for PDF/JPEG/PNG/TIFF
5. Build ChunkedUploadService with chunk reception, retention (24h), and assembly
6. Implement DocumentStorageService with AES-256-GCM encryption at rest and file hash computation
7. Build upload endpoints: POST /documents/upload/init, PUT /documents/upload/{id}/chunk, POST /documents/upload/{id}/finalize
8. Add SHA-256 duplicate detection and immutable audit record creation on upload completion

## Current Project State

```text
src/
├── Services/
│   └── DocumentService/                    ← NEW
│       ├── Endpoints/
│       │   └── UploadEndpoints.cs
│       ├── Services/
│       │   ├── DocumentStorageService.cs
│       │   ├── FileValidationService.cs
│       │   └── ChunkedUploadService.cs
│       ├── Models/
│       │   ├── ClinicalDocument.cs
│       │   └── DocumentChunk.cs
│       ├── Data/
│       │   └── DocumentDbContext.cs
│       └── Program.cs
```

## Expected Changes

| Action | File Path | Description |
|--------|-----------|-------------|
| CREATE | src/Services/DocumentService/Program.cs | Service entry with DI for EF Core, encryption middleware |
| CREATE | src/Services/DocumentService/Endpoints/UploadEndpoints.cs | POST init, PUT chunk, POST finalize, GET status |
| CREATE | src/Services/DocumentService/Services/DocumentStorageService.cs | AES-256-GCM encryption, file storage, hash computation |
| CREATE | src/Services/DocumentService/Services/FileValidationService.cs | MIME magic bytes verification for PDF/JPEG/PNG/TIFF |
| CREATE | src/Services/DocumentService/Services/ChunkedUploadService.cs | Chunk reception, 24h retention, assembly on finalize |
| CREATE | src/Services/DocumentService/Models/ClinicalDocument.cs | Document entity with encrypted blob path, metadata, hash |
| MODIFY | src/ApiGateway/yarp.json | Add route cluster for DocumentService |

## External References

- [AES-256-GCM Encryption in .NET](https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.aesgcm)
- [ASP.NET Core File Upload](https://learn.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-8.0)
- [MIME Type Detection (Magic Bytes)](https://en.wikipedia.org/wiki/List_of_file_signatures)

## Build Commands

- [Refer to applicable technology stack build commands](.propel/build/)

## Implementation Validation Strategy

- [ ] Unit tests pass for FileValidationService (valid/invalid MIME, malicious file detection)
- [ ] Unit tests pass for DocumentStorageService (encryption, decryption, hash computation)
- [ ] Integration tests pass (chunked upload flow, duplicate detection, audit log creation)

## Implementation Checklist

- [ ] Create DocumentService project with EF Core and encryption middleware registration — maps to AC-4
- [ ] Define ClinicalDocument entity and create EF Core migration for document schema — maps to AC-4
- [ ] Implement FileValidationService with magic bytes MIME verification for PDF/JPEG/PNG/TIFF — maps to AC-1
- [ ] Build ChunkedUploadService with chunk reception, 24-hour retention, and assembly — maps to AC-3
- [ ] Implement DocumentStorageService with AES-256-GCM encryption at rest and SHA-256 hash — maps to AC-4
- [ ] Build upload endpoints (init, chunk, finalize) with metadata persistence — maps to AC-2, AC-3
- [ ] Add SHA-256 duplicate detection returning warning response for previously uploaded files — maps to edge cases
- [ ] Create immutable audit record on upload completion with uploader ID and timestamp — maps to AC-4
