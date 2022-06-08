<script setup lang="ts">
import { computed, defineComponent, ref, onMounted, ComputedRef } from "vue";
import { gameStore } from "../stores";
import FooterSection from "./FooterSection.vue";
import StartSection from "./StartSection.vue";
import CharacterSection from "./CharacterSection.vue";
import PlayGameSection from "./PlayGameSection.vue";
import EndSection from "./EndSection.vue";
import { GamePhase } from "../types/GamePhase";

const store = gameStore();

function isStart() {
    return store.phase === GamePhase.Start;
}

function isCharacterSelection() {
    return store.phase === GamePhase.Character;
}

function isPlayGame() {
    return store.phase === GamePhase.Play;
}

function isGameOver() {
    return store.phase === GamePhase.End;
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
