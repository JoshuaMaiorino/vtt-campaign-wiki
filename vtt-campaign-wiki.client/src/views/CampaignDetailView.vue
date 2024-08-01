<template>
    <div class="campaign-detail-view">
        <div class="hero-section">
            <v-parallax :src="`/api/image/${campaign.imageId}`" cover max-height="320">
                <v-container class="fill-height" fluid>
                    <v-row class="fill-height">
                        <v-col class="d-flex align-center justify-center">
                            <h1 class="hero-title">{{ campaign.title }}</h1>
                        </v-col>
                    </v-row>
                </v-container>
            </v-parallax>
        </div>

        <v-sheet v-if="campaign.content" color="surface-bright" class="d-flex align-center justify-center pa-12">
            <div v-html="campaign.content"></div>
        </v-sheet>

        <v-container>
            <v-row>
                <v-col v-for="item in campaign.items"
                       :key="item.id"
                       cols="12"
                       md="6"
                       lg="4">
                    <ItemCard :item="item" @selected="editItem" @click="$router.push(`/campaigns/${campaign.id}/${item.id}`)" />
                </v-col>
            </v-row>
        </v-container>

        <v-fab v-if="campaign.authorId === userId" position="static" icon="mdi-dots-horizontal" class="mb-6" location="bottom end" app appear color="primary" offset @click="sidePanel = !sidePanel"></v-fab>

        <v-navigation-drawer temporary :model-value="sidePanel" location="right" width="450">
            <CampaignItemTree v-if="campaign" v-model:currentItem="campaign" v-model:items="campaign.items" @edit="edit"/>
        </v-navigation-drawer>

        <ItemEdit title="Edit Campaign" v-model="editDialog" v-model:item="editCampaign" @save="save" @close="editDialog=false" @delete="deleteItem" />
    </div>
</template>

<script setup>
    import { ref, computed, onMounted } from 'vue';
    import { useCampaignStore } from '@/stores/campaign';
    import { useAuthStore } from '@/stores/auth';
    import { toFormData } from '@/utils/formData';

    import ItemCard from '@/components/ItemCard.vue';
    import CampaignItemTree from '@/components/CampaignItemTree.vue'
    import ItemEdit from '@/components/ItemEdit.vue'
    import axios from '@/utils/axios';

    const campaignStore = useCampaignStore();
    const campaign = campaignStore.selectedCampaign;

    const authStore = useAuthStore();
    const userId = authStore.userId;

    const sidePanel = ref(false);
    const editDialog = ref(false);
    const editCampaign = ref(null);

    const edit = () => {
        editCampaign.value = {...campaign}
        editDialog.value = true
    }

    const save = async () => {
        let formData = toFormData(editCampaign.value);

        try {
        
            const response = await axios.put(`/api/campaigns/${campaign.id}`, formData, {
                headers: {
                    'Content-Type': 'multipart/form-data',
                },
            });
            
            Object.assign(campaign, response.data);

            editDialog.value = false;
        } catch (error) {
            console.error('Failed to save campaign item', error);
        }
    }

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
