import { createRouter, createWebHashHistory } from "vue-router";
import Login from "@/views/Login.vue";
import Signup from "@/views/Signup.vue";
// import EventList from "@/views/EventList.vue";

const routes = [
//   { path: "/", component: EventList },
  { path: "/login", component: Login },
  { path: "/signup", component: Signup },
];

const router = createRouter({
  history: createWebHashHistory(), // Change from createWebHistory() to createWebHashHistory()
  routes,
});

export default router;
