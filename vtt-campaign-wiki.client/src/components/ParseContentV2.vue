<template>
    <div>
        <div v-html="htmlWithPlaceholders" v-dynamic-component="{ phrases, components, items }"></div>
    </div>
</template>

<script>
    import { ref, computed, defineComponent, h, resolveComponent, createApp, markRaw } from 'vue';
    import vuetify from '@/plugins/vuetify'

    // Define your custom component
    const CustomComponent = markRaw(defineComponent({
      props: ['item'],
      template: `
        <v-tooltip :text="item.title" max-width="600" theme="dark">
          <template v-slot:activator="{ props }">
            <v-chip class="pa-0" color="primary" variant="text" v-bind="props">{{ item.title }}</span>
          </template>
          <v-img v-if="item.imageId" max-height="280" class="my-2" cover :src="\`/api/image/\${item.imageId}\`"></v-img>
          <h4>{{ item.title }}</h4>
          <div v-if="item.content" v-html="item.content"></div>
        </v-tooltip>
      `,
    }));

    const flattenItems = (items) => {
        let flatList = [];
        const recurse = (itemList) => {
            if (itemList) {
                itemList.forEach(item => {
                    flatList.push(item);
                    if (item.children && item.children.length) {
                        recurse(item.children);
                    }
                });
            }
        };
        recurse(items);
        return flatList;
    };

    export default {
        name: 'DynamicHtmlRenderer',
        components: {
            CustomComponent
        },
        props: ['content', 'itemId', 'campaignItems'],
        setup (props) {
            const htmlString = ref('<p>This is a test string with a phrase to match.</p>');
            const items = computed(() => {
                return flattenItems(props.campaignItems)
                    .filter(item => item.id !== props.itemId && (item.content || item.imageId ) );
            })

            const phrases = computed(() => items.value.map(item => item.title));

            const components = ref([{ name: 'CustomComponent', component: CustomComponent }]);

            const replacePhrasesWithPlaceholders = (html, phrases) => {
              phrases.forEach((phrase, index) => {
                const regex = new RegExp(phrase, 'gi');
                html = html.replace(regex, `<span class="component-placeholder" id="component-placeholder-${index}"></span>`);
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
                items
            };
            },
        directives: {
            dynamicComponent: {
                mounted (el, binding) {
                    const { phrases, components, items } = binding.value;
                    const componentsToRender = phrases.map((phrase, index) => ({
                        type: components.find(comp => comp.name === 'CustomComponent').component,
                        id: `component-placeholder-${index}`,
                        item: items[ index ] // Assuming items are in the same order as phrases
                    }));

                    componentsToRender.forEach(component => {
                        const placeholder = el.querySelector(`#${component.id}`);
                        if (placeholder) {
                            const app = createApp({
                                render () {
                                    return h(component.type, { item: component.item });
                                }
                            });
                            app.use(vuetify); // Ensure Vuetify is used by the app
                            app.mount(placeholder);
                        }
                    });
                }
            }
        }
    }

</script>

<style scoped>
    .component-placeholder {
        display: inline-block;
    }
</style>
