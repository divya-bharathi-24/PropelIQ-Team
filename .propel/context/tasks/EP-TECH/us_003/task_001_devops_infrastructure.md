# Task - task_001_devops_infrastructure

## Requirement Reference

- **User Story:** us_003
- **Story Location:** `.propel/context/tasks/EP-TECH/us_003/us_003.md`
- **Acceptance Criteria:**
  - AC-1: Docker containerization for all services — `docker compose up` starts all 7 microservices and API Gateway in isolated containers with health-check endpoints responding within 30 seconds
  - AC-2: CI pipeline runs on every pull request — executes build, unit tests, linting; fails PR if any step fails; completes within 5 minutes
  - AC-3: CD pipeline deploys to Railway/Render — builds Docker images, pushes to container registry, deploys with zero-downtime rolling updates
  - AC-4: Environment-specific configuration management — 3 environments (dev, staging, production) with correct config from env vars; no hard-coded secrets
  - AC-5: Health monitoring and structured logging — Serilog structured JSON with correlation IDs; health endpoints return degraded status within 10 seconds of failure
- **Edge Cases:**
  - Docker build cache invalidation → multi-stage builds ensure layer caching; worst-case full rebuild in under 3 minutes
  - Railway/Render free-tier cold start → health check retry with exponential backoff (max 60s) before marking deployment failed
  - Secret rotation during deployment → new values picked up on next container restart without code changes

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
| Backend Framework | ASP.NET Core Web API | 8.0 | TR-002 — Container target runtime for all microservices |
| Containerization | Docker | Latest | TR-011 — Isolated container deployment for each service |
| CI/CD | GitHub Actions | Latest | TR-010 — Automated build, test, deploy pipeline |
| Backend Hosting | Railway or Render | Free tier | NFR-012 — Zero-cost container deployment with PostgreSQL add-on |
| Logging | Serilog + Seq | 3.x / Free | TR-009 — Structured JSON logging with centralized aggregation |
| Database | PostgreSQL | 16.x | TR-004 — Health check target for database connectivity |
| Cache | Upstash Redis | Free tier | TR-005 — Health check target for cache connectivity |

---

## Task Overview

Establish the complete DevOps infrastructure for the platform including Docker containerization of all services, GitHub Actions CI/CD pipelines, environment-specific configuration management, and health monitoring with structured logging. This enables automated build-test-deploy workflows and production-grade observability.

## Dependent Tasks

- task_001_backend_api_scaffolding (US_002) — Backend service projects must exist before containerization

## Impacted Components

- New: `docker-compose.yml` — Multi-service Docker Compose orchestration
- New: `src/Gateway/HealthPlatform.Gateway/Dockerfile` — Gateway container definition
- New: `src/Services/*/Dockerfile` — Microservice container definitions (7 files)
- New: `.github/workflows/ci.yml` — CI pipeline configuration
- New: `.github/workflows/cd.yml` — CD pipeline configuration
- New: `src/Gateway/HealthPlatform.Gateway/appsettings.Development.json` — Dev environment config
- New: `src/Gateway/HealthPlatform.Gateway/appsettings.Staging.json` — Staging environment config
- New: `src/Gateway/HealthPlatform.Gateway/appsettings.Production.json` — Production environment config

## Implementation Plan

1. Create multi-stage Dockerfiles for each microservice and the API Gateway optimizing for layer caching and minimal image size.
2. Create `docker-compose.yml` with service definitions, health-check configurations, network isolation, and dependency ordering.
3. Configure GitHub Actions CI pipeline (`.github/workflows/ci.yml`) — triggered on PR, runs `dotnet build`, `dotnet test`, and linting with 5-minute timeout.
4. Configure GitHub Actions CD pipeline (`.github/workflows/cd.yml`) — triggered on main branch merge, builds Docker images, pushes to registry, deploys to Railway/Render.
5. Create environment-specific `appsettings.{Environment}.json` files for all services with placeholder tokens for secret injection.
6. Configure Serilog structured JSON logging with correlation ID enricher across all services.
7. Implement `/health` and `/ready` endpoints with database, Redis, and Gemini API connectivity checks.
8. Configure health check retry with exponential backoff for free-tier cold start recovery.

## Current Project State

```text
d:\PropelIQ-Team\
├── .propel/
│   └── context/
├── src/
│   ├── HealthPlatform.sln
│   ├── Gateway/
│   │   └── HealthPlatform.Gateway/
│   │       ├── Program.cs
│   │       └── appsettings.json
│   └── Services/
│       ├── Auth/HealthPlatform.Auth.Api/
│       ├── Appointment/HealthPlatform.Appointment.Api/
│       ├── Intake/HealthPlatform.Intake.Api/
│       ├── Document/HealthPlatform.Document.Api/
│       ├── ClinicalIntelligence/HealthPlatform.ClinicalIntelligence.Api/
│       ├── Notification/HealthPlatform.Notification.Api/
│       └── Admin/HealthPlatform.Admin.Api/
└── README.md
```

## Expected Changes

| Action | File Path | Description |
|--------|-----------|-------------|
| CREATE | `docker-compose.yml` | Multi-service orchestration with health checks and dependency ordering |
| CREATE | `src/Gateway/HealthPlatform.Gateway/Dockerfile` | Multi-stage Dockerfile for API Gateway |
| CREATE | `src/Services/Auth/HealthPlatform.Auth.Api/Dockerfile` | Multi-stage Dockerfile for Auth service |
| CREATE | `src/Services/Appointment/HealthPlatform.Appointment.Api/Dockerfile` | Multi-stage Dockerfile for Appointment service |
| CREATE | `src/Services/Intake/HealthPlatform.Intake.Api/Dockerfile` | Multi-stage Dockerfile for Intake service |
| CREATE | `src/Services/Document/HealthPlatform.Document.Api/Dockerfile` | Multi-stage Dockerfile for Document service |
| CREATE | `src/Services/ClinicalIntelligence/HealthPlatform.ClinicalIntelligence.Api/Dockerfile` | Multi-stage Dockerfile for Clinical Intelligence service |
| CREATE | `src/Services/Notification/HealthPlatform.Notification.Api/Dockerfile` | Multi-stage Dockerfile for Notification service |
| CREATE | `src/Services/Admin/HealthPlatform.Admin.Api/Dockerfile` | Multi-stage Dockerfile for Admin service |
| CREATE | `.github/workflows/ci.yml` | CI pipeline — build, test, lint on PR |
| CREATE | `.github/workflows/cd.yml` | CD pipeline — build, push, deploy on main merge |
| CREATE | `.env.example` | Environment variable template for local development |

## External References

- [Docker Multi-Stage Builds](https://docs.docker.com/build/building/multi-stage/)
- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [Railway Deployment Guide](https://docs.railway.app/)
- [Render Docker Deploy](https://docs.render.com/docker)
- [Serilog ASP.NET Core](https://github.com/serilog/serilog-aspnetcore)
- [ASP.NET Core Health Checks](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-8.0)
- [Seq Free License](https://datalust.co/seq)

## Build Commands

- Refer to [.propel/build/](.propel/build/) for applicable technology stack build commands

## Implementation Validation Strategy

- [ ] Unit tests pass — health check services resolve dependencies
- [ ] Integration tests pass — `docker compose up` starts all services within 30 seconds
- [ ] CI pipeline completes build+test within 5 minutes on PR
- [ ] CD pipeline deploys successfully to Railway/Render

## Implementation Checklist

- [x] Create multi-stage Dockerfiles for all 7 microservices and API Gateway → AC-1
- [x] Create docker-compose.yml with health-check endpoints responding within 30 seconds → AC-1
- [x] Configure GitHub Actions CI pipeline with build, test, lint on every PR (5-min timeout) → AC-2
- [x] Configure GitHub Actions CD pipeline for Railway/Render with zero-downtime rolling updates → AC-3
- [x] Create environment-specific configuration (dev/staging/prod) with env var injection, no hard-coded secrets → AC-4
- [x] Configure Serilog structured JSON logging with correlation IDs across all services → AC-5
- [x] Implement `/health` and `/ready` endpoints with degraded status detection within 10 seconds → AC-5
- [x] Configure health check retry with exponential backoff for free-tier cold start (max 60s) → AC-1 (edge case)
