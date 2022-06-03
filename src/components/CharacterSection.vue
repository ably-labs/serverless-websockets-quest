<script setup lang="ts">
import { defineComponent, ref } from "vue";
import PlayersSection from "./PlayersSection.vue";

async function startQuest() {
    console.log("Start Quest");
}

</script>

<template>
    <h1>Chose your character</h1>
    <PlayersSection v-bind="{ useHealth:false, includeMonster:false, isPlayerSelect:true }" />
    <input type="text" v-model="playerId" placeholder="Character name" />
    <button @click="joinQuest">Join quest</button>

</template>

<style scoped></style>
