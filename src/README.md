# TimeHacker Backend - Developer Guide

> **Note:** For frontend documentation, see [TimeHacker.UI/README.md](TimeHacker.UI/README.md)

## Architecture

Clean Architecture with 4 layers:

```
Domain → Application → Infrastructure → API
```

**Projects:**
- `TimeHacker.Domain` - Core entities, interfaces, domain models
- `TimeHacker.Domain.Services` - Business logic & algorithms
- `TimeHacker.Application.Api(.Contracts)` - Service implementations & DTOs
- `TimeHacker.Infrastructure(.Identity)` - EF Core, repositories, auth
- `TimeHacker.Api` - REST controllers
- `TimeHacker.Migrations(.Identity)` - Database migrations
- `TimeHacker.Helpers.*` - Shared utilities

**Request Flow:**
```
HTTP Request → Controller → AppService → Repository/Service/Processor → Response
```

## Core Entities

**Tasks:**
- `FixedTask` - Time-specific tasks (start/end times)
- `DynamicTask` - Flexible tasks (min/max/optimal duration)

**Organization:**
- `Category` & `Tag` - Many-to-many with both task types

**User Scoping (multi-tenant):**
- All entities inherit from `UserScopedEntityBase` (`UserId` column)
- Isolation is enforced by **PostgreSQL Row-Level Security (RLS)**, not application LINQ filters. Each
  user-scoped table has a policy `USING (UserId = current_setting('app.user_id')::uuid)`. The
  `UserSessionInterceptor` sets `app.user_id` on every connection from the authenticated user. The
  repository layer only stamps `UserId` on insert and acts as a second line of defense.
- **Two DB roles**: the app runs as `application_user` (RLS-bound); migrations run as `postgres`
  (table owner, bypasses RLS, and is needed to enable RLS / create policies). RLS policies are generated
  automatically by `RlsMigrationsModelDiffer` from the `Rls:Enabled` annotation that
  `UserScopedEntityConfigurationBase` adds to every user-scoped entity.

## Scheduled Entities System

Handles recurring tasks and categories.

**Key Entities:**

1. **ScheduleEntity** - Defines recurring schedule
   - Links to parent `FixedTask` or `Category`
   - Contains `RepeatingEntityDto` (daily/weekly/monthly/yearly patterns)
   - Tracks creation history and optional end date

2. **ScheduleSnapshot** - Parent entity for a specific date
   - Created when "RefreshTasksForDay" is called
   - Contains all scheduled entities for that day
   - Links to `ScheduledTasks` and `ScheduledCategories`

3. **ScheduledTask** / **ScheduledCategory** - Generated instances
   - Created from parent + `ScheduleEntity`
   - Stores snapshot of data for specific occurrence
   - Links back to parent and `ScheduleSnapshot`

**Relationships:**
```
ScheduleEntity (recurring definition)
    └─ Parent: FixedTask or Category

When day is generated:
    ↓
ScheduleSnapshot (for specific date)
    ├─ ScheduledTasks (generated task instances)
    └─ ScheduledCategories (generated category instances)
```

**Generation Flow:**
1. User requests tasks for a date
2. System checks if `ScheduleSnapshot` exists
3. If not, generates it:
   - Fetches regular tasks
   - Finds `ScheduleEntities` matching date
   - Generates `ScheduledTask`/`ScheduledCategory` instances
   - Runs `TaskTimelineProcessor` to optimize schedule
   - Saves everything under `ScheduleSnapshot`
4. Returns results

## Key Components

**Services:**
- `TaskService` - Orchestrates task retrieval & generation
- `ScheduleEntityService` - Manages recurring schedules
- `FixedTaskService` / `DynamicTaskService` - CRUD operations

**Processors:**
- `TaskTimelineProcessor` - Schedules fixed tasks in slots, fills gaps with dynamic tasks

**Database:**
- PostgreSQL with EF Core
- ASP.NET Identity for auth
- Automatic UTC conversion for DateTimes
- Row-Level Security for per-user data isolation (see User Scoping above)

## Observability / Telemetry

The API is instrumented with **OpenTelemetry** (logs, traces, metrics). For local development, `docker-compose`
runs a **Grafana LGTM stack** as the backend:

```
TimeHacker.Api ──OTLP──▶ Grafana Alloy ─┬─ logs ────▶ Loki
                                        ├─ traces ──▶ Tempo ──(span/service-graph metrics)──▶ Prometheus
                                        └─ metrics ─(remote-write, w/ exemplars)─▶ Prometheus
                                                         │
                                                   Grafana (UI over all three)
```

- **View it** at **http://localhost:3000** (Grafana; credentials from `GF_SECURITY_ADMIN_*` in `.env`,
  default `admin`/`admin`). The Prometheus/Loki/Tempo datasources are auto-provisioned — use **Explore** to
  query logs (`{service_name="TimeHacker.Api"}`), traces, and metrics. **Grafana Alloy** (Grafana's OTel
  Collector distribution) has its own live pipeline UI at **http://localhost:12345/graph**.
- **Config** lives under `src/observability/**` — Alloy's is split per signal under `observability/alloy/`
  (`receiver.alloy` + `logs.alloy` / `metrics.alloy` / `traces.alloy`; Alloy merges the whole directory),
  plus Tempo, Prometheus, and Grafana datasources. Loki uses its built-in default config.
- **Export is env-driven** (`Program.cs` → `AddOpenTelemetry`). OTLP is used only when
  `OTEL_EXPORTER_OTLP_ENDPOINT` is set (compose sets it to `http://alloy:4317`, `grpc`,
  `OTEL_SERVICE_NAME=TimeHacker.Api`). Without it (plain `dotnet run`, tests) telemetry **falls back to the
  console**. In production, override the `OTEL_*` env vars to point at any OTLP-compatible backend — no code change.
- **Errors show in both places:** routine info/warning logs go to Grafana only, while error-level records
  (unhandled exceptions from `LogExceptionFilter`) are **also mirrored to the console**.
- **The three signals are cross-linked** so you can pivot between them:
  - **span→logs** (`tracesToLogsV2`), **span→metrics** (`tracesToMetrics` over the span RED metrics),
    **log→trace** (Loki `trace_id` derived field; Tempo's `traceQuery` time-shift keeps the lookup from
    failing on a zero-width time range), **service map / node graph** (Tempo metrics-generator).
  - **metric→trace via exemplars** (Prometheus `exemplarTraceIdDestinations` → Tempo): graph a histogram
    with exemplars enabled (e.g. `db_client_operation_duration_seconds_bucket`, `http_server_request_duration_seconds_bucket`,
    the business histograms, or Tempo's `traces_spanmetrics_latency`) → click an exemplar dot → the trace opens.
  - App metrics reach Prometheus via **remote-write** (Alloy `otelcol.exporter.prometheus` → `prometheus.remote_write`
    with `send_exemplars`), which — unlike Prometheus's native OTLP ingestion — preserves classic-histogram
    exemplars. Tempo remote-writes its span/service-graph metrics to the same receiver.
- **Every signal is tagged** with `service.name`, `service.version`, `deployment.environment`, and
  `service.instance.id`, so you can filter dev vs. prod, versions, and instances.
- **What's instrumented:**
  - *Traces* — ASP.NET Core requests, outbound `HttpClient`, **Npgsql database commands** (each SQL command
    is a span nested inside its request trace), and **business spans**: a `timeline.generate` span wraps each
    day's timeline generation, tagged with the date, `enduser.id`, and result counts. Every authenticated
    request span also carries `enduser.id` for tenant attribution under RLS.
  - *Metrics* — ASP.NET Core + `HttpClient` request duration, **.NET runtime** (GC, thread pool, memory),
    **Npgsql** DB metrics (query duration `db.client.operation.duration`, connection-pool state), **EF Core**
    query/compilation counts, and **business metrics**: `timehacker.snapshots.requested` (tagged
    `outcome = cache_hit | generated` — the snapshot cache-hit ratio), `timehacker.timeline.generation.duration`,
    and `timehacker.scheduled_tasks.generated`. Both DbContexts use a **named `NpgsqlDataSource`**
    (`TimeHacker` / `TimeHackerIdentity`) so pool metrics are tagged per database.

## Testing

- **Unit tests** — xUnit v3 + Moq + MockQueryable + AutoBogus + AwesomeAssertions. Cover app services,
  domain services, and domain models (`TimeHacker.*.Tests`).
- **DB integration tests** (`TimeHacker.Integration.Db.Tests`) — run against a **real PostgreSQL** via
  Testcontainers, with Respawner resetting the DB between tests. They verify RLS user isolation, cascade
  deletes, JSON columns, value converters, DB constraints, optimistic concurrency, and full
  app-service-over-real-DB flows. Tests *act* as `application_user` (RLS-bound) and *assert* via an admin
  connection. **A running Docker daemon is required.**
- **API (end-to-end) tests** (`TimeHacker.Integration.Api.Tests`) — drive the **real API over HTTP**
  through `WebApplicationFactory<Program>` against Testcontainers PostgreSQL, exercising the full pipeline
  (cookie auth, CSRF, RLS, exception filter, EF, business logic). Endpoints are called through a
  strongly-typed **Refit** client (`ITimeHackerApi`); side effects are asserted via an admin DbContext.
  **A running Docker daemon is required.** See CLAUDE.md §8 for the fixture/client layout.

Run from `src/`:
```
dotnet test
```
