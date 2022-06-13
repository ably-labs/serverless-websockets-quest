import { Types } from "ably";
import { Realtime } from "ably/promises";
import { defineStore, storeToRefs } from "pinia";
import { toHandlers } from "vue";
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
            title: "You encounter a monster! Prepare for battle!",
            phase: GamePhase.Start,
            characterClass: CharacterClass.Fighter,
            monster: { characterClass: CharacterClass.Monster, name: "Monstarrr", health: 100, damage: 20, isAvailable: true, isAttacking: false, isUnderAttack: false },
            fighter: { characterClass: CharacterClass.Fighter, name: "Edge messaging fighter", health: 0, damage: 0, isAvailable: true, isAttacking: false, isUnderAttack: false },
            ranger: { characterClass: CharacterClass.Ranger, name: "Realtime ranger", health: 0, damage: 0, isAvailable: true, isAttacking: false, isUnderAttack: false },
            mage: { characterClass: CharacterClass.Mage, name: "Open sourcerer", health: 0, damage: 0, isAvailable: true, isAttacking: false, isUnderAttack: false },
            isPlayerAdded: false,
            players: Array<string>(),
            currentPlayer: "",
            messages: Array<string>(),
            realtimeClient: undefined,
            channelInstance: undefined,
            isConnected: false,
        }),
    getters: {
        getChannelName: (state) => state.questId,
        getClientId: (state) => state.playerName,
        isFighterDisabled: (state) => !state.fighter.isAvailable || state.isPlayerAdded,
        isRangerDisabled: (state) => !state.ranger.isAvailable || state.isPlayerAdded,
        isMageDisabled: (state) => !state.mage.isAvailable || state.isPlayerAdded,
        getMonsterDamage: (state) => state.monster.damage > 0 ? `-${state.monster.damage}` : "",
        getMonsterName: (state) => state.monster.name,
        getFighterDamage: (state) => state.fighter.damage > 0 ? `-${state.fighter.damage}` : "",
        getFighterName: (state) => state.characterClass === CharacterClass.Fighter && state.playerName !== "" ? state.playerName : state.fighter.name,
        getRangerDamage: (state) => state.ranger.damage > 0 ? `-${state.ranger.damage}` : "",
        getRangerName: (state) => state.characterClass === CharacterClass.Ranger && state.playerName !== "" ? state.playerName : state.ranger.name,
        getMageDamage: (state) => state.mage.damage > 0 ? `-${state.mage.damage}` : "",
        getMageName: (state) => state.characterClass === CharacterClass.Mage && state.playerName !== "" ? state.playerName : state.mage.name,
        getMonsterAsset: (state) => {
            let asset = "";
            if (state.monster.health <= 0) {
                asset = "monster_dead.png";
            }
            else if (state.monster.isAttacking) {
                asset = "monster_attack.gif";
            } else {
                asset = "monster.png";
            }
            return `/src/assets/${asset}`;
        },
        getFighterAsset: (state) => {
            let asset = "";
            if (state.fighter.health <= 0) {
                asset = "fighter.png";
            }
            else if (state.fighter.isAttacking) {
                asset = "fighter_attack.gif";
            } else {
                asset = "fighter.png";
            }
            return `/src/assets/${asset}`;
        },
        getRangerAsset: (state) => {
            let asset = "";
            if (state.ranger.health <= 0) {
                asset = "ranger.png";
            }
            else if (state.ranger.isAttacking) {
                asset = "ranger_attack.gif";
            } else {
                asset = "ranger.png";
            }
            return `/src/assets/${asset}`;
        },
        getMageAsset: (state) => {
            let asset = "";	
            if (state.mage.health <= 0) {
                asset = "ranger.png";
            }
            else if (state.mage.isAttacking) {
                asset = "mage_attack.gif";
            } else {
                asset = "mage.png";
            }
            return `/src/assets/${asset}`;
        },
        isPlayerTurn: (state) => state.playerName === state.currentPlayer,
        isMonsterActive: (state) => state.monster.name === state.currentPlayer,
        isFighterActive: (state) => state.fighter.name === state.currentPlayer,
        isRangerActive: (state) => state.ranger.name === state.currentPlayer,
        isMageActive: (state) => state.mage.name === state.currentPlayer,
        numberOfPlayersJoined: (state) => state.players.length,
        numberOfPlayersRemaining: (state) => 4 - state.players.length
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
                "update-message",
                (message: Types.Message) => {
                    this.handleUpdateMessage(message);
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
            this.title = message.data.title;
            this.phase = message.data.phase;
        },
        handleUpdateMessage(message: Types.Message) {
            this.title = message.data.title !== undefined ? message.data.title : this.title;
            this.messages.unshift(message.data.message);
        },
        handleUpdatePlayer(message: Types.Message) {
            const playerName: string = message.data.name;
            const characterClass: CharacterClass = message.data.characterClass as CharacterClass;
            const health: number = message.data.health;
            const damage: number = message.data.damage;
            this.updatePlayer(playerName, characterClass, health, damage, false);
        },
        handleCheckPlayerTurn(message: Types.Message) {
            this.messages.unshift(message.data.message);
            const playerName = message.data.name;
            this.currentPlayer = message.data.name;
            if (playerName === this.monster.name) {
                this.monsterAttack();
            }
        },
        async monsterAttack() {
            await window.fetch("/api/ExecuteTurn", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({
                    questId: this.questId,
                    playerName: this.monster.name,
                    characterClass: this.monster.characterClass,
                    })
                });
        }
    },
});
