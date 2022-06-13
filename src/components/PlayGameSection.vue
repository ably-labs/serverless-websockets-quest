<script setup lang="ts">
import { defineComponent, ref } from "vue";
import { gameStore } from "../stores";
import PlayersSection from "./PlayersSection.vue";

const store = gameStore();

async function fight() {
    console.log("Fight");
}

</script>

<template>
    <h1>Quest: <span class="pink">{{ store.questId }}</span></h1>
    <h2>{{ store.title }}</h2>
    <PlayersSection v-bind="{ useHealth:true, includeMonster:true, isPlayerSelect:false }" />
    <div class=flex-container>
        <button v-if="store.isPlayerTurn" @click="fight">Attack</button>
        <p v-if="!store.isPlayerTurn" class="message">{{ store.message }}</p>
    </div> 
</template>

<style scoped>

.message {
    color: #55ffff;
    margin: 10px;
    padding: 10px;
}
.message::before {
    content: "<< ";
}

.message::after {
    content: " >>";
}

</style>
