<template>
    <v-card>
        <v-toolbar color="transparent" :title="item?.title">
        </v-toolbar>
        <v-img v-if="item?.imageId" color="surface-variant"
               height="200"
               :src="`https://localhost:7128/image/${item.imageId}`"
               cover />

        <v-card-text>
            <div v-html="item?.content"></div>
        </v-card-text>
        <v-card-actions>
            <v-btn color="primary" size="small">Read More</v-btn>
            <v-spacer></v-spacer>
            <v-btn size="small" icon="mdi-dots-horizontal" @click.stop="$emit('selected',item)" />
        </v-card-actions>
    </v-card>
</template>

<script setup>
    const props = defineProps({
        item: Object
    })

    const emit = defineEmits([ 'selected' ])
</script>

<style>
</style>