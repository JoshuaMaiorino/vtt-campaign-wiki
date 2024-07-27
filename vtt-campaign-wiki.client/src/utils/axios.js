import axios from 'axios';
import { getActivePinia } from 'pinia';
import { useAuthStore } from '@/stores/auth';

const instance = axios.create({
    baseURL: 'https://localhost:7128', // Update with your backend URL
    withCredentials: true, // Ensure credentials are included if needed
});

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

export default instance;