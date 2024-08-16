<template>
    <div>
        <div v-html="htmlWithPlaceholders" v-dynamic-component="{ phrases, components, items, placeHolders }"></div>
    </div>
</template>

<script>
    import { ref, computed, defineComponent, h, resolveComponent, createApp, markRaw, nextTick } from 'vue';
    import vuetify from '@/plugins/vuetify'
    import router from '@/router' 

// Define your custom component
const CustomComponent = markRaw(defineComponent({
  props: ['item'],
  template: `
    <v-tooltip  max-width="600" class="pa-0" v-model="tooltipVisible">
      <template v-slot:activator="{ props }">
        <span @click="navigate" class="text-primary" v-bind="props">{{ item.title }}</span>
      </template>
      <v-card elevation="8">
      <v-toolbar color="surface-variant" density="compact" :title="item.title" class="mb-0" />
      <v-img v-if="item.imageId" max-height="280" cover :src="\`/api/image/\${item.imageId}\`" class="mt-0"></v-img>
      <v-card-text v-if="item.content" v-html="item.content"></v-card-text>
      </v-card>
    </v-tooltip>
  `,
    data () {
        return {
            tooltipVisible: false,
        }
    },
    methods: {
        async navigate () {
            this.tooltipVisible = false
            await nextTick()
            this.$router.push(`/campaigns/${this.item.campaignId}/${this.item.topLevelParent?.id ?? this.item.id}#${this.item.id}`) 
        }
    }
}));

    const flattenItems = (items, topLevelParent = null) => {
        let flatList = [];

        const recurse = (itemList, parent = topLevelParent) => {
            if (itemList) {
                itemList.forEach(item => {
                    // Add the current item to the list with the top-level parent reference
                    flatList.push({
                        ...item,
                        topLevelParent: parent, // Attach the top-level parent
                    });

                    // Recursively process the children
                    if (item.children && item.children.length) {
                        const currentParent = parent || item; // Set the current item as the top-level parent if none exists
                        recurse(item.children, currentParent);
                    }
                });
            }
        };

        recurse(items);
        return flatList;
    };

const removeInlineStyles = (html) => {
        // Parse the HTML string into a document
        const parser = new DOMParser();
        const doc = parser.parseFromString(html, 'text/html');

        // Remove all inline styles
        const elements = doc.querySelectorAll('[style]');
        elements.forEach(element => {
            element.removeAttribute('style');
        });

        // Serialize the document back to a string
        return doc.body.innerHTML;
    }

export default {
    name: 'DynamicHtmlRenderer',
    components: {
        CustomComponent
    },
    props: ['content', 'itemId', 'campaignItems'],
    setup (props) {
        const items = computed(() => {
            return flattenItems(props.campaignItems)
                .filter(item => item.id !== props.itemId && (item.content || item.imageId));
        });

        const phrases = computed(() => items.value.map(item => item.title));

        const components = ref([{ name: 'CustomComponent', component: CustomComponent }]);

        const placeHolders = ref([]);
        let uniqueId = 0;
        const replacePhrasesWithPlaceholders = (html, phrases) => {
          phrases.forEach((phrase, index) => {
              const regex = new RegExp(`\\b${phrase}\\b`, 'gi');
              html = html.replace(regex, () => {
                  const id = `component-placeholder-${uniqueId}`;
                  placeHolders.value.push({ id, index });
                  uniqueId++;
                  return `<span class="component-placeholder" id="component-placeholder-${id}"></span>`;
              });
          });
            return html;
        };

        const htmlWithPlaceholders = computed(() => {
          return replacePhrasesWithPlaceholders(props.content, phrases.value);
        });

        return {
            htmlWithPlaceholders,
            phrases,
            components,
            placeHolders,
            items,
        };
    },
    directives: {
        dynamicComponent: {
            mounted (el, binding) {
                const { phrases, components, items, placeHolders } = binding.value;

                const componentsToRender = placeHolders.map((ph, index) => ({
                    type: components.find(comp => comp.name === 'CustomComponent').component,
                    id: `component-placeholder-${ph.id}`,
                    item: items[ ph.index ] // Assuming items are in the same order as phrases
                }));

                componentsToRender.forEach(component => {
                    const placeholder = el.querySelector(`#${component.id}`);
                    if (placeholder) {
                        try {
                            
                            const app = createApp({
                                render () {
                                    return h(component.type, { item: component.item });
                                }
                            });
                            app.use(vuetify); // Ensure Vuetify is used by the app
                            app.use(router);
                            app.mount(placeholder);
                        } catch (error) {
                            console.error(`Error mounting component: ${error.message}`, component);
                        }
                    } else {
                        console.warn(`Placeholder not found for component ID: ${component.id}`);
                    }
                });
            }
        }
    }
}
</script>

<style>
    .component-placeholder {
        display: inline-block;
    }

    .v-overlay__content {
        --v-theme-surface-variant: 255, 255, 255, 0;
        --v-theme-on-surface-variant: 0, 0, 0, 0;
        border: none;
    }
</style>
