<script setup lang="ts">
import { ref } from 'vue'
import FooterSection from './FooterSection.vue';

const count = ref(0)
</script>

<template>
  <h1>Serverless Websockets Quest</h1>
  <img class="small" alt="monster" src="../assets/Monster.png" />
  <img class="small" alt="monster" src="../assets/Fighter.png" />
  <img class="small" alt="monster" src="../assets/Ranger.png" />
  <img class="small" alt="monster" src="../assets/Mage.png" />

  <p>
    Start a new quest or join an existing quest and provide the quest ID.
  </p>

  <button @click="foo">Start new quest</button>
  <button @click="foo">Join quest</button>

  <button type="button" @click="count++">count is: {{ count }}</button>
  
  <FooterSection />
</template>

<style scoped>

</style>
