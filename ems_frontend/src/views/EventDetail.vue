<template>
    <v-container>
      <v-row justify="center">
        <v-col cols="12" md="8">
          <v-card v-if="event" class="pa-4">
            <v-card-title>{{ event.name }}</v-card-title>
            <v-card-subtitle>{{ event.categoryName }} - {{ event.location }}</v-card-subtitle>
  
            <v-card-text>
              <p><strong>Start:</strong> {{ formatDate(event.startDate) }}</p>
              <p><strong>End:</strong> {{ formatDate(event.endDate) }}</p>
              <p><strong>Price:</strong> {{ event.ticketPrice }} VND</p>
              <p><strong>Tickets Left:</strong> {{ event.ticketsLeft }}</p>
              <p><strong>Description:</strong> {{ event.description }}</p>
            </v-card-text>
  
            <!-- Event Actions -->
            <v-card-actions>
              <!-- Manage Event -->
              <v-btn v-if="event.ownerId === userId" color="primary" @click="manageEvent">
                Manage Event
              </v-btn>
  
              <!-- View Ticket -->
              <v-btn v-if="isRegistered" color="secondary" @click="viewMyTicket">
                My Ticket
              </v-btn>
  
              <!-- Register for Event -->
              <v-btn v-if="(!isRegistered && event.ownerId !== userId) && event.registerAvaliable" color="success" @click="registerForEvent">
                Register for Event
              </v-btn>
  
              <v-btn 
                v-if="(!isRegistered && event.ownerId !== userId)&&!event.registerAvaliable" 
                :disabled="!event.registerAvaliable" 
                color="error"
                >
                Registration Unavailable
                </v-btn>

                <!-- Go to Events List -->
                <v-btn color="info" @click="goToEvents">
                    Back to Events
                </v-btn>
            </v-card-actions>
          </v-card>
        </v-col>
      </v-row>
    </v-container>
  </template>
  
  <script setup>
  import { ref, onMounted } from "vue";
  import { useRoute, useRouter } from "vue-router";
  import apiClient from "@/services/api";
  
  const route = useRoute();
  const router = useRouter();
  
  const event = ref(null);
  const userId = ref(null);
  const ticketId = ref(null);
  const isRegistered = ref(false);
  const registerAvailable = ref(false);
  
  // Fetch user from localStorage
  const getUserFromLocalStorage = () => {
    const user = localStorage.getItem("user");
    return user ? JSON.parse(user) : null;
  };
  
  // Format date
  const formatDate = (dateString) => {
    return new Date(dateString).toLocaleString();
  };
  
  // Fetch event details
  const fetchEventDetail = async () => {
    try {
      const response = await apiClient.get(`/events/${route.params.id}`);
      event.value = response.data;
  
      // Check if registration is available
      registerAvailable.value =
        event.value.registerAvailable &&
        event.value.ownerId !== userId.value &&
        !isRegistered.value;
  
      // Check user registration status
      if (userId.value) {
        await checkUserRegistration();
      }
    } catch (error) {
      console.error("Error fetching event:", error);
    }
  };
  
  // Check if user is registered for this event
  const checkUserRegistration = async () => {
    try {
      const response = await apiClient.get(`/tickets/user/${userId.value}/event/${event.value.id}`);
      if (response.data) {
        isRegistered.value = true;
        ticketId.value = response.data.id;
      } else {
        isRegistered.value = false;
        ticketId.value = null;
      }
    } catch (error) {
      console.error("Error checking user registration:", error);
      isRegistered.value = false;
      ticketId.value = null;
    }
  };
  
  // Register for event
  const registerForEvent = async () => {
    try {
      const response = await apiClient.post(`/tickets/book/${userId.value}/${event.value.id}`);
      if (response.data && response.data.id) {
        router.push(`/ticket/${response.data.id}`);
      } else {
        console.error("Failed to register.");
      }
    } catch (error) {
      console.error("Error registering:", error);
    }
  };
  
  // Navigate to event management page
  const manageEvent = () => {
    router.push(`/myevents/${event.value.id}`);
  };
  
  // Navigate to My Ticket page
  const viewMyTicket = () => {
    if (ticketId.value) {
      router.push(`/ticket/${ticketId.value}`);
    }
  };

const goToEvents = () => {
  router.push("/events");
};

  
  // On mount: get user & fetch event
  onMounted(() => {
    const user = getUserFromLocalStorage();
    if (user && user.id) {
      userId.value = user.id;
      fetchEventDetail();
    } else {
      router.push("/login");
    }
  });
  </script>
  