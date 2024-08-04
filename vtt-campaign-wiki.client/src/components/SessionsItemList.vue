<template>

    <v-toolbar title="Sessions" class="d-flex align-center">
        <v-pagination density="compact"
                      :length="pages"
                      :show-first-last-page="pages > 5"
                      v-model="page"
                      @update:modelValue="fetchSessions"
                      v-if="pages > 1" />
        <v-btn icon="mdi-plus" @click="addNew"></v-btn>
    </v-toolbar>


    <v-list class="ml-2">
        <v-list-item v-for="( session, index ) in items" :key="session.id" :title="`${session.number}:${session.title}`" @click="edit(index)" />
    </v-list>

    <ItemEdit title="New Session" v-model="editDialog" v-model:item="editSession" @save="save" @close="editDialog = false" @delete="deleteItem" />
</template>

<script setup>
    import { ref, computed, nextTick, onMounted } from 'vue'

    import ItemEdit from '@/components/ItemEdit.vue';

    import { useAuthStore } from '@/stores/auth.js'
    import { useCampaignStore } from '@/stores/campaign.js'
    import axios from '@/utils/axios';
    import { toFormData } from '@/utils/formData';

    const campaignStore = useCampaignStore();
    const campaign = campaignStore.selectedCampaign;

    const emit = defineEmits(['update'])

    const editDialog = ref(false)

    const defaultSession = {
        id: -1,
        title: '',
        content: '',
        externalLink: '',
        imageId: 0,
        authorId: null,
        image: null,
        date: null,
        number: null
    };

    const editSession = ref({ ...defaultSession });
    const editIndex = ref(-1)

    const edit = (index) => {
        editIndex.value = index
        editSession.value = { ...items.value[ index ] }
        editDialog.value = true;
    }

    const addNew = () => {
        editSession.value = { ...defaultSession }
        editIndex.value = -1
        editDialog.value = true;
    }

    const save = async () => {
        const formData = toFormData(editSession.value);

        try {
            if (editIndex.value !== -1) {
                const response = await axios.put(`/api/campaigns/${campaign.id}/sessions/${editSession.value.id}`, formData, {
                    headers: {
                        'Content-Type': 'multipart/form-data',
                    },
                });

                Object.assign(items.value[ editIndex.value ], response.data);
                
            } else {
                const response = await axios.post(`/api/campaigns/${campaign.id}/sessions`, formData, {
                    headers: {
                        'Content-Type': 'multipart/form-data',
                    },
                });

                items.value.unshift(response.data);

            }
            close();
        } catch (error) {
            console.error('Failed to save campaign item', error);
        }
    }


    const deleteItem = async () => {
        if (editSession.value.id) {
            try {
                await axios.delete(`/api/campaigns/${campaign.id}/sessions/${editSession.value.id}`);
                items.value.splice(editIndex.value, 1);
            } catch (error) {
                console.error('Failed to delete campaign', error)
            }
        }
        close();
    }

    const close = () => {
        emit('update')
        editDialog.value = false;
        nextTick(() => {
            setTimeout(() => {
                editIndex.value = -1;
                editSession.value = { ...defaultSession };
            }, 300);
        });
    };

    const items = ref([])
    const itemsLength = ref(0);
    const page = ref(1);
    const itemsPerPage = ref(10);
    const sortBy = ref([ { key: 'number', order: 'desc' } ]);
    const search = ref('');

    const pages = computed(() => Math.ceil(itemsLength.value / itemsPerPage.value) )

    const fetchSessions = async () => {
        try {
            const params = new URLSearchParams();
            params.append('page', page.value);
            params.append('itemsPerPage', itemsPerPage.value);
            sortBy.value.forEach((sortItem, index) => {
                params.append(`sortBy[${index}][key]`, sortItem.key);
                params.append(`sortBy[${index}][order]`, sortItem.order);
            });
            if (search.value) {
                params.append('search', search.value);
            }

            if (campaign && campaign.id) {
                const response = await axios.get(`/api/campaigns/${campaign.id}/sessions`, { params });
                itemsLength.value = response.data.itemsLength;
                items.value = response.data.items;
            } else {
                const response = await axios.get(`/api/sessions`, { params });
                itemsLength.value = response.data.itemsLength;
                items.value = response.data.items;
            }
        } catch (error) {
            console.error('Failed to fetch sessions:', error);
        }
    }

    onMounted(async () => {
        await fetchSessions()
    })

</script>

<style>
    
</style>