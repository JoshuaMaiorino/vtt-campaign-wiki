<template>
    <v-container>
        <v-form @submit.prevent="login">
            <v-alert v-if="message" :text="message" :type="type" class="mb-4"/>
            <v-text-field v-model="username" label="Username" required></v-text-field>
            <v-text-field v-model="password" label="Password" type="password" required></v-text-field>
            <v-btn type="submit" color="primary">Login</v-btn>
        </v-form>
    </v-container>
</template>

<script setup>
import { ref } from 'vue';
import { useAuthStore } from '@/stores/auth';

import { useRouter } from 'vue-router'
import axios from '@/utils/axios';

const router = useRouter();

const authStore = useAuthStore();
const username = ref('');
const password = ref('');
const message = ref(null);
const type = ref('');;

const login = async () => {
  try {
    const response = await axios.post('/login', {
      username: username.value,
      password: password.value,
    });

    authStore.token = response.data.token;
    authStore.user = response.data.userName;

    type.value = "success"
    message.value = response.data.message;

    router.push("/campaigns");

  } catch (error) {
      console.log(error)
    type.value = "error"
    message.value = error.message
  }
};
</script>
