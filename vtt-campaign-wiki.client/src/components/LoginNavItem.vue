<template>
    <v-btn v-if="!authStore.user" text to="/login" class="ml-4 mr-4">Login</v-btn>
    <v-avatar v-else color="primary" size="32" class="ml-4 mr-4">
        <span>{{ authStore.user.charAt(0) }}</span>
    </v-avatar>
</template>

<script setup>
    import { useAuthStore } from '@/stores/auth.js'

    const authStore = useAuthStore();
</script>

<style>

</style>