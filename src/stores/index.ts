import { defineStore } from "pinia";

export type State = {
  playerId: string,
  questId: string,
  view: string,
  character: string,
  monsterName: string,
  fighterName: string,
  rangerName: string,
  mageName: string,
}

export const gameStore = defineStore('game', {
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
    setPlayerId(state: State, input: string) {
      state.playerId = input;
    },
    setQuestId(state: State, input: string) {
      state.questId = input;
    },
    setCharacterId(state: State, input: string) {
      state.character = input;
    },
  }
});
