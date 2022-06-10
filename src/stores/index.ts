import { Types } from "ably";
import { Realtime } from "ably/promises";
import { defineStore } from "pinia";
import { CharacterClass } from "../types/CharacterClass";
import { GamePhase } from "../types/GamePhase";
import { GameState } from "../types/GameState";

export const gameStore = defineStore("game", {
    state: (): GameState =>
        ({
            playerId: "",
            isHost: false,
            questId: "",
            phase: GamePhase.Start,
            characterClass: CharacterClass.Fighter,
            monster: { name: "Monstarrr", health: 100, damage: 20 },
            fighter: { name: "Edge messaging fighter", health: 0, damage: 0 },
            ranger: { name: "Realtime ranger", health: 0, damage: 0 },
            mage: { name: "Open sourcerer", health: 0, damage: 0 },
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
            return state.playerId;
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
            return state.characterClass === CharacterClass.Fighter && state.playerId !== "" ? state.playerId : state.fighter.name;
        },
        getRangerDamage(state) {
            return state.ranger.damage > 0 ? `-${state.ranger.damage}` : "";
        },
        getRangerName(state) {
            return state.characterClass === CharacterClass.Ranger && state.playerId !== "" ? state.playerId : state.ranger.name;
        },
        getMageDamage(state) {
            return state.mage.damage > 0 ? `-${state.mage.damage}` : "";
        },
        getMageName(state) {
            return state.characterClass === CharacterClass.Mage && state.playerId !== "" ? state.playerId : state.mage.name;
        },
        havePlayersJoined: (state) => state.players.length > 1,
        numberOfPlayersJoined: (state) => state.players.length - 1, //since there is 1 monster
    },
    actions: {
        addPlayer(playerId: string) {
            if (!this.players.includes(playerId)) {
                this.players.push(playerId);
            }
        },
        removePlayer(playerId: string) {
            this.players.splice(
                this.players.findIndex((player: string) => player === playerId),
                1
            );
        },
        async createRealtimeConnection(playerId: string, questId: string) {
            if (!this.isConnected) {
                console.log("Not connected");
                const realtimeClient = new Realtime.Promise({
                    authUrl: `/api/CreateTokenRequest/${playerId}`,
                    echoMessages: false,
                });
                console.log(`Connection state ${realtimeClient.connection.state}`);
                this.realtimeClient = realtimeClient;
                
                realtimeClient.connection.on("connected", async (message: Types.ConnectionStateChange) => {
                    console.log(`Connection state ${realtimeClient.connection.state}`);
                    this.isConnected = true;
                    await this.attachToChannel(questId);
                    await this.enterPresence();
                    await this.getPresenceSet();
                    this.subscribeToPresence();
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

        async enterPresence() {
            console.log("Entering presence!");
            await this.channelInstance?.presence.enter({
                id: this.getClientId,
            });
        },

        async getPresenceSet() {
            var presenceMessages = await this.channelInstance?.presence.get();
            if (presenceMessages !== undefined) {
                for (let i = 0; i < presenceMessages.length; i++) {
                    this.addPlayer(presenceMessages[i].clientId);
                }
            }
        },

        subscribeToPresence() {
            this.channelInstance?.presence.subscribe(
                "enter",
                (message: Types.PresenceMessage) => {
                    this.addPlayer(message.clientId);
                }
            );
            this.channelInstance?.presence.subscribe(
                "leave",
                (message: Types.PresenceMessage) => {
                    this.removePlayer(message.clientId);
                }
            );
        },

        subscribeToMessages() {
            this.channelInstance?.subscribe(
                "update-phase",
                (message: Types.Message) => {
                    this.handleUpdatePhase(message);
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

        handleUpdatePhase(message: Types.Message) {
            this.phase = message.data.phase;
        },
        handleUpdatePlayer(message: Types.Message) {
            const playerId: string = message.data.playerId;
            const characterClass: string = message.data.characterClass;
            const health: number = message.data.health;
            const damage: number = message.data.damage;

            if (this.playerId === playerId) {
                this.characterClass = characterClass;
            }
            if (characterClass === CharacterClass.Fighter) {
                this.fighter.name = playerId;
                this.fighter.health = health;
                this.fighter.damage = damage;
            } else if (characterClass === CharacterClass.Ranger) {
                this.ranger.name = playerId;
                this.ranger.health = health;
                this.ranger.damage = damage;
            } else if (characterClass === CharacterClass.Mage) {
                this.mage.name = playerId;
                this.mage.health = health;
                this.mage.damage = damage;
            } else if (characterClass === CharacterClass.Monster) {
                this.monster.health = health;
                this.monster.damage = damage;
            }
        },
        handleCheckPlayerTurn(message: Types.Message) {
            const playerId = message.data.playerId;
            if (this.playerId === playerId) {
                // It's the players turn, enable the buttons.
            } else {
                // It's not the players turn, disable the buttons.
            }
        },
    },
});
