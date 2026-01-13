# Distributed Architecture Pattern Catalog — One Page

> A leveled menu of reusable patterns you can point GitHub Copilot at. Keep cards tiny, runnable, and consistent.

---

## Level 0 — Service Kernel (per‑service, mandatory)

**Identity & Access**: OIDC/OAuth2, Managed Identity/Workload Identity, token exchange, role mapping
**Config & Flags**: App Config hierarchy, Key Vault, feature flags, kill switches
**Resilience & Networking**: timeouts, retries (exp backoff + jitter), circuit breaker, bulkhead, idempotency keys, correlation (W3C tracecontext)
**Data Access**: SQL repo/unit-of-work, Cosmos aggregate mappers, event store (append-only), blob/file I/O, optimistic concurrency
**Observability**: structured logs, metrics, distributed tracing, /healthz, PII scrubbing
**Messaging Client**: producer/consumer baselines, dead-letter & quarantine, outbox/inbox, backpressure
**Contracts (intra‑service)**: API versioning, schema validation, canonical error envelope, DTO↔domain mapping
**Background Work**: schedulers/timers, durable tasks, compensation utils
**Security Posture**: secret mgmt, TLS, data classification & encryption defaults

*Deliverables:* starter libraries, policy set (HTTP & messaging), OpenTelemetry setup, sample tests.

---

## Level 1 — Domain Interaction (within/between bounded contexts)

**Sync Collaboration**: BFF per UI, API Gateway, GraphQL federation, composite UI (micro‑frontends)
**Async Collaboration**: domain vs. integration events, pub/sub, request‑reply over messaging, deferred responses
**Workflow & Consistency**: Saga (choreography) vs Orchestration (process manager), transactional outbox/inbox, compensations
**Boundary Protection**: Anti‑corruption layer (ACL), CQRS projections, reference data sync (CDC)
**File & Batch**: ingestion pipeline (staging, validation, checksum, chunking, replay), poison file quarantine
**Scatter‑Gather**: fan‑out/fan‑in with quorum/timeout, map‑reduce style aggregation
**Contracts Between Domains**: shared kernel vs published language, contract publishing (OpenAPI/AsyncAPI/Avro/Protobuf), consumer‑driven contracts (CDC tests)
**Domain Gateways**: composite integration layer/facade, canonical messages, correlation map

*Deliverables:* sequence sketches, message schemas, runnable samples (HTTP + messaging), CDC tests.

---

## Level 2 — Platform Composition (cross‑domain enablement)

**Orchestration & Event Mesh**: enterprise journeys, routing rules, duplicate/out‑of‑order handling
**Policy & Guardrails**: centralized policy (OPA), API policies (throttle/JWT/CORS), data residency/routing
**Deployment & Traffic**: blue‑green, canary, shadow, progressive delivery
**Reliability at Edges**: global circuit breakers, backpressure, load shedding
**Contract Governance**: MAJOR/MINOR/patch, deprecation windows, schema evolution workflow
**Shared Infra Services**: composite search, distributed cache strategy, file vault & AV scanning, KMS & rotation
**Data Products / Mesh**: discoverable domain data products, SLO‑backed, lineage

*Deliverables:* platform policies, schema registry, SLOs & alerts, shared services.

---

## Level 3 — Enterprise Operating Model (run, change, govern)

**Platform Engineering & Golden Paths**: template repos, cookie‑cutters, IDP with paved roads & scorecards
**Governance & Risk**: ADRs, RFCs, tech radar, audit trails, access reviews
**SRE & FinOps**: SLO taxonomy, error budgets, incident mgmt, cost guardrails & tags, rightsizing
**Resilience & DR**: RPO/RTO tiers, multi‑AZ/region, chaos experiments & game days
**Change & Lifecycle**: GitOps/IaC promotions, env parity, API/schema deprecation policy
**Knowledge & Enablement**: pattern catalog portal, runnable examples, training labs

*Deliverables:* golden‑path docs, workflows, governance calendar, playbooks.

---

## Pattern Card Template (for every pattern)

**Context** · **Problem/Forces** · **Solution sketch** (diagram + sequence) · **Standards/SLOs/Security** · **Tech anchors** · **Code starter** (runnable) · **Tests** · **Pitfalls** · **References**

---

## Golden‑Path Repo Layout (for Copilot)

```
/patterns/
  L0/Resilience/CircuitBreaker/{README.md,prompt.md,samples/,tests/}
  L1/Workflow/Saga-Orchestrated/{...}
  L1/FileIngestion/{...}
  L2/ContractGovernance/{...}
.github/workflows/{cdc.yml,slo-check.yml,security.yml}
/docs/pattern-index.md
/templates/{api-svc,worker-svc,bff,func}
```

---

## Copilot Prompt Recipes (examples)

* **L0 Resilience**: “Apply *CircuitBreaker* to HttpClient for Service X. Timeout 2s, retry 3 (exp backoff + jitter), add W3C tracecontext; expose `/healthz`; emit OpenTelemetry; include unit tests.”
* **L1 Saga**: “Scaffold *Saga (orchestrated)* for `Order→Payment→Inventory` using Durable Functions and Service Bus. Use outbox/inbox, idempotency keys, and compensations. Provide integration tests.”
* **L1 File Ingestion**: “Generate *FileIngestion* pipeline: staging→validate→checksum→quarantine on failure; chunk files >50MB; produce `FileValidated` event; write tests for edge cases.”
* **L2 Contract Governance**: “Add *ContractGovernance* workflow: publish AsyncAPI to registry, run CDC tests, block on breaking changes, open deprecation issue on MINOR bump.”

---

### Why this helps Copilot

Named, leveled patterns + runnable examples give Copilot concrete anchors. Prompts reference the **pattern name** and **options** (e.g., `--persistence=sql|cosmos`, `--transport=http|servicebus`), so Copilot assembles opinionated code and tests on the paved road—fast, safe, and consistent.
