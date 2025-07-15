<template>
    <v-container>
        <v-card class="mx-auto pa-6" max-width="600">
            <v-card-title>Ticket Details</v-card-title>
            <v-divider></v-divider>
            <v-card-text v-if="ticket">
                <p><strong>Event:</strong> {{ ticket.eventName }}</p>
                <p><strong>Date:</strong> {{ formatDate(ticket.eventStart) }} - {{ formatDate(ticket.eventEnd) }}</p>
                <p><strong>Location:</strong> {{ ticket.eventLocation }}</p>
                <p><strong>Status:</strong> 
                    {{ ticket.status === 0 ? "Not Redeemed" : ticket.status === 1 ? "Redeemed" : "Refunded" }}
                </p>

            </v-card-text>
            <v-alert v-else type="error" class="mt-3">Ticket not found.</v-alert>
            <v-card-actions>
                <v-btn color="secondary" @click="goToEvent">Go to Event</v-btn>
                <v-btn color="primary" @click="goBack">Back to My Tickets</v-btn>
            </v-card-actions>
        </v-card>
    </v-container>
</template>

<script setup>
import { ref, onMounted } from "vue";
import { useRoute, useRouter } from "vue-router";
import apiClient from "@/services/api";

const route = useRoute();
const router = useRouter();
const ticket = ref(null);

// Fetch Ticket Details
const fetchTicketDetails = async () => {
    try {
        const response = await apiClient.get(`/tickets/${route.params.id}`);
        ticket.value = response.data;
    } catch (error) {
        console.error("Error fetching ticket details:", error);
    }
};

// Format date
const formatDate = (dateString) => {
    return new Date(dateString).toLocaleString();
};

// Navigate back to My Tickets
const goBack = () => {
    router.push("/mytickets");
};

// navigate to event
const goToEvent = () => {
    if (ticket.value) {
        router.push(`/events/${ticket.value.eventId}`);
    }
};


onMounted(fetchTicketDetails);
</script>
