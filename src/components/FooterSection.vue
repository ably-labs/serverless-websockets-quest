<script setup lang="ts">
import { defineComponent } from "@vue/runtime-core";
</script>

<template>
    <hr />
    <p class="lighter">
        Made with <span class="pink">&#10084;</span> by
        <a href="https://twitter.com/marcduiker" target="_blank">@marcduiker</a>.
        Powered by <a href="https://www.ably.com" target="_blank">ably.com</a> 🚀.
        Look at the source on
        <a
            href="https://github.com/ably-labs/serverless-websockets-quest/"
            target="_blank"
            >GitHub</a
        >
        👩‍💻.
    </p>
</template>

<style scoped>
.pink {
    color: #ff55ff;
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

</style>
