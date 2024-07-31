<template>
    <v-card flat rounded="0" >
        <v-img v-if="item?.imageId" color="surface-variant"
               height="200"
               :src="`https://localhost:7128/image/${item.imageId}`"
               cover />
        <v-toolbar color="transparent" :title="item?.title">
            <template v-if="item.authorId === authStore.userId" v-slot="append">
                <v-btn size="small" icon="mdi-dots-horizontal" @click.stop="$emit('selected',item)" />
            </template>
        </v-toolbar>
        <v-card-text>
            <div v-html="item?.content"></div>
        </v-card-text>
    </v-card>
</template>

<script setup>
    const props = defineProps({
        item: Object
    })

    const emit = defineEmits([ 'selected' ])

    import { useAuthStore } from '@/stores/auth.js'

    const authStore = useAuthStore();

</script>

<style>
</style>