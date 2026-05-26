# Epic Decomposition

## Epic Summary Table

| Epic ID | Epic Title | Mapped Requirement IDs |
|---------|------------|------------------------|
| EP-TECH | Project Scaffolding & Infrastructure | TR-001, TR-002, TR-003, TR-008, TR-009, TR-010, TR-011, TR-012, TR-013 |
| EP-DATA | Database Schema & Data Layer | TR-004, TR-005, DR-001, DR-002, DR-003, DR-004, DR-005, DR-006, DR-007, DR-008, DR-009, DR-010, DR-011 |
| EP-001 | Authentication & Authorization | FR-001, FR-002, FR-003, FR-004, FR-005, FR-006, NFR-002, NFR-007, NFR-008, NFR-009 |
| EP-002 | Patient Profile & Dashboard | FR-007, FR-008, FR-009, FR-010 |
| EP-003 | Appointment Booking & Insurance Pre-Check | FR-011, FR-012, FR-013, FR-014, FR-015, FR-038, FR-039, FR-040, NFR-003, NFR-006, NFR-011 |
| EP-004 | Preferred Slot Swap & Calendar Sync | FR-020, FR-021, FR-022, FR-023, FR-035, FR-036, FR-037, NFR-004 |
| EP-005 | Digital Intake Management | FR-016, FR-017, FR-018, FR-019, AIR-001, TR-006 |
| EP-006 | Walk-In & Queue Management | FR-024, FR-025, FR-026, FR-027 |
| EP-007 | Reminders & No-Show Risk Prediction | FR-028, FR-029, FR-030, FR-031, FR-032, FR-033, FR-034, AIR-006, TR-007 |
| EP-008 | Clinical Document Upload & Processing | FR-041, FR-042, FR-043, FR-044, AIR-002, AIR-008 |
| EP-009 | 360-Degree Patient View & Conflict Resolution | FR-045, FR-046, FR-047, FR-048, FR-049, FR-054, FR-055, FR-056, AIR-005, AIR-007, AIR-009, NFR-005 |
| EP-010 | Medical Coding — ICD-10 & CPT | FR-050, FR-051, FR-052, FR-053, AIR-003, AIR-004 |
| EP-011 | Admin Management & Audit Compliance | FR-057, FR-058, FR-059, FR-060, FR-061, FR-062, NFR-001, NFR-010, NFR-012, NFR-013, NFR-014, AIR-010 |

## Epic Description

### EP-TECH: Project Scaffolding & Infrastructure

**Business Value**: Establishes the foundational development environment, CI/CD pipeline, and shared infrastructure upon which all feature epics are built. Without EP-TECH, no feature work can begin.

**Description**: Set up the Angular 17 frontend project with Standalone Components, Reactive Forms, and RxJS. Scaffold the .NET 8 backend with minimal API pattern, YARP API Gateway, and 7 microservice project structure. Configure Serilog structured logging, EF Core migration tooling, health check endpoints, request compression, and PWA manifest. Establish the monorepo structure, CI/CD pipeline, and developer environment.

**UI Impact**: Yes

**Screen References**: Application shell (nav, sidebar, route outlet), health check dashboard (DevOps).

**Key Deliverables**:

- Angular 17 project with Standalone Components, Angular Material, and route-based lazy loading (TR-001, TR-008)
- .NET 8 solution with 7 microservice projects + YARP API Gateway (TR-002, TR-003)
- Serilog logging with correlation IDs and PHI masking (TR-009)
- EF Core migration tooling integrated with CI/CD (TR-010)
- Health check endpoints (/health, /ready) per microservice (TR-011)
- Brotli/gzip response compression middleware (TR-012)
- PWA manifest and service worker for static asset caching (TR-013)

**Dependent EPICs**:
- None

---

### EP-DATA: Database Schema & Data Layer

**Business Value**: Creates the complete data persistence layer — PostgreSQL schemas, Entity Framework Core models, Redis caching, and seed data — enabling all feature epics to store and retrieve domain data.

**Description**: Implement schema-per-service isolation on the shared PostgreSQL 16 instance. Define all domain entity models via EF Core code-first migrations: Patient, User, Appointment, PreferredSwapEntry, IntakeRecord, ClinicalDocument, ExtractedDataElement, PatientView360, DataConflict, MedicalCode, AuditLogEntry, NotificationEvent, InsuranceDummyRecord. Configure Upstash Redis for session storage, API caching, and CQRS read models. Implement automated daily backup strategy and 6-year HIPAA retention policy.

**UI Impact**: No

**Screen References**: N/A

**Key Deliverables**:

- PostgreSQL schema-per-service with EF Core 8 + Npgsql provider (TR-004)
- Upstash Redis integration for cache, sessions, and CQRS read models (TR-005)
- Patient demographic schema (DR-001)
- Appointment lifecycle model with status tracking (DR-002)
- Encrypted clinical document storage model (DR-003)
- Extracted data element schema with provenance (DR-004)
- Medical code storage schema (DR-005)
- Immutable audit log table (DR-006)
- Preferred slot waitlist FIFO queue model (DR-007)
- Dummy insurance record table (DR-008)
- Notification delivery event log (DR-009)
- 6-year HIPAA retention and archival policy (DR-010)
- Automated daily encrypted backup with PITR (DR-011)

**Dependent EPICs**:
- EP-TECH - Foundational - Requires project scaffolding and infrastructure

---

### EP-001: Authentication & Authorization

**Business Value**: Secures the platform with HIPAA-compliant authentication, RBAC, and session management — the mandatory prerequisite for every user-facing feature.

**Description**: Implement ASP.NET Core Identity with JWT Bearer authentication and refresh token rotation. Enforce three mutually exclusive roles (Patient, Staff, Admin) at the YARP API Gateway and service level. Build patient self-registration with email verification, staff-created patient accounts for walk-ins, 15-minute session auto-timeout, and admin-exclusive account management. Implement AES-256 at-rest encryption and TLS 1.2+ enforcement. Record all authentication events to the immutable audit log.

**UI Impact**: Yes

**Screen References**: UC-001 Steps 1–3 (registration flow), Domain A screens (login, register, session timeout).

**Key Deliverables**:

- RBAC enforcement with Patient/Staff/Admin roles (FR-001, NFR-009)
- Patient self-registration with email verification (FR-002)
- Staff-created patient account during walk-in (FR-003)
- 15-minute session auto-timeout with re-authentication (FR-004, NFR-002)
- Admin-exclusive user account CRUD (FR-005)
- Authentication event audit logging (FR-006)
- HIPAA Privacy and Security Rule compliance (NFR-007)
- AES-256 at rest + TLS 1.2+ in transit encryption (NFR-008)

**Dependent EPICs**:
- EP-TECH - Foundational - Requires project scaffolding and infrastructure
- EP-DATA - Foundational - Requires User and Patient entity schemas

---

### EP-002: Patient Profile & Dashboard

**Business Value**: Provides patients with self-service access to their profile and a unified dashboard showing appointments, intake status, and document upload history — reducing front-desk inquiries and improving patient engagement.

**Description**: Build the patient profile view and edit interface for non-locked fields (contact info, emergency contact, communication preferences). Implement structured demographic data persistence. Create the patient dashboard with upcoming appointments, pending intake items, document upload history, and 360-degree view availability indicator. Enable self-service intake editing.

**UI Impact**: Yes

**Screen References**: Domain B screens (profile view/edit, dashboard), UC-001 Step 10 (dashboard after booking).

**Key Deliverables**:

- Patient profile view and edit for non-locked fields (FR-007)
- Structured demographic data storage and display (FR-008)
- Patient dashboard: appointments, intake, uploads, 360° indicator (FR-009)
- Self-service intake edit flow without staff intervention (FR-010)

**Dependent EPICs**:
- EP-TECH - Foundational - Requires frontend and backend infrastructure
- EP-DATA - Foundational - Requires Patient entity schema

---

### EP-003: Appointment Booking & Insurance Pre-Check

**Business Value**: Enables patients to self-schedule appointments through a single-workflow booking experience with real-time slot availability, concurrency-safe reservation, PDF confirmation, and soft insurance validation — the core patient-facing transaction.

**Description**: Implement real-time appointment slot display with ≤2-second staleness. Build the end-to-end booking workflow (slot selection → insurance pre-check → intake mode selection → confirmation) as a single unified flow. Apply optimistic locking for double-booking prevention. Generate branded PDF confirmations sent via SendGrid within 60 seconds. Implement appointment cancellation and rescheduling with immediate slot release. Build the soft insurance pre-check against dummy insurer records with clear status display (Verified/Not Recognized/Incomplete).

**UI Impact**: Yes

**Screen References**: UC-001 Steps 4–10 (full booking flow), Domain C screens (slot display, booking wizard), Domain J screens (insurance pre-check).

**Key Deliverables**:

- Real-time available slot display with ≤2s staleness (FR-011, NFR-006)
- Single-workflow booking: slot → insurance → intake mode → confirm (FR-012)
- Optimistic locking for double-booking prevention (FR-013, NFR-011)
- Appointment cancellation and rescheduling (FR-014)
- PDF confirmation email within 60 seconds via SendGrid (FR-015, NFR-003)
- Soft insurance pre-check against dummy records (FR-038)
- Insurance validation status display with guidance (FR-039)
- Non-blocking insurance flow — booking proceeds regardless (FR-040)

**Dependent EPICs**:
- EP-TECH - Foundational - Requires frontend and backend infrastructure
- EP-DATA - Foundational - Requires Appointment and InsuranceDummyRecord schemas

---

### EP-004: Preferred Slot Swap & Calendar Sync

**Business Value**: Maximizes slot utilization and reduces patient scheduling friction by automatically swapping patients into their preferred slots when cancellations occur, and keeping external calendars synchronized — directly impacting revenue recovery from unfilled cancellations.

**Description**: Implement preferred slot designation during booking (waitlist queue registration). Build a background monitoring service (Hangfire) that continuously detects when waitlisted preferred slots become available via cancellation or rescheduling events. Execute automatic slot swaps within 60 seconds — cancelling the current booking, assigning the preferred slot, and releasing the original. Send dual-channel (email + SMS) swap notifications. Implement calendar sync to Google Calendar and Microsoft Outlook via iCalendar/ICS format, with event creation, update on reschedule, and removal on cancellation.

**UI Impact**: Yes

**Screen References**: UC-001 Step 5 (preferred slot designation), UC-004 (swap execution), Domain E screens (preferred slot UI), Domain I screens (calendar sync settings).

**Key Deliverables**:

- Preferred slot designation during booking with waitlist registration (FR-020)
- Continuous monitoring of waitlisted preferred slot availability (FR-021)
- Automatic slot swap execution within 60 seconds (FR-022, NFR-004)
- Dual-channel swap notification (email + SMS) with delivery logging (FR-023)
- Google Calendar and Outlook sync via free APIs/ICS (FR-035)
- Calendar event creation with full appointment details (FR-036)
- Calendar event update on reschedule / removal on cancel (FR-037)

**Dependent EPICs**:
- EP-TECH - Foundational - Requires backend infrastructure and Hangfire setup
- EP-DATA - Foundational - Requires Appointment and PreferredSwapEntry schemas

---

### EP-005: Digital Intake Management

**Business Value**: Streamlines patient intake with an AI-conversational option that reduces time-to-complete and a manual fallback that ensures accessibility — collecting structured health data before appointments to improve clinical preparation.

**Description**: Build an AI-assisted conversational intake experience using Google Gemini API (gemini-1.5-flash) with structured output mode and versioned prompt templates. The chat interface dynamically guides patients through health history questions, interprets free-text responses via NLU, and auto-populates structured intake fields. Implement the traditional manual intake form as a fully functional alternative. Enable seamless mode switching (AI ↔ manual) with data preservation. Allow patients to return and edit previously submitted intake data. Integrate Polly circuit breaker (3-failure threshold, 60-second recovery) with graceful degradation to manual-only mode.

**UI Impact**: Yes

**Screen References**: UC-002 (AI-assisted intake), UC-003 (manual intake), Domain D screens (intake interface, mode switching).

**Key Deliverables**:

- AI-assisted conversational intake via Gemini API (FR-016, AIR-001)
- Manual intake form with all required fields (FR-017)
- Seamless AI ↔ manual mode switching with data preservation (FR-018)
- Post-submission intake editing without duplicates (FR-019)
- Gemini API calls with structured output, circuit breaker, and retry (TR-006)

**Dependent EPICs**:
- EP-TECH - Foundational - Requires frontend and backend infrastructure
- EP-DATA - Foundational - Requires IntakeRecord entity schema

---

### EP-006: Walk-In & Queue Management

**Business Value**: Enables front-desk staff to efficiently process walk-in patients with optional account creation and manage the same-day appointment queue with arrival tracking — critical for daily clinic operations.

**Description**: Build the staff-exclusive walk-in booking interface with optional patient account creation (name, contact, email) or anonymous visit tracking. Implement the same-day queue view displaying all daily appointments with patient name, time, booking type (self-booked/walk-in), and arrival status. Restrict "Arrived" status marking to Staff-role users only — no patient self-check-in mechanism.

**UI Impact**: Yes

**Screen References**: UC-005 (walk-in booking), UC-006 (queue management), Domain F screens (walk-in form, queue view, arrival marking).

**Key Deliverables**:

- Staff-exclusive walk-in appointment creation (FR-024)
- Optional patient account creation during walk-in (FR-025)
- Same-day queue view with all daily appointments (FR-026)
- Staff-only "Arrived" status marking (FR-027)

**Dependent EPICs**:
- EP-TECH - Foundational - Requires frontend and backend infrastructure
- EP-DATA - Foundational - Requires Appointment entity schema

---

### EP-007: Reminders & No-Show Risk Prediction

**Business Value**: Reduces patient no-show rates from the 15% baseline through intelligent risk scoring and multi-channel automated reminders — directly improving appointment slot utilization and clinic revenue.

**Description**: Implement a deterministic weighted scoring algorithm (no LLM) that calculates no-show risk scores (Low/Medium/High) based on prior no-show history, booking lead time, appointment type, intake completion status, and reminder response patterns. Run via nightly Hangfire batch job. Display color-coded risk scores (green/amber/red) on the Staff appointment interface. Auto-flag High-risk appointments with recommended interventions. Build multi-channel reminder scheduling (email + SMS) at configurable intervals (default: 48h, 24h, 2h). Support staff-triggered ad-hoc reminders. Log all delivery events to the audit trail.

**UI Impact**: Yes

**Screen References**: UC-007 (no-show risk review), Domain G screens (risk scores, flags), Domain H screens (reminder settings, delivery log).

**Key Deliverables**:

- No-show risk score calculation via weighted model (FR-028, AIR-006)
- Risk score display with color-coded severity (FR-029)
- High-risk auto-flagging with intervention recommendations (FR-030)
- Multi-channel automated reminders at configurable intervals (FR-031, FR-032)
- Staff-triggered ad-hoc reminders (FR-033)
- Reminder delivery event logging (FR-034)
- Hangfire background job for batch scoring and scheduling (TR-007)

**Dependent EPICs**:
- EP-TECH - Foundational - Requires backend infrastructure and Hangfire setup
- EP-DATA - Foundational - Requires Appointment and NotificationEvent schemas

---

### EP-008: Clinical Document Upload & Processing

**Business Value**: Enables patients and staff to upload clinical PDFs with HIPAA-compliant encrypted storage, and initiates the AI extraction pipeline that feeds the 360-degree patient view — the entry point for the clinical intelligence workflow.

**Description**: Build the patient-facing document upload interface accessible from the dashboard, supporting PDF format with 25 MB per-file and 20-file batch limits. Implement AES-256 at-rest encryption and TLS 1.2+ in-transit for all uploads. Allow staff to upload post-visit clinical notes linked to encounters. Trigger Hangfire background jobs for OCR preprocessing (Tesseract.NET) followed by Gemini API extraction of structured clinical data (vitals, medications, diagnoses, allergies, immunizations, surgical history) with per-field confidence scores. Implement cross-document de-duplication with source provenance chains.

**UI Impact**: Yes

**Screen References**: UC-008 (document upload), Domain K screens (upload interface, status tracker).

**Key Deliverables**:

- Patient PDF upload interface from dashboard (FR-041)
- Batch upload: 25 MB/file, 20 files/batch (FR-042)
- AES-256 at rest + TLS 1.2+ in transit encryption (FR-043)
- Staff upload of post-visit clinical notes (FR-044)
- OCR + LLM extraction pipeline with confidence scores (AIR-002)
- Cross-document de-duplication with provenance (AIR-008)

**Dependent EPICs**:
- EP-TECH - Foundational - Requires backend infrastructure and Hangfire setup
- EP-DATA - Foundational - Requires ClinicalDocument and ExtractedDataElement schemas

---

### EP-009: 360-Degree Patient View & Conflict Resolution

**Business Value**: Aggregates extracted clinical data into a single verified patient profile — reducing clinical retrieval from 20 minutes to 2 minutes — and surfaces data conflicts for mandatory staff resolution, ensuring clinical accuracy and patient safety.

**Description**: Aggregate all extracted, de-duplicated data elements into a unified 360-degree patient view with verification status workflow (Unverified → Verified → Complete). Build the staff review interface for verification. Construct a chronological clinical timeline ordered by event date with source attribution. Detect conflicting data across documents (medications, dosages, allergies, diagnoses) using rule-based pre-filter plus Gemini API semantic similarity analysis. Classify conflicts by severity (Critical/Warning). Highlight conflicts with side-by-side values and source identification. Require mandatory conflict resolution before profile completion. Maintain source traceability for every data element.

**UI Impact**: Yes

**Screen References**: UC-009 (360° review), UC-010 (conflict resolution), Domain L screens (aggregated view, timeline), Domain N screens (conflict panel).

**Key Deliverables**:

- AI extraction of structured data from uploaded PDFs (FR-045)
- Data aggregation into de-duplicated patient view (FR-046)
- Staff verification interface with status workflow (FR-047)
- 2-minute clinical retrieval verification workflow (FR-048, NFR-005)
- Source traceability for every extracted data element (FR-049)
- AI conflict detection across multiple documents (FR-054, AIR-005)
- Conflict highlighting with source identification (FR-055)
- Mandatory conflict resolution before profile completion (FR-056)
- Chronological clinical timeline with source attribution (AIR-007)
- Unified 360-degree aggregation with verification status (AIR-009)

**Dependent EPICs**:
- EP-TECH - Foundational - Requires backend infrastructure and Gemini API integration
- EP-DATA - Foundational - Requires PatientView360, DataConflict, and ExtractedDataElement schemas

---

### EP-010: Medical Coding — ICD-10 & CPT

**Business Value**: Automates ICD-10 and CPT code suggestion from aggregated clinical data, reducing manual coding time and improving accuracy — with staff confirmation ensuring clinical and billing compliance.

**Description**: Analyze aggregated patient clinical data to generate up to 5 ranked ICD-10 code suggestions per encounter, each mapped to the specific diagnosis or clinical finding supporting it. Generate up to 5 ranked CPT code suggestions per encounter mapped to documented procedures. Present all suggestions in a side-by-side staff review interface (code, description, supporting evidence) requiring explicit confirmation or rejection. Store confirmed codes linked to patient ID, encounter reference, confirming staff ID, and confirmation timestamp.

**UI Impact**: Yes

**Screen References**: UC-011 (medical coding review), Domain M screens (ICD-10/CPT suggestion table, evidence panel).

**Key Deliverables**:

- ICD-10 code suggestions with supporting evidence (FR-050, AIR-003)
- CPT code suggestions with supporting evidence (FR-051, AIR-004)
- Staff review interface with confirm/reject per code (FR-052)
- Confirmed code storage with encounter and staff linkage (FR-053)

**Dependent EPICs**:
- EP-TECH - Foundational - Requires backend infrastructure and Gemini API integration
- EP-DATA - Foundational - Requires MedicalCode entity schema

---

### EP-011: Admin Management & Audit Compliance

**Business Value**: Provides admin-exclusive user lifecycle management and a HIPAA-compliant immutable audit trail — ensuring regulatory compliance, operational governance, and platform reliability monitoring.

**Description**: Build the admin panel for Staff and Admin account CRUD (create, view, edit, soft-delete), role assignment and modification, and re-authentication before destructive actions. Implement the immutable append-only audit log capturing all patient data access events, clinical data modifications (with before/after state), and authentication events. Restrict audit log access to Admin read-only. Enforce 99.9% uptime via health monitoring. Ensure free-tier deployment compliance. Support 100 concurrent users without degradation. Implement stateless cold-start recovery. Integrate Polly circuit breaker for all Gemini API calls with graceful degradation.

**UI Impact**: Yes

**Screen References**: Domain O screens (audit log viewer), Domain P screens (admin user management panel).

**Key Deliverables**:

- Immutable audit log for all patient data access (FR-057)
- Clinical data modification logging with before/after state (FR-058)
- Admin-only read-only audit log access (FR-059)
- Admin CRUD on Staff and Admin accounts (FR-060)
- Admin role and permission assignment (FR-061)
- Admin re-authentication before destructive actions (FR-062)
- 99.9% uptime with automated health monitoring (NFR-001)
- Immutable append-only audit log enforcement (NFR-010)
- Free-tier deployment compliance (NFR-012)
- 100 concurrent users and 1,000 patient records capacity (NFR-013)
- Stateless design for sub-30s cold-start recovery (NFR-014)
- Circuit breaker for all AI calls with graceful degradation (AIR-010)

**Dependent EPICs**:
- EP-TECH - Foundational - Requires backend infrastructure and logging framework
- EP-DATA - Foundational - Requires AuditLogEntry and User entity schemas
