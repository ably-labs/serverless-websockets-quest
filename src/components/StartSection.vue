<script setup lang="ts">
import { defineComponent, ref } from "vue";
import { generateQuestId } from '../util/questIdGenerator';
import PlayersSection from "./PlayersSection.vue";
import { useStore } from "../store";
import ErrorMessageSection from "./ErrorMessageSection.vue";

const store = useStore();
const errorMessage = ref<String>("");

async function createQuest() {
    console.log("Start new Quest");
    let questId = generateQuestId();
    store.questId = questId;

    await window.fetch("/api/CreateQuest", {
        method: "POST",
        headers: {
            "Content-Type": "application/text"
        },
        body: questId
    });

    const linkWithQuestId = `${window.location.href}character/${questId}`;
    navigator.clipboard.writeText(linkWithQuestId);
    window.location.href = linkWithQuestId;
}

async function joinQuest() {
    console.log("Join a Quest");
    if (store.questId)
    {
        console.log(`/api/GetQuestExists/${store.questId}`);
        const result = await window.fetch(`/api/GetQuestExists/${store.questId}`);

        if (result.ok) {
            const linkWithQuestId = `${window.location.origin}/character/${store.questId}`;
            window.location.href = linkWithQuestId;
        } else {
            errorMessage.value = `${store.questId} quest was not found`;
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
