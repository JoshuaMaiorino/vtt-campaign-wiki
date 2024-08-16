<template>
    <v-btn v-if="!authStore.user" text to="/login" class="ml-4 mr-4">Login</v-btn>
    <v-menu v-else offset-y>
        <template v-slot:activator="{ props }">
            <v-avatar v-bind="props"
                      color="primary"
                      size="32"
                      class="ml-4 mr-4"
                      style="cursor: pointer;">
                <span>{{ authStore.user.charAt(0).toUpperCase() }}</span>
            </v-avatar>
        </template>
        <v-list>
            <v-list-item to="/campaigns">
                <v-list-item-title>My Campaigns</v-list-item-title>
            </v-list-item>
            <v-list-item @click="logout">
                <v-list-item-title>Logout</v-list-item-title>
            </v-list-item>
        </v-list>
    </v-menu>
</template>

<script setup>
    import { useAuthStore } from '@/stores/auth.js'
    import { useRouter } from 'vue-router'

    const router = useRouter();

    const authStore = useAuthStore();

    const logout = () => {
        authStore.clearAuth();
        router.push("/");
    }


</script>

<style>

</style>