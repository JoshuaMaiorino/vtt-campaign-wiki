<template>
    <v-container>
        <v-row>
            <v-col v-for="campaign in campaigns" :key="campaign.id" cols="6">
                <ItemCard :item="campaign" @selected="editItem"/>
            </v-col>
        </v-row>

    </v-container>
    <v-fab icon="mdi-dots-horizontal"
           class="mb-12"
           location="bottom end"
           absolute
           app
           appear
           color="secondary"
           offset
           @click="addNew"></v-fab>
    <ItemEdit :title="formTitle" v-model="editDialog" v-model:item="selectedCampaign" @save="save" @close="close" />
</template>

<script setup>
    import { ref, nextTick, computed } from 'vue'

    import ItemCard from '@/components/ItemCard.vue'
    import ItemEdit from '@/components/ItemEdit.vue'

    const campaigns = ref([
        {
            id: 1,
            title: "Here's my Campaign",
            content: "<p>Here's some details</p><li>Here's a buttet Item</li><li>Here's another</li>",
            externalLink: "",
            imageId: 1,
            AuthorId: 1
        },
        {
            id: 2,
            title: "Here's Another Campaign",
            content: "<p>Here's other details</p>",
            externalLink: "",
            imageId:null,
            AuthorId: 1
        },
    ])

    // Edit Campaign Data & Methods
    const formTitle = computed(() => editIndex.value === -1 ? 'New Campaign' : 'Edit Campaign' )

    const defaultCampaign = {
        id: -1,
        title: '',
        content: '',
        externalLink: '',
        imageId: null,
        authorId: null
    }

    const selectedCampaign = ref({
        id: -1,
        title: '',
        content: '',
        externalLink: '',
        imageId: null,
        authorId: null
    })

    const editDialog = ref(false)
    const editIndex = ref(-1)

    const addNew = () => {
        editIndex.value = -1
        selectedCampaign.value = Object.assign({}, defaultCampaign)
        editDialog.value = true
    }
    const editItem = (item) => {
        editIndex.value = campaigns.value.indexOf(item)
        selectedCampaign.value = Object.assign({}, item)
        editDialog.value = true
    }
    const save = () => {
        if (editIndex.value > -1 ) {
            Object.assign(campaigns.value[ editIndex.value ], selectedCampaign.value)
        } else {
            campaigns.value.push( selectedCampaign.value )
        }
        close()
    }
    const close = () => {
        editDialog.value = false
        nextTick(() => {
            setTimeout(() => {
                editIndex.value = -1
                selectedCampaign.value = Object.assign({}, defaultCampaign)
            }, 300 )
        })  
    }

</script>

<style>
</style>