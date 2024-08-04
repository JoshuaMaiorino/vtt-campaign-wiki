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
                
                <v-img :src="imageUrl" class="d-flex align-top justify-end mb-4" @click="triggerFileInput" min-height="300">
                    <template #placeholder>
                        <v-sheet color="surface-light" class="d-flex align-center justify-center fill-height">
                            <v-icon size="64" color="grey">mdi-image</v-icon>
                        </v-sheet>
                    </template>


                    <v-btn v-if="imageUrl" variant="text" class="ma-2" color="grey-lighten-4" @click.stop="clearImage" icon>
                        <v-icon>mdi-delete</v-icon>
                    </v-btn>
                </v-img>

                <v-file-input v-model="item.image" label="Upload Image" accept="image/*" @change="handleFileChange" prepend-icon="mdi-file-image" style="display:none;">
                </v-file-input>

                <v-row>
                    <v-col>
                        <v-text-field v-if="item.hasOwnProperty('number')" label="Session Number" type="number" v-model="item.number" />
                    </v-col>
                    <v-col>
                        <v-text-field v-if="item.hasOwnProperty('date')" label="Date" type="date" v-model="item.date" />
                    </v-col>
                </v-row>

                <v-text-field v-model="item.title"
                              label="Name" />

                <v-label>Content</v-label>
                <QuillEditor placeholder="Content..."
                             v-model:content="item.content"
                             contentType="html"
                             toolbar="minimal"></QuillEditor>


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
        imagePreview.value = null
    }

    function saveItem () {
        emit('save', item.value)
        imagePreview.value = null
    }

    function clearImage () {
        item.value.imageId = null
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

    .ql-editor {
        min-height: 300px; /* Set your desired minimum height */
    }

</style>
