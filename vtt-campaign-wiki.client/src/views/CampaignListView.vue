<template>
    <v-container>
        <v-row>
            <v-col v-for="campaign in campaigns" :key="campaign.id" cols="6">
                <ItemCard :item="campaign" @selected="editItem" @click="$router.push(`/campaigns/${campaign.id}`)"/>
            </v-col>
        </v-row>

    </v-container>
    <v-fab icon="mdi-dots-horizontal"
           class="mb-12"
           location="bottom end"
           absolute
           app
           appear
           color="secondary"
           offset
           @click="addNew"></v-fab>
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
            const response = await axios.get('/campaigns');
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
                await axios.put(`/campaigns/${selectedCampaign.value.id}`, formData, {
                    headers: {
                        'Content-Type': 'multipart/form-data'
                    }
                });
                Object.assign(campaigns.value[ editIndex.value ], selectedCampaign.value);
            } else {
                const response = await axios.post('/campaigns', formData, {
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
                await axios.delete(`/campaigns/${selectedCampaign.value.id}`);
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