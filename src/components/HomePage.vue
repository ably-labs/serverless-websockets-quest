<script setup lang="ts">
import { computed, defineComponent, ref, onMounted, ComputedRef } from "vue";
import { generateQuestId } from '../util/questIdGenerator';
import FooterSection from "./FooterSection.vue";
import StartSection from "./StartSection.vue";
import CharacterSection from "./CharacterSection.vue";

const routes: any = {
  '/': StartSection,
  '/character': CharacterSection
}

let currentPath: String = computed(()=> window.location.hash).value;
const currentView: String = computed(()=> routes[currentPath.slice(1) || '/']).value;

onMounted(() => {
   window.addEventListener('hashchange', () => {
    currentPath = window.location.hash;
    })
})

</script>

<template>
    <component :is="currentView" />
    <FooterSection />
</template>

<style scoped></style>
