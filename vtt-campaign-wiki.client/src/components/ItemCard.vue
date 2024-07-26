<template>
    <v-card>
        <v-toolbar color="transparent" :title="item?.title">
        </v-toolbar>
        <v-img v-if="item?.imageId" color="surface-variant"
               height="200"
               :src="`https://picsum.photos/400/600?random=${item.imageId}`"
               cover />

        <v-card-text>
            <div v-html="item?.content"></div>
        </v-card-text>
        <v-card-actions>
            <v-btn color="primary" size="small">Read More</v-btn>
            <v-spacer></v-spacer>
            <v-btn size="small" icon="mdi-dots-horizontal" @click="$emit('selected',item)" />
        </v-card-actions>
    </v-card>
</template>

<script setup>
    const props = defineProps({
        item: Object
    })
</script>

<style>
</style>