<script setup>
</script>

<template>
    <v-app>
        <v-app-bar scroll-behavior="elevate"
                   color="grey-darken-4">
            <v-toolbar-title>Campaign Wikis</v-toolbar-title>
            <v-spacer></v-spacer>
            <v-btn text to="/">Campaigns</v-btn>
            <v-btn text to="/about">Login</v-btn>
        </v-app-bar>
        <v-main>
            <router-view></router-view>
        </v-main>
    </v-app>
</template>

<style>
</style>
