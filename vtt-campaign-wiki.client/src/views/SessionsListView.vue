<template>
    <div class="campaign-detail-view">
        <div class="hero-section">
            <v-parallax :src="heroImageUrl"
                        gradient="to top right, rgba(100,115,201,.33), rgba(25,32,72,.7)"
                        cover max-height="320"
                        color="surface-variant">
                <v-container class="fill-height" fluid>
                    <v-row class="fill-height">
                        <v-col class="d-flex align-center justify-center">
                            <h1 class="hero-title">Sessions</h1>
                        </v-col>
                    </v-row>
                </v-container>
            </v-parallax>
        </div>
    </div>

    <v-container>
        <v-row>
            <v-col v-for="(item, index) in sessions"
                   :key="item.id"
                   cols="12">
                <v-card elevation="0" rounded="0">
                    <v-card-title class="d-flex align-baseline">Session {{ item?.number}}: {{ item?.title }}<v-chip size="small" variant="text" class="ml-2">{{ formatDate(item.date) }}</v-chip></v-card-title>
                    <v-card-text v-if="item?.content">
                        <v-row>
                            <v-col v-if="item?.imageId" :cols="12" :order="0">
                                <v-img :src="`/api/image/${item?.imageId}`" height="400" style="object-position:bottom left"/>
                            </v-col>
                            <v-col>
                                <ParsedContent :content="item?.content" :campaignItems="campaign?.items" :itemId="0" />
                            </v-col>
                        </v-row>
                    </v-card-text>
                </v-card>
            </v-col>
        </v-row>
        <v-row v-if="pages > 1">
            <v-col>
                <v-pagination density="compact"
                              :length="pages > 5 ? 5 : pages"
                              :show-first-last-page="pages > 5"
                              v-model="page"
                              
                              @update:modelValue="fetchSessions"></v-pagination>
            </v-col>
        </v-row>
    </v-container>

    <v-fab v-if="campaign.authorId === userId" position="static" icon="mdi-dots-horizontal" class="mb-6" location="bottom end" app appear color="primary" offset @click="sidePanel = !sidePanel"></v-fab>

    <v-navigation-drawer temporary :model-value="sidePanel" location="right" width="450">
        <SessionsItemList @update="fetchSessions" />
    </v-navigation-drawer>
</template>

<script setup>

    import { ref, computed, onMounted } from 'vue';
    import { useCampaignStore } from '@/stores/campaign';
    import { useAuthStore } from '@/stores/auth';
    import axios from '@/utils/axios';
    

    import ParsedContent from '@/components/ParseContentV2.vue'
    import SessionsItemList from '@/components/SessionsItemList.vue'

    const campaignStore = useCampaignStore();
    const campaign = campaignStore.selectedCampaign;

    const authStore = useAuthStore();
    const userId = authStore.userId;

    const sidePanel = ref(false)

    const sessions = ref([]);
    const sessionsLength = ref(0);
    const page = ref(1);
    const itemsPerPage = ref(3);
    const sortBy = ref([ { key: 'number', order: 'desc' } ]);
    const search = ref('');

    const pages = computed(() => Math.ceil( sessionsLength.value / itemsPerPage.value ) )

    const formatDate = (date) => {
        if (date) {
            return new Date(date).toLocaleDateString('en-us')
        }
    }

    const heroImageUrl = computed(() => {
        if (campaign && campaign.id) {
            return `/api/campaigns/${campaign.id}/sessions/hero-image`
        }
        return `/api/sessions/hero-image`
    })

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
                sessionsLength.value = response.data.itemsLength;
                sessions.value = response.data.items;
            } else {
                const response = await axios.get(`/api/sessions`, { params });
                sessionsLength.value = response.data.itemsLength;
                sessions.value = response.data.items;
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
    .v-img__img--contain {
        object-position: left;
    }
</style>