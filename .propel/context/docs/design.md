# Architecture Design

## Project Overview

The **Unified Patient Access & Clinical Intelligence Platform** is a standalone healthcare system that integrates patient-centric appointment booking with a "Trust-First" clinical intelligence engine. It serves three user roles — Patient, Staff, and Admin — providing self-service appointment scheduling with dynamic preferred slot swap, flexible digital intake (AI conversational or manual form), clinical document processing with AI-powered data extraction, and a verified 360-Degree Patient View with ICD-10/CPT code mapping. The platform is built on Angular 17 and .NET 8 microservices with PostgreSQL, deployed exclusively on free-tier infrastructure, and designed for full HIPAA compliance.

## Architecture Goals

- **Architecture Goal 1**: HIPAA-first security — every data path encrypts PHI at rest and in transit; every access event is immutably logged; RBAC is enforced at the API gateway and service level.
- **Architecture Goal 2**: Requirements-driven technology selection — every stack choice is justified by a constraint (NFR, DR, or AIR), not by convention or prior project defaults.
- **Architecture Goal 3**: Trust-First AI — all AI-generated outputs (clinical data extraction, medical codes, risk scores) require explicit human verification before persisting to patient records.
- **Architecture Goal 4**: Resilient free-tier deployment — stateless microservices enable rapid cold-start recovery; graceful degradation preserves core scheduling when AI or notification services are unavailable.
- **Architecture Goal 5**: Domain-aligned microservices — each business domain (Appointment, Patient, Clinical Intelligence, Notification, Admin) is independently deployable and scalable within free-tier constraints.

## Non-Functional Requirements

- NFR-001: [SOURCE:INPUT] System MUST achieve 99.9% uptime measured over a rolling 30-day window, with automated health checks detecting service degradation within 60 seconds.
  Basis: BRD Section 7 states "Targeting 99.9% uptime"; spec Success Criteria confirms rolling 30-day measurement.
- NFR-002: [SOURCE:INPUT] System MUST automatically terminate all authenticated sessions after exactly 15 consecutive minutes of user inactivity, requiring full re-authentication to resume.
  Basis: FR-004 mandates 15-minute session timeout; BRD Section 7 confirms "15-minute automatic timeout."
- NFR-003: [SOURCE:INPUT] System MUST generate a branded PDF appointment confirmation and deliver it via email within 60 seconds of booking confirmation.
  Basis: FR-015 specifies "within 60 seconds of booking confirmation."
- NFR-004: [SOURCE:INPUT] System MUST execute a preferred slot swap — cancel current booking, assign preferred slot, release original — within 60 seconds of the preferred slot becoming available.
  Basis: FR-022 specifies "within 60 seconds of the preferred slot opening."
- NFR-005: [SOURCE:INPUT] System MUST enable a staff member to load, review, and verify the aggregated 360-degree patient view within 2 minutes for a patient with up to 10 uploaded documents, measured from page load to verification action.
  Basis: FR-048 defines the 2-minute verification workflow target.
- NFR-006: [SOURCE:INFERRED] System MUST display appointment slot availability in real time, with no more than 2 seconds of staleness between a slot state change and its reflection in the patient or staff booking interface.
  Basis: FR-011 requires "real time" display; 2-second threshold inferred as measurable bound for "real time" in a web context.
- NFR-007: [SOURCE:INPUT] System MUST comply with HIPAA Privacy Rule and Security Rule for all patient data handling, transmission, storage, and access control workflows.
  Basis: BRD Section 7 mandates "100% HIPAA-compliant data handling, transmission, and storage."
- NFR-008: [SOURCE:INPUT] System MUST encrypt all Protected Health Information (PHI) at rest using AES-256 and in transit using TLS 1.2 or higher.
  Basis: FR-043 specifies "AES-256 encryption" at rest and "TLS 1.2 or higher" in transit.
- NFR-009: [SOURCE:INPUT] System MUST enforce role-based access control with three mutually exclusive roles (Patient, Staff, Admin), where no role may perform actions outside its defined permission set.
  Basis: FR-001 defines RBAC with three roles and exclusive permission boundaries.
- NFR-010: [SOURCE:INPUT] System MUST maintain an immutable, append-only audit log for every patient data access and modification event, with no update or delete capability exposed through any application interface.
  Basis: FR-057 mandates "immutable, append-only audit log"; FR-059 restricts to Admin read-only.
- NFR-011: [SOURCE:INPUT] System MUST prevent concurrent booking of the same appointment slot by applying optimistic locking or an equivalent concurrency-safe reservation mechanism.
  Basis: FR-013 requires concurrency-safe booking; "optimistic locking" is explicitly stated.
- NFR-012: [SOURCE:INPUT] System MUST be deployable and operational exclusively on free, open-source-friendly hosting platforms with no paid cloud infrastructure dependencies.
  Basis: Spec Constraint 1 prohibits AWS, Azure, GCP; limits to Netlify, Vercel, GitHub Codespaces.
- NFR-013: [SOURCE:INFERRED] System MUST support a minimum of 100 concurrent authenticated users and 1,000 registered patient records without performance degradation below stated NFR thresholds.
  Basis: Free-tier infrastructure capacity ceiling; consistent with pilot-scale deployment target.
- NFR-014: [SOURCE:INFERRED] System MUST implement stateless service design to enable sub-30-second cold-start recovery on free-tier hosting platforms that suspend idle instances.
  Basis: Free-tier platforms (Railway, Render) suspend idle containers; stateless design mitigates cold-start latency.

## Data Requirements

- DR-001: [SOURCE:INPUT] System MUST store structured patient demographic data including legal name, date of birth, biological sex, primary contact number, email address, mailing address, and insurance details (insurer name, member ID, group number) in a normalized relational schema.
  Basis: FR-008 defines the patient demographic field set; relational schema inferred from PostgreSQL selection.
- DR-002: [SOURCE:INPUT] System MUST model appointments with a full status lifecycle (Available → Reserved → Confirmed → Arrived → Completed | Cancelled | No-Show), tracking slot time, patient reference, booking type (self-booked/walk-in), and preferred swap linkage.
  Basis: FR-011 (available slots), FR-012 (booking), FR-014 (cancel/reschedule), FR-027 (arrived marking), FR-020–FR-022 (preferred slot swap).
- DR-003: [SOURCE:INPUT] System MUST store uploaded clinical documents as encrypted binary objects (AES-256 at rest) with metadata including upload timestamp, patient reference, uploader role (Patient or Staff), file size, processing status, and encounter reference (for staff-uploaded notes).
  Basis: FR-041–FR-044 define upload requirements; FR-043 mandates AES-256 encryption.
- DR-004: [SOURCE:INPUT] System MUST store each extracted clinical data element (vital, medication, diagnosis, allergy, immunization, surgical history) with references to the originating document name, page number (where applicable), upload timestamp, extraction confidence score, and de-duplication status.
  Basis: FR-045 (extraction), FR-049 (source traceability), FR-046 (de-duplication).
- DR-005: [SOURCE:INPUT] System MUST store staff-confirmed ICD-10 and CPT codes linked to the patient's unique identifier, the specific visit encounter, the confirming staff member's ID, and the confirmation timestamp.
  Basis: FR-053 specifies these exact linkage fields.
- DR-006: [SOURCE:INPUT] System MUST implement an append-only audit log table capturing accessor user ID, role, patient record accessed, action type (view/create/update/delete), UTC timestamp, originating IP address, and full before-and-after state for modification events.
  Basis: FR-057 (access logging), FR-058 (modification logging with before/after state).
- DR-007: [SOURCE:INPUT] System MUST model a preferred slot waitlist as a FIFO queue linking patient appointment references to preferred slot identifiers, with registration timestamp for ordering.
  Basis: FR-020 (waitlist registration), UC-004 Extension 2a (FIFO ordering).
- DR-008: [SOURCE:INPUT] System MUST store predefined dummy insurance records (insurer name, member ID pattern) in an admin-maintainable table for soft pre-check validation.
  Basis: FR-038 specifies matching against "a predefined internal set of dummy insurer records."
- DR-009: [SOURCE:INPUT] System MUST log every notification delivery event including channel (email/SMS), timestamp, delivery status (sent/failed/retried), triggering source (system scheduler or staff ID), and appointment reference.
  Basis: FR-034 mandates logging of "every reminder delivery event" with these fields.
- DR-010: [SOURCE:INFERRED] System MUST retain all patient data, clinical documents, and audit logs for a minimum of 6 years in compliance with HIPAA retention requirements, with automated archival for records past the active retention window.
  Basis: HIPAA requires minimum 6-year retention for medical records; no explicit retention period stated in spec.
- DR-011: [SOURCE:INFERRED] System MUST implement automated daily database backups with point-in-time recovery capability, stored in an encrypted, geographically separate location from the primary database.
  Basis: HIPAA Security Rule requires contingency planning (§164.308(a)(7)); no backup strategy stated in spec.

### Domain Entities

- **Patient**: Represents a registered individual. Attributes: patientId (PK), email (unique), passwordHash, fullName, dateOfBirth, biologicalSex, phone, address, insurerName, memberId, groupNumber, registrationDate, emailVerified, accountStatus. Relationships: 1:N Appointments, 1:N ClinicalDocuments, 1:1 PatientView360, 1:N IntakeRecords.
- **User**: Authentication identity for all roles. Attributes: userId (PK), email (unique), passwordHash, role (Patient|Staff|Admin), accountStatus (Active|Inactive|Locked), lastLoginAt, createdBy, createdAt. Relationships: 1:1 Patient (for Patient role), 1:N AuditLogEntries (as actor).
- **Appointment**: A scheduled visit slot. Attributes: appointmentId (PK), patientId (FK), slotStart, slotEnd, status (Available|Reserved|Confirmed|Arrived|Completed|Cancelled|NoShow), bookingType (SelfBooked|WalkIn), referenceNumber, preferredSwapId (FK nullable), createdAt, updatedAt. Relationships: N:1 Patient, 1:1 PreferredSwapEntry (nullable).
- **PreferredSwapEntry**: Waitlist entry for preferred slot. Attributes: swapId (PK), appointmentId (FK), preferredSlotStart, preferredSlotEnd, registeredAt, status (Pending|Executed|Expired). Relationships: N:1 Appointment.
- **IntakeRecord**: Structured intake data. Attributes: intakeId (PK), appointmentId (FK), patientId (FK), mode (AI|Manual), completionStatus, createdAt, updatedAt, autoSavedAt. Contains nested fields for demographics, medical history, medications, allergies, conditions. Relationships: N:1 Patient, N:1 Appointment.
- **ClinicalDocument**: Uploaded PDF. Attributes: documentId (PK), patientId (FK), uploaderUserId (FK), uploaderRole, fileName, fileSizeBytes, encryptedBlobPath, processingStatus (Queued|Processing|Completed|Failed), encounterReference (nullable), uploadedAt. Relationships: N:1 Patient, 1:N ExtractedDataElements.
- **ExtractedDataElement**: Single data point from extraction. Attributes: elementId (PK), documentId (FK), patientId (FK), category (Vital|Medication|Diagnosis|Allergy|Immunization|Surgery), fieldName, fieldValue, confidenceScore, sourcePageNumber, deduplicationStatus, createdAt. Relationships: N:1 ClinicalDocument.
- **PatientView360**: Aggregated patient profile. Attributes: viewId (PK), patientId (FK unique), verificationStatus (Unverified|Verified|Complete), verifiedByUserId (FK nullable), verifiedAt, lastAggregatedAt. Relationships: 1:1 Patient, 1:N DataConflicts.
- **DataConflict**: Conflicting values across documents. Attributes: conflictId (PK), viewId (FK), fieldName, valueA, sourceDocumentA (FK), valueB, sourceDocumentB (FK), severity (Critical|Warning), resolutionStatus (Open|Resolved|Acknowledged), resolvedByUserId (FK nullable), resolvedAt. Relationships: N:1 PatientView360.
- **MedicalCode**: AI-suggested or confirmed code. Attributes: codeId (PK), patientId (FK), encounterId, codeType (ICD10|CPT), codeValue, codeDescription, supportingEvidence, confidenceScore, status (Suggested|Confirmed|Rejected), confirmedByUserId (FK nullable), confirmedAt, createdAt. Relationships: N:1 Patient.
- **AuditLogEntry**: Immutable access/change record. Attributes: logId (PK), userId (FK), userRole, patientRecordId, actionType (View|Create|Update|Delete), resourceType, resourceId, beforeState (JSONB nullable), afterState (JSONB nullable), ipAddress, timestampUtc. No update/delete operations permitted.
- **NotificationEvent**: Delivery tracking. Attributes: eventId (PK), appointmentId (FK), channel (Email|SMS), status (Sent|Failed|Retried), triggerSource (System|StaffId), sentAt, failedAt, retryCount. Relationships: N:1 Appointment.
- **InsuranceDummyRecord**: Admin-maintained validation set. Attributes: recordId (PK), insurerName, memberIdPattern, createdAt, updatedAt. No foreign key relationships.

## AI Consideration

- Status: **Applicable**
- Rationale: The spec contains 8 [AI-CANDIDATE] tags (FR-017 conversational intake, FR-018 adaptive follow-ups, FR-045 clinical data extraction, FR-047 clinical timeline, FR-050/FR-051 ICD-10 and CPT suggestion, FR-054 data conflict detection, FR-030 no-show risk scoring) and 4 [HYBRID] tags (FR-019 auto-save with intelligent resume, FR-046 de-duplication, FR-048 360-degree view aggregation, FR-052 evidence highlighting). These require LLM inference (Google Gemini API), ML-based scoring, and structured data extraction pipelines that cannot be implemented with deterministic logic alone.

## AI Requirements

- AIR-001: [SOURCE:INPUT] System MUST provide a conversational AI intake flow using Google Gemini API that collects demographics, medical history, medications, allergies, and conditions through natural-language prompts, supporting free-text patient responses.
  Basis: FR-017 specifies "AI-powered conversational flow" as [AI-CANDIDATE]; FR-018 specifies adaptive follow-ups.
- AIR-002: [SOURCE:INPUT] System MUST extract structured clinical data elements (vitals, medications, diagnoses, allergies, immunizations, surgical history) from uploaded PDF documents using OCR + LLM pipeline with per-field confidence scores.
  Basis: FR-045 specifies extraction of six categories with confidence scores as [AI-CANDIDATE].
- AIR-003: [SOURCE:INPUT] System MUST generate ICD-10 code suggestions from extracted clinical narrative text with supporting evidence snippets, presenting up to 5 ranked suggestions per encounter for staff confirmation.
  Basis: FR-050 specifies ICD-10 suggestions with "supporting evidence snippets" as [AI-CANDIDATE].
- AIR-004: [SOURCE:INPUT] System MUST generate CPT code suggestions from extracted clinical narrative text with supporting evidence snippets, presenting up to 5 ranked suggestions per encounter for staff confirmation.
  Basis: FR-051 specifies CPT suggestions mirroring ICD-10 suggestion pattern as [AI-CANDIDATE].
- AIR-005: [SOURCE:INPUT] System MUST detect and flag data conflicts (contradictory vitals, duplicate medications, mismatched demographics) across multiple clinical documents for the same patient, classifying conflicts by severity (Critical/Warning).
  Basis: FR-054 specifies AI-powered conflict detection across sources as [AI-CANDIDATE].
- AIR-006: [SOURCE:INPUT] System MUST compute a no-show risk score (0–100) for each upcoming appointment using a weighted scoring model based on historical no-show rate, booking lead time, and time-of-day patterns.
  Basis: FR-030 specifies "no-show risk score" with these input factors as [AI-CANDIDATE].
- AIR-007: [SOURCE:INPUT] System MUST construct a chronological clinical timeline from extracted data elements, ordered by event date, with source document attribution for each entry.
  Basis: FR-047 specifies clinical timeline construction as [AI-CANDIDATE].
- AIR-008: [SOURCE:INPUT] System MUST de-duplicate extracted data elements across multiple documents for the same patient, merging equivalent entries and retaining source provenance chains.
  Basis: FR-046 specifies de-duplication across uploads as [HYBRID].
- AIR-009: [SOURCE:INPUT] System MUST aggregate all extracted, de-duplicated, and conflict-resolved data into a unified 360-degree patient view, with a verification status indicator (Unverified → Verified → Complete) controlled by staff.
  Basis: FR-048 defines the aggregation and verification workflow as [HYBRID].
- AIR-010: [SOURCE:INFERRED] All AI inference calls (Gemini API) MUST include a circuit breaker with 3-failure threshold and 60-second recovery window; when the breaker is open, the system MUST gracefully degrade to manual-only workflows (manual intake form, manual code entry) without data loss.
  Basis: Free-tier Gemini API has rate limits (15 RPM); no explicit fallback strategy in spec.

### AI Architecture Pattern

The platform adopts a **Human-in-the-Loop (HITL) AI** pattern where all AI outputs (extracted data, suggested codes, conflict flags, risk scores) are presented as suggestions requiring explicit staff confirmation before persisting to the verified record. This ensures clinical accuracy and regulatory compliance while leveraging AI for efficiency.

**AI Pipeline Architecture**:

1. **Intake Pipeline**: Patient ↔ Angular 17 Chat UI → .NET 8 Intake Service → Gemini API (structured prompt with schema enforcement) → Parsed intake JSON → PostgreSQL.
2. **Document Processing Pipeline**: Upload → .NET 8 Document Service → Background Job (Hangfire) → OCR preprocessing → Gemini API (extraction prompt with field taxonomy) → ExtractedDataElement records → De-duplication Service → PatientView360 aggregation.
3. **Medical Coding Pipeline**: Staff triggers coding → .NET 8 Coding Service → Gemini API (ICD-10/CPT prompt with clinical text + evidence requirement) → Ranked suggestions → Staff confirmation UI → Confirmed MedicalCode records.
4. **Conflict Detection Pipeline**: New extraction complete → .NET 8 Conflict Service → Rule-based pre-filter + Gemini API (semantic similarity for ambiguous conflicts) → DataConflict records → Staff resolution UI.
5. **No-Show Risk Pipeline**: Nightly batch job → .NET 8 Risk Service → Weighted scoring algorithm (deterministic, no LLM) → Risk scores persisted → Staff dashboard.

## Architecture and Design Decisions

### Decision 1: Microservices Decomposition by Domain

- **Decision**: Decompose the backend into 7 bounded-context microservices: Auth Service, Appointment Service, Intake Service, Document Service, Clinical Intelligence Service (coding + conflict detection), Notification Service, and Admin/Audit Service.
- **Rationale**: The spec defines 16 functional domains (A–P) with distinct data ownership boundaries. Grouping related domains into services (e.g., Booking + Slot Swap + Walk-in → Appointment Service) achieves independent deployability, targeted scaling, and fault isolation without exceeding free-tier container limits.
- **Justification**: NFR-005 (modular architecture), NFR-010 (independent scaling), NFR-014 (cold-start resilience). Each service owns its database schema (shared PostgreSQL instance, schema-per-service) to enforce data boundaries within free-tier constraints.
- **Trade-off**: Schema-per-service on a single PostgreSQL instance sacrifices full database isolation but avoids the cost of multiple database instances on free tier. Cross-service queries require explicit API calls or event-based synchronization.

### Decision 2: API Gateway with RBAC Enforcement

- **Decision**: Implement a lightweight API gateway (.NET 8 YARP reverse proxy) as the single entry point for all client requests, enforcing JWT validation, role-based access control, and rate limiting before routing to downstream services.
- **Rationale**: FR-001 through FR-007 define three distinct roles (Patient, Staff, Admin) with differentiated access. Centralizing RBAC at the gateway ensures consistent enforcement across all services and simplifies per-service authorization to scope-based checks.
- **Justification**: NFR-001 (RBAC enforcement), NFR-002 (TLS 1.2+), NFR-003 (audit logging). YARP is selected over Ocelot for its superior performance and native .NET 8 integration.
- **Trade-off**: Single gateway is a potential bottleneck; mitigated by stateless design enabling horizontal scaling if traffic exceeds free-tier limits.

### Decision 3: Event-Driven Communication via In-Process Message Bus

- **Decision**: Use MediatR as an in-process mediator/event bus for intra-service communication and a lightweight PostgreSQL-backed outbox pattern for inter-service events (appointment status changes, document processing completion, notification triggers).
- **Rationale**: Free-tier constraints preclude dedicated message brokers (RabbitMQ, Azure Service Bus). PostgreSQL outbox with polling provides reliable at-least-once delivery without additional infrastructure cost.
- **Justification**: NFR-005 (service decoupling), NFR-010 (fault tolerance). The outbox pattern ensures no event loss during service restarts.
- **Trade-off**: Polling-based outbox introduces latency (configurable 5–30s polling interval) compared to push-based brokers. Acceptable for notification and async processing use cases.

### Decision 4: CQRS-Lite for Clinical Read Models

- **Decision**: Apply Command-Query Responsibility Segregation (CQRS) for the 360-Degree Patient View (Domain L) and Clinical Timeline (FR-047), maintaining separate denormalized read models updated by domain events from extraction and conflict resolution pipelines.
- **Rationale**: The 360-degree view aggregates data from multiple services (documents, intake, coding). Building this view on every read would require expensive cross-service joins. Pre-computed read models served from Upstash Redis provide sub-200ms response times.
- **Justification**: NFR-008 (≤200ms read response), DR-004 (traceability through source references in read model), AIR-009 (aggregation requirement).
- **Trade-off**: Eventual consistency between write and read models (target: ≤5 seconds lag). Staff verification status changes propagate synchronously to avoid stale verification indicators.

### Decision 5: Background Job Processing with Hangfire

- **Decision**: Use Hangfire with PostgreSQL storage for all background and scheduled jobs: document OCR/extraction pipeline, no-show risk batch scoring, notification scheduling, preferred slot swap matching, and outbox event processing.
- **Rationale**: FR-045 (document extraction) and FR-030 (no-show scoring) are computationally intensive and must not block API response threads. Hangfire provides persistent job storage, automatic retries, and a built-in dashboard — all backed by the existing PostgreSQL instance.
- **Justification**: NFR-007 (background processing), NFR-010 (retry with backoff), AIR-010 (circuit breaker integration for Gemini calls within jobs).
- **Trade-off**: Hangfire on free-tier PostgreSQL shares database resources with application queries. Mitigated by scheduling heavy jobs during off-peak hours and limiting concurrent worker threads.

### Decision 6: Encryption Middleware for PHI Protection

- **Decision**: Implement a .NET 8 middleware layer that transparently encrypts PHI fields (patient name, DOB, SSN, medical data) before database persistence using AES-256-GCM with envelope encryption (data encryption keys wrapped by a master key stored in environment variables), and decrypts on read for authorized requests only.
- **Rationale**: HIPAA requires encryption at rest for all PHI. Column-level encryption provides defense-in-depth beyond PostgreSQL's native TDE, ensuring PHI remains protected even if database backups are compromised.
- **Justification**: NFR-001 (AES-256 at rest), NFR-002 (defense-in-depth), DR-003 (document encryption), FR-043 (AES-256 for uploads).
- **Trade-off**: Column-level encryption prevents database-side searching on encrypted fields. Mitigated by maintaining indexed non-PHI lookup columns (patientId, appointmentId) and using application-layer search for PHI fields.

## Technology Stack

| Layer | Technology | Version | Justification |
|---|---|---|---|
| Frontend Framework | Angular | 17 | Spec constraint; standalone components reduce bundle size; reactive forms for complex intake UI |
| Frontend State | RxJS + NgRx SignalStore | 17.x / 17.x | Reactive data flow for real-time appointment status; signal-based lightweight store for local state |
| Frontend UI Library | Angular Material | 17.x | WCAG 2.1 AA accessible components; HIPAA-grade form controls with built-in validation |
| Frontend Hosting | Netlify or Vercel | Free tier | NFR-012 zero-cost constraint; CDN-backed static hosting with SSL |
| API Gateway | .NET 8 + YARP | 8.0 / 2.1 | Centralized RBAC, JWT validation, rate limiting; native .NET integration |
| Backend Framework | ASP.NET Core Web API | 8.0 | Spec constraint; high-performance minimal APIs; native DI container |
| Backend Hosting | Railway or Render | Free tier | NFR-012 zero-cost constraint; container-based deployment with PostgreSQL add-on |
| ORM | Entity Framework Core | 8.0 | Code-first migrations; LINQ query composition; PostgreSQL provider (Npgsql) |
| Primary Database | PostgreSQL | 16.x | Spec constraint; JSONB for flexible clinical data; robust free-tier availability |
| Cache / Session Store | Upstash Redis | Free tier | NFR-008 sub-200ms reads; serverless Redis with encryption at rest; 10K commands/day free |
| Background Jobs | Hangfire | 1.8.x | PostgreSQL-backed persistent jobs; automatic retries; dashboard monitoring |
| In-Process Mediator | MediatR | 12.x | CQRS command/query separation; domain event dispatching; pipeline behaviors for cross-cutting concerns |
| Authentication | ASP.NET Core Identity + JWT | 8.0 | NFR-001 RBAC; bcrypt password hashing; refresh token rotation |
| Email Service | SendGrid | Free tier | NFR-012 constraint; 100 emails/day free; transactional email API |
| AI / LLM Provider | Google Gemini API | Free tier | AIR-001 through AIR-009; 15 RPM free; structured output mode for clinical extraction |
| OCR Preprocessing | Tesseract.NET | 5.x | AIR-002 document text extraction; open-source; no API cost |
| API Documentation | Swashbuckle (Swagger) | 6.x | Auto-generated OpenAPI spec from controller metadata |
| Logging | Serilog + Seq (free single-user) | 3.x | Structured logging; centralized log aggregation; HIPAA audit trail support |
| Testing | xUnit + Moq + Playwright | Latest | Unit, integration, and E2E testing; .NET ecosystem standard |

### AI Component Stack

| Component | Technology | Purpose | Free-Tier Limit |
|---|---|---|---|
| LLM Provider | Google Gemini API (gemini-1.5-flash) | Conversational intake, clinical extraction, ICD-10/CPT suggestion, conflict analysis | 15 RPM, 1M tokens/day |
| OCR Engine | Tesseract.NET (via Tesseract wrapper) | PDF text extraction preprocessing before LLM analysis | Unlimited (local) |
| Prompt Management | Custom .NET 8 PromptTemplate service | Versioned prompt templates with variable injection for each AI pipeline | N/A |
| Confidence Scoring | Custom scoring layer | Normalizes Gemini confidence outputs to 0.0–1.0 scale per extracted field | N/A |
| Circuit Breaker | Polly (.NET resilience library) | AIR-010 circuit breaker (3-failure threshold, 60s recovery) for all Gemini calls | N/A |
| Risk Scoring Engine | Custom weighted algorithm | No-show risk calculation (deterministic, no LLM dependency) | N/A |

### Alternative Technology Options

| Layer | Selected | Alternative 1 | Alternative 2 | Rejection Reason |
|---|---|---|---|---|
| Frontend | Angular 17 | React 18 | Vue 3 | Spec mandates Angular; team expertise alignment |
| Backend | .NET 8 | Node.js (Express) | Java (Spring Boot) | Spec mandates .NET; Entity Framework integration |
| Database | PostgreSQL | SQL Server Express | MySQL | SQL Server Express has 10GB limit; MySQL lacks JSONB; PostgreSQL free-tier availability superior |
| Cache | Upstash Redis | Memcached | In-memory cache | Memcached has no persistence; in-memory cache lost on restart; Upstash provides encrypted serverless Redis |
| AI Provider | Google Gemini | OpenAI GPT-4o-mini | Anthropic Claude | Gemini free tier is most generous (15 RPM vs 3 RPM); structured output mode simplifies extraction |
| Message Bus | PostgreSQL Outbox | RabbitMQ | Redis Streams | RabbitMQ requires dedicated infrastructure (cost); Redis Streams on Upstash free tier has insufficient throughput |
| Background Jobs | Hangfire | Quartz.NET | .NET BackgroundService | Quartz.NET lacks built-in dashboard; BackgroundService lacks persistence and retry logic |
| API Gateway | YARP | Ocelot | Kong | Ocelot is less performant; Kong requires separate infrastructure |

### Technology Decision Records

| Decision ID | Decision | Rationale | Alternatives Considered | Trade-offs |
|---|---|---|---|---|
| TD-001 | Angular 17 with Standalone Components | Spec constraint + reduced NgModule boilerplate + tree-shakable | React 18, Vue 3 | Angular's larger bundle size mitigated by lazy loading |
| TD-002 | .NET 8 Minimal APIs | Spec constraint + 40% less boilerplate than traditional controllers + native AOT-ready | Express.js, Spring Boot | Minimal API lacks built-in model validation; added via FluentValidation |
| TD-003 | PostgreSQL with schema-per-service | Free tier availability + JSONB for flexible clinical data + single instance cost optimization | Separate DB instances, MongoDB | Cross-schema queries require explicit service calls; no multi-document ACID |
| TD-004 | Upstash Redis for caching | Serverless + encrypted at rest + free 10K commands/day + global replication | In-memory, Memcached | 10K command limit requires aggressive cache TTL management |
| TD-005 | Google Gemini (gemini-1.5-flash) | Most generous free tier (15 RPM, 1M tokens/day) + structured output mode + multimodal | OpenAI, Anthropic | Less mature than GPT-4; structured output requires careful prompt engineering |
| TD-006 | Hangfire on PostgreSQL storage | Zero additional infrastructure + persistent jobs + retry + dashboard | Quartz.NET, custom BackgroundService | Shares DB resources; mitigated by off-peak scheduling |
| TD-007 | YARP as API Gateway | Native .NET 8 integration + high performance + built-in load balancing | Ocelot, standalone gateway | Less ecosystem tooling than Kong; sufficient for project scale |
| TD-008 | Polly for resilience | .NET-native circuit breaker + retry + timeout policies + Gemini API protection | Custom implementation, Hystrix | Additional dependency; justified by AIR-010 requirement |
| TD-009 | Serilog + Seq for logging | Structured logging + centralized aggregation + free single-user Seq license | NLog, ELK Stack | Seq free tier limited to single user; sufficient for development/pilot |
| TD-010 | Entity Framework Core 8 | Code-first migrations + LINQ + Npgsql provider + audit interceptors | Dapper, raw ADO.NET | EF Core overhead acceptable for this scale; audit interceptors simplify NFR-003 |

## Technical Requirements

- TR-001: [SOURCE:INPUT] The frontend MUST be built using Angular 17 with Standalone Components (no NgModules), Reactive Forms for all data entry surfaces, and RxJS for reactive state management.
  Basis: BRD and spec mandate Angular 17; standalone components are Angular 17's recommended pattern.
- TR-002: [SOURCE:INPUT] The backend MUST be built as ASP.NET Core 8.0 Web API services using the minimal API pattern with dependency injection, following a 3-tier layered architecture (Controller/Service/Repository) within each microservice.
  Basis: BRD mandates .NET 8; 3-tier architecture specified in technology constraints.
- TR-003: [SOURCE:INPUT] All inter-service and client-server communication MUST use RESTful HTTP/JSON APIs over TLS 1.2+ with JWT Bearer token authentication, following OpenAPI 3.0 specification documented via Swashbuckle.
  Basis: NFR-002 (TLS 1.2+), NFR-001 (JWT authentication), standard REST API design.
- TR-004: [SOURCE:INPUT] The system MUST use PostgreSQL as the sole relational database, with Entity Framework Core 8 as the ORM, implementing code-first migrations and schema-per-service isolation on a single database instance.
  Basis: BRD mandates PostgreSQL; free-tier constraint requires single instance; Decision 1 specifies schema-per-service.
- TR-005: [SOURCE:INPUT] The system MUST implement Upstash Redis (free tier) for session token storage, API response caching (TTL-based), and CQRS read model serving, with all cached data encrypted via Upstash's built-in encryption.
  Basis: NFR-008 (sub-200ms reads), Decision 4 (CQRS read models), Upstash selection for zero-cost serverless Redis.
- TR-006: [SOURCE:INPUT] All AI inference calls to Google Gemini API MUST use structured output mode (JSON schema enforcement) with versioned prompt templates, implementing Polly circuit breaker (3-failure threshold, 60-second recovery) and exponential backoff retry (max 3 retries).
  Basis: AIR-001 through AIR-009 (Gemini usage), AIR-010 (circuit breaker), TD-005 (structured output).
- TR-007: [SOURCE:INPUT] The system MUST implement background job processing using Hangfire with PostgreSQL storage for: document OCR/extraction pipeline, no-show risk batch scoring (nightly), notification scheduling, preferred slot swap matching, and outbox event polling.
  Basis: Decision 5 (Hangfire), FR-045 (extraction), FR-030 (risk scoring), FR-034 (notifications), FR-022 (swap matching).
- TR-008: [SOURCE:INPUT] The frontend MUST implement lazy loading for all feature modules, achieving an initial bundle size under 300KB gzipped, with route-based code splitting for Patient, Staff, and Admin dashboards.
  Basis: NFR-008 (≤2s page load), NFR-012 (free-tier CDN hosting optimized for bandwidth), Angular 17 router lazy loading.
- TR-009: [SOURCE:INPUT] The system MUST implement comprehensive logging using Serilog with structured JSON output, including correlation IDs across service boundaries, with all PHI fields masked in log output and audit-relevant events forwarded to the immutable AuditLogEntry table.
  Basis: NFR-003 (audit logging), NFR-001 (PHI protection), Decision 6 (encryption middleware).
- TR-010: [SOURCE:INPUT] The system MUST implement automated database migrations via Entity Framework Core CLI tooling, with migration scripts version-controlled in the repository and applied as part of the CI/CD deployment pipeline.
  Basis: TR-004 (EF Core), standard deployment practice for schema evolution.
- TR-011: [SOURCE:INFERRED] The system MUST implement health check endpoints (/health, /ready) for each microservice, reporting database connectivity, Redis connectivity, and Gemini API availability, consumable by the hosting platform's health monitoring.
  Basis: Free-tier platforms (Railway, Render) use health checks to manage container lifecycle; no explicit health check requirement in spec.
- TR-012: [SOURCE:INFERRED] The system MUST implement request/response compression (Brotli preferred, gzip fallback) for all API responses exceeding 1KB, reducing bandwidth consumption on free-tier hosting.
  Basis: NFR-012 (free-tier optimization), NFR-008 (performance); not explicitly stated in spec.
- TR-013: [SOURCE:INFERRED] The frontend MUST implement a Progressive Web App (PWA) manifest with service worker for offline caching of static assets and previously loaded patient-facing pages, providing graceful degradation when network connectivity is intermittent.
  Basis: Healthcare settings often have unreliable connectivity; not stated in spec but improves NFR-008 user experience.

## Technical Constraints & Assumptions

### Constraints

1. **Free-Tier Infrastructure Only (HARD)**: All hosting, database, cache, AI API, and email services must operate within their respective free-tier limits. No paid upgrades permitted during initial development and pilot phases. This constrains: database size (≤1GB PostgreSQL), Redis commands (≤10K/day Upstash), AI inference rate (≤15 RPM Gemini), email volume (≤100/day SendGrid), and container resources (≤512MB RAM per service on Railway/Render).
2. **Angular 17 Frontend (HARD)**: The frontend framework is fixed at Angular 17 with TypeScript. No React, Vue, or other SPA frameworks. Standalone Components pattern required (no NgModules).
3. **.NET 8 Backend (HARD)**: The backend framework is fixed at ASP.NET Core 8.0. No Node.js, Java, or Python backend services. All microservices must share the same .NET 8 runtime.
4. **PostgreSQL Database (HARD)**: The relational database is fixed at PostgreSQL. No SQL Server, MySQL, MongoDB, or other database engines for primary data storage.
5. **HIPAA Compliance (HARD)**: All design decisions must satisfy HIPAA Security Rule (§164.312) and Privacy Rule requirements. PHI must be encrypted at rest (AES-256) and in transit (TLS 1.2+). Audit logs must be immutable and retained for minimum 6 years.
6. **Single-Region Deployment**: Free-tier hosting constrains deployment to a single region. No multi-region or geo-redundant deployment is possible within budget.
7. **No Dedicated Message Broker**: Free-tier constraints preclude RabbitMQ, Kafka, or Azure Service Bus. All async communication must use the PostgreSQL outbox pattern.
8. **Container Resource Limits**: Free-tier containers on Railway/Render are limited to 512MB RAM and shared CPU. Services must be optimized for minimal memory footprint.

### Assumptions

1. **Pilot Scale**: The system is designed for a single clinic with up to 50 concurrent users (patients + staff), approximately 200 appointments/day, and fewer than 50 document uploads/day. This assumption justifies shared PostgreSQL instance and free-tier API limits.
2. **English Language Only**: All UI, clinical data extraction, and AI prompts target English-only content. Multi-language support is out of scope for the initial release.
3. **PDF Documents Only**: Clinical document upload and AI extraction are scoped to PDF files only. DICOM, HL7 FHIR, and other healthcare-specific formats are out of scope.
4. **No Real Insurance Integration**: Insurance verification uses a predefined dummy record set (FR-038). No integration with real insurance payer APIs is planned.
5. **Staff Pre-Provisioned**: Staff and Admin accounts are created by the Admin role through the admin panel (FR-062). There is no self-registration for Staff or Admin roles.
6. **Browser-Only Access**: The platform targets modern desktop browsers (Chrome 90+, Firefox 90+, Edge 90+, Safari 15+). Native mobile applications are out of scope; the Angular 17 frontend provides responsive design for mobile browsers.
7. **Gemini API Stability**: The Google Gemini API free tier is assumed to remain available with current rate limits (15 RPM, 1M tokens/day) through the development and pilot phases. API deprecation would require migration to an alternative LLM provider.
8. **UTC Timezone for Backend**: All server-side timestamps are stored in UTC. Timezone conversion occurs at the frontend presentation layer based on the clinic's configured timezone.
9. **Single Tenant**: The system is designed for a single clinic/organization. Multi-tenancy is not supported in the initial architecture.
