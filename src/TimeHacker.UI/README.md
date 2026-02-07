# TimeHacker.UI - Developer Guide

## Tech Stack

- React 19 + TypeScript
- React Router for routing
- Ant Design for UI components
- Tailwind CSS for styling
- Axios for HTTP requests
- react-big-calendar for calendar views
- Vite for build tooling

## Structure

```
src/
├── api/          # Backend API integration layer
├── components/   # Reusable UI components
├── config/       # App configuration (routes, axios)
├── contexts/     # React Context providers (Auth, Theme)
├── hooks/        # Custom React hooks for state management
├── pages/        # Page components (routes)
├── types/        # TypeScript definitions
├── App.tsx       # Root component with providers
└── main.tsx      # Entry point
```

## Communication with Backend

**Base URL:** `https://localhost:8081` (configurable via `VITE_BASE_URL`)

**Data Flow:**
```
Component → Custom Hook → API Function → Axios (with Bearer token) → Backend
```

**API Layer Examples:**
- `api/tasks.ts` - Calendar operations (fetch tasks for day, refresh)
- `api/fixedTasks.ts` - Fixed task CRUD + scheduling
- `api/dynamicTasks.ts` - Dynamic task CRUD

All API responses follow: `{ success: boolean; data: T; errors?: ... }`

## State Management

**Global State (React Context):**
- `AuthContext` - User authentication, persisted to localStorage
- `ThemeContext` - Dark mode preference, persisted to localStorage

**Local State (Custom Hooks):**
- Custom hooks like `useFixedTasks`, `useDynamicTasks` manage API state
- Pattern: fetch on mount, provide CRUD methods, handle loading/error states

**Forms:**
- Ant Design Form components manage form state
- Modal visibility managed by local state

## Routing

Routes defined in `config/AppRoutes.tsx`:
- Layout wrapper with sidebar for main routes
- Protected routes use `PrivateRoute` wrapper
- Separate `/login` route without layout

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

**Path aliases configured in vite.config.ts** (e.g., `api`, `components`, `hooks`)

## Notes

- Bearer token authentication via Axios interceptor
- All requests include credentials
- Multi-tenant by design (user-scoped data)
