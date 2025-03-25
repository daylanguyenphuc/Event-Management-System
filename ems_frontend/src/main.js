import { createApp } from 'vue'
import App from './App.vue'
import vuetify from './plugins/vuetify'
import Antd from "ant-design-vue";
// import "ant-design-vue/dist/antd.css";
import { loadFonts } from './plugins/webfontloader'
import router from "./router";

loadFonts()

createApp(App)
  .use(router)
  .use(vuetify)
  .use(Antd)
  .mount('#app')
