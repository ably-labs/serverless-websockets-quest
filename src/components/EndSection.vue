<script setup lang="ts">
import { defineComponent, ref } from "vue";
import PlayersSection from "./PlayersSection.vue";
import { gameStore } from "../stores";

const store = gameStore();

async function playAgain() {
    console.log("Play Again");
}



</script>

<template>
    <h1>Quest: <span class="pink">{{ store.questId }}</span></h1>
    <hr />
    <h2>You won!</h2>
    <PlayersSection v-bind="{ useHealth:false, includeMonster:false, isPlayerSelect:false }" />
    <button @click="playAgain">Play again</button>

</template>

<style scoped></style>
