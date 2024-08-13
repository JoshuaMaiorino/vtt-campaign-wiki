<template>
    <v-toolbar :title="currentItem?.title">
        <v-btn icon="mdi-plus" @click="addNew()"></v-btn>
        <v-btn icon="mdi-pencil" @click="$emit('edit')" />
    </v-toolbar>
    <v-list class="ml-2">
        <Draggable ref="tree" class="mtl-tree" v-model="items" treeLine :defaultOpen="false" triggerClass="mdi-reorder-horizontal" @change="update">
            <template v-slot:default="{ node, stat }">
                <OpenIcon v-if="stat.children.length"
                          :open="stat.open"
                          class="mtl-mr"
                          @click.native="stat.open = !stat.open" />
                <v-list-item :title="node.title" class="flex-grow-1">
                    <template v-slot:prepend>
                        <v-icon>mdi-reorder-horizontal</v-icon>
                    </template>
                    <template v-slot:append>
                        <v-btn size="small" variant="plain" icon="mdi-plus" @click="addNew(stat)" />
                        <v-btn size="small" variant="plain" icon="mdi-pencil" @click="editItem(stat)" />
                        <!-- <v-btn :disabled="stat.children.length" size="small" variant="plain" icon="mdi-trash-can"/> -->
                    </template>
                </v-list-item>
            </template>
        </Draggable>
    </v-list>

    <ItemEdit :title="formTitle" v-model="editDialog" v-model:item="selectedCampaignItem" @save="save" @close="close" @delete="deleteItem" />

</template>
<script setup>
    import { ref, computed, nextTick } from 'vue'

    import { BaseTree, Draggable, pro, OpenIcon, dragContext } from '@he-tree/vue'
    import '@he-tree/vue/style/default.css'
    import '@he-tree/vue/style/material-design.css'

    import ItemEdit from '@/components/ItemEdit.vue';

    import { useAuthStore } from '@/stores/auth.js'
    import { useCampaignStore } from '@/stores/campaign.js'
    import axios from '@/utils/axios';
    import { toFormData } from '@/utils/formData';

    const items = defineModel('items')
    const currentItem = defineModel('currentItem')
    const emit = defineEmits(['edit']);

    const props = defineProps( {
        isCampaign: {
            type: Boolean,
            default: false
        }
    })

    const authStore = useAuthStore();
    const campaignStore = useCampaignStore();

    const campaign = campaignStore.selectedCampaign;

    const dragStart = (stat) => {

        console.log(stat)

    }

    const formTitle = computed(() => selectedCampaignItem.value.id === -1 ? 'New Item' : 'Edit Item');

    const defaultCampaignItem = {
        id: -1,
        title: '',
        content: '',
        externalLink: '',
        imageId: 0,
        authorId: 0,
        image: null,
        parentEntityId: null
    };

    const selectedCampaignItem = ref({ ...defaultCampaignItem });
    const selectedStat = ref(null)
    const editDialog = ref(false);

    const tree=ref(null)

    const addNew = (parentStat) => {
        selectedCampaignItem.value = { ...defaultCampaignItem };
        console.log("parentStat", parentStat)
        if (parentStat)
        {
            selectedCampaignItem.value.parentEntityId = parentStat.data.id
            selectedStat.value = parentStat
        } else {{
            selectedCampaignItem.value.parentEntityId = props.isCampaign ? 0 : currentItem.value.id
        }}
        
        editDialog.value = true;
    };

    const editItem = (stat) => {
        selectedStat.value = stat
        selectedCampaignItem.value = { ...stat.data };
        editDialog.value = true;
    };

    const save = async () => {
        if (selectedCampaignItem.value.imageId === null) selectedCampaignItem.value.imageId = 0


        let formData = toFormData(selectedCampaignItem.value);

        try {
            if (selectedCampaignItem.value.id !== -1) {
                const response = await axios.put(`/api/campaigns/${campaign.id}/items/${selectedCampaignItem.value.id}`, formData, {
                    headers: {
                        'Content-Type': 'multipart/form-data',
                    },
                });
                
                updateItemById(items.value, selectedCampaignItem.value.id, (item) => {
                    Object.assign(item, response.data);
                });

            } else {
                const response = await axios.post(`/api/campaigns/${campaign.id}/items`, formData, {
                    headers: {
                        'Content-Type': 'multipart/form-data',
                    },
                });

                console.log("SelectedStat", selectedStat, "SelectedItem", selectedCampaignItem.value)

                // Add new item to the local items array
                if (selectedStat) {
                    addItem(items.value, selectedCampaignItem.value.parentEntityId, (item) => {
                        item.children.push(response.data);
                    })
                    tree.value.add(response.data, selectedStat.value)
                } else {
                    items.value.push(response.data);
                    tree.value.add(response.data)
                }
            }
            close();
        } catch (error) {
            console.error('Failed to save campaign item', error);
        }
    }


    const deleteItem = async () => {
        if (selectedCampaignItem.value.id) {
            try {
                await axios.delete(`/api/campaigns/${campaign.id}/items/${selectedCampaignItem.value.id}`);
                tree.value.remove(selectedStat.value)
            } catch (error) {
                console.error('Failed to delete campaign', error)
            }
        }
        close();
    }

    const close = () => {
        editDialog.value = false;
        nextTick(() => {
            setTimeout(() => {
                selectedStat.value = null;
                selectedCampaignItem.value = { ...defaultCampaignItem };
            }, 300);
        });
    };

    const update = async () => {
        

        console.log( dragContext.targetInfo.dragNode)
        
        const itemId = dragContext.targetInfo.dragNode.data.id
        const newParentId = dragContext.targetInfo.parent?.data?.id
        const newIndex = dragContext.targetInfo.indexBeforeDrop
        const currentParentId = dragContext.targetInfo.dragNode.data.parentEntityId
        const currentIndex = dragContext.startInfo.indexBeforeDrop
        const siblings = dragContext.targetInfo.siblings

        if( newParentId !== currentParentId || newIndex !== currentIndex ){

            const req =  {
                itemId,
                campaignId: campaign.id,
                parentId: newParentId || null,
                priorPosition: siblings[newIndex - 1]?.data?.position || null,
                nextPosition: siblings[newIndex + 1]?.data?.position || null
            }



             try {
                response = await axios.post(`/api/campaigns/${campaign.id}/items/${itemId}/position`, req )

                updateItemById(items.value, itemId, (item) => {
                    Object.assign(item, response.data);
                });
                
             } catch( error ){
                     console.error('Failed to update campaign item', error)
             }

        }
    }

    const updateItemById = (array, id, updateFn) => {
        for (const item of array) {
            if (item.id === id) {
                updateFn(item);
                return true;
            }
            if (item.children && item.children.length > 0) {
                if (updateItemById(item.children, id, updateFn)) {
                    return true;
                }
            }
        }
        return false;
    };

    const addItem = (array, parentId, updateFn) => {
        for (const item of array) {
            if (item.parentEntityId === parentId) {
                updateFn(item);
                return true;
            }
            if (item.children && item.children.length > 0) {
                if (addItem(item.children, parentId, updateFn)) {
                    return true;
                }
            }
        }
        return false;
    }

</script>