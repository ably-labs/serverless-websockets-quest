import { createStore } from "vuex";
import axios from "axios";

export const store = createStore({
  state: {
    title: null,
    defaultTitle: "This is the default title",

  },
  getters: {
    getTitle: (state) => state.title ?? state.defaultTitle,
    isDefaultTitle: (state) => state.title === null,
  },
  mutations: {
    setTitle(state, input) {
      state.title = input;
    }
  },
  actions: {
    async setTitleViaApi(vueContext) {
      const response = await axios.get("/api/getTitleFromApi");
      vueContext.commit("setTitle", response.data.title);
    },
    resetTitle(vueContext) {
      vueContext.commit("setTitle", null);
    }
  }
});
