<script setup lang="ts">
import { gameStore } from "../stores";
import { Player } from "../types/Player";
import { CharacterClass } from "../types/CharacterClass";

const props = defineProps({
    unit: Object,
    isPlayerSelect: Boolean,
    useHealth: Boolean,
    showStats: Boolean,
});

const store = gameStore();
const targetUnit: Player = props.unit as Player;

function isActive(): boolean { return targetUnit.name === store.currentPlayer; }
function isDisabled(): boolean { return !targetUnit.isAvailable || store.isPlayerAdded; }
function showDamage(): boolean { return targetUnit.damage > 0; }
function getName(): string { return targetUnit.characterClass === store.characterClass && store.playerName !== "" ? store.playerName : targetUnit.name; }

function getAsset(): string {
    const assetPrefix = `/src/assets/${targetUnit.characterClass}`;

    if (targetUnit.isDefeated) {
        return `${assetPrefix}_dead.png`;
    }
    else if (targetUnit.isAttacking) {
        return `${assetPrefix}_attack.gif`;
    } else if (targetUnit.isUnderAttack && targetUnit.characterClass === "monster") {
        return `${assetPrefix}_damage.gif`;
    } else {
        return `${assetPrefix}_idle.png`;
    }
};
</script>

<template>

    <div v-if="!props.isPlayerSelect">
        <p v-if="props.useHealth">
            <span class="health">{{ targetUnit.health }} HP</span>
            <span class="damage" v-if="showDamage()">-{{ targetUnit.damage }}</span>
        </p>
        <p class="stats" v-if="props.showStats">
            <span>Damage caused:</span>
            <span class="info">{{ targetUnit.totalDamageApplied }}</span>
        </p>
        <img v-bind:class="{ isActive: isActive(), isDefeated: targetUnit.isDefeated }" :alt="targetUnit.characterClass" :src="getAsset()" />
        <figcaption>{{ targetUnit.name }}</figcaption>
    </div>

    <div v-if="props.isPlayerSelect">
        <input type="radio" :id="targetUnit.characterClass" name="character" :value="targetUnit.characterClass" v-model="store.characterClass" @click="store.playerName=getName()" :disabled="isDisabled()" />
        <label :for="targetUnit.characterClass">
            <img :alt="targetUnit.characterClass" :src="getAsset()" />
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
