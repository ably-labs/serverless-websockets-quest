<script setup lang="ts">
import { defineComponent, ref } from "vue";
import { gameStore } from "../stores";

const store = gameStore();

</script>

<template>
    <div>
        <ul class="messages" v-if="store.messages.length > 0">
            <li v-for="message in store.messages">{{ message }}</li>
        </ul>
    </div>
</template>

<style scoped>

ul {
    display: inline-block;
    max-width: 500px;
    min-width: 200px;
    background-color: #55ffff;
    padding: 20px;
    height: 100px;
    overflow: auto;
}

li {
    list-style: none;
    color: #000;
    font-size: smaller;
    text-align: left;
}

</style>