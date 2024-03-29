<script setup lang="ts">
    import { ref } from "vue";
    import { v4 as uuidv4 } from "uuid";
    import { generateQuestId } from '../util/questIdGenerator';
    import PlayersSection from "./PlayersSection.vue";
    import { gameStore } from "../stores";
    import ErrorMessageSection from "./ErrorMessageSection.vue";

    const store = gameStore();
    const errorMessage = ref<String>("");

    async function createQuest() {
        store.questId = generateQuestId();
        store.clientId = uuidv4();
        store.isHost = true;
        await store.createRealtimeConnection(store.clientId, store.questId);
        const response = await window.fetch("/api/CreateQuest", {
            method: "POST",
            headers: {
                "Content-Type": "application/text"
            },
            body: store.questId
        });
        if (response.ok) {
            store.phase = await response.text();
            navigator.clipboard.writeText(store.questId);
        }
    }

    async function joinQuest() {
        if (store.questId)
        {
            const response = await window.fetch(`/api/GetQuestExists/${store.questId}`);
            const data = await response.text();
            if (response.ok) {
                store.clientId = uuidv4();
                await store.createRealtimeConnection(store.clientId, store.questId);
                store.phase = data;
            } else {
                errorMessage.value = data;
            }
        } else {
            errorMessage.value = "Please enter a quest ID";
        }
    }

    function clearError() {
        errorMessage.value = "";
    }
</script>

<template>
    <h1>Serverless Websockets Quest</h1>
    <PlayersSection v-bind="{ useHealth:false, includeMonster:true, isPlayerSelect:false, showStats:false }" />
    <div v-if="!store.isHost">
        <button @click="createQuest">Start quest</button>
        <br>or<br>
        <input type="text" v-model="store.questId" placeholder="quest ID" @input="clearError" />
        <button @click="joinQuest">Join quest</button>
    </div>
    <p class="info">This game requires 3 people to play. You can simulate multiple players yourself by opening more tabs, one for each player.</p>
    <div v-if="store.isHost">
        <p class="message" >We're rolling the dice to setup your quest...</p>
    </div>
    <ErrorMessageSection :errorMessage="errorMessage" />
</template>
