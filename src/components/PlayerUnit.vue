<script setup lang="ts">
import { gameStore } from "../stores";
import { Player } from "../types/Player";
import { CharacterClass } from "../types/CharacterClass";

const props = defineProps({
    player: Object,
    isPlayerSelect: Boolean,
    useHealth: Boolean,
    showStats: Boolean,
});

const store = gameStore();
const targetPlayer: Player = props.player as Player;

function isActive(): boolean { return targetPlayer.name === store.currentPlayer; }
function isDisabled(): boolean { return !targetPlayer.isAvailable || store.isPlayerAdded; }
function showDamage(): boolean { return targetPlayer.damage > 0; }
function getName(): string { return targetPlayer.characterClass === store.characterClass && store.playerName !== "" ? store.playerName : targetPlayer.name; }

function getAsset(): string {
    if (store.teamHasWon && (targetPlayer.characterClass !== CharacterClass.Monster) || (store.teamHasWon === false && targetPlayer.characterClass === CharacterClass.Monster)) {
        return targetPlayer.assets.win;
    } else if (targetPlayer.isDefeated) {
        return targetPlayer.assets.dead;
    } else if (targetPlayer.isAttacking) {
        return targetPlayer.assets.attack;
    } else if (targetPlayer.isUnderAttack && targetPlayer.characterClass === CharacterClass.Monster) {
        return targetPlayer.assets.damage;
    } else {
        return targetPlayer.assets.idle;
    }
};
</script>

<template>

    <div v-if="!props.isPlayerSelect">
        <p v-if="props.useHealth">
            <span class="health">{{ targetPlayer.health }} HP</span>
            <span class="damage" v-if="showDamage()">-{{ targetPlayer.damage }}</span>
        </p>
        <p class="stats" v-if="props.showStats">
            <span>Damage caused:</span>
            <span class="info">{{ targetPlayer.totalDamageApplied }}</span>
        </p>
        <img v-bind:class="{ isActive: isActive(), isDefeated: targetPlayer.isDefeated }" :alt="targetPlayer.characterClass" :src="getAsset()" />
        <figcaption>{{ targetPlayer.name }}</figcaption>
    </div>

    <div v-if="props.isPlayerSelect">
        <input type="radio" :id="targetPlayer.characterClass" name="character" :value="targetPlayer.characterClass" v-model="store.characterClass" @click="store.playerName=getName()" :disabled="isDisabled()" />
        <label :for="targetPlayer.characterClass">
            <img :alt="targetPlayer.characterClass" :src="getAsset()" />
            <figcaption>{{ getName() }}</figcaption>
        </label>
    </div>

</template>

<style scoped>
.flex-container{
    padding: 0;
    gap: 20px;
    margin: 10px;
    list-style: none;
    display: flex;
    justify-content: center;
    flex-wrap: wrap;
    align-items: flex-start ;
}

figcaption, img {
    display: flex;
    justify-content: center;
    padding: 5px
}

img {
    border: 4px solid transparent;
}

input[type=radio] {
    visibility: hidden;
}

input[type=radio]:checked + label > img {
    border: 4px solid #ff55ff;
}

input[type=radio] + label > img {
    border: 4px solid #55ffff;
}

input[type=radio]:disabled + label > img {
    border: 4px solid #000;
}

/* input[type=radio]:disabled:checked + label > img {
  border-color: #fff;
} */

.health {
    color: #ff55ff;
    padding: 5px;
    margin: 5px;
}

.damage {
    background-color: #ff55ff;
    color: #000;
    padding: 5px;
    margin: 5px;
}

.stats {
    font-size: smaller;
}

.isActive {
    border: 4px solid #55ffff;
}

.isDefeated {
    filter: grayscale(100%);
}
</style>
