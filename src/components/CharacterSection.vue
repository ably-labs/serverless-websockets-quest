<script setup lang="ts">
import { defineComponent, ref } from "vue";

async function startQuest() {
    console.log("Start Quest");
}

</script>

<template>
    <h1>Chose your character</h1>
    <img class="small" alt="monster" src="../assets/Fighter.png" />
    <img class="small" alt="monster" src="../assets/Ranger.png" />
    <img class="small" alt="monster" src="../assets/Mage.png" />

    <button @click="startQuest">Start quest</button>

</template>

<style scoped></style>
