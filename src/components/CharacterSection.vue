<script setup lang="ts">
import { ref } from "vue";
import PlayersSection from "./PlayersSection.vue";
import { gameStore } from "../stores";
import ErrorMessageSection from "./ErrorMessageSection.vue";
import { CharacterClass } from "../types/CharacterClass";

const store = gameStore();
const errorMessage = ref<string>("");

async function addPlayer() {
    let selectedCharacter: string = "";
    if (store.characterClass === CharacterClass.Fighter && !store.fighter.isAvailable) {
        selectedCharacter = CharacterClass.Fighter;
    } else if (store.characterClass === CharacterClass.Ranger && !store.ranger.isAvailable) {
        selectedCharacter = CharacterClass.Ranger;
    } else if (store.characterClass === CharacterClass.Mage && !store.mage.isAvailable) {
        selectedCharacter = CharacterClass.Mage;
    }
    if (selectedCharacter) {
        errorMessage.value = `The ${selectedCharacter} is already chosen by another player. Please select another character.`;
        return;
    } else {
        errorMessage.value = "";
    }

    if (store.players.includes(store.playerName)) {
        errorMessage.value = `The name ${store.playerName} is already chosen by another player. Please choose another name.`;
        return
    } else {
        errorMessage.value = "";
    }

    store.isPlayerAdded = true;
    const questExistsResponse = await window.fetch(`/api/GetQuestExists/${store.questId}`);
    const questExistsMessage = await questExistsResponse.text();
    if (questExistsResponse.ok) {
        await window.fetch("/api/AddPlayer", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            questId: store.questId,
            playerName: store.playerName,
            characterClass: store.characterClass
            })
        });
    }
    else {
        errorMessage.value = questExistsMessage;
    }
}

function numberOfPlayersRemaining(): number { return  4 - store.players.length; }

function isAddDisabled(): boolean {
    if (store.playerName === "" && !store.isPlayerAdded) {
        return true;
    } else if (store.playerName !== "" && !store.isPlayerAdded) {
        return false;
    } else {
        return true;
    }
}

</script>

<template>
    <h1>Quest: <span class="pink">{{ store.questId }}</span></h1>
    <p class="info" v-if="store.isHost">Quest ID has been copied to your clipboard! Send this to two other players so they can join.</p>
    <h2>Select and name your character</h2>
    <PlayersSection v-bind="{ useHealth:false, includeMonster:false, isPlayerSelect:true, showStats:false }" />
    <div v-if="!store.isPlayerAdded">
        <input type="text" v-model="store.playerName" :disabled="isAddDisabled()" placeholder="Character name" />
        <button @click="addPlayer" :disabled="isAddDisabled()">Add player</button>
    </div>
    <p class="message">Waiting for {{ numberOfPlayersRemaining() }} player(s) to join.</p>
    <ErrorMessageSection :errorMessage=errorMessage />
</template>
