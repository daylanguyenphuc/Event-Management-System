<template>
    <v-container>
        <v-card class="mx-auto pa-6" max-width="600">
            <v-tabs v-model="activeTab" background-color="primary" dark>
                <v-tab value="profile">Edit Profile</v-tab>
                <v-tab value="password">Change Password</v-tab>
            </v-tabs>

            <v-card-text>
                <v-window v-model="activeTab">
                    <!-- Edit Profile Tab -->
                    <v-window-item value="profile">
                        <v-form ref="profileForm" v-model="isProfileValid">
                            <v-text-field v-model="profile.firstname" label="First Name" required :rules="[rules.required]" />
                            <v-text-field v-model="profile.lastname" label="Last Name" required :rules="[rules.required]" />
                            <v-text-field v-model="profile.email" label="Email" type="email" disabled />
                            <v-text-field v-model="profile.phone" label="Phone Number" type="tel" required :rules="[rules.phone]" />
                            <v-text-field 
                                v-model="profile.dob" 
                                label="Date of Birth" 
                                type="date" 
                                required 
                                @change="formatDOB"
                            />
                        </v-form>
                        <!-- Thông báo trạng thái -->
                        <v-alert v-if="profileMessage" :type="profileSuccess ? 'success' : 'error'" class="mt-3">
                            {{ profileMessage }}
                        </v-alert>
                        <v-btn color="primary" block :loading="loading" @click="updateProfile" class="mt-3">
                            Save Changes
                        </v-btn>
                    </v-window-item>

                    <!-- Change Password Tab -->
                    <v-window-item value="password">
                        <v-form ref="passwordForm" v-model="isPasswordValid">
                            <v-text-field v-model="passwords.oldPassword" label="Old Password" type="password" required :rules="[rules.required]" />
                            <v-text-field v-model="passwords.newPassword" label="New Password" type="password" required :rules="[rules.password]" />
                            <v-text-field v-model="passwords.confirmPassword" label="Confirm Password" type="password" required :rules="[rules.matchPassword]" />
                        </v-form>
                        <!-- Thông báo trạng thái -->
                        <v-alert v-if="passwordMessage" :type="passwordSuccess ? 'success' : 'error'" class="mt-3">
                            {{ passwordMessage }}
                        </v-alert>
                        <v-btn color="primary" block :loading="loading" @click="changePassword" class="mt-3">
                            Change Password
                        </v-btn>
                    </v-window-item>
                </v-window>
            </v-card-text>
        </v-card>
    </v-container>
</template>

<script setup>
import { ref, onMounted, computed } from "vue";
import { useStore } from "vuex";
import apiClient from "@/services/api";

// State
const activeTab = ref("profile");
const loading = ref(false);

//Curent User
const store = useStore();
const user = computed(() => store.getters.user);

// Profile Data
const profile = ref({
    firstName: "",
    lastName: "",
    email: "",
    phone: "",
    dob: ""
});
const isProfileValid = ref(false);

// Password Data
const passwords = ref({
    oldPassword: "",
    newPassword: "",
    confirmPassword: ""
});
const isPasswordValid = ref(false);

// Trạng thái hiển thị thông báo
const profileMessage = ref("");
const profileSuccess = ref(false);

const passwordMessage = ref("");
const passwordSuccess = ref(false);

// Validation Rules
const rules = {
    required: v => !!v || "This field is required",
    phone: v => /^[0-9]{10,11}$/.test(v) || "Invalid phone number",
    password: v => v.length >= 6 || "Password must be at least 6 characters",
    matchPassword: v => v === passwords.value.newPassword || "Passwords do not match"
};

// Fetch User Profile
const fetchProfile = async () => {
    try {
        const response = await apiClient.get(`/users/${user.value.id}`);

        // Convert DOB format "2004-03-14T00:00:00" -> "2004-03-14"
        response.data.dob = response.data.dob.split("T")[0];

        profile.value = response.data;
    } catch (error) {
        console.error("Error fetching profile:", error);
    }
};

// Format DOB before sending API
const formatDOB = () => {
    if (profile.value.dob) {
        profile.value.dob = `${profile.value.dob}T00:00:00`;
    }
};

// Update Profile
const updateProfile = async () => {
    if (!isProfileValid.value) return;

    loading.value = true;
    profileMessage.value = "";
    try {
        await apiClient.put(`/users/${profile.value.id}`, profile.value);
        profileMessage.value = "Profile updated successfully!";
        profileSuccess.value = true;
    } catch (error) {
        console.error("Error updating profile:", error);
        profileMessage.value = "Failed to update profile.";
        profileSuccess.value = false;
    } finally {
        loading.value = false;
    }
};

// Change Password
const changePassword = async () => {
    if (!isPasswordValid.value) return;

    loading.value = true;
    passwordMessage.value = "";
    try {
        await apiClient.post("/auth/change-password", {
            userId: profile.value.id,
            currentPassword: passwords.value.oldPassword,
            newPassword: passwords.value.newPassword
        });
        passwordMessage.value = "Password changed successfully!";
        passwordSuccess.value = true;
    } catch (error) {
        console.error("Error changing password:", error);
        passwordMessage.value = "Failed to change password.";
        passwordSuccess.value = false;
    } finally {
        loading.value = false;
    }
};

onMounted(fetchProfile);
</script>
