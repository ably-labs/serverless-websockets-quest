<script setup lang="ts">
import { defineComponent, ref } from "vue";
import PlayersSection from "./PlayersSection.vue";
import { useStore } from "../store";

const store = useStore();

async function joinQuest() {
    console.log("Join Quest");
}


let playerId: String;

</script>

<template>
    <h1>Select and name your character</h1>
    <PlayersSection v-bind="{ useHealth:false, includeMonster:false, isPlayerSelect:true }" />
    <input type="text" v-model="store.playerId" placeholder="Character name" />
    <button @click="joinQuest">Join quest</button>

</template>

<style scoped></style>
