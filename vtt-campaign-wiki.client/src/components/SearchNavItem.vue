<template>
    <v-text-field v-if="searchOpen"
                  v-model="searchQuery"
                  label="Search"
                  clearable
                  autofocus
                  @input="performSearch"
                  @blur="onBlur"
                  hide-details 
                  ref="searchField"/>

    <v-menu v-if="searchResults.length"
            v-model="menuOpen"
            :close-on-content-click="false"
            :activator="$refs.searchField"
            :max-width="$refs.searchField ? $refs.searchField.offsetWidth : 'auto'"
            offset-y>
        <v-list class="rounded-t-0" >
            <v-list-item v-for="item in searchResults" :key="item.id" @click="goToItem(item)">
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
    import { useCampaignStore } from '@/stores/campaign.js';
    import { useRouter } from 'vue-router'

    const campaignStore = useCampaignStore();
    const campaign = campaignStore.selectedCampaign;
    const router = useRouter();


    const searchOpen = defineModel('searchOpen', {
        default: false,
    });

    const menuOpen = ref(false);
    const searchQuery = ref('');

    const flattenItems = (items, topLevelParent = null) => {
        let result = [];

        items.forEach((item) => {
            // Add the current item with the top-level parent reference if it has content or images
            if (item.content || item.imageId) {
                result.push({
                    ...item,
                    topLevelParent, // Attach the top-level parent
                });
            }
            // Recursively flatten children
            if (item.children && item.children.length) {
                // If this is the first level, set this item as the top-level parent
                const parent = topLevelParent || item;
                result = result.concat(flattenItems(item.children, parent));
            }
        });

        return result;
    };

    const flattenedItems = computed(() => flattenItems(campaign.items));

    const searchResults = computed(() => {
        if (!searchQuery.value) return [];

        const query = searchQuery.value.toLowerCase();

        // First, prioritize title matches
        const titleMatches = flattenedItems.value.filter((item) =>
            item.title.toLowerCase().includes(query)
        );

        // Then, find content matches but exclude items already found by title
        const contentMatches = flattenedItems.value.filter(
            (item) =>
                item.content?.toLowerCase().includes(query) && !titleMatches.includes(item)
        );

        // Combine title matches first, then content matches
        return [ ...titleMatches, ...contentMatches ];
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
        const itemId = item.topLevelParent?.id ?? item.id
        router.push(`/campaigns/1/${itemId}#${item.id}`)
        menuOpen.value = false;
    };

    const onBlur = () => {
        setTimeout(() => {
            // Only close the search bar if the menu is not open
            if (!menuOpen.value) {
                toggleSearchBar();
            }
        }, 100);  // Delay to allow the click event to trigger first
    };
</script>

<style>
</style>