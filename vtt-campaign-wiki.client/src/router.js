import { createRouter, createWebHistory } from 'vue-router'
import CampaignListView from '@/views/CampaignListView.vue'
import AboutView from '@/views/AboutView.vue'

const routes = [
    {
        path: '/',
        name: 'Campaigns',
        component: CampaignListView,
    },
    {
        path: '/about',
        name: 'About',
        component: AboutView,
    },
]

const router = createRouter({
    history: createWebHistory(),
    routes,
})

export default router
