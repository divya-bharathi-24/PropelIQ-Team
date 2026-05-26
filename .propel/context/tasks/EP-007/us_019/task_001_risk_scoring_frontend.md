# Task - task_001_risk_scoring_frontend

## Requirement Reference

- **User Story:** US_019
- **Story Location:** .propel/context/tasks/EP-007/us_019/us_019.md
- **Acceptance Criteria:**
  - AC-2: Risk categorization with thresholds — Medium and High risk appointments flagged in provider dashboard
  - AC-3: Risk factors explainability — top 3 contributing factors with relative weights displayed on click
- **Edge Cases:**
  - New patient with no history → display medium risk (50) with "New patient — limited data" indicator
  - Patient confirms appointment → risk score refreshes, category may change

---

## Design References

| Reference Type | Value |
|----------------|-------|
| **UI Impact** | Yes |
| **Figma URL** | N/A |
| **Wireframe Status** | N/A |
| **Wireframe Type** | N/A |
| **Wireframe Path/URL** | N/A |
| **Screen Spec** | SCR-RISK-001 |
| **UXR Requirements** | Risk score as circular gauge (green/yellow/red); explainability as expandable accordion below score; provider dashboard flags high-risk with pulsing red dot; hover tooltip shows score and top factor |
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
| Frontend | Angular | 17 | TR-001 — Standalone components for risk display |
| Frontend | Angular Material | 17.x | TR-001 — Tooltips, expansion panels, badge components |
| Frontend | RxJS | 7.x | TR-001 — Reactive score updates on confirmation events |
| Frontend | TypeScript | 5.x | TR-001 — Type-safe risk score models |

---

## Task Overview

Implement the frontend risk scoring display components for the provider dashboard. This includes a circular gauge visualization for risk scores (0-100) with color-coded categories (green/yellow/red), expandable risk factor explainability accordion showing top 3 contributing factors with relative weights, pulsing red dot indicators for high-risk appointments, and hover tooltips with score summary.

## Dependent Tasks

- US_001/task_001 — Frontend scaffolding (Angular project, routing, shared module)

## Impacted Components

- `src/app/features/risk/` — New risk scoring display module
- `src/app/features/risk/risk-gauge/` — Circular gauge component
- `src/app/features/risk/risk-explainability/` — Expandable factor accordion
- `src/app/features/risk/services/risk.service.ts` — Risk score API service
- `src/app/shared/components/risk-badge/` — Reusable risk badge for dashboard integration

## Implementation Plan

1. Define TypeScript interfaces for RiskScore, RiskCategory, RiskFactor, and AppointmentRisk
2. Implement RiskService with methods to fetch risk scores and factor details
3. Build RiskGaugeComponent as circular SVG gauge with color transitions (green 0-30, yellow 31-60, red 61-100)
4. Build RiskExplainabilityComponent as accordion showing top 3 factors with percentage weights
5. Create reusable RiskBadgeComponent with pulsing red dot for high-risk and hover tooltip

## Current Project State

```text
src/
├── app/
│   └── features/
│       └── risk/                           ← NEW
│           ├── risk-gauge/
│           │   ├── risk-gauge.component.ts
│           │   ├── risk-gauge.component.html
│           │   └── risk-gauge.component.scss
│           ├── risk-explainability/
│           │   ├── risk-explainability.component.ts
│           │   └── risk-explainability.component.html
│           ├── services/
│           │   └── risk.service.ts
│           └── models/
│               └── risk.model.ts
├── shared/
│   └── components/
│       └── risk-badge/                     ← NEW
│           └── risk-badge.component.ts
```

## Expected Changes

| Action | File Path | Description |
|--------|-----------|-------------|
| CREATE | src/app/features/risk/models/risk.model.ts | RiskScore, RiskCategory, RiskFactor interfaces |
| CREATE | src/app/features/risk/services/risk.service.ts | HTTP service for risk score and factor retrieval |
| CREATE | src/app/features/risk/risk-gauge/risk-gauge.component.ts | Circular SVG gauge with color-coded risk visualization |
| CREATE | src/app/features/risk/risk-gauge/risk-gauge.component.scss | Gauge styles, color transitions, responsive sizing |
| CREATE | src/app/features/risk/risk-explainability/risk-explainability.component.ts | Expandable accordion showing top 3 factors with weights |
| CREATE | src/app/shared/components/risk-badge/risk-badge.component.ts | Reusable badge with pulsing red dot and hover tooltip |

## External References

- [Angular Material Expansion Panel](https://material.angular.io/components/expansion/overview)
- [Angular Material Tooltip](https://material.angular.io/components/tooltip/overview)
- [SVG Circular Progress](https://developer.mozilla.org/en-US/docs/Web/SVG/Tutorial/Paths)

## Build Commands

- [Refer to applicable technology stack build commands](.propel/build/)

## Implementation Validation Strategy

- [ ] Unit tests pass for RiskGaugeComponent (color thresholds, score display, "estimated" badge)
- [ ] Unit tests pass for RiskExplainabilityComponent (factor rendering, weight percentages)
- [ ] Integration tests pass (risk badge integration in appointment list)

## Implementation Checklist

- [ ] Define RiskScore, RiskCategory, and RiskFactor TypeScript interfaces — maps to AC-2
- [ ] Implement RiskService with getScore and getFactors API methods — maps to AC-2, AC-3
- [ ] Build RiskGaugeComponent as circular SVG gauge with green/yellow/red thresholds (0-30/31-60/61-100) — maps to AC-2
- [ ] Build RiskExplainabilityComponent as expandable accordion showing top 3 factors with relative weights — maps to AC-3
- [ ] Create reusable RiskBadgeComponent with pulsing red dot for high-risk and hover tooltip — maps to AC-2
