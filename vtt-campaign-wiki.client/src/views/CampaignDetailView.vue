<template>
    <div class="campaign-detail-view">
        <div class="hero-section">
            <v-parallax :src="`/api/image/${campaign.imageId}`" cover max-height="320" gradient="to top right, rgba(100,115,201,.33), rgba(25,32,72,.7)">
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
        <v-container v-if="session">
            <v-list lines="three">
                <v-list-item @click="$router.push(`/campaigns/${campaign.id}/sessions`)">
                    <v-list-item-title><h3>Latest Recap - Session {{ session.number }}: {{ session.title }}</h3></v-list-item-title>
                    <v-list-item-subtitle v-if="session.content"><ParsedContent :content="session.content" :campaignItems="campaign?.items" :itemId="0" /></v-list-item-subtitle>
                </v-list-item>
            </v-list>
        </v-container>
        

        <v-container>
            <v-row>
                <v-col
                cols="12"
                md="6"
                lg="4">
                    <v-card elevation="0" @click="$router.push(`/campaigns/${campaign.id}/sessions`)">
                        <v-img :src="`/api/campaigns/${campaign.id}/sessions/hero-image`"
                               cover
                                :aspect-ratio="4.0/3.0" />
                        <v-card-title>Session Recaps</v-card-title>
                    </v-card>
                </v-col>
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
            <CampaignItemTree v-if="campaign" v-model:currentItem="campaign" v-model:items="campaign.items" @edit="$router.push(`/campaigns/${campaign.id}/edit`)" :isCampaign="true"/>
        </v-navigation-drawer>

        <ItemEdit title="Edit Campaign" v-model="editDialog" v-model:item="editCampaign" @save="save" @close="editDialog=false" @delete="deleteItem" />
    </div>
</template>

<script setup>
    import { ref, computed, onMounted } from 'vue';
    import { useCampaignStore } from '@/stores/campaign';
    import { useAuthStore } from '@/stores/auth';
    import { toFormData } from '@/utils/formData';
    
    import ParsedContent from '@/components/ParseContentV2.vue'
    import ItemCard from '@/components/ItemCard.vue';
    import CampaignItemTree from '@/components/CampaignItemTree.vue'
    import ItemEdit from '@/components/ItemEdit.vue'
    import axios from '@/utils/axios';

    const campaignStore = useCampaignStore();
    const campaign = campaignStore.selectedCampaign;

    const authStore = useAuthStore();
    const userId = authStore.userId;

    const session = ref(null)

    const sidePanel = ref(false);

    const fetchLatestSession = async () => {
        try {
            const params = new URLSearchParams();
            params.append('page', 1);
            params.append('itemsPerPage', 1);
            params.append(`sortBy[0][key]`, 'number');
            params.append(`sortBy[0][order]`, 'desc');

            const response = await axios.get(`/api/campaigns/${campaign.id}/sessions`, { params });
            session.value = response.data.items[0];
        
        } catch (error) {
            console.error('Failed to fetch sessions:', error);
        }
    }


    onMounted( fetchLatestSession );

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
