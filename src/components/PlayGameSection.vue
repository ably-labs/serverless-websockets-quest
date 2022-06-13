<script setup lang="ts">
import { defineComponent, ref } from "vue";
import { gameStore } from "../stores";
import PlayersSection from "./PlayersSection.vue";

const store = gameStore();

async function fight() {
     await window.fetch("/api/ExecuteTurn", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            questId: store.questId,
            playerName: store.playerName,
            characterClass: store.characterClass
            })
        });
}

</script>

<template>
    <h1>Quest: <span class="pink">{{ store.questId }}</span></h1>
    <h2>You encounter a monster! Prepare for battle!</h2>
    <PlayersSection v-bind="{ useHealth:true, includeMonster:true, isPlayerSelect:false }" />
    <button v-if="store.isPlayerTurn" @click="fight">Attack</button>
    <p v-if="!store.isPlayerTurn" class="message">Wait for your turn</p>
</template>

<style scoped></style>
