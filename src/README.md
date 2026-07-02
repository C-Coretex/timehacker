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
