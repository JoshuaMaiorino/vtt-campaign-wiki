<template>
    <v-container>
        <v-row>
            <v-col>
                <h1>Edit Campaign</h1>
            </v-col>
        </v-row>
        <v-row>
            <v-col>
                <v-text-field v-model="campaign.title"
                              label="Name" />
            </v-col>
            <v-col></v-col>
        </v-row>
        <v-row>
            <v-col>
                <v-img :src="imageUrl" class="d-flex align-top justify-end" @click="triggerFileInput" v-bind="props" min-height="294">
                    <template #placeholder>

                        <v-sheet :color="isHovering ? 'grey-lighten-3' : 'grey-lighten-4'" v-bind="props"
                                 class="d-flex align-center justify-center fill-height border-b-sm"
                                 height="300">
                            <v-icon size="64" color="grey">mdi-image</v-icon>
                        </v-sheet>
                    </template>

                    <v-btn v-if="imageUrl" variant="text" class="ma-2" color="grey-lighten-4" @click.stop="clearImage" icon>
                        <v-icon>mdi-delete</v-icon>
                    </v-btn>

                </v-img>

                <v-file-input v-model="campaign.image" label="Upload Image" accept="image/*" @change="handleFileChange" prepend-icon="mdi-file-image" style="display:none;">
                </v-file-input>

            </v-col>

            <v-col>
                <v-sheet>
                    <QuillEditor placeholder="Content..."
                                 v-model:content="campaign.content"
                                 contentType="html"
                                 toolbar="minimal"></QuillEditor>
                </v-sheet>


                <v-toolbar density="compact"
                           title="Players"
                           class="mt-4">
                    <v-spacer></v-spacer>
                    <v-autocomplete v-if="searching"
                                    v-model="selectedPlayer"
                                    v-model:search="playerSearch"
                                    :items="players"
                                    item-title="userName"
                                    item-value="id"
                                    return-object
                                    label="Username" variant="solo"
                                    density="compact" class="mr-1"
                                    hide-details
                                    prepend-inner-icon="mdi-magnify"
                                    @update:modelValue="addPlayer"
                                    @update:search="loadPlayers" />
                    <v-btn size="small"
                           :icon="searching ? 'mdi-close' : 'mdi-plus'"
                           @click="searching = !searching" />
                </v-toolbar>
                <v-list v-if="campaign.players && campaign.players.length">
                    <v-list-item-group>
                        <template v-for="player in campaign.players" :key="player.id">
                            <v-list-item :title="player?.player?.userName">
                                <template v-slot:prepend>
                                    <v-tooltip>
                                        <template v-slot:activator="{ on, attrs }">
                                            <v-icon @click="player.isDM = !player.isDM"
                                                    v-bind="attrs"
                                                    v-on="on"
                                                    :color="player.isDM ? 'primary' : ''"
                                                    size="36">
                                                mdi-dice-d20
                                            </v-icon>
                                        </template>
                                        Is a Game Master for this Campaign
                                    </v-tooltip>
                                </template>
                                <template v-slot:append>
                                    <v-btn variant="plain" icon="mdi-delete" @click="removePlayer(player)"/>
                                </template>
                            </v-list-item>
                        </template>
                    </v-list-item-group>
                </v-list>

            </v-col>
        </v-row>

        <v-row>
            <v-col>
                <v-btn variant="text" color="primary" @click="save">Save</v-btn>
                <v-btn variant="text" @click="$router.go(-1)">Cancel</v-btn>
            </v-col>
        </v-row>
    </v-container>
    
</template>

<script setup>

    import { ref, computed } from 'vue'

    import { useAuthStore } from '@/stores/auth.js'
    import { useCampaignStore } from '@/stores/campaign.js'
    import axios from '@/utils/axios';
    import { toFormData } from '@/utils/formData';

    import { useRouter } from 'vue-router'

    const router = useRouter()

    const campaignStore = useCampaignStore();
    const campaign = campaignStore.selectedCampaign;

    const imagePreview = ref(null)

    const imageUrl = computed(() => {
        if (campaign.imageId && campaign.imageId !== 0) {
            return `/api/image/${campaign.imageId}`
        }
        if (imagePreview.value) {
            return imagePreview.value
        }
    })

    const tab = ref("content")

    const searching = ref(false)
    const playerSearch = ref(null)
    const selectedPlayer = ref(null)
    const players = ref([])

    const addPlayer = () => {

        const playerExists = campaign.players.some(player => player?.playerId === selectedPlayer.value.id);

        if( !playerExists )
        {
            campaign.players.push({
                playerId: selectedPlayer.value.id,
                player: { ...selectedPlayer.value },
                isDM: false,
                campaignId: campaign.id 
            })
        }
        
        playerSearch.value = null
        searching.value = false
        selectedPlayer.value = null
    }

    const removePlayer = (player) => {
        campaign.players = campaign.players.filter( p => p.playerId !== player.playerId )
    }

    function clearImage () {
        campaign.imageId = 0
        campaign.image = null
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
            campaign.image = file
            campaign.imageId = 0
        }
    }

    const loadPlayers = async () => {
        try {
            const params = new URLSearchParams();
            params.append('itemsPerPage', 10);
            params.append('search', playerSearch.value);
            
            
            const response = await axios.get('/api/players', params);
            players.value = response.data;
        } catch (error) {
            console.error('Failed to fetch campaigns', error);
        }
    }

    const save = async () => {
        let formData = toFormData(campaign);

        try {
            const response = await axios.put(`/api/campaigns/${campaign.id}`, formData, {
                headers: {
                    'Content-Type': 'multipart/form-data',
                },
            });
        
            campaignStore.selectedCampaign.value = response.data
            router.go(-1)
            }

            catch( err )

            {

                console.error(`Failed to save campaign: ${err}`)
            }



    }

</script>

<style>
    .ql-editor {
        min-height: 250px; /* Set your desired minimum height */
    }
</style>