# TimeHacker.UI - Developer Guide

## Tech Stack

| Category | Technology |
|----------|-----------|
| Framework | React |
| Language | TypeScript (strict mode) |
| Build Tool | Vite |
| Routing | React Router |
| HTTP Client | Axios |
| Data Fetching | TanStack Query (React Query) |
| UI Components | Ant Design |
| Styling | Design tokens (`src/theme/`) → antd theme + `--th-*` CSS variables, plain per-component CSS |
| Fonts | Inter (`@fontsource-variable/inter`), DSEG7 for the digital time field (`dseg`) |
| Internationalization | i18next + react-i18next |
| Date Handling | dayjs |
| Calendar | react-big-calendar |
| Docker Base | Node Alpine |

## Structure

```
src/
├── api/          # Backend API integration layer (tasks, fixedTasks, dynamicTasks)
├── assets/       # Static assets (images, icons)
├── components/   # Reusable UI components (Layout, PrivateRoute, modals)
├── config/       # App configuration (routes, axios)
├── contexts/     # React Context providers (Auth, Theme)
├── hooks/        # Custom React hooks (useQuery, useMutation, useFixedTasks, etc.)
├── i18n/         # Internationalization config and locale files (en, ru)
├── pages/        # Page components (Calendar, Today, Tasks, Categories, Profile, Settings, etc.)
├── theme/        # Design palette (single colour source), antd theme, CSS variables, fonts
├── types/        # TypeScript type definitions
├── utils/        # Utility functions and helpers
├── App.tsx       # Root component with provider stack
└── main.tsx      # Entry point
```

## Communication with Backend

**Base URL:** `https://localhost:8081` (configurable via `VITE_BASE_URL`)

**Authentication:** Cookie-based authentication with ASP.NET Core Identity
- Axios configured with `withCredentials: true` to send cookies
- 401 responses automatically redirect to `/login?expired=true` (except for auth endpoints)

**Data Flow:**
```
Component → Custom Hook → API Function → Axios (with cookies) → Backend
```

**API Layer Examples:**
- `api/tasks.ts` - Calendar operations (fetch tasks for day/days, refresh snapshots)
- `api/fixedTasks.ts` - Fixed task CRUD + recurring schedule creation
- `api/dynamicTasks.ts` - Dynamic task CRUD

**Response Format:** API endpoints return raw data directly (not wrapped in a standard envelope)

## Authentication Flow

1. **App Mount** → `AuthProvider` calls `GET /api/User/GetCurrent` (validates session via cookies)
2. **Unauthenticated** → User redirected to `/login`, protected routes show loading spinner
3. **Login** → `POST /login?useCookies=true&useSessionCookies=${!rememberMe}` (ASP.NET Identity endpoint)
4. **Register** → `POST /register` then auto-login with returned user data
5. **Session Expiry** → 401 interceptor redirects to `/login?expired=true` (shows expiry alert)
6. **Storage** → Cookie-based auth (server-managed) + user profile cached in localStorage

## State Management

**Global State (React Context):**
- `AuthContext` - User authentication state, persisted to localStorage
  - Methods: `login()`, `logout()`, `fetchCurrentUser()`
  - Hook: `useAuth()`
- `ThemeContext` - Dark mode preference, persisted to localStorage (key: `"dark-mode"`)
  - Toggles `"dark"` class on `<html>` element
  - Hook: `useTheme()`

**Server State (TanStack Query):**
- Custom `useQuery` wrapper with auto-select functionality
- Custom `useMutation` wrapper for data mutations
- Automatic caching, background refetching, and error handling

**Local State (Custom Hooks):**
- `useFixedTasks` - Fixed task CRUD state + API calls
- `useDynamicTasks` - Dynamic task CRUD state + API calls
- Pattern: fetch on mount, provide CRUD methods, manage loading/error states

**Persisted State:**
- Dark mode: localStorage key `"dark-mode"`
- Language preference: localStorage key `"language"`
- User profile: cached in localStorage after login

**Forms:**
- Ant Design Form components manage form state
- Modal visibility managed by local state

## Internationalization (i18n)

**Framework:** i18next + react-i18next + browser language detector

**Supported Languages:**
- English (`en`) - Default/fallback
- Russian (`ru`)

**Language Detection:**
1. localStorage (key: `"language"`)
2. Browser language
3. Fallback to English

**Usage:**
```typescript
import { useTranslation } from 'react-i18next';

const { t, i18n } = useTranslation();

// Use translations
t('nav.planning')

// Change language
i18n.changeLanguage('ru')
```

**Translation Namespaces:**
- `nav.*` - Navigation labels (planning, to do, tasks, categories, settings, …)
- `shell.*` - App-shell accessibility labels (menu open/close, main navigation)
- `priority.*` - The 1 (Highest) – 5 (Lowest) priority labels
- `today.*` - The "Tasks for today" page
- `login.*` / `register.*` - Authentication form labels and messages
- `profile.*` - Profile management UI
- `settings.*` - Theme, language, time format and week start
- `tasks.*` / `taskForm.*` / `dynamicTaskForm.*` - Task tables and the task form
- `calendar.*` - Planner views, toolbar, re-plan, today summary, event details

## Routing

Routes defined in `config/AppRoutes.tsx`:

| Route | Component | Auth | Description |
|-------|-----------|------|-------------|
| `/` | CalendarPage | Required | Planner (D/3D/W/M), quick-add on empty slots |
| `/today` | TodayPage | Required | "Tasks for today" list (read-only) |
| `/tasks` | TasksPage | Required | Tabbed fixed/dynamic task management |
| `/categories` | CategoriesPage | Required | Categories and their time windows |
| `/profile` | ProfilePage | Required | User profile view/edit |
| `/about` | AboutPage | Public | About the application |
| `/settings` | SettingsPage | Public | Light/dark theme tiles, language, time format, week start |
| `/login` | LoginPage | Public | Login/registration tabs (outside Layout) |
| `/*` | NotFoundPage | Public | 404 catch-all |

**Protected Routes:**
- Wrapped with `<PrivateRoute auth={true}>` component
- Shows Ant Design Spin loader during authentication check
- Redirects to `/login` if unauthenticated

**Layout:**
- All routes except `/login` are wrapped in `<Layout>` component
- One backdrop for the whole app with a cursor-following glow (`Layout/CursorGradient`); the sidebar and
  pages paint no surface of their own, only components do. Phones: sticky top bar + full-screen menu

## Key Components

**Layout** (`components/Layout/`)
- `DesktopSidebar`: logo, section nav (`NavMenu`), the planner's month card (`MiniCalendar`, with the visible
  week / 3 days as a band), then `AccountNav` (settings, about, profile, log out)
- `MobileNav`: logo + hamburger opening the same `NavMenu` / `AccountNav` in a full-screen drawer
- Desktop vs phone is decided in CSS at 768px (antd `md`), so the page never re-mounts on resize
- Content area renders child routes via `<Outlet />`

**UnifiedTaskFormModal** (`components/UnifiedTaskFormModal/`)
- Create/edit dialog for both task types (Fixed/Dynamic pill switch, locked when editing), full screen on phones
- Shared fields: Name, Priority (1 Highest – 5 Lowest), Categories, Description
- Fixed: `WhenFields` — calendar card with From/To clocks and the "⟳ repeats" picker (frequency pills,
  weekday circles, month-day grid, yearly date, "Ends" pills; create only — editing shows the existing
  recurrence read-only)
- Dynamic: min/max/optimal duration as H:MM clocks
- Every time and duration is a `ClockStepper`: a 7-segment clock that takes typed times ("1422", "12", "12:11"),
  − / + buttons that move it by 5 minutes, and a picker in 5-minute steps
- Returns `FixedTaskFormData` + optional `ScheduleFormPayload`, or `InputDynamicTask`

**PrivateRoute** (`components/PrivateRoute/index.tsx`)
- Route guard wrapper component
- Checks authentication state from `AuthContext`
- Shows loading spinner during auth check
- Redirects to `/login` if user is not authenticated

**CalendarPage** (`pages/CalendarPage/`)
- Planner views: Month, Week, Day, custom 3-Day, switched from `PlannerToolbar` (rbc's own toolbar is off)
- Fetches task timeline via `fetchTasksForDays()` API call
- Converts API tasks to calendar events with proper date/time handling
- The magic-wand button re-plans (regenerates snapshots via `refreshTasksForDays()`) for visible days from today on
- Hovering an empty hour shows a quick-add slot whose "+" opens the task form for that hour; dragging (or a
  long press on touch) opens it for the selected range
- Events adapt to their size (card → two lines → one line → "…") via CSS container queries, at 88px an hour;
  each shows its category dots and description when there is room, Highest priority is ringed red, Lowest fades
- Event click shows `EventDetailModal`; phones get a week strip + "tasks left today" summary in day view
- Month on phones: each day shows its number and a dot per task (`MonthDateHeader`) and opens that day when tapped;
  desktop keeps rbc's event pills

**TodayPage** (`pages/TodayPage/`)
- Today's timeline as a list with re-plan / add-task buttons; read-only, since the API does not track completion

**ListCard** (`components/ListCard/`)
- The design's list row: accent rail, title with category dots, description, meta line, a summary on the right
  (`ListCardRange` for a from – to pair), tap to open and an optional delete button; `ListCardList` adds the
  loading and empty states
- Today's rows, and the phone lists of tasks and categories

**TasksPage** (`pages/TasksPage.tsx`)
- Tabbed interface: Fixed Tasks / Dynamic Tasks
- Ant Design tables with inline CRUD actions on desktop; on phones the same data as `ListCard` rows (tap to edit,
  🗑 to delete)
- Modals for create/edit operations
- Fixed Tasks table shows recurring type badge and ends-on date
- CategoriesPage works the same way: a table with expandable time windows on desktop, cards on phones whose
  windows open under the card

## Custom Hooks

**`useFixedTasks`** (`hooks/useFixedTasks.ts`)
- Fixed task CRUD state management + API integration
- Returns: tasks array, loading state, CRUD methods (create, update, delete)

**`useDynamicTasks`** (`hooks/useDynamicTasks.ts`)
- Dynamic task CRUD state management + API integration
- Converts between minutes (UI) and TimeSpan format (API)

**`useQuery`** (`hooks/useQuery.ts`)
- TanStack Query wrapper with auto-select functionality
- Simplifies query setup with automatic data transformation

**`useMutation`** (`hooks/useMutation.ts`)
- TanStack mutation wrapper for data mutations
- Provides consistent error handling and loading states

**`useIsMobile`** (`hooks/useIsMobile.ts`)
- Responsive media query hook using Ant Design breakpoints
- Returns boolean indicating if viewport is mobile-sized

## Styling Approach

**Component Library:**
- Ant Design for all UI components (buttons, forms, modals, tables, etc.)

**Design tokens** (`src/theme/`):
- `palette.ts` is the single source of every colour (light + dark), radius and font
- `antdTheme.ts` maps it onto antd seed/component tokens (`ConfigProvider` in `App.tsx`)
- `cssVariables.ts` publishes it as `--th-*` custom properties (`:root` / `:root.dark`) for plain CSS —
  antd scopes its own variables per component, so custom CSS reads `--th-*` only

**Component styles:**
- Plain `styles.css` next to the component, with `th-` prefixed class names

**Dark Mode:**
- Ant Design `theme.darkAlgorithm` + the dark palette applied via `ConfigProvider`
- Switched from the Settings page (the only theme control)
- `"dark"` class toggled on `<html>` by `ThemeContext` (in a layout effect, so the first paint is right),
  which switches every `--th-*` variable

**Responsive Design:**
- Shell and page chrome switch in CSS at 768px (antd `md`)
- Tables on desktop, cards on phones: both render, `.th-desktop-only` / `.th-phone-only` (`index.css`) pick one
- Forms' calendar card folds into a date row on phones; the planner's day headers compact by column width
- `useIsMobile()` hook where behaviour (not just looks) differs, e.g. the planner's initial view

## Development

**Run locally:**
```bash
npm install
npm run dev
```

**Build:**
```bash
npm run build
npm run preview
```

**Docker (Recommended):**
```bash
# From src/ directory, after the one-time certificate setup in ../README.md#running-locally
docker compose --profile dev up
```
- UI service runs on port **5173**
- The profile picks the services (`minimal`, `dev`, `full`); a bare `docker compose up` starts only the database

**Path Aliases** (configured in `vite.config.ts`):
- `api` → `src/api`
- `components` → `src/components`
- `config` → `src/config`
- `contexts` → `src/contexts`
- `hooks` → `src/hooks`
- `pages` → `src/pages`
- `theme` → `src/theme`
- `types` → `src/types`
- `utils` → `src/utils`
- `i18n` → `src/i18n`

## Docker Build

**Multi-stage Node Alpine build:**

1. **Install all dependencies:** `npm ci`
2. **Install production dependencies only:** `npm ci --omit=dev`
3. **Build application:** `npm run build` → outputs to `/app/build`
4. **Runtime stage:** Prod dependencies + build output → `npm run start`

**Port:** 5173 (mapped in `docker-compose.yml`)

**Build Output:** `/app/build`

## Notes

- **Cookie-based authentication** via ASP.NET Core Identity
- All requests include credentials (`withCredentials: true`)
- Multi-tenant by design (user-scoped data)
- Dark mode preference persisted to localStorage
- i18n support with English/Russian languages
- Session expiry automatically redirects to login with alert
- Calendar supports custom 3-day view alongside standard views
- Task timeline stored in daily snapshots to preserve the generated schedule — snapshots are not regenerated automatically, ensuring the day's plan stays stable
