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
  font-size: large;
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

img {
  margin: 10px;
}

figcaption {
  width: 180px;
}

button {
  color: #000;
  background-color: #ff55ff;
  border-color: #fff;
  padding: 10px;
  margin: 10px;
}

button:disabled {
  background-color: #55ffff;
}


input[type=text] {
  color: #000;
  background-color: #fff;
  border-color: #ff55ff;
  padding: 10px;
  margin: 10px;
  width: 300px;
}

.pink {
    color: #ff55ff;
}

</style>
