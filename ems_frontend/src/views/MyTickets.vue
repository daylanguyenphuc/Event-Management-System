<template>
    <v-container>
        <v-card class="mx-auto pa-6" max-width="800">
            <v-card-title>My Tickets</v-card-title>
            <v-divider></v-divider>

            <v-data-table :headers="headers" :items="tickets" class="mt-4" dense>
                <template v-slot:item.status="{ item }">
                    <v-chip :color="item.status === 0 ? 'green' : item.status === 1 ? 'gray' : 'red'" dark>
                        {{ item.status === 0 ? 'Not Redeemed' : item.status === 1 ? 'Redeemed' : 'Refunded' }}
                    </v-chip>

                </template>
                
                <template v-slot:item.actions="{ item }">
                    <v-btn color="primary" size="small" @click="goToDetails(item.id)">Show Details</v-btn>
                </template>
            </v-data-table>

            <v-alert v-if="tickets.length === 0" type="info" class="mt-3">You have no tickets.</v-alert>
        </v-card>
    </v-container>
</template>

<script setup>
import { ref, onMounted } from "vue";
import { useRouter } from "vue-router";
import apiClient from "@/services/api";

const router = useRouter();
const tickets = ref([]);

const headers = [
    { title: "Event", key: "eventName" },
    { title: "Date", key: "eventStart" },
    { title: "Status", key: "status" },
    { title: "Actions", key: "actions", sortable: false },
];

// Fetch Tickets
const fetchTickets = async () => {
    try {
        const user = JSON.parse(localStorage.getItem("user"));
        if (!user) throw new Error("User not found");

        const response = await apiClient.get(`/tickets/user/${user.id}`);
        tickets.value = response.data;
    } catch (error) {
        console.error("Error fetching tickets:", error);
    }
};

// Navigate to Ticket Details Page
const goToDetails = (ticketId) => {
    router.push(`/ticket/${ticketId}`);
};

onMounted(fetchTickets);
</script>
