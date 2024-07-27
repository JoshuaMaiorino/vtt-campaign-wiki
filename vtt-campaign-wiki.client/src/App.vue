<script setup>
    import LoginNavItem from "@/components/LoginNavItem.vue"
    import { useCampaignStore } from "@/stores/campaign"

    const campaignStore = useCampaignStore();

</script>

<template>
    <v-app>
        <v-app-bar scroll-behavior="elevate"
                   color="grey-darken-4">
            <v-toolbar-title>Campaign Wikis</v-toolbar-title>
            <template v-if="campaignStore.selectedCampaign">
                <v-spacer />
                <v-btn text v-for="item in campaignStore.selectedCampaign.items" :key="id">{{ item.title }}</v-btn>
            </template>
            <v-spacer></v-spacer>
            <v-btn text to="/campaigns">Campaigns</v-btn>
            <LoginNavItem />
        </v-app-bar>
        <v-main>
            <router-view></router-view>
        </v-main>
    </v-app>
</template>

<style>
</style>
