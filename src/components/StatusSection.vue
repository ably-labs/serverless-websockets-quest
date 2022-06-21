<script setup lang="ts">
    import { gameStore } from "../stores";

    const store = gameStore();
</script>

<template>
    <p v-if="store.isConnected"><span class="pink">Connected</span> | Players joined: {{ store.players.toString() }}</p>
</template>

<style scoped>
    p {
        font-size: smaller;
        color: #55ffff 
    }
</style>
