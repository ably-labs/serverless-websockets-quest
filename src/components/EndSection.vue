<script setup lang="ts">
import { defineComponent, ref } from "vue";
import PlayersSection from "./PlayersSection.vue";

async function playAgain() {
    console.log("Play Again");
}

</script>

<template>
    <h1>You won!</h1>
    <PlayersSection v-bind="{ useHealth:false, includeMonster:false, isPlayerSelect:false }" />
    <button @click="playAgain">Play again</button>

</template>

<style scoped></style>
