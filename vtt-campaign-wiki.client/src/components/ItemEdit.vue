<template>
    <v-dialog 
              v-model="dialog"
              max-width="500px">
        <v-card>
            <v-card-title>
                <span class="text-h5">{{ props.title }}</span>
            </v-card-title>

            <v-card-text>

                <v-text-field v-model="item.title"
                              label="Name"></v-text-field>

                <v-textarea v-model="item.content"
                            label="Content"></v-textarea>

                <v-text-field v-model="item.externalLink"
                              label="External Link"
                              pre-pendicon-inner="mdi-link"></v-text-field>

                <v-file-input v-model="item.image" 
                              label="Upload Image" 
                              accept="image/*"></v-file-input>

            </v-card-text>

            <v-card-actions>
                <v-spacer></v-spacer>
                <v-btn color="blue-darken-1"
                       variant="text"
                       @click="$emit('close')">
                    Cancel
                </v-btn>
                <v-btn color="blue-darken-1"
                       variant="text"
                       @click="$emit('save')">
                    Save
                </v-btn>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script setup>
    const item = defineModel('item')
    const dialog = defineModel('model')
    const props = defineProps([ 'title' ])

    const emit = defineEmits(['save','close'])
</script>

<style>
</style>