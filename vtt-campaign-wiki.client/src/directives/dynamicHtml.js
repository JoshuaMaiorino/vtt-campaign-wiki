import { h, render } from 'vue';

export default {
    beforeMount (el, binding) {
        const campaignItemMap = binding.value.campaignItemMap;
        const content = binding.value.content;

        const parser = new DOMParser();
        const doc = parser.parseFromString(content, 'text/html');

        const escapeRegExp = (string) => {
            return string.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
        };

        const replaceTextNodes = (node) => {
            if (node.nodeType === Node.TEXT_NODE) {
                let textContent = node.textContent;

                Object.values(campaignItemMap).forEach(item => {
                    const escapedTitle = escapeRegExp(item.title.toLowerCase());
                    const regex = new RegExp(`\\b${escapedTitle}\\b`, 'gi');

                    textContent = textContent.replace(regex, (match) => {
                        const span = document.createElement('span');
                        span.className = 'reference-item';
                        span.textContent = match;

                        const tooltip = h('v-tooltip', { bottom: true }, {
                            activator: ({ on, attrs }) => h('span', { class: 'reference-item', ...attrs, ...on }, match),
                            default: () => h('div', [
                                h('strong', item.title),
                                h('br'),
                                h('span', 'Additional details can go here.')
                            ])
                        });

                        render(tooltip, span);

                        return span.outerHTML;
                    });
                });

                const tempDiv = document.createElement('div');
                tempDiv.innerHTML = textContent;
                while (tempDiv.firstChild) {
                    node.parentNode.insertBefore(tempDiv.firstChild, node);
                }
                node.remove();
            } else {
                node.childNodes.forEach(child => replaceTextNodes(child));
            }
        };

        doc.body.childNodes.forEach(node => replaceTextNodes(node));

        el.innerHTML = doc.body.innerHTML;
    }
};
