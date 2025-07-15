<template>
    <v-alert type="info" color="blue" variant="outlined">
        {{ customText || "No content to show" }}
    </v-alert>
</template>

<script setup>
import { defineProps } from 'vue';

defineProps({
    customText: {
        type: String,
        default: ""
    }
});
</script>