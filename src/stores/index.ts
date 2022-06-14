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
            } else if (state.monster.isUnderAttack) {
                asset = "monster_damage.gif";
            } else {
                asset = "monster_idle.png";
            }
            return `/assets/${asset}`;
        },
        getFighterAsset: (state) => {
            let asset = "";
            if (state.fighter.health <= 0) {
                asset = "fighter_idle.png";
            }
            else if (state.fighter.isAttacking) {
                asset = "fighter_attack.gif";
            } else {
                asset = "fighter_idle.png";
            }
            return `/assets/${asset}`;
        },
        getRangerAsset: (state) => {
            let asset = "";
            if (state.ranger.health <= 0) {
                asset = "ranger_idle.png";
            }
            else if (state.ranger.isAttacking) {
                asset = "ranger_attack.gif";
            } else {
                asset = "ranger_idle.png";
            }
            return `/assets/${asset}`;
        },
        getMageAsset: (state) => {
            let asset = "";	
            if (state.mage.health <= 0) {
                asset = "mage_idle.png";
            }
            else if (state.mage.isAttacking) {
                asset = "mage_attack.gif";
            } else {
                asset = "mage_idle.png";
            }
            return `/assets/${asset}`;
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
                this.updatePlayer(playerName, characterClass, health, 0, false, false, false);
            }
        },
        removePlayer(playerName: string) {
            this.players.splice(
                this.players.findIndex((player: string) => player === playerName),
                1
            );
        },
        updatePlayer(playerName: string, characterClass: CharacterClass, health: number, damage: number, isDefeated: boolean, isAvailable: boolean, isUnderAttack: boolean) {
            if (characterClass === CharacterClass.Fighter) {
                this.$patch({
                    fighter: { 
                        name: playerName,
                        health: health,
                        damage: damage,
                        isAvailable: isAvailable,
                        isUnderAttack: isUnderAttack
                        },
                  });
            } else if (characterClass === CharacterClass.Ranger) {
                this.$patch({
                    ranger: { 
                        name: playerName,
                        health: health,
                        damage: damage,
                        isAvailable: isAvailable,
                        isUnderAttack: isUnderAttack
                        },
                  });
            } else if (characterClass === CharacterClass.Mage) {
                this.$patch({
                    mage: { 
                        name: playerName,
                        health: health,
                        damage: damage,
                        isAvailable: isAvailable,
                        isUnderAttack: isUnderAttack
                        },
                  });
            } else if (characterClass === CharacterClass.Monster) {
                this.$patch({
                    monster: { 
                        name: playerName,
                        health: health,
                        damage: damage,
                        isAvailable: isAvailable,
                        isUnderAttack: isUnderAttack
                        },
                  });
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
                    this.isConnected = true;
                    this.messages.unshift(`Ably connection status: ${realtimeClient.connection.state}`)
                    await this.attachToChannel(questId);
                });

                realtimeClient.connection.on("disconnected", () => {
                    this.isConnected = false;
                    this.messages.unshift(`Ably connection status: ${realtimeClient.connection.state}`)

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
                "player-attacking",
                (message: Types.Message) => {
                    this.handlePlayerIsAttacking(message);
                }
            );
            this.channelInstance?.subscribe(
                "player-under-attack",
                (message: Types.Message) => {
                    this.handlePlayerIsUnderAttack(message);
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
            this.messages.unshift(`Player added: ${playerName} (${characterClass})`);
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
        handlePlayerIsAttacking(message: Types.Message) {
            const playerAttacking: string = message.data.playerAttacking;
            const playerUnderAttack: string = message.data.playerUnderAttack;
            // TODO
            if (playerAttacking === this.monster.name) {
                this.$patch({
                    monster: { isAttacking: true },
                    fighter: { isAttacking: false },
                    ranger: { isAttacking: false },
                    mage: { isAttacking: false },
                  });
            }
            if (playerAttacking === this.fighter.name) {
                this.$patch({
                    monster: { isAttacking: false },
                    fighter: {isAttacking: true },
                    ranger: { isAttacking: false },
                    mage: { isAttacking: false },
                });
            }
            if (playerAttacking === this.ranger.name) {
                this.$patch({
                    monster: { isAttacking: false },
                    fighter: { isAttacking: false },
                    ranger: { isAttacking: true },
                    mage: { isAttacking: false },
                });
            }
            if (playerAttacking === this.mage.name) {
                this.$patch({
                    monster: { isAttacking: false },
                    fighter: { isAttacking: false },
                    ranger: { isAttacking: false },
                    mage: { isAttacking: true },
                });
            }
            
        },
        handlePlayerIsUnderAttack(message: Types.Message) {
            const playerName: string = message.data.name;
            const characterClass: CharacterClass = message.data.characterClass as CharacterClass;
            const health: number = message.data.health;
            const damage: number = message.data.damage;
            const isDefeated: boolean = message.data.isDefeated;
            this.updatePlayer(playerName, characterClass, health, damage, isDefeated, false, true);
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
