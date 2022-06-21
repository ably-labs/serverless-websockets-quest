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

const store = gameStore();

const props = defineProps({
    useHealth: Boolean,
    includeMonster: Boolean,
    isPlayerSelect: Boolean,
    showStats: Boolean,
});

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
