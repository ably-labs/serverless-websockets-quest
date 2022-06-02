import { defineStore } from "pinia";

export type State = {
  playerId: String
  questId: String
  characterId: String
}

export const store = defineStore('main', {
  state: () => ({
    playerId: "",
    questId: "",
    characterId: ""
  }) as State,
  getters: {
    getPlayerId: (state) => state.playerId,
    getQuestId: (state) => state.questId,
    getCharacterId: (state) => state.characterId
  },
  actions: {
    setPlayerId(state: State, input: String) {
      state.playerId = input;
    },
    setQuestId(state: State, input: String) {
      state.questId = input;
    },
    setCharacterId(state: State, input: String) {
      state.characterId = input;
    },
  }
});
