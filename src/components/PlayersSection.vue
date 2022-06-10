<script setup lang="ts">
import { defineComponent, onMounted, ref } from "vue";
import { gameStore } from "../stores";

const store = gameStore();

const props = defineProps({
	useHealth: Boolean,
	includeMonster: Boolean,
	isPlayerSelect: Boolean
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
			<p v-if="props.useHealth">
				<span class="health">{{ store.monster.health }}</span>
				<span class="damage">{{ store.getMonsterDamage }}</span>
			</p>
			<img class="small" alt="monster" src="../assets/Monster.png" />
			<figcaption v-bind:class="{ isActive: isMonsterActive }">{{ store.monster.name }}</figcaption>
		</li>
		<li>
			<p v-if="props.useHealth">
				<span class="health">{{ store.fighter.health }}</span>
				<span class="damage">{{ store.getFighterDamage }}</span>
			</p>
			<img class="small" alt="fighter" src="../assets/Fighter.png" />
            <figcaption v-bind:class="{ isActive: isFighterActive }">{{ store.fighter.name }}</figcaption>
		</li>
		<li>
			<p v-if="props.useHealth">
				<span class="health">{{ store.ranger.health }}</span>
				<span class="damage">{{ store.getRangerDamage }}</span>
			</p>
			<img class="small" alt="ranger" src="../assets/Ranger.png" />
            <figcaption v-bind:class="{ isActive: isRangerActive }">{{ store.ranger.name }}</figcaption>
		</li>
		<li>
			<p v-if="props.useHealth">
				<span class="health">{{ store.mage.health }}</span>
				<span class="damage">{{ store.getMageDamage }}</span>
			</p>
			<img class="small" alt="mage" src="../assets/Mage.png" />
            <figcaption v-bind:class="{ isActive: isMageActive }">{{ store.mage.name }}</figcaption>
		</li>
	</ul>
	<ul v-if="props.isPlayerSelect" class="flex-container">
		<li>
			<input type="radio" id="fighter" name="character" checked value="fighter" v-model="store.characterClass" :disabled="store.isFighterDisabled" />
			<label for="fighter">
                <img class="small" alt="fighter" src="../assets/Fighter.png"/>
                <figcaption>{{ store.getFighterName }}</figcaption>
            </label>
		</li>
		<li>
			<input type="radio" id="ranger" name="character" value="ranger" v-model="store.characterClass" :disabled="store.isRangerDisabled" />
			<label for="ranger">
                <img class="small" alt="ranger" src="../assets/Ranger.png" />
                <figcaption>{{ store.getRangerName }}</figcaption>
            </label>
		</li>
		<li>
			<input type="radio" id="mage" name="character" value="mage" v-model="store.characterClass" :disabled="store.isMageDisabled" />
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

input[type=radio]:disabled + label > img {
  border-color: #55ffff;
}

input[type=radio]:disabled:checked + label > img {
  border-color: #fff;
}

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

.isActive {
    color: #ff55ff;
}

</style>
