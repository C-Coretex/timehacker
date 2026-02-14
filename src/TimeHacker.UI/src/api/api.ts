import axios from 'axios';

const api = axios.create({
    baseURL: 'https://localhost:8081',
    withCredentials: true
});

api.interceptors.response.use(
    (response) => response,
    (error) => {
        const url = error.config?.url ?? '';
        const isAuthCheck = url.includes('/api/User/GetCurrent');
        const isAuthEndpoint = url.includes('/login') || url.includes('/register');

        if (error.response?.status === 401 && !isAuthCheck && !isAuthEndpoint) {
            window.location.href = '/login?expired=true';
        }
        return Promise.reject(error);
    }
);

export default api;
