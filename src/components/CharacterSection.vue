<script setup lang="ts">
import { defineComponent, ref } from "vue";
import PlayersSection from "./PlayersSection.vue";
import { useStore } from "../store";
import ErrorMessageSection from "./ErrorMessageSection.vue";

const store = useStore();
const errorMessage = ref<String>("");

async function addPlayer() {
    console.log("Add player");
    const questId = window.location.pathname.split("/").pop();
    console.log(`/api/GetQuestExists/${questId}`);
    const questExists = await window.fetch(`/api/GetQuestExists/${questId}`);
    if (questExists) {
        const addPlayer = await window.fetch("/api/AddPlayer", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            questId: questId,
            playerId: store.playerId
            })
        })
        if (addPlayer.ok) {
            const linkWithQuestId = `${window.location.origin}/play/${store.questId}`;
            window.location.href = linkWithQuestId;
        } else {
            errorMessage.value = addPlayer.statusText;
        }
    }
    else {
        errorMessage.value = `${questId} quest was not found`;
    }
}

</script>

<template>
    <h1>Select and name your character</h1>
    <PlayersSection v-bind="{ useHealth:false, includeMonster:false, isPlayerSelect:true }" />
    <input type="text" v-model="store.playerId" placeholder="Character name" />
    <button @click="addPlayer">Start quest</button>
    <ErrorMessageSection :errorMessage=errorMessage />
</template>

<style scoped></style>
