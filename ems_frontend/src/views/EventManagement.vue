<template>
    <v-container>
        <v-card class="mx-auto pa-6" max-width="900">
            <v-card-title>Event Management</v-card-title>
            <v-divider></v-divider>

            <!-- Tabs for Event Management -->
            <v-tabs v-model="tab">
                <v-tab>Event Details</v-tab>
                <v-tab>Ticket Check-in</v-tab>
            </v-tabs>

            <v-window v-model="tab">
                <!-- Event Details Tab -->
                <v-window-item>
                    <v-form>
                        <v-text-field v-model="event.name" label="Event Name" :readonly="isReadOnly"></v-text-field>
                        <v-textarea v-model="event.description" label="Event Description" :readonly="isReadOnly"></v-textarea>
                        <v-text-field v-model="event.startDate" label="Start Time" :readonly="isReadOnly"></v-text-field>
                        <v-text-field v-model="event.endDate" label="End Time" :readonly="isReadOnly"></v-text-field>
                        <v-text-field v-model="event.ticketPrice" label="Ticket Price" :readonly="isReadOnly"></v-text-field>
                        <v-text-field v-model="ticketSlots" label="Ticket Slots" :readonly="isReadOnly"></v-text-field>
                        <v-text-field v-model="event.location" label="Location" :readonly="isReadOnly"></v-text-field>
                        <v-select
                            v-model="event.categoryId"
                            :items="categories"
                            item-value="id"
                            item-text="name"
                            label="Category"
                            :readonly="isReadOnly"
                            required
                        >
                        </v-select>

                        <!-- Edit and Cancel Buttons -->
                        <v-btn color="primary" @click="editEvent" v-show="!isReadOnly">Edit</v-btn>
                        <v-btn color="error" @click="openCancelDialog" v-show="!isReadOnly">Cancel Event</v-btn>
                        <v-btn color="secondary" @click="goToMyEvents">Go to My Events</v-btn>

                    </v-form>
                </v-window-item>

                <!-- Ticket Check-in Tab -->
                <v-window-item>
                    <v-data-table :headers="ticketHeaders" :items="tickets" class="mt-4">
                        <template v-slot:item.status="{ item }">
                            <v-chip :color="item.status === 1 ? 'green' : item.status === 2 ? 'red' : 'grey'" dark>
                                {{ getStatus(item.status) }}
                            </v-chip>
                        </template>
                        <template v-slot:item.actions="{ item }">
                            <!-- Show actions only if event is not canceled and ticket status is "Not Redeemed" and current date > start date -->
                            <v-btn v-if="!event.isCanceled && item.status === 0 && new Date() > new Date(event.startDate)" color="primary" size="small" @click="checkInTicket(item.id)">Check-in</v-btn>
                        </template>
                    </v-data-table>
                </v-window-item>
            </v-window>
        </v-card>
    </v-container>

    <!-- Cancel Event Confirmation Dialog -->
    <v-dialog v-model="cancelDialog" max-width="400px">
        <v-card>
            <v-card-title class="headline">Cancel Event</v-card-title>
            <v-card-text>Are you sure you want to cancel this event?</v-card-text>
            <v-card-actions>
                <v-btn text @click="cancelDialog = false">Cancel</v-btn>
                <v-btn color="error" @click="cancelEvent">Confirm</v-btn>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script setup>
import { ref, computed, onMounted } from "vue"; // Added computed import
import { useRouter, useRoute } from "vue-router";
import apiClient from "@/services/api";

const route = useRoute();
const router = useRouter();
const tab = ref(0); // Default tab is "Event Details"
const event = ref({});
const tickets = ref([]);
const ticketSlots = ref(0);
const categories = ref([]);
const cancelDialog = ref(false); // To control the cancel confirmation dialog

// Fetch the event details using the ID from the route
const fetchEventDetails = async () => {
    try {
        const response = await apiClient.get("/categories");
        categories.value = response.data;
    } catch (error) {
        console.error("Error fetching categories:", error);
        // Optionally display a user-friendly message
    }
    try {
        const response = await apiClient.get(`/events/${route.params.id}`);
        event.value = response.data;
        // Calculate ticket slots (Left + Sold)
        ticketSlots.value = event.value.ticketsLeft + event.value.ticketCount;
    } catch (error) {
        console.error("Error fetching event details:", error);
        // Optionally display a user-friendly message
    }
};

// Fetch the list of tickets for the event
const fetchTickets = async () => {
    try {
        const eventId = route.params.id; // Get event ID from the route
        const response = await apiClient.get(`/tickets/event/${eventId}`);
        tickets.value = response.data;
    } catch (error) {
        console.error("Error fetching tickets:", error);
        // Optionally display a user-friendly message
    }
};

// Check if the event is expired (current date > event end date)
const isEventExpired = computed(() => {
    const currentDate = new Date();
    const eventEndDate = new Date(event.value.endDate);
    return currentDate > eventEndDate;
});

// Computed property to check if fields should be read-only
const isReadOnly = computed(() => {
    return event.value.isCanceled || isEventExpired.value;
});

// Navigate to My Events page
const editEvent = async () => {
    try {
        // Prepare the updated event data
        const updatedEvent = {
            ...event.value,
            name: event.value.name,
            description: event.value.description,
            startDate: event.value.startDate,
            endDate: event.value.endDate,
            ticketPrice: event.value.ticketPrice,
            ticketsLeft: ticketSlots.value - event.value.ticketCount, // Updated ticket slots
            location: event.value.location,
            categoryId: event.value.categoryId,
            isCanceled: event.value.isCanceled,
        };

        // Send the PUT request to update the event
        const response = await apiClient.put(`/events/${route.params.id}`, updatedEvent);
        
        if (response.status === 200) {
            window.alert("Event updated successfully!");
            fetchEventDetails(); // Refresh the event data after update
        } else {
            window.alert("Failed to update event. Please try again.");
        }
    } catch (error) {
        console.error("Error updating event:", error);
        window.alert("Failed to update event. Please try again.");
    }
};

// Open cancel confirmation dialog
const openCancelDialog = () => {
    cancelDialog.value = true;
};

// Cancel Event API call
const cancelEvent = async () => {
    try {
        await apiClient.post(`/events/cancel/${route.params.id}`);
        window.alert("Event has been canceled.");
        cancelDialog.value = false; // Close the cancel dialog
        fetchEventDetails(); // Refresh event details
    } catch (error) {
        console.error("Error canceling event:", error);
        // Optionally display a user-friendly message
    }
};

// back to my events
const goToMyEvents = () => {
    router.push("/myevents");
};

// Check-in Ticket
const checkInTicket = async (ticketId) => {
    try {
        await apiClient.post(`/tickets/redeem/${ticketId}`);
        fetchTickets(); // Refresh ticket list after check-in
    } catch (error) {
        console.error("Error checking in ticket:", error);
        // Optionally display a user-friendly message
    }
};

// Get ticket status text
const getStatus = (status) => {
    if (status === 0) return "Not Redeemed";
    if (status === 1) return "Redeemed";
    if (status === 2) return "Refunded";
    return "Unknown";
};

// Headers for ticket table
const ticketHeaders = [
    { title: "Ticket ID", key: "id" },
    { title: "User Firstname", key: "userFirstName" },
    { title: "User Lastname", key: "userLastName" },
    { title: "Status", key: "status" },
    { title: "Actions", key: "actions" }
];

// On mount, fetch event and ticket details
onMounted(() => {
    fetchEventDetails();
    fetchTickets();
});
</script>
