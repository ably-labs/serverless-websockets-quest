<script setup lang="ts">
import { defineComponent, ref } from "vue";
import { useStore } from "../store";
import PlayersSection from "./PlayersSection.vue";

const store = useStore();

async function fight() {
    console.log("Fight");
}

let isPlayerTurn: Boolean = false;

</script>

<template>
    <h1>Quest: <span class="pink">{{ store.questId }}</span></h1>
    <h2>You encouter a monster!</h2>
    <PlayersSection v-bind="{ useHealth:true, includeMonster:true, isPlayerSelect:false }" />
    <div class=flex-container>
        <button v-if="isPlayerTurn" @click="fight">Attack</button>
        <p v-if="!isPlayerTurn" class="message">Waiting for your turn</p>
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
