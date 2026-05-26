# Task - task_001_platform_resilience_backend

## Requirement Reference

- **User Story:** US_029
- **Story Location:** .propel/context/tasks/EP-011/us_029/us_029.md
- **Acceptance Criteria:**
  - AC-1: Polly circuit breaker on all external dependencies — 3 consecutive failures opens for 60s, fallback response (cached/degradation message), recovery test after break
  - AC-2: Retry policies with exponential backoff — 3 retries (1s, 2s, 4s) with jitter, each retry logged, circuit breaker count incremented on exhaustion
  - AC-3: Health check endpoints for all services — /health returns within 1s with aggregate status (Healthy/Degraded/Unhealthy), checking DB, Redis, dependencies, memory/CPU (>90% = Degraded)
  - AC-4: Graceful degradation when non-critical services fail — primary action succeeds, secondary queued for retry, user sees non-blocking warning, no data loss
  - AC-5: Performance monitoring and alerting — structured metrics, alerts within 2 min (p95 > 3s, error > 5%, queue > 100), real-time dashboards at 1-min granularity
- **Edge Cases:**
  - All instances crash → YARP returns 503 with human-readable error, other services unaffected
  - Circuit breaker half-open receives failure → re-open for double duration (120s), max 5 minutes
  - Cascading failures → independent circuit breakers prevent cascade

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
| Backend | ASP.NET Core Web API | 8.0 | TR-002 — Health check middleware, metrics endpoints |
| Backend | Polly | 8.x | TR-006 — Circuit breaker, retry, timeout policies |
| Backend | Serilog | 3.x | TR-009 — Structured logging with metrics emission |
| Backend | YARP | 2.1 | NFR-001 — Gateway-level health monitoring and failover |
| Cache | Upstash Redis | Free tier | TR-005 — Fallback cached responses during circuit breaker open |

---

## Task Overview

Implement cross-cutting platform resilience infrastructure across all microservices. This includes Polly circuit breaker policies (3-failure, 60s open, progressive doubling) on all external dependencies, retry policies with exponential backoff and jitter, standardized health check endpoints per service checking DB/Redis/dependencies/resources, graceful degradation patterns for non-critical service failures, structured performance metrics with Serilog, and alerting pipeline for KPI threshold breaches.

## Dependent Tasks

- US_002/task_001 — Backend API scaffolding (solution structure, YARP gateway)
- US_003/task_001 — DevOps infrastructure (Docker, CI/CD, Serilog setup)

## Impacted Components

- `src/Shared/Resilience/` — New shared resilience library
- `src/Shared/Resilience/CircuitBreakerPolicyFactory.cs` — Centralized Polly circuit breaker configuration
- `src/Shared/Resilience/RetryPolicyFactory.cs` — Centralized retry with exponential backoff
- `src/Shared/Resilience/HealthChecks/` — Standardized health check implementations
- `src/Shared/Resilience/Degradation/` — Graceful degradation patterns
- `src/Shared/Resilience/Metrics/` — Performance metrics emission
- `src/ApiGateway/` — YARP health monitoring configuration

## Implementation Plan

1. Create shared Resilience library project referenced by all microservices
2. Implement CircuitBreakerPolicyFactory with 3-failure threshold, 60s open, progressive doubling (max 5min)
3. Implement RetryPolicyFactory with exponential backoff (1s, 2s, 4s) with jitter and logging
4. Build standardized health check implementations for PostgreSQL, Redis, and HTTP dependencies
5. Create health check aggregation endpoint returning Healthy/Degraded/Unhealthy with resource thresholds
6. Implement GracefulDegradationHandler with primary-succeeds/secondary-queued pattern
7. Build metrics emission pipeline with Serilog structured output for p95, error rate, queue depth
8. Configure YARP health monitoring with 503 failover for completely unavailable services

## Current Project State

```text
src/
├── Shared/
│   └── Resilience/                         ← NEW
│       ├── CircuitBreakerPolicyFactory.cs
│       ├── RetryPolicyFactory.cs
│       ├── HealthChecks/
│       │   ├── PostgresHealthCheck.cs
│       │   ├── RedisHealthCheck.cs
│       │   └── HttpDependencyHealthCheck.cs
│       ├── Degradation/
│       │   └── GracefulDegradationHandler.cs
│       └── Metrics/
│           └── PerformanceMetricsEmitter.cs
├── ApiGateway/
│   └── yarp.json
├── Services/
│   ├── AuthService/
│   ├── AppointmentService/
│   ├── IntakeService/
│   ├── DocumentService/
│   ├── ClinicalIntelligenceService/
│   ├── NotificationService/
│   └── AdminService/
```

## Expected Changes

| Action | File Path | Description |
|--------|-----------|-------------|
| CREATE | src/Shared/Resilience/CircuitBreakerPolicyFactory.cs | Polly circuit breaker: 3-failure, 60s open, progressive doubling, max 5min |
| CREATE | src/Shared/Resilience/RetryPolicyFactory.cs | Exponential backoff (1s, 2s, 4s) with jitter, per-retry logging |
| CREATE | src/Shared/Resilience/HealthChecks/PostgresHealthCheck.cs | PostgreSQL connectivity check with timeout |
| CREATE | src/Shared/Resilience/HealthChecks/RedisHealthCheck.cs | Redis connectivity and latency check |
| CREATE | src/Shared/Resilience/HealthChecks/HttpDependencyHealthCheck.cs | HTTP dependency health check with circuit breaker awareness |
| CREATE | src/Shared/Resilience/Degradation/GracefulDegradationHandler.cs | Primary action succeeds, secondary queued, non-blocking warning |
| CREATE | src/Shared/Resilience/Metrics/PerformanceMetricsEmitter.cs | Serilog metrics for p95, error rate, queue depth with alerting |
| MODIFY | src/ApiGateway/yarp.json | Add health monitoring and 503 failover configuration |

## External References

- [Polly v8 Documentation](https://www.thepollyproject.org/)
- [ASP.NET Core Health Checks](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-8.0)
- [Serilog Structured Logging](https://serilog.net/)

## Build Commands

- [Refer to applicable technology stack build commands](.propel/build/)

## Implementation Validation Strategy

- [ ] Unit tests pass for CircuitBreakerPolicyFactory (open/half-open/closed transitions, progressive doubling)
- [ ] Unit tests pass for RetryPolicyFactory (backoff intervals, jitter, exhaustion logging)
- [ ] Integration tests pass (health check aggregation, graceful degradation, YARP failover)

## Implementation Checklist

- [ ] Create shared Resilience library with Polly circuit breaker (3-failure, 60s, progressive doubling max 5min) — maps to AC-1
- [ ] Implement retry policies with exponential backoff (1s, 2s, 4s) with jitter and per-retry logging — maps to AC-2
- [ ] Build standardized health check implementations for PostgreSQL, Redis, and HTTP dependencies — maps to AC-3
- [ ] Create /health endpoint per service returning aggregate status within 1s with memory/CPU thresholds — maps to AC-3
- [ ] Implement GracefulDegradationHandler: primary succeeds, secondary queued, non-blocking warning — maps to AC-4
- [ ] Build performance metrics emission with Serilog for p95, error rate, queue depth at 1-min granularity — maps to AC-5
- [ ] Add alerting pipeline for KPI breaches (p95 > 3s, error > 5%, queue > 100) within 2 minutes — maps to AC-5
- [ ] Configure YARP 503 failover with human-readable error for completely unavailable services — maps to edge cases
