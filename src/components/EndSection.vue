<script setup lang="ts">
    import PlayersSection from "./PlayersSection.vue";
    import { gameStore } from "../stores";

    const store = gameStore();

    function getGameResult() {
        store.disconnect();
        return store.teamHasWon ? "Your team defeated the monster!" : "The monster won! Better luck next time!";
    }

    async function playAgain() {
        store.reset();
    }
</script>

<template>
    <h1>Quest: <span class="pink">{{ store.questId }}</span></h1>
    <h2>{{ getGameResult() }} </h2>
    <PlayersSection v-bind="{ useHealth:false, includeMonster:true, isPlayerSelect:false, showStats:true }" />
    <button @click="playAgain">Play again</button>
</template>
