<template>
    <v-app-bar app color="primary">
        <v-toolbar-title @click="goToHome" style="cursor: pointer;">
            Event Management System
        </v-toolbar-title>

        <!-- <v-spacer class="d-md-none"></v-spacer> -->

        <!-- Menu cho màn hình lớn -->
        <div class="d-none d-md-flex align-center" v-if="user">
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

        <div class="d-none d-md-flex" v-else>
            <v-btn to="/login" text>Login</v-btn>
            <v-btn to="/register" text>Register</v-btn>
        </div>

        <!-- Menu -->
        <v-app-bar-nav-icon class="d-md-none" @click="drawer = !drawer"></v-app-bar-nav-icon>
    </v-app-bar>

    <!-- Navigation Drawer -->
    <v-navigation-drawer v-model="drawer" app temporary>
        <v-list v-if="user">
            <v-list-item to="/events">Events</v-list-item>
            <v-list-item to="/myevents">My Events</v-list-item>
            <v-list-item to="/mytickets">My Tickets</v-list-item>
            <v-list-item @click="editProfile">Edit Profile</v-list-item>
            <v-list-item @click="logout">Logout</v-list-item>
        </v-list>
        <v-list v-else>
            <v-list-item to="/login">Login</v-list-item>
            <v-list-item to="/register">Register</v-list-item>
        </v-list>
    </v-navigation-drawer>
</template>

<script setup>
import { ref, computed, watchEffect } from "vue";
import { useStore } from "vuex";
import { useRouter } from "vue-router";

const store = useStore();
const router = useRouter();
const user = computed(() => store.state.user);
const drawer = ref(false);
const defaultAvatar = "https://cdn-icons-png.flaticon.com/512/149/149071.png";

const logout = () => {
    store.commit("setUser", null);
    router.push("/login");
};

const goToHome = () => {
    router.push(user.value ? "/dashboard" : "/");
};

const editProfile = () => {
    router.push("/editProfile");
};
</script>