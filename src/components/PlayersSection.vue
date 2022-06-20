<script setup lang="ts">
import { defineComponent, onMounted, ref } from "vue";
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
    <ul v-if="!props.isPlayerSelect" class="flex-container">
        <li v-if="includeMonster">
            <p v-if="props.useHealth">
                <span class="health">{{ store.monster.health }} HP</span>
                <span class="damage" v-if="store.showMonsterDamage">-{{ store.monster.damage }}</span>
            </p>
            <p class="stats" v-if="props.showStats">
                <span></span>Damage caused:
                <span class="info">{{ store.monster.totalDamageApplied }}</span>
            </p>
            <img v-bind:class="{ isActive: store.isMonsterActive, isDefeated: store.monster.isDefeated }" alt="monster" :src="store.getMonsterAsset" />
            <figcaption>{{ store.monster.name }}</figcaption>
        </li>
        <li>
            <p v-if="props.useHealth">
                <span class="health">{{ store.fighter.health }} HP</span>
                <span class="damage" v-if="store.showFighterDamage">-{{ store.fighter.damage }}</span>
            </p>
            <p class="stats" v-if="props.showStats">
                <span></span>Damage caused:
                <span class="info">{{ store.fighter.totalDamageApplied }}</span>
            </p>
            <img v-bind:class="{ isActive: store.isFighterActive, isDefeated: store.fighter.isDefeated }" alt="fighter" :src="store.getFighterAsset" />
            <figcaption>{{ store.fighter.name }}</figcaption>
        </li>
        <li>
            <p v-if="props.useHealth">
                <span class="health">{{ store.ranger.health }} HP</span>
                <span class="damage" v-if="store.showRangerDamage">-{{ store.ranger.damage }}</span>
            </p>
            <p class="stats" v-if="props.showStats">
                <span></span>Damage caused:
                <span class="info">{{ store.ranger.totalDamageApplied }}</span>
            </p>
            <img v-bind:class="{ isActive: store.isRangerActive, isDefeated: store.ranger.isDefeated }" alt="ranger" :src="store.getRangerAsset" />
            <figcaption>{{ store.ranger.name }}</figcaption>
        </li>
        <li>
            <p v-if="props.useHealth">
                <span class="health">{{ store.mage.health }} HP</span>
                <span class="damage" v-if="store.showMageDamage">-{{ store.mage.damage }}</span>
            </p>
            <p class="stats" v-if="props.showStats">
                <span></span>Damage caused:
                <span class="info">{{ store.mage.totalDamageApplied }}</span>
            </p>
            <img v-bind:class="{ isActive: store.isMageActive, isDefeated: store.mage.isDefeated }" alt="mage" :src="store.getMageAsset" />
            <figcaption>{{ store.mage.name }}</figcaption>
        </li>
    </ul>
    <ul v-if="props.isPlayerSelect" class="flex-container">
        <li>
            <input type="radio" id="fighter" name="character" value="fighter" v-model="store.characterClass" @click="store.playerName=store.getFighterName" :disabled="store.isFighterDisabled" />
            <label for="fighter">
                <img alt="fighter" :src="store.getFighterAsset" />
                <figcaption>{{ store.getFighterName }}</figcaption>
            </label>
        </li>
        <li>
            <input type="radio" id="ranger" name="character" value="ranger" v-model="store.characterClass" @click="store.playerName=store.getRangerName" :disabled="store.isRangerDisabled" />
            <label for="ranger">
                <img alt="ranger" :src="store.getRangerAsset" />
                <figcaption>{{ store.getRangerName }}</figcaption>
            </label>
        </li>
        <li>
            <input type="radio" id="mage" name="character" value="mage" v-model="store.characterClass" @click="store.playerName=store.getMageName" :disabled="store.isMageDisabled" />
            <label for="mage">
                <img alt="mage" :src="store.getMageAsset"/>
                <figcaption>{{ store.getMageName }}</figcaption>
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
