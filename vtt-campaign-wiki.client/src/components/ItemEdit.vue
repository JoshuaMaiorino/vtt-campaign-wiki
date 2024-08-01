<template>
    <v-dialog v-model="dialogDelete" max-width="500px">
        <v-card>
            <v-card-title class="text-h5">Are you sure you want to delete this item?</v-card-title>
            <v-card-actions>
                <v-spacer></v-spacer>
                <v-btn color="primary" variant="text" @click="dialogDelete = false">Cancel</v-btn>
                <v-btn color="primary" variant="text" @click="{ $emit('delete'); dialogDelete = false; }">OK</v-btn>
                <v-spacer></v-spacer>
            </v-card-actions>
        </v-card>
    </v-dialog>

    <v-dialog v-model="model"
              max-width="700px">
        <v-card>
            <v-card-title>
                <span class="text-h5">{{ title }}</span>
            </v-card-title>

            <v-card-text>

                <v-img v-if="imageUrl" :src="imageUrl" class="mb-4" @click="triggerFileInput">
                    <template #placeholder>
                        <v-skeleton-loader type="image" />
                    </template>
                </v-img>

                <!-- Upload Image Input -->
                <v-file-input v-model="item.image" label="Upload Image" accept="image/*" @change="handleFileChange" prepend-icon="mdi-file-image">
                    <template #append>
                        <v-btn variant="plain" @click="clearImage" icon>
                            <v-icon>mdi-delete</v-icon>
                        </v-btn>
                    </template>
                </v-file-input>

                <v-text-field v-model="item.title"
                              label="Name"></v-text-field>

                <v-label>Content</v-label>
                <QuillEditor placeholder="Content..."
                             v-model:content="item.content"
                             contentType="html"
                             toolbar="minimal"
                             
                             ></QuillEditor>


                <v-text-field v-model="item.externalLink"
                              label="External Link"
                              pre-pendicon-inner="mdi-link"
                              class="mt-4"></v-text-field>

            </v-card-text>

            <v-card-actions>
                <v-btn v-if="item.id > 0"
                       variant="text"
                       @click="dialogDelete = true">
                    Delete
                </v-btn>
                <v-spacer></v-spacer>
                <v-btn color="primary"
                       variant="text"
                       @click="closeDialog">
                    Cancel
                </v-btn>
                <v-btn color="primary"
                       variant="text"
                       @click="saveItem">
                    Save
                </v-btn>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script setup>
    import { ref, computed } from 'vue'
    import { defineProps, defineEmits, defineModel } from 'vue'

    const props = defineProps({
        title: {
            type: String,
            required: true
        }
    })

    const model = defineModel()
    const item = defineModel('item')

    const emit = defineEmits([ 'save', 'close', 'delete' ])

    const dialogDelete = ref(false)

    const imagePreview = ref(null)

    const imageUrl = computed(() => {
        if (item.value.imageId && item.value.imageId !== 0) {
            return `/api/image/${item.value.imageId}`
        }
        if (imagePreview.value) {
            return imagePreview.value
        }
    } )

    function closeDialog () {
        emit('close')
    }

    function saveItem () {
        emit('save', item.value)
    }

    function clearImage () {
        item.value.imageId = 0
        item.value.image = null
        imagePreview.value = null
    }

    function triggerFileInput () {
        const fileInput = document.querySelector('input[type="file"]')
        if (fileInput) fileInput.click()
    }

    function handleFileChange (event) {
        const file = event.target.files[ 0 ]
        if (file) {
            // Generate a preview URL for the image
            imagePreview.value = URL.createObjectURL(file)
            item.value.image = file
            item.value.imageId = 0
        }
    }
</script>

<style>
    .current-image {
        cursor: pointer;
        max-width: 100%;
        height: auto;
    }
</style>
