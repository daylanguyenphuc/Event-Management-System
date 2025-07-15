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

            <CustomAlert v-if="tickets.length === 0" customText="You have no tickets. Why don't you just get one?"/>
        </v-card>
    </v-container>
</template>

<script setup>
import { ref, onMounted, computed } from "vue";
import { useStore } from "vuex";
import { useRouter } from "vue-router";
import apiClient from "@/services/api";
import CustomAlert from "@/components/CustomAlert.vue";

const router = useRouter();
const tickets = ref([]);

//Curent User
const store = useStore();
const user = computed(() => store.getters.user);

const headers = [
    { title: "Event", key: "eventName" },
    { title: "Date", key: "eventStart" },
    { title: "Status", key: "status" },
    { title: "Actions", key: "actions", sortable: false },
];

// Fetch Tickets
const fetchTickets = async () => {
    try {
        const response = await apiClient.get(`/tickets/user/${user.value.id}`);
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
