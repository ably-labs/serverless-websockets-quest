import { defineStore } from "pinia";

export type State = {
  playerId: String
  questId: String,
  view: String,
  character: String,
  monsterName: String,
  fighterName: String,
  rangerName: String,
  mageName: String,
}

export const useStore = defineStore('main', {
  state: () => ({
    playerId: "",
    questId: "",
    view: "start",
    character: "fighter",
    monsterName: "Monstarrr",
    fighterName: "Edge messaging fighter",
    rangerName: "Realtime ranger",
    mageName: "Open sourcerer",
  }) as State,
  getters: {
    getMonsterName(state) {
      return state.monsterName;
    },
    getFighterName(state) {
      return state.character == "fighter" && state.playerId != "" ? state.playerId : state.fighterName;
    },
    getRangerName(state) {
      return state.character == "ranger" && state.playerId != "" ? state.playerId : state.rangerName;
    },
    getMageName(state) {
      return state.character == "mage" && state.playerId != "" ? state.playerId : state.mageName;
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
      state.character = input;
    },
  }
});
