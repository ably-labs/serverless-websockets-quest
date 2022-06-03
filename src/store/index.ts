import { defineStore } from "pinia";

export type State = {
  playerId: String
  questId: String
  character: String,
  fighterName: String,
  rangerName: String,
  mageName: String,
}

export const useStore = defineStore('main', {
  state: () => ({
    playerId: "",
    questId: "",
    character: "",
    fighterName: "",
    rangerName: "",
    mageName: "",
  }) as State,
  getters: {
    getPlayerId(state) {
      return state.playerId;
    },
    getQuestId(state) {
      return state.questId;
    },
    getCharacter(state) {
      return state.character;
    },
    getFighterName(state) {
      return state.fighterName;
    },
    getRangerName(state) {
      return state.rangerName;
    },
    getMageName(state) {
      return state.mageName;
    },
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
