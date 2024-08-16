<template>
    <v-text-field v-if="searchOpen"
                  v-model="searchQuery"
                  label="Search"
                  clearable
                  autofocus
                  @input="performSearch"
                  hide-details 
                  ref="searchField"/>

    <v-menu v-if="searchResults.length"
            v-model="menuOpen"
            :close-on-content-click="false"
            :activator="$refs.searchField"
            offset-y>
        <v-list class="rounded-t-0" >
            <v-list-item v-for="item in searchResults" :key="item.id">
                <template v-slot:prepend v-if="item?.imageId">
                    <v-avatar>
                        <v-img :src="`/api/image/${item.imageId}`"></v-img>
                    </v-avatar>
                </template>
                    <v-list-item-title v-html="highlight(item.title)"></v-list-item-title>
                    <v-list-item-subtitle v-html="highlight(item.content)"></v-list-item-subtitle>
            </v-list-item>
        </v-list>
    </v-menu>

        <v-btn icon @click="toggleSearchBar">
            <v-icon>mdi-magnify</v-icon>
        </v-btn>
</template>

<script setup>
    import { ref, computed } from 'vue';
    import { useCampaignStore } from '@/stores/campaign.js'

    const campaignStore = useCampaignStore();
    const campaign = campaignStore.selectedCampaign

    const searchOpen = defineModel('searchOpen', {
        default:false
    })

    const menuOpen = ref(false);
    const searchQuery = ref('');

    const searchResults = computed(() => {
      if (!searchQuery.value) return [];
  
      const query = searchQuery.value.toLowerCase();
  
      // First, prioritize title matches
      const titleMatches = campaign.items.filter(item =>
        item.title.toLowerCase().includes(query)
      );
  
      // Then, find content matches but exclude items already found by title
      const contentMatches = campaign.items.filter(item =>
        item.content.toLowerCase().includes(query) &&
        !titleMatches.includes(item)
      );
  
      // Combine title matches first, then content matches
      return [...titleMatches, ...contentMatches];
    });

    const toggleSearchBar = () => {
      searchOpen.value = !searchOpen.value;
      if (!searchOpen.value) {
        searchQuery.value = '';
        menuOpen.value = false;
      }
    };

    const performSearch = () => {
      menuOpen.value = !!searchResults.value.length;
    };

    const highlight = (text) => {
      if (!searchQuery.value) return text;
  
      const query = searchQuery.value.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
      const regex = new RegExp(`(${query})`, 'gi');
      return text.replace(regex, '<mark>$1</mark>');
    };

    const goToItem = (item) => {
      // Logic to navigate to the item or handle the selection
      console.log('Navigating to item:', item);
      menuOpen.value = false;
    };
</script>

<style>
</style>