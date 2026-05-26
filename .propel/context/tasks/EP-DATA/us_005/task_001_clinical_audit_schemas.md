# Task - task_001_clinical_audit_schemas

## Requirement Reference

- **User Story:** us_005
- **Story Location:** `.propel/context/tasks/EP-DATA/us_005/us_005.md`
- **Acceptance Criteria:**
  - AC-1: Document domain schema created — Documents, DocumentVersions, DocumentMetadata, OcrResults tables with file references (not inline BLOBs), MIME types, and processing status enums
  - AC-2: ClinicalIntelligence domain schema created — AiSuggestions, MedicalCodes, ConflictDetections, RiskScores, IntakeResponses with JSON columns for AI outputs and indexes on (patient_id, created_at)
  - AC-3: Notification domain schema created — NotificationTemplates, NotificationLogs, DeliveryAttempts with indexes on (patient_id, channel, status) and TTL-based archival column
  - AC-4: Immutable audit log schema created — append-only AuditEvents table with (event_id, actor_id, action, resource_type, resource_id, timestamp, payload_json), no UPDATE/DELETE permissions, partitioned index on (timestamp, resource_type)
  - AC-5: AES-256 encryption at rest for PII columns — EF Core value converters encrypt before storage and decrypt on read; encryption keys via environment variables
- **Edge Cases:**
  - Audit table exceeds free-tier storage (1GB) → partition by month with archival of partitions older than 12 months
  - Encryption key rotation → dual-key reading during rotation; background re-encryption job
  - OcrResults exceed size → 500KB max payload; overflow stored as file reference

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
| Database | PostgreSQL | 16.x | TR-004 — Spec constraint; JSONB for AI model outputs; partitioning for audit log |
| ORM | Entity Framework Core | 8.0 | TR-004 — Code-first migrations; value converters for encryption |
| ORM Provider | Npgsql | 8.x | DR-003 — PostgreSQL-specific provider with JSONB support |
| Backend Framework | ASP.NET Core Web API | 8.0 | TR-002 — Host for DbContext registration |
| Encryption | AES-256-GCM | N/A | NFR-008 — HIPAA-mandated PHI encryption at rest |

---

## Task Overview

Design and create PostgreSQL schemas for clinical domains (Document, ClinicalIntelligence, Notification) and the immutable audit log, completing the data layer for HIPAA-compliant clinical feature development. Includes AES-256 encryption via EF Core value converters for all PII columns, append-only audit table with revoked UPDATE/DELETE permissions, and monthly partitioning for audit log scalability.

## Dependent Tasks

- task_001_backend_api_scaffolding (US_002) — Backend service projects must exist before database contexts can be configured

## Impacted Components

- New: `src/Services/Document/HealthPlatform.Document.Api/Data/DocumentDbContext.cs` — Document EF Core context
- New: `src/Services/Document/HealthPlatform.Document.Api/Data/Entities/` — Document domain entities
- New: `src/Services/ClinicalIntelligence/HealthPlatform.ClinicalIntelligence.Api/Data/ClinicalDbContext.cs` — Clinical Intelligence EF Core context
- New: `src/Services/ClinicalIntelligence/HealthPlatform.ClinicalIntelligence.Api/Data/Entities/` — Clinical entities
- New: `src/Services/Notification/HealthPlatform.Notification.Api/Data/NotificationDbContext.cs` — Notification EF Core context
- New: `src/Services/Admin/HealthPlatform.Admin.Api/Data/AuditDbContext.cs` — Audit EF Core context
- New: `src/Shared/HealthPlatform.Shared/Encryption/AesValueConverter.cs` — AES-256 EF Core value converter

## Implementation Plan

1. Create Document schema entities (Document, DocumentVersion, DocumentMetadata, OcrResult) with file references, MIME type columns, processing status enum, and 500KB payload constraint.
2. Create ClinicalIntelligence schema entities (AiSuggestion, MedicalCode, ConflictDetection, RiskScore, IntakeResponse) with JSONB columns for AI outputs and composite indexes.
3. Create Notification schema entities (NotificationTemplate, NotificationLog, DeliveryAttempt) with channel/status indexes and TTL archival column.
4. Create immutable AuditEvents table with append-only design — revoke UPDATE/DELETE at PostgreSQL role level; add partitioned index on (timestamp, resource_type).
5. Implement AES-256-GCM value converter for PII columns with envelope encryption pattern.
6. Configure encryption key injection via environment variables with dual-key support for rotation.
7. Generate EF Core migrations for all clinical/audit schemas.
8. Add monthly partition strategy for audit table with archival for partitions older than 12 months.

## Current Project State

```text
src/
├── HealthPlatform.sln
├── Services/
│   ├── Auth/HealthPlatform.Auth.Api/
│   │   └── Data/ (AuthDbContext, entities, migrations)
│   ├── Appointment/HealthPlatform.Appointment.Api/
│   │   └── Data/ (AppointmentDbContext, entities, migrations)
│   ├── Document/HealthPlatform.Document.Api/
│   ├── ClinicalIntelligence/HealthPlatform.ClinicalIntelligence.Api/
│   ├── Notification/HealthPlatform.Notification.Api/
│   └── Admin/HealthPlatform.Admin.Api/
└── Shared/
    └── HealthPlatform.Shared/
        └── Data/ (PatientDbContext, entities)
```

## Expected Changes

| Action | File Path | Description |
|--------|-----------|-------------|
| CREATE | `src/Services/Document/HealthPlatform.Document.Api/Data/DocumentDbContext.cs` | Document DbContext with `document` schema |
| CREATE | `src/Services/Document/HealthPlatform.Document.Api/Data/Entities/Document.cs` | Document entity with file reference and processing status |
| CREATE | `src/Services/Document/HealthPlatform.Document.Api/Data/Entities/DocumentVersion.cs` | Document versioning entity |
| CREATE | `src/Services/Document/HealthPlatform.Document.Api/Data/Entities/DocumentMetadata.cs` | Document metadata with MIME type |
| CREATE | `src/Services/Document/HealthPlatform.Document.Api/Data/Entities/OcrResult.cs` | OCR result with 500KB JSON payload limit |
| CREATE | `src/Services/ClinicalIntelligence/HealthPlatform.ClinicalIntelligence.Api/Data/ClinicalDbContext.cs` | Clinical Intelligence DbContext with `clinical_intelligence` schema |
| CREATE | `src/Services/ClinicalIntelligence/HealthPlatform.ClinicalIntelligence.Api/Data/Entities/AiSuggestion.cs` | AI suggestion entity with JSONB output column |
| CREATE | `src/Services/ClinicalIntelligence/HealthPlatform.ClinicalIntelligence.Api/Data/Entities/MedicalCode.cs` | Medical code entity |
| CREATE | `src/Services/ClinicalIntelligence/HealthPlatform.ClinicalIntelligence.Api/Data/Entities/ConflictDetection.cs` | Conflict detection entity |
| CREATE | `src/Services/ClinicalIntelligence/HealthPlatform.ClinicalIntelligence.Api/Data/Entities/RiskScore.cs` | Risk score entity |
| CREATE | `src/Services/ClinicalIntelligence/HealthPlatform.ClinicalIntelligence.Api/Data/Entities/IntakeResponse.cs` | Intake response entity with JSONB |
| CREATE | `src/Services/Notification/HealthPlatform.Notification.Api/Data/NotificationDbContext.cs` | Notification DbContext with `notification` schema |
| CREATE | `src/Services/Notification/HealthPlatform.Notification.Api/Data/Entities/NotificationTemplate.cs` | Notification template entity |
| CREATE | `src/Services/Notification/HealthPlatform.Notification.Api/Data/Entities/NotificationLog.cs` | Notification log with TTL archival column |
| CREATE | `src/Services/Notification/HealthPlatform.Notification.Api/Data/Entities/DeliveryAttempt.cs` | Delivery attempt tracking entity |
| CREATE | `src/Services/Admin/HealthPlatform.Admin.Api/Data/AuditDbContext.cs` | Audit DbContext with `audit` schema, append-only |
| CREATE | `src/Services/Admin/HealthPlatform.Admin.Api/Data/Entities/AuditEvent.cs` | Immutable audit event entity |
| CREATE | `src/Shared/HealthPlatform.Shared/Encryption/AesValueConverter.cs` | AES-256-GCM EF Core value converter for PII |
| CREATE | `src/Shared/HealthPlatform.Shared/Encryption/EncryptionKeyProvider.cs` | Encryption key management from environment variables |

## External References

- [EF Core 8.0 Value Converters](https://learn.microsoft.com/en-us/ef/core/modeling/value-conversions?tabs=data-annotations)
- [PostgreSQL 16 Table Partitioning](https://www.postgresql.org/docs/16/ddl-partitioning.html)
- [Npgsql JSONB Mapping](https://www.npgsql.org/efcore/mapping/json.html)
- [AES-256-GCM in .NET 8](https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.aesgcm)
- [PostgreSQL REVOKE Permissions](https://www.postgresql.org/docs/16/sql-revoke.html)

## Build Commands

- Refer to [.propel/build/](.propel/build/) for applicable technology stack build commands

## Implementation Validation Strategy

- [ ] Unit tests pass — AES-256 value converter encrypts/decrypts correctly
- [ ] Integration tests pass — migrations apply to PostgreSQL 16; audit table rejects UPDATE/DELETE
- [ ] All 4 schemas created with correct table structures and indexes
- [ ] PII columns encrypted at rest, decryptable on read

## Implementation Checklist

- [x] Create Document domain schema (Documents, DocumentVersions, DocumentMetadata, OcrResults) with file references and processing status → AC-1
- [x] Create ClinicalIntelligence schema (AiSuggestions, MedicalCodes, ConflictDetections, RiskScores, IntakeResponses) with JSONB and indexes → AC-2
- [x] Create Notification schema (NotificationTemplates, NotificationLogs, DeliveryAttempts) with indexes and TTL archival column → AC-3
- [x] Create immutable AuditEvents table (append-only, no UPDATE/DELETE permissions, partitioned index) → AC-4
- [x] Implement AES-256 EF Core value converters for PII columns with env var key injection → AC-5
- [x] Add monthly partition strategy for audit table with 12-month archival → AC-4 (edge case)
- [x] Implement dual-key reading for encryption key rotation → AC-5 (edge case)
- [x] Enforce 500KB max payload constraint on OcrResults JSON column → AC-1 (edge case)
