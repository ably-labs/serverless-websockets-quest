<script setup lang="ts">
import { defineComponent, onMounted, ref } from "vue";
import { gameStore } from "../stores";

const store = gameStore();

const props = defineProps({
	useHealth: Boolean,
	includeMonster: Boolean,
	isPlayerSelect: Boolean,
    playerId: String,
});

async function fight() {
	console.log("Fight");
}

let isMonsterActive: Boolean = false;
let isFighterActive: Boolean = false;
let isRangerActive: Boolean = false;
let isMageActive: Boolean = false;

</script>

<template>
	<ul v-if="!props.isPlayerSelect" class="flex-container">
		<li v-if="includeMonster">
			<p v-if="props.useHealth" class="health">{{ store.monster.health }}</p>
			<img class="small" alt="monster" src="../assets/Monster.png" />
			<figcaption v-bind:class="{ isActive: isMonsterActive }">{{ store.monster.name }}</figcaption>
		</li>
		<li>
			<p v-if="props.useHealth" class="health">{{ store.fighter.health }}</p>
			<img class="small" alt="fighter" src="../assets/Fighter.png" />
            <figcaption v-bind:class="{ isActive: isFighterActive }">{{ store.fighter.name }}</figcaption>
		</li>
		<li>
			<p v-if="props.useHealth" class="health">{{ store.ranger.health }}</p>
			<img class="small" alt="ranger" src="../assets/Ranger.png" />
            <figcaption v-bind:class="{ isActive: isRangerActive }">{{ store.ranger.name }}</figcaption>
		</li>
		<li>
			<p v-if="props.useHealth" class="health">{{ store.mage.health }}</p>
			<img class="small" alt="mage" src="../assets/Mage.png" />
            <figcaption v-bind:class="{ isActive: isMageActive }">{{ store.mage.name }}</figcaption>
		</li>
	</ul>
	<ul v-if="props.isPlayerSelect" class="flex-container">
		<li>
			<input type="radio" id="fighter" name="character" checked value="fighter" v-model="store.characterClass" :disabled="store.isPlayerAdded" />
			<label for="fighter">
                <img class="small" alt="fighter" src="../assets/Fighter.png"/>
                <figcaption>{{ store.getFighterName }}</figcaption>
            </label>
		</li>
		<li>
			<input type="radio" id="ranger" name="character" value="ranger" v-model="store.characterClass" :disabled="store.isPlayerAdded" />
			<label for="ranger">
                <img class="small" alt="ranger" src="../assets/Ranger.png" />
                <figcaption>{{ store.getRangerName }}</figcaption>
            </label>
		</li>
		<li>
			<input type="radio" id="mage" name="character" value="mage" v-model="store.characterClass" :disabled="store.isPlayerAdded" />
			<label for="mage">
                <img class="small" alt="mage" src="../assets/Mage.png"/>
                <figcaption>{{ store.getMageName }}</figcaption>
            </label>
		</li>
	</ul>
</template>

<style scoped>
.flex-container{
	padding: 0;
	margin: 10px;
	list-style: none;
	display: flex;
	justify-content: center;
	flex-wrap: wrap;
}

figcaption, img {
    display: flex;
    justify-content: center;
}

input[type=radio] {
	visibility: hidden;
}

input[type=radio]:checked + label > img {
	border: 4px solid #ff55ff;
}

input[type=radio] + label > img {
	border: 4px solid #000;
}

input[type=radio]:disabled:checked + label > img {
  border-color: #55ffff;
}

.health {
    color: #ff55ff;
}

.isActive {
    color: #ff55ff;
}

</style>
