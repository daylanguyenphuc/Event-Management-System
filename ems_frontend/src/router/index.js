import { createRouter, createWebHashHistory } from "vue-router";
import Login from "@/views/Login.vue";
import Register from "@/views/Register.vue";
import Dashboard from "@/views/Dashboard.vue";
import Events from "@/views/Events.vue";
import EventDetail from "@/views/EventDetail.vue";
import MyEvents from "@/views/MyEvents.vue";
import EventManagement from "@/views/EventManagement.vue";
import MyTickets from "@/views/MyTickets.vue";
import MyTicketsDetail from "@/views/MyTicketsDetail.vue";
import EditProfile from "@/views/EditProfile.vue";

const routes = [
  { path: "/login", component: Login },
  { path: "/register", component: Register },
  { path: "/dashboard", component: Dashboard },
  { path: "/events", component: Events },
  { path: "/events/:id", component: EventDetail, props: true },
  { path: "/myevents", component: MyEvents },
  { path: "/myevents/:id", component: EventManagement, props: true },
  { path: "/mytickets", component: MyTickets },
  { path: "/ticket/:id", component: MyTicketsDetail, props: true },
  { path: "/editprofile", component: EditProfile }
];

const router = createRouter({
  history: createWebHashHistory(), // Change from createWebHistory() to createWebHashHistory()
  routes,
});

export default router;
