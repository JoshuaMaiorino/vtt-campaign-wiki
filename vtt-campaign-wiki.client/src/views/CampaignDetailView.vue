<template>
  <div class="campaign-detail-view">
    <div class="hero-section">
      <v-img :src="`https://localhost:7128/image/${campaign.imageId}`" cover max-height="320">
        <v-container class="fill-height" fluid>
          <v-row class="fill-height">
            <v-col class="d-flex align-center justify-center">
              <h1 class="hero-title">{{ campaign.title }}</h1>
            </v-col>
          </v-row>
        </v-container>
      </v-img>
    </div>
    <v-container>
      <v-row>
        <v-col
          v-for="item in campaign.items"
          :key="item.id"
          cols="12"
          md="6"
          lg="4"
        >
          <ItemCard :item="item" />
        </v-col>
      </v-row>
    </v-container>
  </div>
</template>

<script setup>
    import { ref, onMounted } from 'vue';
    import { useCampaignStore } from '@/stores/campaign';

    import ItemCard from '@/components/ItemCard.vue';

    const campaignStore = useCampaignStore();
    const campaign = campaignStore.getSelectedCampaign;

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
