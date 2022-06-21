<script setup lang="ts">
import PlayerUnit from "./PlayerUnit.vue";
import { gameStore } from "../stores";

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
            <PlayerUnit :player="store.monster" :is-player-select="props.isPlayerSelect" :use-health="props.useHealth" :show-stats="props.showStats" />
        </li>
        <li>
            <PlayerUnit :player="store.fighter" :is-player-select="props.isPlayerSelect" :use-health="props.useHealth" :show-stats="props.showStats" />
        </li>
        <li>
            <PlayerUnit :player="store.ranger" :is-player-select="props.isPlayerSelect" :use-health="props.useHealth" :show-stats="props.showStats" />
        </li>
        <li>
            <PlayerUnit :player="store.mage" :is-player-select="props.isPlayerSelect" :use-health="props.useHealth" :show-stats="props.showStats" />
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
