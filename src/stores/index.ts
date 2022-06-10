import { Types } from "ably";
import { Realtime } from "ably/promises";
import { defineStore, storeToRefs } from "pinia";
import { CharacterClass } from "../types/CharacterClass";
import { GamePhase } from "../types/GamePhase";
import { GameState } from "../types/GameState";

export const gameStore = defineStore("game", {
    state: (): GameState =>
        ({
            clientId: "",
            playerName: "",
            isHost: false,
            questId: "",
            phase: GamePhase.Start,
            characterClass: CharacterClass.Fighter,
            monster: { name: "Monstarrr", health: 100, damage: 20, isAvailable: true },
            fighter: { name: "Edge messaging fighter", health: 0, damage: 0, isAvailable: true },
            ranger: { name: "Realtime ranger", health: 0, damage: 0, isAvailable: true },
            mage: { name: "Open sourcerer", health: 0, damage: 0, isAvailable: true },
            isPlayerAdded: false,
            players: Array<string>(),
            realtimeClient: undefined,
            channelInstance: undefined,
            isConnected: false,
        }),
    getters: {
        getChannelName(state) {
            return state.questId;
        },
        getClientId(state) {
            return state.playerName;
        },
        isFighterDisabled(state) {
            return !state.fighter.isAvailable || state.isPlayerAdded;
        },
        isRangerDisabled(state) {
            return !state.ranger.isAvailable || state.isPlayerAdded;
        },
        isMageDisabled(state) {
            return !state.mage.isAvailable || state.isPlayerAdded;
        },
        getMonsterDamage(state) {
            return state.monster.damage > 0 ? `-${state.monster.damage}` : "";
        },
        getMonsterName(state) {
            return state.monster.name;
        },
        getFighterDamage(state) {
            return state.fighter.damage > 0 ? `-${state.fighter.damage}` : "";
        },
        getFighterName(state) {
            return state.characterClass === CharacterClass.Fighter && state.playerName !== "" ? state.playerName : state.fighter.name;
        },
        getRangerDamage(state) {
            return state.ranger.damage > 0 ? `-${state.ranger.damage}` : "";
        },
        getRangerName(state) {
            return state.characterClass === CharacterClass.Ranger && state.playerName !== "" ? state.playerName : state.ranger.name;
        },
        getMageDamage(state) {
            return state.mage.damage > 0 ? `-${state.mage.damage}` : "";
        },
        getMageName(state) {
            return state.characterClass === CharacterClass.Mage && state.playerName !== "" ? state.playerName : state.mage.name;
        },
        havePlayersJoined: (state) => state.players.length > 1,
        numberOfPlayersJoined: (state) => state.players.length - 1, //since there is 1 monster
    },
    actions: {
        addPlayer(playerName: string, characterClass: CharacterClass, health: number) {
            if (!this.players.includes(playerName)) {
                this.players.push(playerName);
                this.updatePlayer(playerName, characterClass, health, 0, false);
            }
        },
        removePlayer(playerName: string) {
            this.players.splice(
                this.players.findIndex((player: string) => player === playerName),
                1
            );
        },
        updatePlayer(playerName: string, characterClass: CharacterClass, health: number, damage: number, isAvailable: boolean) {
            if (characterClass === CharacterClass.Fighter) {
                this.fighter.name = playerName;
                this.fighter.health = health;
                this.fighter.damage = damage;
                this.fighter.isAvailable = isAvailable;
            } else if (characterClass === CharacterClass.Ranger) {
                this.ranger.name = playerName;
                this.ranger.health = health;
                this.ranger.damage = damage;
                this.ranger.isAvailable = isAvailable;
            } else if (characterClass === CharacterClass.Mage) {
                this.mage.name = playerName;
                this.mage.health = health;
                this.mage.damage = damage;
                this.mage.isAvailable = isAvailable;
                
            } else if (characterClass === CharacterClass.Monster) {
                this.monster.name = playerName;
                this.monster.health = health;
                this.monster.damage = damage;
                this.monster.isAvailable = isAvailable;
            }
        },
        async createRealtimeConnection(clientId: string, questId: string) {
            if (!this.isConnected) {
                const realtimeClient = new Realtime.Promise({
                    authUrl: `/api/CreateTokenRequest/${clientId}`,
                    echoMessages: false,
                });
                this.realtimeClient = realtimeClient;
                realtimeClient.connection.on("connected", async (message: Types.ConnectionStateChange) => {
                    console.log(`Connection state ${realtimeClient.connection.state}`);
                    this.isConnected = true;
                    await this.attachToChannel(questId);
                });

                realtimeClient.connection.on("disconnected", () => {
                    this.isConnected = false;
                });
            }
        },
        disconnect() {
            this.realtimeClient?.connection.close();
        },
        async attachToChannel(channelName: string) {
            console.log("Attaching to channel!");
            const channelInstance = this.realtimeClient?.channels.get(
                channelName,
                {
                    params: { rewind: "2m" },
                }
            );
            this.channelInstance = channelInstance;
            this.subscribeToMessages();
        },

        async enterPresence(playerName: string) {
            console.log("Entering presence!");
            await this.channelInstance?.presence.enter({
                name: this.playerName
            });
        },

        subscribeToMessages() {
            this.channelInstance?.subscribe(
                "update-phase",
                (message: Types.Message) => {
                    this.handleUpdatePhase(message);
                }
            );
            this.channelInstance?.subscribe(
                "add-player",
                (message: Types.Message) => {
                    this.handleAddPlayer(message);
                }
            );
            this.channelInstance?.subscribe(
                "update-player",
                (message: Types.Message) => {
                    this.handleUpdatePlayer(message);
                }
            );
            this.channelInstance?.subscribe(
                "check-player-turn",
                (message: Types.Message) => {
                    this.handleCheckPlayerTurn(message);
                }
            );
        },
        handleAddPlayer(message: Types.Message) {
            const playerName: string = message.data.name;
            const characterClass: CharacterClass = message.data.characterClass as CharacterClass;
            const health: number = message.data.health;
            this.addPlayer(playerName, characterClass, health);
        },
        handleUpdatePhase(message: Types.Message) {
            this.phase = message.data.phase;
        },
        handleUpdatePlayer(message: Types.Message) {
            const playerName: string = message.data.name;
            const characterClass: CharacterClass = message.data.characterClass as CharacterClass;
            const health: number = message.data.health;
            const damage: number = message.data.damage;
            this.updatePlayer(playerName, characterClass, health, damage, false);
        },
        handleCheckPlayerTurn(message: Types.Message) {
            const playerName = message.data.name;
            if (this.playerName === playerName) {
                // It's the players turn, enable the buttons.
            } else {
                // It's not the players turn, disable the buttons.
            }
        },
    },
});
