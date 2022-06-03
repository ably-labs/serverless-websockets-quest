import { defineStore } from "pinia";

export type State = {
  playerId: String
  questId: String
  characterId: String
}

export const store = defineStore('main', {
  state: () => ({
    playerId: "abc",
    questId: "wowie-world-quest",
    characterId: "ranger"
  }) as State,
  getters: {
    getPlayerId(state) {
      return state.playerId;
    },
    getQuestId(state) {
      return state.questId;
    },
    getCharacterId(state) {
      return state.characterId;
    }
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
