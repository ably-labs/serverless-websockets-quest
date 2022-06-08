import { Types } from "ably";
import { Realtime } from "ably/promises";
import { defineStore } from "pinia";
import { CharacterClass } from "../types/CharacterClass";
import { GamePhase } from "../types/GamePhase";
import { GameState } from "../types/GameState";

export const gameStore = defineStore("game", {
    state: () =>
        ({
            playerId: "",
            isHost: false,
            questId: "",
            phase: GamePhase.Start,
            characterClass: CharacterClass.Fighter,
            monster: { name: "Monstarrr" },
            fighter: { name: "Edge messaging fighter" },
            ranger: { name: "Realtime ranger" },
            mage: { name: "Open sourcerer" },
            isPlayerAdded: false,
            players: Array<string>(),
            realtimeClient: undefined,
            channelInstance: undefined,
            isConnected: false,
        } as GameState),
    getters: {
        getChannelName(state) {
            return state.questId;
        },
        getClientId(state) {
            return state.playerId;
        },
        getMonsterName(state) {
            return state.monster.name;
        },
        getFighterName(state) {
            return state.characterClass == CharacterClass.Fighter && state.playerId != "" ? state.playerId : state.fighter.name;
        },
        getRangerName(state) {
            return state.characterClass == CharacterClass.Ranger && state.playerId != "" ? state.playerId : state.ranger.name;
        },
        getMageName(state) {
            return state.characterClass == CharacterClass.Mage && state.playerId != "" ? state.playerId : state.mage.name;
        },
        havePlayersJoined: (state) => state.players.length > 1,
        numberOfPlayersJoined: (state) => state.players.length - 1, //since there is 1 monster
    },
    actions: {
        addPlayer(clientId: string) {
            if (!this.players.includes(clientId)) {
                this.players.push(clientId);
            }
        },
        removePlayer(clientId: string) {
            this.players.splice(
                this.players.findIndex((player: string) => player === clientId),
                1
            );
        },
        createRealtimeConnection(playerId: string, questId: string) {
            if (!this.isConnected) {
                console.log("Not connected");
                const realtimeClient = new Realtime({
                    authUrl: "/api/CreateTokenRequest/playerId",
                    echoMessages: false,
                    authCallback: (tokenParams, err) => {
                        if (err) {
                            console.log(err);
                        }
                    }
                });
                console.log("Created client");
                realtimeClient.connection.on("connected", () => {
                    console.log("On Connected!");
                    this.playerId = playerId;
                    this.questId = questId;
                    this.isConnected = true;
                    this.realtimeClient = realtimeClient;
                    this.attachToChannel().then(() => {
                        this.enterPresence();
                        this.getPresenceSet().then(() => {
                            this.subscribeToPresence();
                        });
                    });
                });
                realtimeClient.connection.on("disconnected", () => {
                    this.isConnected = false;
                });
            }
        },
        closeAblyConnection() {
            this.realtimeClient?.connection.close();
        },
        async attachToChannel() {
            console.log("Attaching to channel!");
            const channelInstance = this.realtimeClient?.channels.get(
                this.getChannelName,
                {
                    params: { rewind: "2m" },
                }
            );
            this.channelInstance = channelInstance;
            this.subscribeToMessages();
        },

        enterPresence() {
            this.channelInstance?.presence.enter({
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
            const className: string = message.data.className;
            const health: number = message.data.health;
            const damage: number = message.data.damage;

            if (this.playerId === playerId) {
                this.characterClass = className;
            }
            if (className === CharacterClass.Fighter) {
                this.fighter.name = playerId;
                this.fighter.health = health;
                this.fighter.damage = damage;
            } else if (className === CharacterClass.Ranger) {
                this.ranger.name = playerId;
                this.ranger.health = health;
                this.ranger.damage = damage;
            } else if (className === CharacterClass.Mage) {
                this.mage.name = playerId;
                this.mage.health = health;
                this.mage.damage = damage;
            } else if (className === CharacterClass.Monster) {
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
