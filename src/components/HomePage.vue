<script setup lang="ts">
import { computed, defineComponent, ref, onMounted, ComputedRef } from "vue";
import { useStore } from "../store";
import FooterSection from "./FooterSection.vue";
import StartSection from "./StartSection.vue";
import CharacterSection from "./CharacterSection.vue";
import PlayGameSection from "./PlayGameSection.vue";
import EndSection from "./EndSection.vue";
import { GamePhase } from "../types/GamePhases";

const store = useStore();

function isStart() {
    return store.view === GamePhase.Start;
}

function isCharacterSelection() {
    return store.view === GamePhase.Character;
}

function isPlayGame() {
    return store.view === GamePhase.Play;
}

function isGameOver() {
    return store.view === GamePhase.End;
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
