<template>
  <v-container class="d-flex justify-center align-center" style="height: 100vh">
    <v-card class="pa-5" width="400">
      <v-card-title class="text-h5">Login</v-card-title>
      <v-card-text>
        <v-form @submit.prevent="login">
          <v-text-field 
            v-model="email"
            label="Email"
            type="email"
            required
            :rules="[rules.required, rules.email]"
          ></v-text-field>

          <v-text-field 
            v-model="password"
            label="Password"
            type="password"
            required
            :rules="[rules.required, rules.password]"
          ></v-text-field>

          <v-alert v-if="errorMessage" type="error" dense class="mb-3">
            {{ errorMessage }}
          </v-alert>

          <v-btn type="submit" color="primary" block :loading="loading">Login</v-btn>
        </v-form>
      </v-card-text>
      <v-card-actions class="justify-center">
        <router-link to="/register">Don't have an account? Register</router-link>
      </v-card-actions>
    </v-card>
  </v-container>
</template>

<script setup>
  import { ref } from 'vue';
  import { useRouter } from 'vue-router';
  import apiClient from "@/services/api";

  const router = useRouter();
  const email = ref('');
  const password = ref('');
  const loading = ref(false);
  const errorMessage = ref('');

  const rules = {
    required: v => !!v || 'This field is required',
    email: v => /.+@.+\..+/.test(v) || 'E-mail must be valid',
    password: v => (v && v.length >= 6) || 'Password must be at least 6 characters',
  };

  const login = async () => {
    loading.value = true;
    errorMessage.value = '';

    try {
      const response = await apiClient.post("/auth/login", {
        email: email.value,
        password: password.value,
      });

      // Lưu thông tin user vào localStorage
      localStorage.setItem('user', JSON.stringify(response.data));

      // Phát sự kiện đăng nhập
      window.dispatchEvent(new Event("auth-changed"));

      // Chuyển hướng đến dashboard
      router.push('/dashboard');
    } catch (error) {
      errorMessage.value = error.response.data;
    } finally {
      loading.value = false;
    }
  };
</script>
