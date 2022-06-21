<script setup lang="ts">
import Unit from "./Unit.vue";
import { gameStore } from "../stores";
import { CharacterClass } from "../types/CharacterClass";

import monsterIdle from "../assets/monster_idle.png"
import monsterAttack from "../assets/monster_attack.gif"
import monsterDead from "../assets/monster_dead.png"
import monsterDamage from "../assets/monster_damage.gif"
import monsterWin from "../assets/monster_win.gif"
import fighterIdle from "../assets/fighter_idle.png"
import fighterAttack from "../assets/fighter_attack.gif"
import fighterWin from "../assets/fighter_win.gif"
import rangerIdle from "../assets/ranger_idle.png"
import rangerAttack from "../assets/ranger_attack.gif"
import rangerWin from "../assets/ranger_win.gif"
import mageIdle from "../assets/mage_idle.png"
import mageAttack from "../assets/mage_attack.gif"
import mageWin from "../assets/mage_win.gif"

const props = defineProps({
    useHealth: Boolean,
    includeMonster: Boolean,
    isPlayerSelect: Boolean,
    showStats: Boolean,
});

function isMonsterActive(): boolean { return store.monster.name === store.currentPlayer && store.teamHasWon === undefined; }
function isFighterActive(): boolean { return store.fighter.name === store.currentPlayer && store.teamHasWon === undefined; }
function isRangerActive(): boolean { return store.ranger.name === store.currentPlayer && store.teamHasWon === undefined; }
function isMageActive(): boolean { return store.mage.name === store.currentPlayer && store.teamHasWon === undefined; }
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
     if (store.teamHasWon === false) {
        return monsterWin;
    } else if (store.monster.isDefeated) {
        return monsterDead;
    } else if (store.monster.isAttacking) {
        return monsterAttack;
    } else if (store.monster.isUnderAttack) {
        return monsterDamage;
   
    } else {
        return monsterIdle;
    }
};
function getFighterAsset(): string {
    if (store.teamHasWon) {
        return fighterWin;
    } else if (store.fighter.isDefeated) {
        return fighterIdle;
    } else if (store.fighter.isAttacking) {
        return fighterAttack;
    } else {
        return fighterIdle;
    }
};
function getRangerAsset(): string {
    if (store.teamHasWon) {
        return rangerWin;
    } else if (store.ranger.isDefeated) {
        return rangerIdle;
    } else if (store.ranger.isAttacking) {
        return rangerAttack;
    } else {
        return rangerIdle;
    }
};
function getMageAsset(): string {
     if (store.teamHasWon) {
        return mageWin;
    } else if (store.mage.isDefeated) {
        return mageIdle;
    } else if (store.mage.isAttacking) {
        return mageAttack;
    } else {
        return mageIdle;
    }
};

</script>

<template>
    <ul class="flex-container">
        <li v-if="includeMonster && !props.isPlayerSelect">    
            <Unit :unit="store.monster" :is-player-select="props.isPlayerSelect" :use-health="props.useHealth" :show-stats="props.showStats" />
        </li>
        <li>            
            <Unit :unit="store.fighter" :is-player-select="props.isPlayerSelect" :use-health="props.useHealth" :show-stats="props.showStats" />
        </li>
        <li>
            <Unit :unit="store.ranger" :is-player-select="props.isPlayerSelect" :use-health="props.useHealth" :show-stats="props.showStats" />
        </li>
        <li>            
            <Unit :unit="store.mage" :is-player-select="props.isPlayerSelect" :use-health="props.useHealth" :show-stats="props.showStats" />
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
</style>
