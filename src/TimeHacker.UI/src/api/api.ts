import axios from 'axios';

/** Router basename — keep in sync with <Router basename> in App.tsx. */
export const APP_BASE = '/app';

export const api = axios.create({
    baseURL: import.meta.env.VITE_BASE_URL ?? 'https://localhost:8081',
    withCredentials: true
});

// --- CSRF / antiforgery ---
// The backend exposes an antiforgery token at GET /api/antiforgery/token. We fetch
// it once (after auth) and echo it back in the X-XSRF-TOKEN header on every
// state-changing request, which the server validates.
let csrfToken: string | null = null;
const MUTATING_METHODS = new Set(['post', 'put', 'delete', 'patch']);

export async function loadCsrfToken(): Promise<void> {
    const response = await api.get<{ token: string }>('/api/antiforgery/token');
    csrfToken = response.data.token;
}

api.interceptors.request.use((config) => {
    const method = (config.method ?? 'get').toLowerCase();
    if (csrfToken && MUTATING_METHODS.has(method)) {
        config.headers['X-XSRF-TOKEN'] = csrfToken;
    }
    return config;
});

api.interceptors.response.use(
    (response) => response,
    (error) => {
        const url = error.config?.url ?? '';
        const isAuthCheck = url.includes('/api/users/me');
        const isAuthEndpoint = url.includes('/login') || url.includes('/register');

        if (error.response?.status === 401 && !isAuthCheck && !isAuthEndpoint) {
            window.location.href = `${APP_BASE}/login?expired=true`;
        }
        return Promise.reject(error);
    }
);

