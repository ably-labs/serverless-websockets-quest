<template>
  <div>
    <h1>{{ getTitle }}</h1>
    <button
      :disabled="!isDefaultTitle"
      @click="setTitleViaApi"
    >
      Set title via API
    </button>
    <button
      :disabled="isDefaultTitle"
      @click="resetTitle"
    >
      Reset title
    </button>
    <p>Use <a href="https://github.com/marcduiker/staticwebapp-vue-vite">this GitHub template repo</a> to make a cool <a href="https://docs.microsoft.com/azure/static-web-apps/">Azure Static Web App</a> yourself! 🚀</p>
  </div>
</template>

<script>

import { mapGetters, mapActions } from "vuex";

export default {
  components: {
  },
   computed: {
    ...mapGetters([
        "getTitle",
        "isDefaultTitle"
      ])
  },
  methods: {
    ...mapActions([
      "setTitleViaApi",
      "resetTitle"
    ]),
  },
};
</script>

<style scope>
body {
  font-family: 'Comic Sans MS'
}
</style>
