<script setup lang="ts">
import { defineComponent, ref } from "vue";
import PlayersSection from "./PlayersSection.vue";
import { gameStore } from "../stores";

const store = gameStore();

function getGameResult() {
    return store.teamHasWon ? "Your team defeated the monster!" : "The monster won! Better luck next time!";
}

async function playAgain() {
    store.reset();
}

</script>

<template>
    <h1>Quest: <span class="pink">{{ store.questId }}</span></h1>
    <h2>{{ getGameResult() }} </h2>
    <PlayersSection v-bind="{ useHealth:false, includeMonster:true, isPlayerSelect:false }" />
    <button @click="playAgain">Play again</button>

</template>

<style scoped></style>
