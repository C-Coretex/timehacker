// api.ts
import axios from 'axios';

const api = axios.create({
    baseURL: 'https://localhost:8081',
    withCredentials: true
});

export default api;
