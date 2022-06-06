<script setup lang="ts">
import { defineComponent, ref } from "vue";

const props = defineProps<{
    isError: boolean;
    errorMessage?: String;
}>();

</script>

<template v-if="isError">
    <p class="error">{{ errorMessage}} </p>
</template>

<style scoped>

.error {
  color: #ff55ff;
}

.error::before {
    content: "!!! ";
}

.error::after {
    content: " !!!";
}

</style>