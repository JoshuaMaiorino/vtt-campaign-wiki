<template>
    <div class="campaign-detail-view">
        <div class="hero-section">
            <v-img :src="`https://localhost:7128/image/${campaignItem?.imageId}`" cover max-height="320" color="surface-variant">
                <v-container class="fill-height" fluid>
                    <v-row class="fill-height">
                        <v-col class="d-flex align-center justify-center">
                            <h1 class="hero-title">{{ campaignItem?.title }}</h1>
                        </v-col>
                    </v-row>
                </v-container>
            </v-img>
        </div>

        <v-sheet v-if="campaignItem?.content" color="surface-variant" class="d-flex align-center justify-center pa-12">
            <div v-html="campaignItem?.content"></div>
        </v-sheet>

        <ItemContent v-for="child in campaignItem?.children" :key="child.id" :item="child" />

        <v-fab v-if="campaign.authorId === userId" position="static" icon="mdi-dots-horizontal" class="mb-6" location="bottom end" app appear color="primary" offset @click="sidePanel = !sidePanel"></v-fab>

        <v-navigation-drawer temporary :model-value="sidePanel" location="right" width="450">
            <CampaignItemTree v-if="campaignItem" :title="campaignItem?.title" v-model:items="campaignItem.children" />
        </v-navigation-drawer>
    </div>

</template>

<script setup>
    import { ref, computed, onMounted, nextTick } from 'vue';
    import { useCampaignStore } from '@/stores/campaign';
    import { useAuthStore } from '@/stores/auth';
    import axios from '@/utils/axios';
    import { toFormData } from '@/utils/formData';

    import ItemContent from '@/components/ItemContent.vue';
    import CampaignItemTree from '@/components/CampaignItemTree.vue'
    
    import ItemEdit from '@/components/ItemEdit.vue';

    import { useRoute } from 'vue-router'

    const sidePanel = ref(false)

    const route = useRoute()

    onMounted( async () => {
        try {
            const response = await axios.get(`/campaigns/${campaign.id}/items/${route.params.itemId}`);
            campaignItem.value = response.data;
        } catch(error) {
            console.error('Failed to fetch campaign item:', error);
        }
    })

    const campaignStore = useCampaignStore();
    const campaign = campaignStore.getSelectedCampaign;

    const campaignItem = ref(null);

    const authStore = useAuthStore();
    const userId = authStore.userId;


</script>

<style scoped>
.campaign-detail-view {
  margin-bottom: 2rem;
}

.hero-section {
  position: relative;
}

.hero-image {
  height: 300px;
  object-fit: cover;
}

.hero-title {
  color: white;
  text-shadow: 2px 2px 4px rgba(0, 0, 0, 0.7);
}

.campaign-item-card {
  margin: 1rem 0;
}

.campaign-item-image {
  height: 150px;
  object-fit: cover;
}
</style>
