import { defineStore } from 'pinia';
import axios from '@/utils/axios';

export const useCampaignStore = defineStore('campaign', {
    state: () => ({
        selectedCampaign: null,
    }),
    getters: {
        getSelectedCampaign: (state) => state.selectedCampaign,
    },
    actions: {
        async fetchCampaign (campaignId) {
            try {
                const response = await axios.get(`/campaigns/${campaignId}`);
                this.selectedCampaign = response.data;
            } catch (error) {
                console.error('Failed to fetch campaign:', error);
            }
            try {
                const response = await axios.get(`/campaigns/${campaignId}/items`);
                console.log(response)
                this.selectedCampaign.items = response.data;
            } catch (error) {
                console.error('Failed to fetch campaign items:', error);
            }
        },
        setSelectedCampaign (campaign) {
            this.selectedCampaign = campaign;
        },
        clearSelectedCampaign () {
            this.selectedCampaign = null;
        },
    },
});