import { createApp } from 'vue'
import App from './App.vue'
import vuetify from './plugins/vuetify'
import router from './router'
import { createPinia } from 'pinia'

import 'vuetify/styles'
import '@mdi/font/css/materialdesignicons.css'
import '@fortawesome/fontawesome-free/css/all.css'

const pinia = createPinia()
const app = createApp(App)

app.use(pinia)
app.use(vuetify)
app.use(router)

app.mount('#app')
