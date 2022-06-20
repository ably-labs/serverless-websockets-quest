<script setup lang="ts">
    const props = defineProps<{
        errorMessage?: String;
    }>();

</script>

<template>
    <p v-if="errorMessage" class="error">{{ errorMessage }}</p>
</template>

<style scoped>
    .error {
    color: #ff55ff;
    }

    .error::before {
        content: "! ";
    }

    .error::after {
        content: " !";
    }
</style>
