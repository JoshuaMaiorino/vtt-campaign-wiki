<template>
    <div class="campaign-detail-view">
        <div class="hero-section">
            <v-parallax :src="`/api/image/${campaignItem?.imageId}`" cover max-height="320" color="surface-variant" gradient="to top right, rgba(100,115,201,.33), rgba(25,32,72,.7)">
                <v-container class="fill-height" fluid>
                    <v-row class="fill-height">
                        <v-col class="d-flex align-center justify-center">
                            <h1 class="hero-title">{{ campaignItem?.title }}</h1>
                        </v-col>
                    </v-row>
                </v-container>
            </v-parallax>
        </div>

        <v-sheet v-if="campaignItem?.content" color="surface-bright" class="pa-12">
            <v-container>
                <div v-html="campaignItem?.content"></div>
            </v-container>
        </v-sheet>

        <ItemContent v-for="child in campaignItem?.children" :key="child.id" :item="child" />

        <v-fab v-if="campaign.authorId === userId" position="static" icon="mdi-dots-horizontal" class="mb-6" location="bottom end" app appear color="primary" offset @click="sidePanel = !sidePanel"></v-fab>

        <v-navigation-drawer temporary :model-value="sidePanel" location="right" width="450">
            <CampaignItemTree v-if="campaignItem" v-model:currentItem="campaignItem" v-model:items="campaignItem.children" @edit="edit" />
        </v-navigation-drawer>
    </div>

    <ItemEdit title="Edit Item" v-model="editDialog" v-model:item="editItem" @save="save" @close="close" @delete="deleteItem" />

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
            const response = await axios.get(`/api/campaigns/${campaign.id}/items/${route.params.itemId}`);
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

     const editDialog = ref(false);
     const editItem = ref(null);

     const edit = () => {
         editItem.value = {...campaignItem.value}
         editDialog.value = true
     }

     const save = async () => {
         let formData = toFormData(editItem.value);

         try {
     
             const response = await axios.put(`/api/campaigns/${campaign.id}/items/${campaignItem.value.id}`, formData, {
                 headers: {
                     'Content-Type': 'multipart/form-data',
                 },
             });
         
             Object.assign(campaignItem.value, response.data);

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
