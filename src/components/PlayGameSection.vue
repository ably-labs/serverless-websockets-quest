<script setup lang="ts">
import { defineComponent, ref } from "vue";
import { gameStore } from "../stores";
import PlayersSection from "./PlayersSection.vue";

const store = gameStore();

async function fight() {
    console.log("Fight");
}

let isPlayerTurn: Boolean = false;

</script>

<template>
    <h1>Quest: <span class="pink">{{ store.questId }}</span></h1>
    <h2>You encouter a monster!</h2>
    <PlayersSection v-bind="{ useHealth:true, includeMonster:true, isPlayerSelect:false }" />
    <button v-if="store.isPlayerTurn" @click="fight">Attack</button>
    <p v-if="!store.isPlayerTurn" class="message">Wait for your turn</p>
    <div class="container">
        <ul class="messages">
            <li v-for="message in store.messages">{{ message }}</li>
        </ul>
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
