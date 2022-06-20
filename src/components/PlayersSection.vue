<script setup lang="ts">
import { defineComponent, onMounted, ref } from "vue";
import { gameStore } from "../stores";
import { CharacterClass } from "../types/CharacterClass";
import monsterIdle from "../assets/monster_idle.png"
import monsterAttack from "../assets/monster_attack.gif"
import monsterDead from "../assets/monster_dead.png"
import monsterDamage from "../assets/monster_damage.gif"
import fighterIdle from "../assets/fighter_idle.png"
import fighterAttack from "../assets/fighter_attack.gif"
import rangerIdle from "../assets/ranger_idle.png"
import rangerAttack from "../assets/ranger_attack.gif"
import mageIdle from "../assets/mage_idle.png"
import mageAttack from "../assets/mage_attack.gif"

const store = gameStore();

const props = defineProps({
    useHealth: Boolean,
    includeMonster: Boolean,
    isPlayerSelect: Boolean,
    showStats: Boolean,
});

function isMonsterActive(): boolean { return store.monster.name === store.currentPlayer; }
function isFighterActive(): boolean { return store.fighter.name === store.currentPlayer; }
function isRangerActive(): boolean { return store.ranger.name === store.currentPlayer; }
function isMageActive(): boolean { return store.mage.name === store.currentPlayer; }
function isFighterDisabled(): boolean { return !store.fighter.isAvailable || store.isPlayerAdded; }
function isRangerDisabled(): boolean { return !store.ranger.isAvailable || store.isPlayerAdded; }
function isMageDisabled(): boolean { return !store.mage.isAvailable || store.isPlayerAdded; }
function showMonsterDamage(): boolean { return store.monster.damage > 0; }
function showFighterDamage(): boolean { return store.fighter.damage > 0; }
function getFighterName(): string { return store.characterClass === CharacterClass.Fighter && store.playerName !== "" ? store.playerName : store.fighter.name; }
function showRangerDamage(): boolean { return store.ranger.damage > 0; }
function getRangerName(): string { return store.characterClass === CharacterClass.Ranger && store.playerName !== "" ? store.playerName : store.ranger.name; }
function showMageDamage(): boolean { return store.mage.damage > 0; }
function getMageName(): string { return store.characterClass === CharacterClass.Mage && store.playerName !== "" ? store.playerName : store.mage.name; }
function getMonsterAsset(): string {
    if (store.monster.isDefeated) {
        return monsterDead;
    }
    else if (store.monster.isAttacking) {
        return monsterAttack;
    } else if (store.monster.isUnderAttack) {
        return monsterDamage;
    } else {
        return monsterIdle;
    }
};
function getFighterAsset(): string {
    if (store.fighter.isDefeated) {
        return fighterIdle;
    }
    else if (store.fighter.isAttacking) {
        return fighterAttack;
    } else {
        return fighterIdle;
    }
};
function getRangerAsset(): string {
    if (store.ranger.isDefeated) {
        return rangerIdle;
    }
    else if (store.ranger.isAttacking) {
        return rangerAttack;
    } else {
        return rangerIdle;
    }
};
function getMageAsset(): string {
    if (store.mage.isDefeated) {
        return mageIdle;
    }
    else if (store.mage.isAttacking) {
        return mageAttack;
    } else {
        return mageIdle;
    }
};

</script>

<template>
    <ul v-if="!props.isPlayerSelect" class="flex-container">
        <li v-if="includeMonster">
            <p v-if="props.useHealth">
                <span class="health">{{ store.monster.health }} HP</span>
                <span class="damage" v-if="showMonsterDamage()">-{{ store.monster.damage }}</span>
            </p>
            <p class="stats" v-if="props.showStats">
                <span>Damage caused:</span>
                <span class="info">{{ store.monster.totalDamageApplied }}</span>
            </p>
            <img v-bind:class="{ isActive: isMonsterActive(), isDefeated: store.monster.isDefeated }" alt="monster" :src="getMonsterAsset()" />
            <figcaption>{{ store.monster.name }}</figcaption>
        </li>
        <li>
            <p v-if="props.useHealth">
                <span class="health">{{ store.fighter.health }} HP</span>
                <span class="damage" v-if="showFighterDamage()">-{{ store.fighter.damage }}</span>
            </p>
            <p class="stats" v-if="props.showStats">
                <span>Damage caused:</span>
                <span class="info">{{ store.fighter.totalDamageApplied }}</span>
            </p>
            <img v-bind:class="{ isActive: isFighterActive(), isDefeated: store.fighter.isDefeated }" alt="fighter" :src="getFighterAsset()" />
            <figcaption>{{ store.fighter.name }}</figcaption>
        </li>
        <li>
            <p v-if="props.useHealth">
                <span class="health">{{ store.ranger.health }} HP</span>
                <span class="damage" v-if="showRangerDamage()">-{{ store.ranger.damage }}</span>
            </p>
            <p class="stats" v-if="props.showStats">
                <span>Damage caused:</span>
                <span class="info">{{ store.ranger.totalDamageApplied }}</span>
            </p>
            <img v-bind:class="{ isActive: isRangerActive(), isDefeated: store.ranger.isDefeated }" alt="ranger" :src="getRangerAsset()" />
            <figcaption>{{ store.ranger.name }}</figcaption>
        </li>
        <li>
            <p v-if="props.useHealth">
                <span class="health">{{ store.mage.health }} HP</span>
                <span class="damage" v-if="showMageDamage()">-{{ store.mage.damage }}</span>
            </p>
            <p class="stats" v-if="props.showStats">
                <span>Damage caused:</span>
                <span class="info">{{ store.mage.totalDamageApplied }}</span>
            </p>
            <img v-bind:class="{ isActive: isMageActive(), isDefeated: store.mage.isDefeated }" alt="mage" :src="getMageAsset()" />
            <figcaption>{{ store.mage.name }}</figcaption>
        </li>
    </ul>
    <ul v-if="props.isPlayerSelect" class="flex-container">
        <li>
            <input type="radio" id="fighter" name="character" value="fighter" v-model="store.characterClass" @click="store.playerName=getFighterName()" :disabled="isFighterDisabled()" />
            <label for="fighter">
                <img alt="fighter" :src="getFighterAsset()" />
                <figcaption>{{ getFighterName() }}</figcaption>
            </label>
        </li>
        <li>
            <input type="radio" id="ranger" name="character" value="ranger" v-model="store.characterClass" @click="store.playerName=getRangerName()" :disabled="isRangerDisabled()" />
            <label for="ranger">
                <img alt="ranger" :src="getRangerAsset()" />
                <figcaption>{{ getRangerName() }}</figcaption>
            </label>
        </li>
        <li>
            <input type="radio" id="mage" name="character" value="mage" v-model="store.characterClass" @click="store.playerName=getMageName()" :disabled="isMageDisabled()" />
            <label for="mage">
                <img alt="mage" :src="getMageAsset()"/>
                <figcaption>{{ getMageName() }}</figcaption>
            </label>
        </li>
    </ul>
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
