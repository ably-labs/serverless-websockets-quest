<script setup lang="ts">
import { defineComponent, ref } from "vue";
import { gameStore } from "../stores";

const store = gameStore();

</script>

<template>
    <p>Connected: {{ store.isConnected }} | Players connected: {{ store.players.toString() }}</p>
</template>

<style scoped>

p {
    font-size: smaller;
    color: #55ffff 
}

</style>