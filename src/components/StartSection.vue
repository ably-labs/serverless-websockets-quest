<script setup lang="ts">
import { defineComponent, ref } from "vue";
import { generateQuestId } from '../util/questIdGenerator';
import PlayersSection from "./PlayersSection.vue";

const props = defineProps({
  questId: { type: String, required: false }
})

async function createQuest() {
    console.log("Start new Quest");
    const questId = generateQuestId();
    window.location.href = "/character";

    
    // TODO: Generate a playerId
    // await window.fetch("/api/CreateQuest", {
    //     method: "POST",
    //     headers: {
    //         "Content-Type": "application/json"
    //     },
    //     body: JSON.stringify({
    //         "questId": generateQuestId(),
    //         "playerId": "player1"
    //     })
    //});
}

async function joinQuest() {
    console.log("Join a Quest");
    // Read questId from window location
    // await this.$router.replace({
    //     path: '/',
    //     query: { sessionId: this.sessionId },
    //   });
    //   navigator.clipboard.writeText(window.location.href);
    await window.fetch("/api/CreateQuest", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            "questId": generateQuestId(),
            "playerId": "player1"
        })
    });
}
</script>

<template>
    <h1>Serverless Websockets Quest</h1>
    <PlayersSection v-bind="{ useHealth:false, includeMonster:true, isPlayerSelect:false }" />
    <button @click="createQuest">Create quest</button>
    <br>or<br>
    <input type="text" v-model="questId" placeholder="quest ID" />
    <button @click="joinQuest">Join quest</button>
</template>

<style scoped></style>
