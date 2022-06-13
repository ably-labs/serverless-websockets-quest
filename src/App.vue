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
  padding: 20px;
  margin: 10px;
}

button:disabled, input[type=text]:disabled {
  background-color: #55ffff;
  border-color: #fff;
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

.info {
  color: #55ffff;
}

.lighter {
  font-weight: lighter;
  font-size: smaller;
}

hr {
    margin-top: 25px;
    width: 80%;
    border-color: #fff;
    height: 1px;
    background-color: #55ffff;
}

.message {
    color: #55ffff;
    margin: 10px;
    padding: 10px;
}
.message::before {
    content: "<< ";
}

.message::after {
    content: " >>";
}

</style>
