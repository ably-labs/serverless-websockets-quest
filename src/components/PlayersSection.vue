<script setup lang="ts">
import { defineComponent, ref } from "vue";

const props = defineProps({ 
    useHealth: Boolean,
    includeMonster: Boolean,
    isPlayerSelect: Boolean
})

async function fight() {
    console.log("Fight");
}

let monsterHealth: Number = 100;
let fighterHealth: Number = 50;
let rangerHealth: Number = 50;
let mageHealth: Number = 50;

</script>

<template>
    <ul v-if="!props.isPlayerSelect" class="flex-container">
        <li v-if="includeMonster"><img class="small" alt="monster" src="../assets/Monster.png" /><p>{{ props.useHealth ? monsterHealth : "&nbsp;" }}</p></li>
        <li><img class="small" alt="fighter" src="../assets/Fighter.png" /><p>{{ props.useHealth ? fighterHealth : "&nbsp;" }}</p></li>
        <li><img class="small" alt="ranger" src="../assets/Ranger.png" /><p>{{ props.useHealth ? rangerHealth : "&nbsp;" }}</p></li>
        <li><img class="small" alt="mage" src="../assets/Mage.png" /><p>{{ props.useHealth ? mageHealth : "&nbsp;" }}</p></li>
    </ul>
    <ul v-if="props.isPlayerSelect" class="flex-container">
        <li>
            <input type="radio" id="fighter" name="character" checked="true" />
            <label for="fighter"><img class="small" alt="fighter" src="../assets/Fighter.png" tabindex="1" /></label>
        </li>
        <li>
            <input type="radio" id="ranger" name="character"/>
            <label for="ranger"><img class="small" alt="ranger" src="../assets/Ranger.png" /></label>
        </li>
        <li>
            <input type="radio" id="mage" name="character"/>
            <label for="mage"><img class="small" alt="mage" src="../assets/Mage.png" /></label>
        </li>
    </ul>
</template>

<style scoped>
.flex-container, input {
  padding: 0;
  margin: 0;
  list-style: none;
  display: flex;
  justify-content: center;
  flex-wrap: wrap;
}

input[type=radio] {
    visibility: hidden;
}

input[type=radio]:checked + label>img {
  border: 4px solid #ff55ff;
}

input[type=radio] + label>img {
  border: 4px solid #000;
}


</style>
