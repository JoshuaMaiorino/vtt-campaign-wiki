<template>
    <template v-if="!level || level == 0">
        <v-img :src="hasImage ? `https://localhost:7128/image/${item.imageId}` : ''"
               cover
               max-height="320"
               color="surface-variant"
               class="mb-2">
            <v-container class="fill-height">
                <v-row>
                    <v-col class="d-flex flex-column">
                        <h1 :class="hasImage ? 'hero-title' : ''">{{ item?.title }}</h1>
                        <ParsedContent v-if="item?.content" :content="item.content" :itemId="item.id" :campaignItems="campaign?.items" />
                    </v-col>
                </v-row>
            </v-container>
        </v-img>

        <v-container v-if="hasChildren">
            <v-row>
                <v-col v-for="child in item?.children" :key="child.id" :cols="level === 0 ? 4 : 12">
                    <ItemContent :item="child" @selected="child" :level="level + 1" />
                </v-col>
            </v-row>
        </v-container>
    </template>

    <template v-if="level === 1">
        <v-card flat rounded="0">
            <v-img v-if="hasImage && level < 2"
                   color="surface-variant"
                   height="320"
                   :src="`https://localhost:7128/image/${item.imageId}`"
                   cover />
            <v-toolbar :density="level >= 2 ? 'compact' : 'default'" color="transparent" :title="item?.title">
            </v-toolbar>
            <v-card-text v-if="item.content">
                <ParsedContent v-if="item?.content" :content="item.content" :itemId="item.id" :campaignItems="campaign?.items" />
            </v-card-text>
        </v-card>
        <template v-if="hasChildren">
            <v-divider />
            <ItemContent v-for="child in item?.children" :key="child.id" :item="child" :level="level + 1" />
        </template>
    </template>

    <template v-if="level > 1">
        <v-list>
            <v-list-item>
                <template v-slot:prepend v-if="item?.imageId">
                    <v-avatar>
                        <v-img :src="`https://localhost:7128/image/${item.imageId}`"></v-img>
                    </v-avatar>
                </template>
                <v-list-item-title>{{ item?.title }}</v-list-item-title>
                <template v-if="item.content">
                    <ParsedContent v-if="item?.content" :content="item.content" :itemId="item.id" :campaignItems="campaign?.items" />
                </template>
            </v-list-item>
        </v-list>
    </template>
</template>

<script setup>
    import { computed } from 'vue'
    import { useAuthStore } from '@/stores/auth.js'
    import { useCampaignStore } from '@/stores/campaign.js'
    import ItemContent from '@/components/ItemContent.vue'
    import ParsedContent from '@/components/ParseContentV2.vue'

    const authStore = useAuthStore()
    const campaignStore = useCampaignStore()

    const campaign = campaignStore.selectedCampaign

    const props = defineProps({
        item: {
            type: Object,
            required: true
        },
        level: {
            type: Number,
            default: 0
        }
    })

    const hasImage = computed(() => props.item.imageId && props.item.imageId !== 0)
    const hasChildren = computed(() => props.item.children && props.item.children.length)
    const hasGrandChildren = computed(() => 
        props.item.children && props.item.children.some(child => child.children && children.length ))
    const hasChildrenWithImages = computed(() =>
        props.item.children && props.item.children.some(child => child.imageId && child.imageId !== 0)
    )

    const emit = defineEmits([ 'selected' ])

</script>

<style>
    ul {
        margin-left: 30px;
    }

    .v-container {
        max-width: 1280px;
    }
</style>
