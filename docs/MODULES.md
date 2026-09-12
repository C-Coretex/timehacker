# Module map

| Module | Path | Responsibility |
|---|---|---|
| [TimeHacker.Domain](#timehackerdomain) | `src/TimeHacker.Domain` | Entities, repository/processor/service interfaces, ReturnModels/InputModels, business exceptions and the OpenTelemetry meter/ActivitySource constants. No EF or ASP.NET dependency. |
| [TimeHacker.Application.Api](#timehackerapplicationapi) | `src/TimeHacker.Application.Api` | Application services (one per aggregate: TaskService, FixedTaskService, CategoryService, CategoryScheduleService, ScheduleEntityAppService, UserService...) orchestrating repositories and domain services; holds the QueryPipelineSteps include-expansions. |
| [TimeHacker.Application.Api.Contracts](#timehackerapplicationapicontracts) | `src/TimeHacker.Application.Api.Contracts` | DTO records shared across the application boundary plus the IAppService interfaces (I*AppService, split per aggregate). |
| [TimeHacker.Domain.Services](#timehackerdomainservices) | `src/TimeHacker.Domain.Services` | Pure-domain logic: TaskTimelineProcessor (the randomized scheduling algorithm) and ScheduleEntityService (recurrence expansion, LastEntityCreated progress marker). |
| [TimeHacker.Infrastructure](#timehackerinfrastructure) | `src/TimeHacker.Infrastructure` | TimeHackerDbContext (pooled), EF configurations, repositories, UserSessionInterceptor/TimestampInterceptor, value converters and RegisterRepositories. |
| [TimeHacker.Api](#timehackerapi) | `src/TimeHacker.Api` | Host: controllers, Input/Return view models, JSON converters, filters (antiforgery + exception→ProblemDetails), UserAccessor middleware, Program.cs composition, development seeding, Dockerfile. |
| [TimeHacker.Migrations](#timehackermigrations) | `src/TimeHacker.Migrations` | Design-time DbContext, RlsMigrationsModelDiffer and all forward migrations for the TimeHacker database; ApplyMigrations is also called at API startup. |
| [TimeHacker.Migrations.Identity](#timehackermigrationsidentity) | `src/TimeHacker.Migrations.Identity` | The same design-time pattern for the separate TimeHackerIdentity database. |
| [TimeHacker.Infrastructure.Identity](#timehackerinfrastructureidentity) | `src/TimeHacker.Infrastructure.Identity` | TimeHackerIdentityDbContext and RegisterIdentity for ASP.NET Identity storage. |
| [TimeHacker.Helpers.Domain](#timehackerhelpersdomain) | `src/TimeHacker.Helpers.Domain` | Reusable building blocks: GuidDbEntity, IDbEntity/ICreatable/IUpdatable, IRepositoryBase, QueryPipelineStep delegate, RandomValuesHelper, collection/validation extensions. |
| [TimeHacker.Helpers.DB](#timehackerhelpersdb) | `src/TimeHacker.Helpers.DB` | DbContextBase, ContextFactoryBase and RepositoryBase — where the IQueryable/QueryPipelineStep repository contract and ExecuteUpdate timestamp handling live. Note the directory is spelled DB while solution/manifest paths spell it Db. |
| [TimeHacker.UI](#timehackerui) | `src/TimeHacker.UI` | React SPA: pages (CalendarPage, TasksPage, CategoriesPage, ...), components, contexts, hand-written api modules, react-query hooks and i18n; built as a second entry under /app. |
| [TimeHacker.Tests.Helpers](#timehackertestshelpers) | `src/TimeHacker.Tests.Helpers` | Shared test scaffolding referenced by the DB integration tests. |
| [TimeHacker.Integration.Db.Tests](#timehackerintegrationdbtests) | `src/TimeHacker.Integration.Db.Tests` | Testcontainers Postgres + Respawn tests of schema, RLS, interceptors and app-service flows against a real database. |
| [TimeHacker.Integration.Api.Tests](#timehackerintegrationapitests) | `src/TimeHacker.Integration.Api.Tests` | WebApplicationFactory<Program> over two Postgres containers, driving the real HTTP surface through Refit clients. |

## TimeHacker.Domain

`src/TimeHacker.Domain`

Entities, repository/processor/service interfaces, ReturnModels/InputModels, business exceptions and the OpenTelemetry meter/ActivitySource constants. No EF or ASP.NET dependency.

Closely related to [TimeHacker.Helpers.Domain](#timehackerhelpersdomain), [TimeHacker.Helpers.DB](#timehackerhelpersdb).

## TimeHacker.Application.Api

`src/TimeHacker.Application.Api`

Application services (one per aggregate: TaskService, FixedTaskService, CategoryService, CategoryScheduleService, ScheduleEntityAppService, UserService...) orchestrating repositories and domain services; holds the QueryPipelineSteps include-expansions.

Closely related to [TimeHacker.Application.Api.Contracts](#timehackerapplicationapicontracts), [TimeHacker.Domain.Services](#timehackerdomainservices), [TimeHacker.Domain](#timehackerdomain).

## TimeHacker.Application.Api.Contracts

`src/TimeHacker.Application.Api.Contracts`

DTO records shared across the application boundary plus the IAppService interfaces (I*AppService, split per aggregate).

Closely related to [TimeHacker.Domain](#timehackerdomain).

## TimeHacker.Domain.Services

`src/TimeHacker.Domain.Services`

Pure-domain logic: TaskTimelineProcessor (the randomized scheduling algorithm) and ScheduleEntityService (recurrence expansion, LastEntityCreated progress marker).

Closely related to [TimeHacker.Domain](#timehackerdomain).

## TimeHacker.Infrastructure

`src/TimeHacker.Infrastructure`

TimeHackerDbContext (pooled), EF configurations, repositories, UserSessionInterceptor/TimestampInterceptor, value converters and RegisterRepositories.

Closely related to [TimeHacker.Domain](#timehackerdomain), [TimeHacker.Helpers.DB](#timehackerhelpersdb), [TimeHacker.Helpers.Domain](#timehackerhelpersdomain).

## TimeHacker.Api

`src/TimeHacker.Api`

Host: controllers, Input/Return view models, JSON converters, filters (antiforgery + exception→ProblemDetails), UserAccessor middleware, Program.cs composition, development seeding, Dockerfile.

Closely related to [TimeHacker.Application.Api](#timehackerapplicationapi), [TimeHacker.Application.Api.Contracts](#timehackerapplicationapicontracts), [TimeHacker.Infrastructure](#timehackerinfrastructure), [TimeHacker.Infrastructure.Identity](#timehackerinfrastructureidentity), [TimeHacker.Migrations](#timehackermigrations).

## TimeHacker.Migrations

`src/TimeHacker.Migrations`

Design-time DbContext, RlsMigrationsModelDiffer and all forward migrations for the TimeHacker database; ApplyMigrations is also called at API startup.

Closely related to [TimeHacker.Infrastructure](#timehackerinfrastructure).

## TimeHacker.Migrations.Identity

`src/TimeHacker.Migrations.Identity`

The same design-time pattern for the separate TimeHackerIdentity database.

Closely related to [TimeHacker.Infrastructure.Identity](#timehackerinfrastructureidentity).

## TimeHacker.Infrastructure.Identity

`src/TimeHacker.Infrastructure.Identity`

TimeHackerIdentityDbContext and RegisterIdentity for ASP.NET Identity storage.

Closely related to [TimeHacker.Domain](#timehackerdomain).

## TimeHacker.Helpers.Domain

`src/TimeHacker.Helpers.Domain`

Reusable building blocks: GuidDbEntity, IDbEntity/ICreatable/IUpdatable, IRepositoryBase, QueryPipelineStep delegate, RandomValuesHelper, collection/validation extensions.

## TimeHacker.Helpers.DB

`src/TimeHacker.Helpers.DB`

DbContextBase, ContextFactoryBase and RepositoryBase — where the IQueryable/QueryPipelineStep repository contract and ExecuteUpdate timestamp handling live. Note the directory is spelled DB while solution/manifest paths spell it Db.

Closely related to [TimeHacker.Helpers.Domain](#timehackerhelpersdomain).

## TimeHacker.UI

`src/TimeHacker.UI`

React SPA: pages (CalendarPage, TasksPage, CategoriesPage, ...), components, contexts, hand-written api modules, react-query hooks and i18n; built as a second entry under /app.

Closely related to [TimeHacker.Api](#timehackerapi).

## TimeHacker.Tests.Helpers

`src/TimeHacker.Tests.Helpers`

Shared test scaffolding referenced by the DB integration tests.

Closely related to [TimeHacker.Domain](#timehackerdomain).

## TimeHacker.Integration.Db.Tests

`src/TimeHacker.Integration.Db.Tests`

Testcontainers Postgres + Respawn tests of schema, RLS, interceptors and app-service flows against a real database.

Closely related to [TimeHacker.Application.Api](#timehackerapplicationapi), [TimeHacker.Domain.Services](#timehackerdomainservices), [TimeHacker.Migrations](#timehackermigrations), [TimeHacker.Tests.Helpers](#timehackertestshelpers).

## TimeHacker.Integration.Api.Tests

`src/TimeHacker.Integration.Api.Tests`

WebApplicationFactory<Program> over two Postgres containers, driving the real HTTP surface through Refit clients.

Closely related to [TimeHacker.Api](#timehackerapi), [TimeHacker.Tests.Helpers](#timehackertestshelpers).

The shape these modules make up is described in [ARCHITECTURE.md](ARCHITECTURE.md).

---

Generated by DiffHacker on 2026-09-12.