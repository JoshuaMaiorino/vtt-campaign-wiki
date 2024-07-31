<template>
    <div v-if="DynamicComponent">
        <component :is="DynamicComponent"></component>
    </div>
</template>

<script setup>
    import { ref, onMounted, defineComponent, h, computed } from 'vue';
    import { compile } from '@vue/compiler-dom';

    const props = defineProps({
        content: {
            type: String,
            required: true
        },
        itemId: {
            type: Number,
            required: true
        },
        campaignItems: {
            type: Array,
            required: true
        }
    });

    const DynamicComponent = ref(null);

    // Function to escape special characters in strings for use in regular expressions
    const escapeRegExp = (string) => {
        return string.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
    };

    // Function to flatten nested items into a single array
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

    // Function to build the template string
    const buildTemplateString = (parts) => {
        return `
    <div>
      ${parts.map(part => `
        <span ${part.isTooltip ? 'v-tooltip="{ content: `' + part.tooltipContent + '` }"' : ''}>
          ${part.isTooltip
                ? `<span class="reference-item">${part.text}</span>
              ${part.tooltip.imageId ? `<v-img height="200" cover class="my-2" src="https://localhost:7128/image/${part.tooltip.imageId}" />` : ''}
              <h2>${part.tooltip.title}</h2>
              <div>${part.tooltip.content}</div>`
                : part.text}
        </span>`).join('')}
    </div>
  `;
    };

    // Function to compile the template string into a render function
    const compileTemplate = (template) => {
        const { render } = compile(template);
        return render;
    };

    // Function to create a dynamic component
    const createDynamicComponent = (template) => {
        const render = compileTemplate(template);
        return defineComponent({ render });
    };

    // Function to process content and generate parts for rendering
    const processContent = (content, campaignItemMap) => {
        let parts = [];
        let lastIndex = 0;
        const matches = [];

        Object.values(campaignItemMap).forEach(item => {
            const escapedTitle = escapeRegExp(item.title.toLowerCase());
            const regex = new RegExp(`\\b${escapedTitle}\\b`, 'gi');

            let match;
            while ((match = regex.exec(content)) !== null) {
                matches.push({
                    text: match[ 0 ],
                    index: match.index,
                    isTooltip: true,
                    tooltip: item
                });
            }
        });

        matches.sort((a, b) => a.index - b.index);

        matches.forEach(match => {
            if (match.index > lastIndex) {
                parts.push({ text: content.substring(lastIndex, match.index), isTooltip: false });
            }
            parts.push({ text: match.text, isTooltip: true, tooltip: match.tooltip });
            lastIndex = match.index + match.text.length;
        });

        if (lastIndex < content.length) {
            parts.push({ text: content.substring(lastIndex), isTooltip: false });
        }

        return parts;
    };

    // Compute the campaign item map
    const campaignItemMap = computed(() => {
        const flattenedItems = flattenItems(props.campaignItems);
        const sortedItems = flattenedItems
            .filter(item => item.id !== props.itemId)
            .sort((a, b) => b.title.length - a.title.length);
        return sortedItems.reduce((map, item) => {
            map[ item.title.toLowerCase() ] = item;
            return map;
        }, {});
    });

    // Mount the component and set the dynamic component
    onMounted(() => {
        const parts = processContent(props.content, campaignItemMap.value);
        const template = buildTemplateString(parts);
        DynamicComponent.value = createDynamicComponent(template);
    });
</script>

<style scoped>
    .reference-item {
        color: #007bff;
        cursor: pointer;
        text-decoration: underline;
    }

        .reference-item:hover {
            color: #0056b3;
        }
</style>
