import axios from 'axios';
import { getActivePinia } from 'pinia';
import { useAuthStore } from '@/stores/auth';

const instance = axios.create({
    withCredentials: true, // Ensure credentials are included if needed
});

// Request interceptor to add the token to headers
instance.interceptors.request.use(config => {
    const pinia = getActivePinia();
    if (!pinia) {
        throw new Error('[🍍]: "getActivePinia()" was called but there was no active Pinia.');
    }

    const authStore = useAuthStore(pinia);
    const token = authStore.token;
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});

// Response interceptor to handle token expiration
instance.interceptors.response.use(
    response => response,
    error => {
        const pinia = getActivePinia();
        if (!pinia) {
            throw new Error('[🍍]: "getActivePinia()" was called but there was no active Pinia.');
        }

        const authStore = useAuthStore(pinia);
        if (error.response && error.response.status === 401 && window.location.href != '/login') {
            authStore.clearAuth();
            window.location.href = '/login';
        }
        return Promise.reject(error);
    }
);

export default instance;
