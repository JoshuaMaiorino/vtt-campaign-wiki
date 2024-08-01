import { createRouter, createWebHistory } from 'vue-router'
import { useCampaignStore } from '@/stores/campaign'

import HomeView from '@/views/HomeView.vue'


const routes = [
    {
        path: '/',
        name: 'home',
        component: HomeView
    },
    {
        path: '/campaigns',
        name: 'Campaigns',
        component: () => import('@/views/CampaignListView.vue'),
    },
    {
        path: '/campaigns/:campaignId',
        name: 'CampaignDetail',
        component: () => import('@/views/CampaignDetailView.vue')
    },
    {
        path: '/campaigns/:campaignId/:itemId',
        name: 'CampaignItemDetail',
        component: () => import('@/views/CampaignItemDetailView.vue')
    },
    {
        path: '/login',
        name: 'Login',
        component: () => import('@/views/LoginView.vue')
    },
    {
        path: '/sign-up',
        name: 'SignUp',
        component: () => import('@/views/SignUpView.vue')
    },
    {
        path: '/sessions',
        name: 'Sessions',
        component: () => import('@/views/SessionsListView.vue')
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
