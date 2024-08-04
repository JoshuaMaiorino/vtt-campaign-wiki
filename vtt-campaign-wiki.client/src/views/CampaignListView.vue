<template>
    <div class="hero-section">
        <v-parallax src="/assets/1cbda947-8b8a-4011-847c-b603c141e4db.webp"
                    gradient="to top right, rgba(100,115,201,.33), rgba(25,32,72,.7)"
                    cover max-height="320"
                    color="surface-variant">
            <v-container class="fill-height" fluid>
                <v-row class="fill-height">
                    <v-col class="d-flex align-center justify-center">
                        <h1 class="hero-title">Campaigns</h1>
                    </v-col>
                </v-row>
            </v-container>
        </v-parallax>
    </div>
    
    <v-container>
        <v-row>
            <v-col v-for="campaign in campaigns" :key="campaign.id" cols="6">
                <ItemCard :item="campaign" @selected="editItem" @click="$router.push(`/campaigns/${campaign.id}`)"/>
            </v-col>
        </v-row>

    </v-container>
    <v-fab position="static" icon="mdi-plus" class="mb-6" location="bottom end" app appear color="primary" offset @click="addNew"></v-fab>
    <ItemEdit :title="formTitle" v-model="editDialog" v-model:item="selectedCampaign" @save="save" @close="close" @delete="deleteItem"/>
</template>

<script setup>
    import { ref, nextTick, computed, onMounted } from 'vue'
    import axios from '@/utils/axios';
    import { toFormData } from '@/utils/formData';

    import ItemCard from '@/components/ItemCard.vue'
    import ItemEdit from '@/components/ItemEdit.vue'

    const campaigns = ref([]);

    // Edit Campaign Data & Methods
    const formTitle = computed(() => editIndex.value === -1 ? 'New Campaign' : 'Edit Campaign' )

    const defaultCampaign = {
        id: -1,
        title: '',
        content: '',
        externalLink: '',
        imageId: null,
        authorId: null,
        image: null
    }

    const selectedCampaign = ref({
        id: -1,
        title: '',
        content: '',
        externalLink: '',
        imageId: null,
        authorId: null,
        image: null
    })

    const fetchCampaigns = async () => {
        try {
            const response = await axios.get('/api/campaigns');
            campaigns.value = response.data;
        } catch (error) {
            console.error('Failed to fetch campaigns', error);
        }
    };

    onMounted(() => {
        fetchCampaigns();
    });

    const editDialog = ref(false)
    const editIndex = ref(-1)

    const addNew = () => {
        editIndex.value = -1
        selectedCampaign.value = Object.assign({}, defaultCampaign)
        editDialog.value = true
    }
    const editItem = (item) => {
        editIndex.value = campaigns.value.indexOf(item)
        selectedCampaign.value = Object.assign({}, item)
        editDialog.value = true
    }
    const save = async () => {

        let formData = toFormData(selectedCampaign.value)

        try {
            if (editIndex.value > -1) {
                await axios.put(`/api/campaigns/${selectedCampaign.value.id}`, formData, {
                    headers: {
                        'Content-Type': 'multipart/form-data'
                    }
                });
                Object.assign(campaigns.value[ editIndex.value ], selectedCampaign.value);
            } else {
                const response = await axios.post('/api/campaigns', formData, {
                    headers: {
                        'Content-Type': 'multipart/form-data'
                    }
                });
                campaigns.value.push(response.data);
            }
            close();
        } catch (error) {
            console.error('Failed to save campaign', error);
        } 
    }

    const deleteItem = async () => {
        if (editIndex.value > -1) {
            try {
                await axios.delete(`/api/campaigns/${selectedCampaign.value.id}`);
                campaigns.value.splice(editIndex.value, 1);
            } catch (error) {
                console.error('Failed to delete campaign', error )
            }
        }
        close();
    }

    const close = () => {
        editDialog.value = false
        nextTick(() => {
            setTimeout(() => {
                editIndex.value = -1
                selectedCampaign.value = Object.assign({}, defaultCampaign)
            }, 300 )
        })  
    }

</script>

<style>
</style>