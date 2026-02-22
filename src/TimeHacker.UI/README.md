# TimeHacker.UI - Developer Guide

## Tech Stack

| Category | Technology | Version |
|----------|-----------|---------|
| Framework | React | 19.2 |
| Language | TypeScript | 5.9 (strict mode) |
| Build Tool | Vite | 7.3 |
| Routing | React Router | 7.13 |
| HTTP Client | Axios | 1.13 |
| Data Fetching | TanStack Query (React Query) | 5.90 |
| UI Components | Ant Design | 6.3 |
| Styling | Tailwind CSS | 4.1 |
| Internationalization | i18next + react-i18next | 25.1 + 15.5 |
| Date Handling | dayjs | 1.11 |
| Calendar | react-big-calendar | 1.19 |
| Docker Base | Node | 24 Alpine |

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
├── pages/        # Page components (Calendar, Tasks, Profile, Settings, etc.)
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
t('nav.calendar')

// Change language
i18n.changeLanguage('ru')
```

**Translation Namespaces:**
- `nav.*` - Navigation labels (calendar, tasks, categories, login, profile, etc.)
- `greeting.*` - Time-based greetings (morning, afternoon, evening) + tagline
- `header.*` - Summary statistics (fixedTasksLeft, dynamicTasksLeft, highPriority)
- `login.*` / `register.*` - Authentication form labels and messages
- `profile.*` - Profile management UI
- `settings.*` - Dark mode and language settings
- `tasks.*` - Task CRUD labels, form fields, recurring schedule options
- `calendar.*` - Calendar view names, refresh button, event details

## Routing

Routes defined in `config/AppRoutes.tsx`:

| Route | Component | Auth | Description |
|-------|-----------|------|-------------|
| `/` | CalendarPage | Required | Main calendar view (Month/Week/Day/3-Day) |
| `/tasks` | TasksPage | Required | Tabbed fixed/dynamic task management |
| `/profile` | ProfilePage | Required | User profile view/edit |
| `/about` | AboutPage | Public | About the application |
| `/settings` | SettingsPage | Public | Dark mode toggle & language selector |
| `/login` | LoginPage | Public | Login/registration tabs (outside Layout) |
| `/*` | NotFoundPage | Public | 404 catch-all |

**Protected Routes:**
- Wrapped with `<PrivateRoute auth={true}>` component
- Shows Ant Design Spin loader during authentication check
- Redirects to `/login` if unauthenticated

**Layout:**
- All routes except `/login` are wrapped in `<Layout>` component
- Layout includes collapsible sidebar + header + content area

## Key Components

**Layout** (`components/Layout/index.tsx`)
- Collapsible sidebar with logo & navigation menu
- Header with time-based greeting, user name, and task summary badges
- Content area renders child routes via `<Outlet />`
- Responsive: sidebar collapses to drawer on mobile (via `useIsMobile()`)

**UnifiedTaskFormModal** (`components/UnifiedTaskFormModal.tsx`)
- Create/edit modal for fixed tasks
- Fields: Name, Description, Priority (1-10), Start/End timestamps (DatePicker)
- **Create mode:** Optional recurring schedule section (type, type-specific fields, ends-on date/occurrence limit)
- **Edit mode:** Read-only schedule metadata display (created date, last generated date, ends-on)
- Returns `FixedTaskFormData` + optional `ScheduleFormPayload`

**PrivateRoute** (`components/PrivateRoute/index.tsx`)
- Route guard wrapper component
- Checks authentication state from `AuthContext`
- Shows loading spinner during auth check
- Redirects to `/login` if user is not authenticated

**CalendarPage** (`pages/CalendarPage/index.tsx`)
- Calendar views: Month, Week, Day, custom 3-Day
- Fetches task timeline via `fetchTasksForDays()` API call
- Converts API tasks to calendar events with proper date/time handling
- Refresh button regenerates snapshots via `refreshTasksForDays()`
- Event click shows detail modal with task information
- Dark mode color adaptation for events

**TasksPage** (`pages/TasksPage.tsx`)
- Tabbed interface: Fixed Tasks / Dynamic Tasks
- Ant Design tables with inline CRUD actions
- Modals for create/edit operations
- Responsive: hides Schedule column on mobile
- Fixed Tasks table shows recurring type badge and ends-on date

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
- Ant Design for all UI components (buttons, forms, modals, tables, menus, navigation, etc.)

**Utility Styling:**
- Tailwind CSS for layout, spacing, responsive utilities, and custom colors

**Dark Mode:**
- Ant Design `theme.darkAlgorithm` applied via `ConfigProvider`
- Tailwind `dark:` utility classes for custom styles
- `"dark"` class toggled on `<html>` element by `ThemeContext`

**Responsive Design:**
- `useIsMobile()` hook for conditional rendering/behavior
- Ant Design Grid breakpoints for responsive layouts
- Sidebar → drawer transition on mobile

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
# From src/ directory
docker compose up
```
- UI service runs on port **5173**
- All services (API, databases, pgAdmin) start together

**Path Aliases** (configured in `vite.config.ts`):
- `api` → `src/api`
- `components` → `src/components`
- `config` → `src/config`
- `contexts` → `src/contexts`
- `hooks` → `src/hooks`
- `pages` → `src/pages`
- `types` → `src/types`
- `utils` → `src/utils`
- `i18n` → `src/i18n`

## Docker Build

**Multi-stage Node 24 Alpine build:**

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
