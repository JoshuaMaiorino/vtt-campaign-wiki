import { createRouter, createWebHistory } from 'vue-router'
import CampaignListView from '@/views/CampaignListView.vue'
import { useCampaignStore } from '@/stores/campaign'

const routes = [
    {
        path: '/campaigns',
        name: 'Campaigns',
        component: CampaignListView,
    },
    {
        path: '/campaigns/:campaignId',
        name: 'CampaignDetail',
        component: () => import('@/views/CampaignDetailView.vue')
    },
    {
        path: '/login',
        name: 'Login',
        component: () => import('@/views/LoginView.vue')
    }
]

const router = createRouter({
    history: createWebHistory(),
    routes,
})

router.beforeEach(async (to, from, next) => {
    const campaignStore = useCampaignStore();
    const campaignId = to.params.campaignId;

    if (campaignId && (!campaignStore.selectedCampaign || campaignStore.selectedCampaign.id !== campaignId)) {
        await campaignStore.fetchCampaign(campaignId);
    }

    if (!campaignId && campaignStore.selectedCampaign) {
        campaignStore.clearSelectedCampaign();
    }
    next();
});

export default router
