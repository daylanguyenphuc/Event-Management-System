<template>
    <v-container>
        <v-card class="mx-auto pa-6" max-width="900">
            <v-card-title>My Events</v-card-title>
            <v-divider></v-divider>

            <!-- Tabs -->
            <v-tabs v-model="tab">
                <v-tab value="my-events">My Events</v-tab>
                <v-tab value="create-event">Create New Event</v-tab>
            </v-tabs>

            <v-window v-model="tab">
                <!-- My Events Tab -->
                <v-window-item value="my-events">
                    <v-data-table :headers="headers" :items="events" class="mt-4" dense>
                        <template v-slot:item.status="{ item }">
                            <v-chip :color="getEventStatusColor(item)" dark>
                                {{ getEventStatus(item) }}
                            </v-chip>
                        </template>

                        <template v-slot:item.actions="{ item }">
                            <v-btn color="primary" size="small" @click="manageEvent(item.id)">Manage event</v-btn>
                        </template>
                    </v-data-table>

                    <v-alert v-if="events.length === 0" type="info" class="mt-3">You have not created any events.</v-alert>
                </v-window-item>

                <!-- Create New Event Tab -->
                <v-window-item value="create-event">
                    <v-form ref="eventForm" @submit.prevent="createEvent">
                        <v-text-field v-model="newEvent.name" label="Event Name" required></v-text-field>
                        <v-textarea v-model="newEvent.description" label="Event Description" required></v-textarea>
                        <v-text-field v-model="newEvent.location" label="Location" required></v-text-field>
                        <v-text-field v-model="newEvent.startDate" label="Start time" type="datetime-local" required></v-text-field>
                        <v-text-field v-model="newEvent.endDate" label="End time" type="datetime-local" required></v-text-field>
                        <v-select
                            v-model="newEvent.categoryId"
                            :items="categories"
                            item-value="id"
                            item-text="name"
                            label="Category"
                            required
                        >
                        </v-select>

                        <v-text-field 
                            v-model="newEvent.ticketPrice" 
                            label="Ticket Price" 
                            type="number" 
                            required
                            suffix="VND"
                        ></v-text-field>
                        <v-text-field v-model="newEvent.ticketsLeft" label="Number of Tickets" type="number" required></v-text-field>
                        <v-btn type="submit" color="primary" class="mt-3">Create Event</v-btn>
                    </v-form>
                </v-window-item>
            </v-window>
        </v-card>
    </v-container>
</template>

<script setup>
import { ref, onMounted } from "vue";
import { useRouter } from "vue-router"; // Add this line
import apiClient from "@/services/api";

const tab = ref("my-events");
const events = ref([]);
const newEvent = ref({
    name: "",
    description: "",
    location: "",
    startDate: null,
    endDate: null,
    ticketPrice: null,
    ticketsLeft: null,
    ownerId: localStorage.getItem("user") ? JSON.parse(localStorage.getItem("user")).id : null,
    categoryId: null, 
});

const categories = ref([]);

const headers = [
    { title: "Event Name", key: "name" },
    { title: "Start Date", key: "startDate" },
    { title: "End Date", key: "endDate" },
    { title: "Status", key: "status" },
    { title: "Actions", key: "actions", sortable: false },
];

// Fetch user's events
const fetchEvents = async () => {
    try {
        const user = JSON.parse(localStorage.getItem("user"));
        if (!user) throw new Error("User not found");

        const response = await apiClient.get(`/events/owner/${user.id}`);
        events.value = response.data;
    } catch (error) {
        console.error("Error fetching events:", error);
    }
};

// Function to get event status based on time and cancellation
const getEventStatus = (event) => {
    const currentTime = new Date();

    // Check if the event is canceled
    if (event.isCanceled) {
        return "Cancelled";
    }

    // If the current time is before the event start time
    if (currentTime < new Date(event.startDate)) {
        return "Check-in not Available";
    }

    // If the current time is between start and end time
    if (currentTime >= new Date(event.startDate) && currentTime <= new Date(event.endDate)) {
        return "Check-in Available";
    }

    // If the current time is after the event end time
    return "Event Finished";
};

// Function to get event status color
const getEventStatusColor = (event) => {
    if (event.isCanceled) {
        return "red"; // Cancelled event is red
    }

    const currentTime = new Date();
    if (currentTime < new Date(event.startDate)) {
        return "blue"; // Not available (before start) is blue
    }

    if (currentTime >= new Date(event.startDate) && currentTime <= new Date(event.endDate)) {
        return "green"; // Available (during event) is green
    }

    return "grey"; // Event finished is grey
};

const router = useRouter();
const manageEvent = (eventId) => {
    // Navigate to the event management page
    router.push(`/myevents/${eventId}`);
};

// Fetch categories
const fetchCategories = async () => {
    try {
        const response = await apiClient.get("/categories");
        categories.value = response.data;
        console.log("Categories:", categories.value);
    } catch (error) {
        console.error("Error fetching categories:", error);
    }
};

// Create a new event
const createEvent = async () => {
    try {
        const user = JSON.parse(localStorage.getItem("user"));
        if (!user) throw new Error("User not found");

        const eventData = { ...newEvent.value, ownerId: user.id };
        await apiClient.post("/events", eventData);

        // Refresh event list
        fetchEvents();
        tab.value = "my-events";
    } catch (error) {
        console.error("Error creating event:", error);
    }
};

// Edit an event (redirect to edit page)
const editEvent = (eventId) => {
    window.alert("Redirect to event edit page: " + eventId);
};

// Cancel an event
const cancelEvent = async (eventId) => {
    if (!confirm("Are you sure you want to cancel this event?")) return;

    try {
        await apiClient.put(`/events/${eventId}/cancel`);
        fetchEvents(); // Refresh event list
    } catch (error) {
        console.error("Error canceling event:", error);
    }
};

onMounted(() => {
    fetchEvents();
    fetchCategories(); 
});
</script>
