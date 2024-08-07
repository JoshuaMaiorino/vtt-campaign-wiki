<template>
    <v-sheet color="surface-light">
        <v-container>
            <v-row>
                <v-col class="d-flex flex-column">
                    <h1>{{ item?.title }}</h1>
                </v-col>
            </v-row>
        </v-container>
    </v-sheet>

    <v-container class="my-4">
        <v-row v-if="item?.imageId || item?.content" class="mb-4">
            <v-col v-if="item?.imageId"
                 :cols="item?.content ? 4 : 12">
                <v-img :src="`/api/image/${item.imageId}`" cover max-width="800"/>
            </v-col>
            <v-col v-if="item?.content">
                <ParsedContent :content="item.content" :itemId="item.id" :campaignItems="campaign?.items" />
            </v-col>
        </v-row>

        <v-row v-if="item?.children && item?.children.length">
            <v-col v-for="child in item?.children" :key="child.id" :cols="cols">
                <ItemContent :item="child" :level="1" />
            </v-col>
        </v-row>

    </v-container>
    
</template>

<script setup>
    import { computed } from 'vue'
    import { useCampaignStore } from '@/stores/campaign.js'
    import ItemContent from '@/components/ItemContent.vue'
    import ParsedContent from '@/components/ParseContentV2.vue'

    const campaignStore = useCampaignStore()
    const campaign = campaignStore.selectedCampaign

    const props = defineProps([ 'item' ])

    const cols = computed(() => {
        if (props.item?.children?.length < 2) {
            return 12
        }
        if (hasChildrenWithImages.value && avgContentLength.value < 1000) {
            return 4
        }
        if (!hasChildrenWithImages.value && avgContentLength.value < 600) {
            return 4
        }

        return 12
    })

    const hasChildrenWithImages = computed(() =>
        props.item.children && props.item.children.some(child => child.imageId && child.imageId !== 0)
    )

    const avgContentLength = computed(() => {
        if (props.item?.children)
        {
            const lengths = (props.item?.children ?? [ { content: '' } ]).map(child => {
                let content = child?.content || '';
                content = stripHtml(content);
                return content.length;
            });
            const totalLength = lengths.reduce((sum, length) => sum + length, 0);
            return totalLength / props.item?.children?.length;
        }
        return 0;
    })

    const stripHtml = (html) => {
        const div = document.createElement("div");
        div.innerHTML = html;
        return div.textContent || div.innerText || "";
    }
</script>