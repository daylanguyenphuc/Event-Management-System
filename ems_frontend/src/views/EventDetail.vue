<template>
    <v-container>
      <v-row justify="center">
        <v-col cols="12" md="8">

          <!-- Event detail Section -->
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

      <!-- New Discussion Form -->
      <v-row justify="center">
        <v-col cols="12" md="8">
          <v-card class="mt-4 pa-4">
            <v-card-title>Add new discussion</v-card-title>
            <v-card-text>
              <v-form @submit.prevent="postDiscussion">
                <v-text-field v-model="newDiscussion.title" label="Discussion Title" required></v-text-field>
                <v-textarea rows="3" v-model="newDiscussion.description" label="Write your discussion" required></v-textarea>
                <v-file-input 
                  v-model="newDiscussion.photos"
                  label="Attach Images"
                  multiple
                  accept="image/*"
                  @update:modelValue="previewImages"
                >
                </v-file-input>
                <!-- display preview image -->
                <v-row v-if="imagePreviews.length">
                  <v-col cols="12"><p>Preview images:</p></v-col>
                  <v-col v-for="(image, index) in imagePreviews" :key="index" cols="4">
                    <v-img :src="image" class="rounded-lg mb-4"></v-img>
                  </v-col>
                </v-row>
                <v-btn :loading="isLoadingPostDiscussion" color="primary" type="submit">
                  Post Discussion
                </v-btn>
              </v-form>
            </v-card-text>
          </v-card>
        </v-col>
      </v-row>

      <!-- Display Discussions -->
      <v-row justify="center">
        <v-col cols="12" md="8">
          <div v-if="discussions.length">
            <v-card v-for="discussion in discussions" :key="discussion.id" class="mb-4 pa-3">
              <!-- User Info -->
              <v-card-title class="d-flex align-center justify-space-between">
                <v-row>
                  <v-col cols="7" md="10">
                    <div class="d-flex align-center">
                      <v-avatar size="40" class="mr-3">
                        <v-icon>mdi-account</v-icon>
                      </v-avatar>
                      <div>
                        <p class="font-weight-bold">{{ discussion.userName }}</p>
                        <!-- <small class="text-grey-darken-1">
                          Created: {{ formatDate(discussion.createdAt) }} 
                          <span v-if="discussion.updatedAt && discussion.updatedAt !== discussion.createdAt">
                            | Updated: {{ formatDate(discussion.updatedAt) }}
                          </span>
                        </small> -->
                        <small class="text-grey-darken-1">{{ formatDate(discussion.createdAt) }}</small>
                      </div>
                    </div>
                  </v-col>
                  <v-col cols="5" md="2" class="d-flex justify-end">
                    <!-- Options Menu (Only if the user owns the post) -->
                    <v-menu v-if="discussion.userId == store.getters.user.id" offset-y>
                      <template v-slot:activator="{ props }">
                        <v-btn icon v-bind="props">
                          <v-icon>mdi-dots-vertical</v-icon>
                        </v-btn>
                      </template>
                      <v-list>
                        <!-- <v-list-item @click="editDiscussion(discussion)">
                          <v-list-item-title>
                            <v-icon left>mdi-pencil</v-icon> Edit
                          </v-list-item-title>
                        </v-list-item> -->
                        <v-list-item @click="deleteDiscussion(discussion.id)">
                          <v-list-item-title class="text-red">
                            <v-icon left class="text-red">mdi-delete</v-icon> Delete
                          </v-list-item-title>
                        </v-list-item>
                      </v-list>
                    </v-menu>
                  </v-col>
                </v-row>
              </v-card-title>

              <!-- Post Content -->
              <v-card-text>
                <p class="text-h6 font-weight-medium">{{ discussion.title }}</p>
                <p>{{ discussion.description }}</p>

                <!-- Display Photos -->
                <v-row v-if="discussion.photos.length">
                  <v-col v-for="photo in discussion.photos" :key="photo" cols="4">
                    <v-img :src="photo" class="rounded-lg"></v-img>
                  </v-col>
                </v-row>
              </v-card-text>
            </v-card>
          </div>
          <CustomAlert v-else customText="No discussions yet. Be the first to start one!"/>
        </v-col>
      </v-row>
    </v-container>
  </template>
  
  <script setup>
  import { ref, onMounted, computed } from "vue";
  import { useStore } from "vuex";
  import { useRoute, useRouter } from "vue-router";
  import apiClient from "@/services/api";
  import CustomAlert from "@/components/CustomAlert.vue";
  
  const route = useRoute();
  const router = useRouter();

  //Curent User
  const store = useStore();
  const user = computed(() => store.getters.user);
  
  const event = ref(null);
  const userId = ref(null);
  const ticketId = ref(null);
  const isRegistered = ref(false);
  const registerAvailable = ref(false);
  const discussions = ref([]);
  const newDiscussion = ref({
    title: "",
    description: "",
    photos: [],
    eventId: route.params.id,
    userId: user.value ? user.value.id : null,
  });
  const imagePreviews = ref([]);
  const isLoadingPostDiscussion = ref(false);
  
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

  // Fetch discussions for the event
  const fetchDiscussions = async () => {
    try {
        const response = await apiClient.get(`/discussions/by-events/${route.params.id}`);
        if (response.data === "No discussions found for this event.") {
            discussions.value = [];
        } else {
            discussions.value = response.data.map(discussion => ({
                ...discussion,
                photos: discussion.photos ? discussion.photos.split(",") : []  // Chuyển chuỗi thành mảng
            }));
        }
    } catch (error) {
        console.error("Error fetching discussions:", error);
        discussions.value = [];
    }
  };

  // Post a new discussion
  const postDiscussion = async () => {
    try {
      isLoadingPostDiscussion.value = true; // Set loading state to true

      const formData = new FormData();
      formData.append("Title", newDiscussion.value.title);
      formData.append("Description", newDiscussion.value.description);
      formData.append("EventId", route.params.id);
      formData.append("UserId", user.value.id);

      // Append each photo
      if (newDiscussion.value.photos.length > 0) {
        newDiscussion.value.photos.forEach((photo) => {
          formData.append("photos", photo);
        });
      }

      // Send request to backend and wait for completion
      await apiClient.post('/discussions', formData);

      // Refresh discussion list after creation is done
      await fetchDiscussions();

      // Clear form
      newDiscussion.value = {
        title: "",
        description: "",
        photos: [],
        eventId: route.params.id,
        userId: user.value.id,
      };
      imagePreviews.value = []; // Reset previews
    } catch (error) {
      console.error("Error creating discussion:", error);
    } finally {
      isLoadingPostDiscussion.value = false; // Set loading state to false
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
  
  // Preview Image Logic
  const previewImages = (event) => {
    if (!event) return; // Check if event is empty

    imagePreviews.value = []; // Reset previews
    newDiscussion.value.photos = []; // Reset selected files

    const files = Array.isArray(event) ? event : [event]; // Ensure it's always an array

    files.forEach(file => {
      newDiscussion.value.photos.push(file); // Store file

      const reader = new FileReader();
      reader.onload = (e) => {
        imagePreviews.value.push(e.target.result); // Push Base64 preview
      };
      reader.readAsDataURL(file);
    });
  };

  // Edit discussion (Open edit modal, etc.)
  // const editDiscussion = (discussion) => {
  //   console.log("Edit discussion:", discussion);
  //   // Implement edit functionality (e.g., open a dialog with discussion details)
  // };

  // Delete discussion (Call API & refresh list)
  const deleteDiscussion = async (discussionId) => {
    try {
      await apiClient.delete(`/discussions/${discussionId}`);
      console.log("Discussion deleted:", discussionId);
      // Refresh discussions after delete
      fetchDiscussions();
    } catch (error) {
      console.error("Error deleting discussion:", error);
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
    const user = store.state.user;
    if (user && user.id) {
      userId.value = user.id;
      fetchEventDetail();
      fetchDiscussions();
    } else {
      router.push("/login");
    }
  });
  </script>
  