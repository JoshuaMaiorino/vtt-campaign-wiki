import { defineStore } from 'pinia';
import axios from '@/utils/axios';
import { useAuthStore } from '@/stores/auth'

export const useCampaignStore = defineStore('campaign', {
    state: () => ({
        selectedCampaign: null,
        isDm: null
    }),
    getters: {
        getSelectedCampaign: (state) => state.selectedCampaign,
    },
    actions: {
        async fetchCampaign (campaignId) {
            try {
                const response = await axios.get(`/api/campaigns/${campaignId}`);
                this.selectedCampaign = response.data;

                // Log the players and userId for clarity
                console.log("Selected Campaign Players:", this.selectedCampaign.players);
                const { userId } = useAuthStore();
                console.log("Auth Store User ID:", userId);

                // Log the result of checking if the user is a DM
                this.isDm = this.selectedCampaign.players.some(p => {
                    console.log("Player ID:", p.playerId, "isDm:", p.isDm);
                    return p.isDM && p.playerId == userId;
                });

                console.log("Is DM:", this.isDm);
            } catch (error) {
                console.error('Failed to fetch campaign:', error);
            }
            try {
                const response = await axios.get(`/api/campaigns/${campaignId}/items`);
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