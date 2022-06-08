<script setup lang="ts">
import { defineComponent, ref } from "vue";
import { generateQuestId } from '../util/questIdGenerator';
import PlayersSection from "./PlayersSection.vue";
import { gameStore } from "../stores";
import ErrorMessageSection from "./ErrorMessageSection.vue";
import { GamePhase } from "../types/GamePhase";

const store = gameStore();
const errorMessage = ref<String>("");

async function createQuest() {
    console.log("Start new Quest");
    const questId = generateQuestId();
    store.questId = questId;
    store.isHost = true;

    console.log(`1 - Quest: ${store.questId}`);
    const response = await window.fetch("/api/CreateQuest", {
        method: "POST",
        headers: {
            "Content-Type": "application/text"
        },
        body: questId
    });
    if (response.ok) {
        console.log(`2 - Quest: ${store.questId}`);
        store.phase = await response.text();
       
        navigator.clipboard.writeText(store.questId);
    }
}

async function joinQuest() {
    console.log("Join a Quest");
    if (store.questId)
    {
        console.log(`GetsQuestExist${store.questId}`);
        const response = await window.fetch(`/api/GetsQuestExist/${store.questId}/${store.phase}`);
        const data = await response.text();
        if (response.ok) {
            store.phase = data;
        } else {
            errorMessage.value = data;
        }
    } else {
        errorMessage.value = "Please enter a quest ID";
    }
}

function clearError()
{
    errorMessage.value = "";
}

</script>

<template>
    <h1>Serverless Websockets Quest</h1>
    <PlayersSection v-bind="{ useHealth:false, includeMonster:true, isPlayerSelect:false }" />
    <button @click="createQuest">Create quest</button>
    <br>or<br>
    <input type="text" v-model="store.questId" placeholder="quest ID" @input="clearError" />
    <button @click="joinQuest">Join quest</button>
    <ErrorMessageSection :errorMessage=errorMessage />
</template>

<style scoped></style>
