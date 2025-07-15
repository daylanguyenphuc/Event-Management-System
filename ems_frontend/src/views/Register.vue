<template>
  <v-container class="d-flex justify-center align-center" style="height: 100vh; margin-top: 100px;">
    <v-card class="pa-5" width="400">
      <v-card-title class="text-h5">Register</v-card-title>
      <v-card-text>
        <v-form v-model="valid" @submit.prevent="register">
          <v-text-field v-model="firstName" label="First Name" :rules="[rules.required]" required></v-text-field>
          <v-text-field v-model="lastName" label="Last Name" :rules="[rules.required]" required></v-text-field>
          <v-text-field v-model="dob" label="Date of Birth" type="date" :rules="[rules.required]" required></v-text-field>
          <v-text-field v-model="email" label="Email" type="email" :rules="[rules.required, rules.email]" required></v-text-field>
          <v-text-field v-model="phone" label="Phone" type="tel" :rules="[rules.required, rules.phone]" required></v-text-field>
          <v-text-field v-model="password" label="Password" type="password" :rules="[rules.required, rules.min]" required></v-text-field>
          <v-text-field v-model="confirmPassword" label="Confirm Password" type="password" :rules="[rules.required, rules.matchPassword]" required></v-text-field>

          <v-alert v-if="errorMessage" type="error" dense class="mb-3">
            {{ errorMessage }}
          </v-alert>

          <v-btn type="submit" color="primary" block :loading="loading">Sign Up</v-btn>
        </v-form>
      </v-card-text>
      <v-card-actions class="justify-center">
        <router-link to="/login">Already have an account? Login</router-link>
      </v-card-actions>
    </v-card>
  </v-container>
</template>

<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { useStore } from "vuex"; // 
import apiClient from "@/services/api";

const router = useRouter();
const store = useStore(); // 

const firstName = ref('');
const lastName = ref('');
const dob = ref('');
const email = ref('');
const phone = ref('');
const password = ref('');
const confirmPassword = ref('');
const loading = ref(false);
const errorMessage = ref('');
const valid = ref(false);

const rules = {
  required: v => !!v || 'This field is required',
  email: v => /.+@.+\..+/.test(v) || 'E-mail must be valid',
  phone: v => /^\d{10,11}$/.test(v) || 'Phone number must be 10-11 digits',
  min: v => (v && v.length >= 6) || 'Password must be at least 6 characters',
  matchPassword: v => v === password.value || 'Passwords must match',
};

const register = async () => {
  if (!valid.value) return;

  loading.value = true;
  errorMessage.value = '';

  try {
    const response = await apiClient.post("/auth/signup", {
      firstname: firstName.value,
      lastname: lastName.value,
      dob: new Date(dob.value).toISOString(),
      email: email.value,
      phone: phone.value,
      password: password.value,
    });

    const user = response.data;

    // Lưu user vào Vuex
    store.commit("setUser", user);

    // Điều hướng đến Dashboard sau khi đăng ký thành công
    router.push('/dashboard');
  } catch (error) {
    errorMessage.value = 'Signup failed. Try again.';
  } finally {
    loading.value = false;
  }
};
</script>