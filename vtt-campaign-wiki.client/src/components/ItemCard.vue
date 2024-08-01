<template>
    <v-card flat rounded="0" >
        <v-img v-if="item?.imageId" color="surface-variant"
               height="200"
               :src="`/api/image/${item.imageId}`"
               cover />
        <v-toolbar color="transparent" :title="item?.title">
        </v-toolbar>
        <v-card-text>
            <ParsedContent v-if="item?.content" :content="item.content" :itemId="item.id" :campaignItems="campaign?.items" />
        </v-card-text>
    </v-card>
</template>

<script setup>

    import ParsedContent from '@/components/ParseContentV2.vue'
    import { useCampaignStore } from '@/stores/campaign.js'

    const props = defineProps({
        item: Object
    })

    const campaignStore = useCampaignStore()

    const campaign = campaignStore.selectedCampaign

</script>

<style>
</style>