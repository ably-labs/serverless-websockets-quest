<script setup lang="ts">
import { defineComponent, ref } from "vue";
import PlayersSection from "./PlayersSection.vue";
import { useStore } from "../store";
import ErrorMessageSection from "./ErrorMessageSection.vue";
import { GamePhase } from "../types/GamePhases";

const store = useStore();
const errorMessage = ref<String>("");

async function addPlayer() {
    console.log("Add player");
    console.log(`/api/GetQuestExists/${store.questId}`);
    const questExists = await window.fetch(`/api/GetQuestExists/${store.questId}`);
    if (questExists) {
        const addPlayer = await window.fetch("/api/AddPlayer", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            questId: store.questId,
            playerId: store.playerId
            })
        })
        if (addPlayer.ok) {
            store.view = GamePhase.Play;
        } else {
            errorMessage.value = addPlayer.statusText;
        }
    }
    else {
        errorMessage.value = `${store.questId} quest was not found`;
    }
}

</script>

<template>
    <h1>Quest: <span class="pink">{{ store.questId }}</span></h1>
    <h2>Select and name your character</h2>
    <PlayersSection v-bind="{ useHealth:false, includeMonster:false, isPlayerSelect:true }" />
    <input type="text" v-model="store.playerId" placeholder="Character name" />
    <button @click="addPlayer">Start quest</button>
    <ErrorMessageSection :errorMessage=errorMessage />
</template>

<style scoped></style>
