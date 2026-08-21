# timehacker
System to help you organize tasks and events.

The system will automatically arrange your tasks, depending on your preferences and interests.

Personalized schedule planning is the task of automatically arranging a set of user-defined tasks into a calendar while satisfying hard constraints and optimizing soft objectives. Unlike classical scheduling problems (manufacturing, workforce), personalized planning must handle highly variable, user-driven input: tasks differ in duration, priority, deadline, allowed time windows, category restrictions, and repetition requirements. The planner must respect what is fixed and optimize what is not.

## Highlights

- **Automatic scheduling** — fixed tasks are placed on the timeline and the gaps are filled with
  flexible ("dynamic") tasks; each day's plan is stored as a snapshot so it stays stable.
- **Recurring tasks and categories** — daily / weekly / monthly / yearly patterns, plus explicit
  one-off date lists.
- **Full observability** — the API emits OpenTelemetry logs, traces and metrics into a local
  Grafana LGTM stack.

## Stack

**Backend** — ASP.NET Core 10 (C# / .NET 10) REST API, EF Core 10 + Npgsql over PostgreSQL,
ASP.NET Core Identity for cookie-based auth, Swagger/Swashbuckle for API docs.

**Frontend** — React 19 + TypeScript (strict) on Vite, TanStack Query + Axios for data, React Router,
Ant Design + Tailwind CSS for UI, i18next for localization, react-big-calendar + dayjs for the calendar.

**Data & infrastructure** — PostgreSQL (separate app and identity databases) with per-user data
isolation enforced by Row-Level Security rather than application-side filtering, Docker Compose for
the whole stack, pgAdmin for DB access.

**Observability** — OpenTelemetry (logs, traces, metrics) exported through Grafana Alloy into the
Grafana **LGTM** stack — Loki, Grafana, Tempo, Prometheus — with provisioned dashboards.

**Testing & quality** — three test layers: unit tests (xUnit v3 with Moq, AutoBogus,
AwesomeAssertions), DB integration tests against a real PostgreSQL (Testcontainers + Respawner), and
end-to-end API tests driving the running API over HTTP (WebApplicationFactory + Refit). .NET analyzers
run at `AnalysisMode=All` with warnings-as-errors, the frontend uses ESLint + TypeScript strict mode,
and CodeQL runs in GitHub Actions.

---
### Backend readme:
[./src/README.md](src/README.md)

### Frontend readme:
[./src/TimeHacker.UI/README.md](src/TimeHacker.UI/README.md)