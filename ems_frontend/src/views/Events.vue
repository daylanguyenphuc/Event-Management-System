<template>
    <v-container>
        <h1 class="text-h4 text-md-h3 text-lg-h2 text-xl-h1 pa-5">All Events</h1>
    </v-container>  

    <v-container>
        <v-form>
            <v-row>
                <v-col cols="12" md="8" lg="8">
                    <v-text-field label="Search for events" v-model="filters.name" clearable></v-text-field>
                </v-col>
                <v-col cols="12" md="4" lg="4">
                    <v-select 
                        :items="categories" 
                        label="Category" 
                        v-model="filters.category" 
                        item-title="name" 
                        item-value="id"
                        clearable
                    ></v-select>
                </v-col>
                <v-col cols="6" md="6" lg="3">
                    <v-text-field v-model="filters.startDate" label="Start time" type="datetime-local" clearable></v-text-field>
                </v-col>
                <v-col cols="6" md="6" lg="3">
                    <v-text-field v-model="filters.endDate" label="End time" type="datetime-local" clearable></v-text-field>
                </v-col>
                <v-col cols="12" md="6" lg="3">
                    <v-range-slider
                        v-model="filters.ticketPrice"
                        :min="0"
                        :max="1000000"
                        :step="10000"
                        label="Price Range"
                        thumb-label
                        color="primary"
                    ></v-range-slider>
                </v-col>
                <v-col cols="12" md="6" lg="3">
                    <v-checkbox
                        v-model="filters.AvailableOnly"
                        label="Show Available Events only"
                        color="primary"
                    ></v-checkbox>
                </v-col>    
            </v-row>
        </v-form>
    </v-container>

    <v-container>
        <v-row v-if="paginatedEvents.length > 0">
            <v-col v-for="event in paginatedEvents" :key="event.id" cols="12" md="4">
                <v-card>
                    <v-card-title>{{ event.name }}</v-card-title>
                    <v-card-subtitle>{{ event.categoryName }}</v-card-subtitle>
                    
                    <v-card-text>
                        <p><strong>Location:</strong> {{ event.location }}</p>
                        <p><strong>Start:</strong> {{ formatDate(event.startDate) }}</p>
                        <p><strong>End:</strong> {{ formatDate(event.endDate) }}</p>
                        <p><strong>Price:</strong> {{ event.ticketPrice }} VND</p>
                        <p><strong>Tickets Left:</strong> {{ event.ticketsLeft }}</p>
                    </v-card-text>
                    
                    <v-card-actions>
                        <v-btn color="primary" :to="`/events/${event.id}`">Go to Event</v-btn>
                    </v-card-actions>
                </v-card>
            </v-col>
        </v-row>

        <v-row v-else>
            <v-col cols="12" class="text-center">
                <CustomAlert customText="No events available!"/>
            </v-col>
        </v-row>

        <!-- Pagination -->
        <v-pagination
            v-model="currentPage"
            :length="totalPages"
            class="my-4"
        ></v-pagination>
    </v-container>
</template>
  
<script setup>
    import { ref, watch, onMounted, computed } from "vue";
    import apiClient from "@/services/api";
    import CustomAlert from "@/components/CustomAlert.vue";

    // Categories
    const categories = ref([]);

    // State
    const events = ref([]);
    const filters = ref({
        name: "",
        ticketPrice: [0, 1000000],
        category: "",
        startDate: null,
        endDate: null,
        AvailableOnly: false,
    });

    // Pagination
    const currentPage = ref(1);
    const itemsPerPage = 9;

    // Fetch Events
    const fetchEvents = async () => {
        try {
            // Fetch categories
            const catResponse = await apiClient.get("/categories");
            categories.value = catResponse.data;
        } catch (error) {
            console.error("Error fetching categories:", error);
        }

        try {
            // Fetch events (lấy tất cả)
            const response = await apiClient.get("/events/filter", {
                params: {
                    minPrice: filters.value.ticketPrice[0],
                    maxPrice: filters.value.ticketPrice[1],
                    startDate: filters.value.startDate,
                    endDate: filters.value.endDate,
                    categoryId: filters.value.category || null,
                    name: filters.value.name || null,
                    AvailableOnly: filters.value.AvailableOnly || false,
                },
            });
            events.value = response.data; // API trả về danh sách đầy đủ
        } catch (error) {
            console.error("Error fetching events:", error);
        }
    };

    // Computed: Lọc & Phân trang dữ liệu
    const filteredEvents = computed(() => {
        return events.value.filter(event => {
            return (!filters.value.name || event.name.toLowerCase().includes(filters.value.name.toLowerCase())) &&
                   (!filters.value.category || event.categoryId === filters.value.category) &&
                   (event.ticketPrice >= filters.value.ticketPrice[0] && event.ticketPrice <= filters.value.ticketPrice[1]) &&
                   (!filters.value.startDate || new Date(event.startDate) >= new Date(filters.value.startDate)) &&
                   (!filters.value.endDate || new Date(event.endDate) <= new Date(filters.value.endDate)) &&
                   (!filters.value.AvailableOnly || event.ticketsLeft > 0);
        });
    });

    // Computed: Sự kiện hiển thị trên trang hiện tại
    const paginatedEvents = computed(() => {
        const start = (currentPage.value - 1) * itemsPerPage;
        return filteredEvents.value.slice(start, start + itemsPerPage);
    });

    // Tổng số trang
    const totalPages = computed(() => Math.ceil(filteredEvents.value.length / itemsPerPage));

    // Watch filters change, reset page về 1 & gọi API
    watch(filters, () => {
        currentPage.value = 1;
        fetchEvents();
    }, { deep: true });

    // Format date
    const formatDate = (dateString) => {
        return new Date(dateString).toLocaleString();
    };

    // Fetch data khi component mounted
    onMounted(fetchEvents);
</script>
