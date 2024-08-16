<script setup>
    import LoginNavItem from "@/components/LoginNavItem.vue"
    import SearchNavItem from "@/components/SearchNavItem.vue"
    import { useCampaignStore } from "@/stores/campaign"
    import { useRouter } from 'vue-router'
    import { computed, ref } from 'vue'

    const campaignStore = useCampaignStore();

    const router = useRouter()

    const searchOpen = ref(false);

    const navigateHome = () => {
        console.log("navigating")
        if( campaignStore.selectedCampaign ){
            router.push(`/campaigns/${campaignStore.selectedCampaign.id}`)
        }else {{

            router.push('/')
        }}
    }

    const navigateSessions = () => {
        if (campaignStore.selectedCampaign) {
            router.push(`/campaigns/${campaignStore.selectedCampaign.id}/sessions`)
        } else {
            {
                router.push('/sessions')
            }
        }
    }

    const navItems = computed( () => {
        return campaignStore.selectedCampaign?.items?.slice(0, 5) ?? []
    })

</script>

<template>
    <v-app full-height>
        <v-app-bar color="grey-darken-4"
                   scroll-behavior="elevate"
                   floating>
            <v-toolbar-title class="cursor-pointer"
                             @click="navigateHome">
                {{ campaignStore.selectedCampaign?.title }} Campaign Wiki
            </v-toolbar-title>
            <v-spacer></v-spacer>

            <template v-if="campaignStore.selectedCampaign && !searchOpen">

                <v-btn text @click="navigateSessions">Sessions</v-btn>

                <v-btn text
                       v-for="item in navItems"
                       :to="`/campaigns/${campaignStore.selectedCampaign.id}/${item.id}`"
                       :key="item.id">
                    {{ item.title }}
                </v-btn>
            </template>

            <SearchNavItem v-if="campaignStore.selectedCampaign" v-model:searchOpen="searchOpen" />

            <v-divider vertical></v-divider>
            <LoginNavItem />
        </v-app-bar>
        <v-main style="min-height: 300px;">
            <router-view :key="$route.fullPath"></router-view>
        </v-main>
    </v-app>
</template>
<style>
    .v-container {
        max-width: 1280px;
    }
    ul, ol {
        margin-left: 30px;
    }
</style>
