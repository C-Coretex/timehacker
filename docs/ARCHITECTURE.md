# Architecture

TimeHacker is a personal day-planning application: a user registers fixed tasks (explicit start/end timestamps), dynamic tasks (a duration range, a priority and min/optimal time to finish), and categories (name, description, ARGB colour) whose dated time windows can repeat via a recurrence blueprint. A heuristic, randomized scheduler weaves those into one concrete timeline per day; the result is persisted as a `ScheduleSnapshot` and rendered on a month/week calendar by a React SPA. Multi-user from the ground up: every domain row is user-scoped both in code and — the load-bearing part — by generated PostgreSQL Row-Level Security policies, so a missing or wrong user id yields no rows rather than someone else's schedule.

## How it fits together

## Two halves, one contract

- `src/TimeHacker.slnx` — .NET 10 solution, 15 projects in numbered solution folders (`01_Presentation`, `02_Application`, `03_Domain`, `04_Infrastructure`, `05_Helpers`, `06_Tests`, the last split into `IntegrationTests`/`UnitTests`).
- `src/TimeHacker.UI/` — React 19 + TypeScript + Vite SPA (`timehacker.ui.esproj` in the solution), antd components, Tailwind, react-query, i18next (en/ru), served under the `/app` basename.
- `src/docker-compose.yml` — profiles `minimal` (ui+api+db), `dev` (+pgAdmin), `full` (+ Alloy/Loki/Tempo/Prometheus/Grafana). `timehacker.infrastructure` has no `profiles:` key, so a bare `docker compose up` starts only Postgres.

## Request flow

SPA (axios, `withCredentials`) → controller in `TimeHacker.Api/Controllers` → app service in `TimeHacker.Application.Api.AppServices` → domain service/processor in `TimeHacker.Domain.Services` → repository in `TimeHacker.Infrastructure` → EF Core (`TimeHackerDbContext`) → PostgreSQL.

## Data model

`UserScopedEntityBase` (Guid id + `UserId`) is the root of every business entity. `FixedTask`/`DynamicTask`/`Category`/`Tag` plus join entities; `CategorySchedule` is a dated window (`Date`, `StartTime`, `EndTime`, optional `Description`) owned by a `Category`. `ScheduleEntity` is the recurrence blueprint (JSON `RepeatingEntity` + `FirstEntityCreated`/`LastEntityCreated`/`EndsOn`) attached polymorphically to *either* a `FixedTask` or a `CategorySchedule`. `ScheduleSnapshot` + `ScheduledTask`/`ScheduledCategory` freeze one generated day. Two databases: `TimeHacker` (domain) and `TimeHackerIdentity` (`TimeHackerIdentityDbContext`, ASP.NET Identity).

## The two mechanisms worth understanding before reading anything else

**1. RLS is generated, not written.** `UserScopedEntityConfigurationBase<T>.ConfigureUserScoped` stamps `Rls:Enabled` / `Rls:TenantColumn` annotations on the EF model. `RlsMigrationsModelDiffer` (`ReplaceService<IMigrationsModelDiffer, ...>` in both `TimeHackerMigrationsDbContext.ApplyMigrations` and the design-time factory) diffs those annotations and appends `ALTER TABLE ... ENABLE ROW LEVEL SECURITY` + `CREATE POLICY ... TO application_user USING ("UserId" = current_setting('app.user_id', true)::uuid)`. `UserSessionInterceptor` sets `app.user_id` on every connection open, resolving `UserAccessorBase` lazily from `TimeHackerDbContext.ScopeServiceProvider` (the interceptor is a Singleton because DbContext pooling bakes it in; the lazy lookup avoids a DI cycle UserAccessor → IUserRepository → DbContext). With no user it sets a fresh random Guid so nothing matches. Consequence for any change touching repositories: the runtime query never adds a `WHERE UserId` filter — isolation is DB-side, and only the owner role (`postgres`) bypasses it, which is why tests assert through an admin context.

**2. A day is snapshotted, not recomputed.** `TaskService.GetTasksForDay(s)` is snapshot-first; only missing dates run `TaskTimelineProcessor.GetTasksForDay`, whose output becomes `ScheduledTask`/`ScheduledCategory` rows via `TasksForDayReturn.CreateOrUpdateScheduleSnapshot()`. `RefreshTasksForDays` is the regeneration path: it `DeleteBy(...)` the existing rows and commits *before* regenerating, because adding and deleting the same `(UserId, Date)` alternate key inside one `SaveChangesAsync` does not guarantee DELETE-before-INSERT. Generation is randomized (`RandomValuesHelper`, truncated at 10 dynamic tasks/day) which is exactly why the snapshot exists — without it the same day would read back differently.

A change typically travels: entity/config → migration (RLS follows automatically if it is user-scoped) → repository interface + `RegisterRepositories` → app service + `RegisterAppServices` → DTO → `Api/Models/Input|Return` → controller → UI `api/*.ts`, `hooks/use*`, page/component, i18n entry.

## Identity and transport quirks

Cookie auth via `AddIdentityCore<IdentityUser>().AddApiEndpoints()` (`MapIdentityApi` gives register/login/refresh), plus a hand-added `POST /logout` that also clears the session cache of the resolved domain id. Redirects to login/denied are replaced with bare 401/403; persistent sign-in pins 14 days. Because the UI and API are cross-site in development, cookies are `SameSite=None; Secure`, which is why antiforgery is mandatory: `GET /api/antiforgery/token` (authorized) → `X-XSRF-TOKEN` header on POST/PUT/DELETE/PATCH, validated by `ValidateAntiforgeryFilter`. Data Protection keys persist to `DataProtection:KeysPath` (`/keys` volume) so cookies survive restarts. `UserAccessorInitMiddleware` runs *after* auth and lazily creates the domain `User` row from the principal's claims before any controller executes.

## Entry points

- `src/TimeHacker.Api/Program.cs` — Top-level-statement host: reads the three connection strings, registers repositories/identity/domain/app services, wires auth, antiforgery, CORS, health checks and OpenTelemetry, applies migrations and seeds in Development, then maps controllers, MapIdentityApi and /logout.
- `src/TimeHacker.Api/Dockerfile` — Builds and runs the API container (ports 8080/8081).
- `src/TimeHacker.Api/Controllers/Tasks/TasksController.cs` — Day/month timeline endpoints — the API surface the calendar UI calls.
- `src/TimeHacker.Api/Controllers/Categories/CategoriesController.cs` — Categories plus nested category-schedule CRUD and the schedule-recurrence endpoint.
- `src/TimeHacker.Api/Controllers/Tasks/FixedTasksController.cs` — Fixed-task CRUD and fixed-task recurrence endpoints.
- `src/TimeHacker.Api/Controllers/Tasks/DynamicTasksController.cs` — Dynamic-task CRUD.
- `src/TimeHacker.Api/Controllers/Users/UsersController.cs` — Current-user read/update/delete (/api/users/me).
- `src/TimeHacker.Api/Seeding/DevelopmentDataSeeder.cs` — Idempotent development seeding of a sample account and data, driven by IDevelopmentSeedStep and DevelopmentSeedContext.
- `src/TimeHacker.Migrations/Factory/TimeHackerMigrationsDbContextFactory.cs` — Design-time DbContext for `dotnet ef` against the admin connection, wired to the RLS-aware model differ.
- `src/TimeHacker.Migrations/Factory/TimeHackerMigrationsDbContext.cs` — Applies TimeHacker migrations (also called at API startup and by test fixtures); hosts ApplyMigrations with the RLS differ registered.
- `src/TimeHacker.UI/src/main.tsx` — SPA bootstrap entry for the /app document.
- `src/TimeHacker.UI/src/App.tsx` — Provider stack (router/theme/settings/auth/calendar-date/react-query/error boundary) and route table dispatch.
- `src/TimeHacker.UI/app/index.html` — The /app HTML entry the router's basename serves; the vite SPA-fallback plugin rewrites /app/* here in dev.
- `src/TimeHacker.UI/src/config/AppRoutes.tsx` — Route definitions, including PrivateRoute guards for the authenticated and admin-only subtrees.
- `src/docker-compose.yml` — Local stack orchestration with minimal/dev/full profiles, the RLS bootstrap SQL mount and the Grafana observability pipeline.

The modules named here are described in [MODULES.md](MODULES.md); the rules they follow are in [CONVENTIONS.md](CONVENTIONS.md).

---

Generated by DiffHacker on 2026-09-12.
