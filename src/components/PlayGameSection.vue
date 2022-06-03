<script setup lang="ts">
import { defineComponent, ref } from "vue";
import PlayersSection from "./PlayersSection.vue";

async function fight() {
    console.log("Fight");
}

</script>

<template>
    <h1>You encouter a monster!</h1>
    <PlayersSection v-bind="{ useHealth:true, includeMonster:true, isPlayerSelect:false }" />

    <button @click="fight">Fight</button>

</template>

<style scoped></style>
