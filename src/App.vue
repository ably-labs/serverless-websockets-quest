<script setup lang="ts">
// This starter template is using Vue 3 <script setup> SFCs
// Check out https://vuejs.org/api/sfc-script-setup.html#script-setup
import HomePage from './components/HomePage.vue'
</script>

<template>
  <HomePage />
</template>

<style>

html {
  background-color: #000;
  color: #fff;
}

html, button, input {
  font-family: 'Courier New', Courier, monospace;
  font-weight: bold;
  font-size: larger;
} 

#app {
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  padding: 20px;
  margin: 0;
}

a {
  color: #55ffff;
}

.small {
  width: 150px;
}

button {
  color: #000;
  background-color: #ff55ff;
  border-color: #fff;
  padding: 10px;
  margin: 10px;
}

input {
  color: #000;
  background-color: #fff;
  border-color: #ff55ff;
  padding: 10px;
  margin: 10px;
  width: 500px;
}

</style>
