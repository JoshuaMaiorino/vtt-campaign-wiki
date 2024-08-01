<template>
    <v-container fluid>
        <v-row class="d-flex align-center justify-center">
            <v-col col="12"
                   sm="6"
                   md="4">
                <v-form @submit.prevent="register">
                    <h1>Sign Up</h1>
                    <v-alert v-if="message" :type="type" class="mb-4">
                        <div v-html="message" />
                    </v-alert>
                    <v-text-field v-model="username" label="Username" required></v-text-field>
                    <v-text-field v-model="email" label="Email" required></v-text-field>
                    <v-text-field v-model="firstName" label="First Name" required></v-text-field>
                    <v-text-field v-model="lastName" label="Last Name" required></v-text-field>
                    <v-text-field v-model="password" label="Password" type="password" required></v-text-field>
                    <v-btn type="submit" color="primary">Sign Up</v-btn>
                </v-form>
            </v-col>
        </v-row>
    </v-container>
</template>

<script setup>
    import { ref } from 'vue';
    import { useRouter } from 'vue-router';
    import axios from '@/utils/axios';

    const router = useRouter();

    const username = ref('');
    const email = ref('');
    const firstName = ref('');
    const lastName = ref('');
    const password = ref('');
    const message = ref(null);
    const type = ref('');

    const register = async () => {
        try {
            const response = await axios.post('/api/register', {
                username: username.value,
                email: email.value,
                firstName: firstName.value,
                lastName: lastName.value,
                password: password.value,
            });

            type.value = "success";
            message.value = response.data.message;

            // Optionally, redirect to login or another page after successful registration
            router.push("/login");

        } catch (error) {
            console.log(error);
            type.value = "error";
            if (error.response && error.response.data && error.response.data.errors) {
                const errorMessages = error.response.data.errors.generalErrors;
                message.value = `<li>${errorMessages.join("</li><li>")}</li>`;
            } else {
                message.value = error.message;
            }
        }
    };
</script>

<style scoped>
</style>
