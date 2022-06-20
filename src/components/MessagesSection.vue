<script setup lang="ts">
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
        height: 100px;
        padding: 20px;
        overflow: auto;
        background-color: #55ffff;
    }

    li {
        font-size: smaller;
        text-align: left;
        list-style: none;
        color: #000;
    }
</style>
