<script setup>
    import LoginNavItem from "@/components/LoginNavItem.vue"
    import { useCampaignStore } from "@/stores/campaign"
    import { useRouter } from 'vue-router'
    import { computed } from 'vue'

    const campaignStore = useCampaignStore();
    const router = useRouter()

    const navigateHome = () => {
        console.log("navigating")
        if( campaignStore.selectedCampaign ){
            router.push(`/campaigns/${campaignStore.selectedCampaign.id}`)
        }else {{

            router.push('/')
        }}
    }

    const navItems = computed( () => {
        return campaignStore.selectedCampaign?.items?.slice(0, 5) ?? []
    })

</script>

<template>
    <v-app full-height>
        <v-app-bar color="grey-darken-4"
                    scroll-behavior="elevate"
                    floating
                    >
            <v-toolbar-title class="cursor-pointer"
                             @click="navigateHome">
                {{ `${campaignStore.selectedCampaign?.title} Campaign Wiki` ?? 'Campaign Wikis' }}
            </v-toolbar-title>
            <template v-if="campaignStore.selectedCampaign">
                <v-spacer></v-spacer>
                <v-btn text 
                       v-for="item in navItems" 
                       :to="`/campaigns/${campaignStore.selectedCampaign.id}/${item.id}`" 
                       :key="item.id">{{ item.title }}
                </v-btn>
            </template>
            <v-spacer></v-spacer>
            <v-btn text to="/campaigns">Campaigns</v-btn>
            <LoginNavItem />
        </v-app-bar>
        <v-main style="min-height: 300px;">
            <router-view :key="$route.fullPath"></router-view>
        </v-main>
    </v-app>
</template>
    <style>
    </style>
