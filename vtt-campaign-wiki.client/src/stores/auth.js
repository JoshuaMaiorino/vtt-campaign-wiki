import { defineStore } from 'pinia'

export const useAuthStore = defineStore('auth', {
    state: () => ({
        token: null,
        user: null,
        userId: null,
    }),
    actions: {
        setToken (token) {
            this.token = token;
        },
        setUser (user) {
            this.user = user;
        },
        setUeserId (userId) {
            this.userId = userId;
        },
        clearAuth () {
            this.token = null;
            this.user = null;
            this.userId = null;
        }
    },
    persist: true
})
