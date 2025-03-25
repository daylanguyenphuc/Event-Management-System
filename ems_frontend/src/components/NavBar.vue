<template>
    <v-app-bar app color="primary">
        <v-toolbar-title @click="goToHome" style="cursor: pointer;">
            Event Management System
        </v-toolbar-title>

        <v-spacer></v-spacer>

        <!-- Nếu đã đăng nhập, hiển thị menu điều hướng -->
        <div v-if="isLoggedIn">
            <v-btn to="/events" text>Events</v-btn>
            <v-btn to="/myevents" text>My Events</v-btn>
            <v-btn to="/mytickets" text>My Tickets</v-btn>
            <v-menu open-on-click offset-y>
                <template v-slot:activator="{ props }">
                    <v-btn icon v-bind="props">
                        <v-avatar size="40">
                            <img :src="defaultAvatar" alt="User Avatar">
                        </v-avatar>
                    </v-btn>
                </template>
                <v-list>
                    <v-list-item @click="editProfile">
                        <v-list-item-title>Edit Profile</v-list-item-title>
                    </v-list-item>
                    <v-list-item @click="logout">
                        <v-list-item-title>Logout</v-list-item-title>
                    </v-list-item>
                </v-list>
            </v-menu>
        </div>

        <!-- Nếu chưa đăng nhập, hiển thị Login & Register -->
        <div v-else>
            <v-btn to="/login" text>Login</v-btn>
            <v-btn to="/register" text>Register</v-btn>
        </div>
        
    </v-app-bar>
</template>

<script setup>
import { ref, onMounted, onUnmounted } from "vue";
import { useRouter } from "vue-router";

const router = useRouter();
const isLoggedIn = ref(false);
const user = ref({});
const defaultAvatar = "https://cdn-icons-png.flaticon.com/512/149/149071.png";

const updateAuthState = () => {
    const userData = localStorage.getItem("user");
    isLoggedIn.value = !!userData;
    user.value = userData ? JSON.parse(userData) : {};
};

// Lắng nghe sự kiện đăng nhập
onMounted(() => {
    updateAuthState();
    window.addEventListener("auth-changed", updateAuthState);
});

// Hủy lắng nghe khi component bị hủy
onUnmounted(() => {
    window.removeEventListener("auth-changed", updateAuthState);
});

const logout = () => {
    localStorage.removeItem("user");
    updateAuthState();  // Cập nhật lại trạng thái
    router.push("/login");
};

const goToHome = () => {
    router.push(isLoggedIn.value ? "/dashboard" : "/");
};

const goToProfile = () => {
    router.push("/profile");
};

const editProfile = () => {
    router.push("/editProfile");
};
</script>
