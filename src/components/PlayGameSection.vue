<script setup lang="ts">
import { defineComponent, ref } from "vue";

async function fight() {
    console.log("Fight");
}

</script>

<template>
    <h1>You encouter a monster!</h1>
    <img class="small" alt="monster" src="../assets/Monster.png" />
    <img class="small" alt="fighter" src="../assets/Fighter.png" />
    <img class="small" alt="ranger" src="../assets/Ranger.png" />
    <img class="small" alt="mage" src="../assets/Mage.png" />

    <button @click="fight">Fight</button>

</template>

<style scoped></style>
