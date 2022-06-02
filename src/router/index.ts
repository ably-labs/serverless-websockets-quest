import { createRouter, createWebHistory } from 'vue-router'
import StartSection from '../components/StartSection.vue'
import CharacterSection from '../components/CharacterSection.vue';

export const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/',
      name: 'start',
      component: StartSection
    },
    {
      path: '/character',
      name: 'character',
      component: CharacterSection
    }
  ]
});
