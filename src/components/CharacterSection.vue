<script setup lang="ts">
import { defineComponent, ref } from "vue";
import PlayersSection from "./PlayersSection.vue";
import { gameStore } from "../stores";
import ErrorMessageSection from "./ErrorMessageSection.vue";

const store = gameStore();
const errorMessage = ref<string>("");

async function addPlayer() {
    store.isPlayerAdded = true;
    const questExistsResponse = await window.fetch(`/api/GetQuestExists/${store.questId}`);
    const questExistsMessage = await questExistsResponse.text();
    if (questExistsResponse.ok) {
        await window.fetch("/api/AddPlayer", {
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
    else {
        errorMessage.value = questExistsMessage;
    }
}

</script>

<template>
    <h1>Quest: <span class="pink">{{ store.questId }}</span></h1>
    <h2>Select and name your character</h2>
    <p class="info" v-if="store.isHost">Quest ID has been copied to your clipboard! Send this to two other players so they can join.</p>
    <PlayersSection v-bind="{ useHealth:false, includeMonster:false, isPlayerSelect:true, showStats:false }" />
    <div v-if="!store.isPlayerAdded">
        <input type="text" v-model="store.playerName" :disabled="store.isPlayerAdded" placeholder="Character name" />
        <button @click="addPlayer" :disabled="store.isPlayerAdded">Add player</button>
    </div>
    <p class="message">Waiting for {{ store.numberOfPlayersRemaining }} player(s) to join.</p>
    <ErrorMessageSection :errorMessage=errorMessage />
</template>
