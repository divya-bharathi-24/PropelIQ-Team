# Task - task_001_data_operations_compliance

## Requirement Reference

- **User Story:** us_006
- **Story Location:** `.propel/context/tasks/EP-DATA/us_006/us_006.md`
- **Acceptance Criteria:**
  - AC-1: Upstash Redis caching layer configured — cache-aside pattern returns data within 50ms for hits; TTL 5 min for schedules, 30s for availability
  - AC-2: Database seeding for development — realistic test data (50 patients, 10 providers, 200 appointments, 5 roles) without constraint violations; completes in under 30 seconds
  - AC-3: Automated backup schedule — daily at 02:00 UTC; point-in-time backup retained 7 days on free tier; verify restore capability
  - AC-4: Data retention policies — HIPAA 6-year minimum; weekly Hangfire job flags records for review; notification logs archived after 90 days; no PII auto-deleted
  - AC-5: Cache invalidation on data mutation — relevant cache keys invalidated within 1 second of appointment booking/cancellation; next read fetches from PostgreSQL
- **Edge Cases:**
  - Upstash Redis connection failure → fallback to direct DB query; log warning, do not fail
  - Backup storage exceeds free-tier limit → alert admin; retain 3 most recent backups minimum
  - Cache stampede on popular provider → probabilistic early expiration (jitter) to prevent thundering herd

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
| Cache | Upstash Redis | Free tier | TR-005 — Serverless Redis with encryption at rest; 10K commands/day free |
| Backend Framework | ASP.NET Core Web API | 8.0 | TR-002 — Host for caching layer and background jobs |
| Background Jobs | Hangfire | 1.8.x | TR-007 — PostgreSQL-backed persistent jobs for retention and backup scheduling |
| Database | PostgreSQL | 16.x | TR-004 — Target for seeding, backup, and retention operations |
| ORM | Entity Framework Core | 8.0 | TR-004 — Data access for seeding and retention queries |

---

## Task Overview

Implement the data operations infrastructure including Upstash Redis caching with cache-aside pattern and TTL management, database seeding for development environments, automated backup scheduling, HIPAA-compliant data retention policies via Hangfire background jobs, and cache invalidation on appointment mutations. Includes Redis fallback to direct DB queries and probabilistic early expiration to prevent cache stampedes.

## Dependent Tasks

- task_001_core_domain_schemas (US_004) — Core domain schemas must exist before caching and seeding can target them

## Impacted Components

- New: `src/Shared/HealthPlatform.Shared/Caching/RedisCacheService.cs` — Redis cache-aside implementation
- New: `src/Shared/HealthPlatform.Shared/Caching/CacheKeyManager.cs` — Cache key generation and invalidation
- New: `src/Services/Appointment/HealthPlatform.Appointment.Api/Caching/SlotCacheInvalidator.cs` — Appointment mutation cache invalidation
- New: `src/Shared/HealthPlatform.Shared/Seeding/DatabaseSeeder.cs` — Development data seeder
- New: `src/Shared/HealthPlatform.Shared/Jobs/DataRetentionJob.cs` — HIPAA retention Hangfire job
- New: `src/Shared/HealthPlatform.Shared/Jobs/NotificationArchivalJob.cs` — 90-day notification log archival
- New: `src/Shared/HealthPlatform.Shared/Jobs/BackupVerificationJob.cs` — Backup schedule and verification

## Implementation Plan

1. Configure Upstash Redis connection with StackExchange.Redis and implement cache-aside pattern service with configurable TTLs (5 min schedules, 30s availability).
2. Implement probabilistic early expiration (jitter) on cache reads to prevent thundering herd scenarios.
3. Add Redis connection failure fallback — catch connection exceptions, log warning, fall through to database query.
4. Create database seeder generating realistic test data (50 patients, 10 providers, 200 appointments, 5 roles) respecting all constraints.
5. Configure Hangfire recurring job for daily backup verification at 02:00 UTC with Railway/Render backup API integration.
6. Implement weekly HIPAA data retention Hangfire job — flag records older than 6 years for admin review; archive notification logs older than 90 days.
7. Implement cache invalidation on appointment booking/cancellation — invalidate provider schedule and slot availability keys within 1 second.
8. Add backup storage monitoring with admin alerting at 80% capacity threshold.

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
│   │   └── Data/ (DocumentDbContext, entities, migrations)
│   ├── ClinicalIntelligence/HealthPlatform.ClinicalIntelligence.Api/
│   │   └── Data/ (ClinicalDbContext, entities, migrations)
│   ├── Notification/HealthPlatform.Notification.Api/
│   │   └── Data/ (NotificationDbContext, entities, migrations)
│   └── Admin/HealthPlatform.Admin.Api/
│       └── Data/ (AuditDbContext, entities, migrations)
└── Shared/
    └── HealthPlatform.Shared/
        ├── Data/ (PatientDbContext, entities)
        └── Encryption/ (AesValueConverter)
```

## Expected Changes

| Action | File Path | Description |
|--------|-----------|-------------|
| CREATE | `src/Shared/HealthPlatform.Shared/Caching/RedisCacheService.cs` | Cache-aside pattern with TTL and jitter |
| CREATE | `src/Shared/HealthPlatform.Shared/Caching/ICacheService.cs` | Cache service interface for DI |
| CREATE | `src/Shared/HealthPlatform.Shared/Caching/CacheKeyManager.cs` | Cache key generation and invalidation logic |
| CREATE | `src/Services/Appointment/HealthPlatform.Appointment.Api/Caching/SlotCacheInvalidator.cs` | Invalidates slot/schedule cache on mutations |
| CREATE | `src/Shared/HealthPlatform.Shared/Seeding/DatabaseSeeder.cs` | Test data generator (50 patients, 10 providers, 200 appointments) |
| CREATE | `src/Shared/HealthPlatform.Shared/Seeding/SeedDataFactory.cs` | Realistic test data factory |
| CREATE | `src/Shared/HealthPlatform.Shared/Jobs/DataRetentionJob.cs` | Weekly HIPAA retention check (6-year flag) |
| CREATE | `src/Shared/HealthPlatform.Shared/Jobs/NotificationArchivalJob.cs` | 90-day notification log archival |
| CREATE | `src/Shared/HealthPlatform.Shared/Jobs/BackupVerificationJob.cs` | Daily backup at 02:00 UTC with verification |

## External References

- [Upstash Redis Documentation](https://upstash.com/docs/redis/overall/getstarted)
- [StackExchange.Redis](https://stackexchange.github.io/StackExchange.Redis/)
- [Hangfire Recurring Jobs](https://docs.hangfire.io/en/latest/background-methods/performing-recurrent-tasks.html)
- [EF Core Seeding](https://learn.microsoft.com/en-us/ef/core/modeling/data-seeding)
- [PostgreSQL Point-in-Time Recovery](https://www.postgresql.org/docs/16/continuous-archiving.html)

## Build Commands

- Refer to [.propel/build/](.propel/build/) for applicable technology stack build commands

## Implementation Validation Strategy

- [ ] Unit tests pass — cache-aside reads/writes with TTL; seeder generates valid data
- [ ] Integration tests pass — Redis cache invalidation within 1 second of mutation; Hangfire jobs schedule correctly

## Implementation Checklist

- [x] Configure Upstash Redis with cache-aside pattern and TTLs (5 min schedules, 30s availability) → AC-1
- [x] Create database seeder with realistic test data (50 patients, 10 providers, 200 appointments, 5 roles) completing in under 30s → AC-2
- [x] Configure automated daily backup at 02:00 UTC with 7-day retention and restore verification → AC-3
- [x] Implement HIPAA data retention Hangfire job (weekly, 6-year flag for review, no auto-delete PII) → AC-4
- [x] Implement notification log archival for logs older than 90 days → AC-4
- [x] Implement cache invalidation on appointment booking/cancellation within 1 second → AC-5
- [x] Add Redis connection failure fallback to direct DB query → AC-1 (edge case)
- [x] Add probabilistic early expiration (jitter) to prevent cache stampede → AC-1 (edge case)
