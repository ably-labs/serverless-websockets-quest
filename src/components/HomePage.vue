<script setup lang="ts">
import { computed, defineComponent, ref, onMounted, ComputedRef } from "vue";
import FooterSection from "./FooterSection.vue";
import StartSection from "./StartSection.vue";
import CharacterSection from "./CharacterSection.vue";
import PlayGameSection from "./PlayGameSection.vue";
import EndSection from "./EndSection.vue";

function isStart() {
    return !isCharacterSelection() && !isGameOver() && !isPlayGame();
}

function isCharacterSelection() {
    return window.location.pathname.endsWith("character");
}

function isPlayGame() {
    return window.location.pathname.includes("play");
}

function isGameOver() {
    return window.location.pathname.endsWith("end");
}

</script>

<template>
    <StartSection v-if="isStart()" />
    <CharacterSection v-if="isCharacterSelection()" />
    <PlayGameSection v-if="isPlayGame()" />
    <EndSection v-if="isGameOver()" />
    <FooterSection />
</template>

<style scoped></style>
