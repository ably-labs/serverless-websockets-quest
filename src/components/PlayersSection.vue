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

function updateName()
{

}

onMounted(() => {
    console.log("Mounted");
});

let monsterHealth: Number = 100;
let fighterHealth: Number = 50;
let rangerHealth: Number = 50;
let mageHealth: Number = 50;

let isMonsterActive: Boolean = false;
let isFighterActive: Boolean = false;
let isRangerActive: Boolean = false;
let isMageActive: Boolean = false;

</script>

<template>
	<ul v-if="!props.isPlayerSelect" class="flex-container">
		<li v-if="includeMonster">
			<p v-if="props.useHealth" class="health">{{ monsterHealth }}</p>
			<img class="small" alt="monster" src="../assets/Monster.png" />
			<figcaption v-bind:class="{ isActive: isMonsterActive }">{{ store.getMonsterName }}</figcaption>
		</li>
		<li>
			<p v-if="props.useHealth" class="health">{{ fighterHealth }}</p>
			<img class="small" alt="fighter" src="../assets/Fighter.png" />
            <figcaption v-bind:class="{ isActive: isFighterActive }">{{ store.getFighterName }}</figcaption>
		</li>
		<li>
			<p v-if="props.useHealth" class="health">{{ rangerHealth }}</p>
			<img class="small" alt="ranger" src="../assets/Ranger.png" />
            <figcaption v-bind:class="{ isActive: isRangerActive }">{{ store.getRangerName }}</figcaption>
		</li>
		<li>
			<p v-if="props.useHealth" class="health">{{ mageHealth }}</p>
			<img class="small" alt="mage" src="../assets/Mage.png" />
            <figcaption v-bind:class="{ isActive: isMageActive }">{{ store.getMageName }}</figcaption>
		</li>
	</ul>
	<ul v-if="props.isPlayerSelect" class="flex-container">
		<li>
			<input type="radio" id="fighter" name="character" checked value="fighter" v-model="store.character" />
			<label for="fighter">
                <img class="small" alt="fighter" src="../assets/Fighter.png"/>
                <figcaption>{{ store.getFighterName }}</figcaption>
            </label>
		</li>
		<li>
			<input type="radio" id="ranger" name="character" value="ranger" v-model="store.character" />
			<label for="ranger">
                <img class="small" alt="ranger" src="../assets/Ranger.png" />
                <figcaption>{{ store.getRangerName }}</figcaption>
            </label>
		</li>
		<li>
			<input type="radio" id="mage" name="character" value="mage" v-model="store.character"/>
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

input[type="radio"] {
	visibility: hidden;
}

input[type="radio"]:checked + label > img {
	border: 4px solid #ff55ff;
}

input[type="radio"] + label > img {
	border: 4px solid #000;
}

.health {
    color: #ff55ff;
}

.isActive {
    color: #ff55ff;
}

</style>
