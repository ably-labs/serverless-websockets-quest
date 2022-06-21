import { Types } from "ably";
import { Realtime } from "ably/promises";
import { defineStore, storeToRefs } from "pinia";
import { CharacterClass } from "../types/CharacterClass";
import { GamePhase } from "../types/GamePhase";
import { GameState } from "../types/GameState";
import monsterIdle from "../assets/monster_idle.png"
import monsterAttack from "../assets/monster_attack.gif"
import monsterDead from "../assets/monster_dead.png"
import monsterDamage from "../assets/monster_damage.gif"
import monsterWin from "../assets/monster_win.gif"
import fighterIdle from "../assets/fighter_idle.png"
import fighterAttack from "../assets/fighter_attack.gif"
import fighterWin from "../assets/fighter_win.gif"
import rangerIdle from "../assets/ranger_idle.png"
import rangerAttack from "../assets/ranger_attack.gif"
import rangerWin from "../assets/ranger_win.gif"
import mageIdle from "../assets/mage_idle.png"
import mageAttack from "../assets/mage_attack.gif"
import mageWin from "../assets/mage_win.gif"

export const gameStore = defineStore("game", {
    state: (): GameState =>
        ({
            clientId: "",
            playerName: "",
            isHost: false,
            questId: "",
            phase: GamePhase.Start,
            characterClass: CharacterClass.Fighter,
            monster: {
                characterClass: CharacterClass.Monster,
                name: "Monstarrr",
                health: 0,
                damage: 0,
                totalDamageApplied: 0,
                isAvailable: true,
                isAttacking: false,
                isUnderAttack: false,
                isDefeated: false,
                assets: {
                    idle: monsterIdle,
                    attack: monsterAttack,
                    damage: monsterDamage,
                    dead: monsterDead,
                    win: monsterWin
                }},
            fighter: {
                characterClass: CharacterClass.Fighter,
                name: "Edge messaging fighter",
                health: 0,
                damage: 0,
                totalDamageApplied: 0,
                isAvailable: true,
                isAttacking: false,
                isUnderAttack: false,
                isDefeated: false,
                assets: {
                    idle: fighterIdle,
                    attack: fighterAttack,
                    damage: fighterIdle,
                    dead: fighterIdle,
                    win: fighterWin
                }},
            ranger: {
                characterClass: CharacterClass.Ranger,
                name: "Realtime ranger",
                health: 0,
                damage: 0,
                totalDamageApplied: 0,
                isAvailable: true,
                isAttacking: false,
                isUnderAttack: false,
                isDefeated: false,
                assets: {
                    idle: rangerIdle,
                    attack: rangerAttack,
                    damage: rangerIdle,
                    dead: rangerIdle,
                    win: rangerWin
                }},
            mage: {
                characterClass: CharacterClass.Mage,
                name: "Open sourcerer",
                health: 0,
                damage: 0,
                totalDamageApplied: 0,
                isAvailable: true,
                isAttacking: false,
                isUnderAttack: false,
                isDefeated: false,
                assets: {
                    idle: mageIdle,
                    attack: mageAttack,
                    damage: mageIdle,
                    dead: mageIdle,
                    win: mageWin
                }},
            isPlayerAdded: false,
            players: Array<string>(),
            currentPlayer: "",
            messages: Array<string>(),
            teamHasWon: undefined,
            realtimeClient: undefined,
            channelInstance: undefined,
            isConnected: false,
        }),
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
            const damageTimeOut = 2000;
            if (characterClass === CharacterClass.Fighter) {
                this.$patch({
                    fighter: {
                        name: playerName,
                        health: health,
                        damage: damage,
                        isAvailable: isAvailable,
                        isUnderAttack: isUnderAttack,
                        isDefeated: isDefeated
                        },
                  });
                setTimeout(() => this.fighter.damage = 0, damageTimeOut);
            } else if (characterClass === CharacterClass.Ranger) {
                this.$patch({
                    ranger: {
                        name: playerName,
                        health: health,
                        damage: damage,
                        isAvailable: isAvailable,
                        isUnderAttack: isUnderAttack,
                        isDefeated: isDefeated
                        },
                  });
                  setTimeout(() => this.ranger.damage = 0, damageTimeOut);
            } else if (characterClass === CharacterClass.Mage) {
                this.$patch({
                    mage: {
                        name: playerName,
                        health: health,
                        damage: damage,
                        isAvailable: isAvailable,
                        isUnderAttack: isUnderAttack,
                        isDefeated: isDefeated
                        },
                  });
                  setTimeout(() => this.mage.damage = 0, damageTimeOut);
            } else if (characterClass === CharacterClass.Monster) {
                this.$patch({
                    monster: {
                        name: playerName,
                        health: health,
                        damage: damage,
                        isAvailable: isAvailable,
                        isUnderAttack: isUnderAttack,
                        isDefeated: isDefeated
                        },
                  });
                  setTimeout(() => this.monster.damage = 0, damageTimeOut);
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
                    const messageText = `Ably connection status: ${realtimeClient.connection.state}`;
                    this.writeMessage(messageText);
                    await this.attachToChannel(questId);
                });

                this.realtimeClient.connection.on("disconnected", () => {
                    this.isConnected = false;
                    const messageText = `Ably connection status: ${realtimeClient.connection.state}`;
                    this.writeMessage(messageText);
                });
                this.realtimeClient.connection.on("closed", () => {
                    this.isConnected = false;
                    const messageText = `Ably connection status: ${realtimeClient.connection.state}`;
                    this.writeMessage(messageText);
                });
            }
        },
        disconnect() {
            this.realtimeClient?.connection.close();
        },
        async attachToChannel(channelName: string) {
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
            const messageText = `Player added: ${playerName} (${characterClass})`;
            this.writeMessage(messageText);
            this.addPlayer(playerName, characterClass, health);
        },
        handleUpdatePhase(message: Types.Message) {
            this.phase = message.data.phase;
            this.teamHasWon = this.teamHasWon ? this.teamHasWon : message.data.teamHasWon;
        },
        handleUpdateMessage(message: Types.Message) {
            this.writeMessage(message.data.message);
        },
        handlePlayerIsAttacking(message: Types.Message) {
            if (this.teamHasWon !== undefined) return;
            const playerAttacking: string = message.data.playerAttacking;
            const playerUnderAttack: string = message.data.playerUnderAttack;
            const damage: number = message.data.damage;
            const messageText = `${playerAttacking} is attacking ${playerUnderAttack}`;
            this.writeMessage(messageText);
            if (playerAttacking === this.monster.name) {
                this.$patch({
                    monster: { isAttacking: true, totalDamageApplied: this.monster.totalDamageApplied + damage },
                    fighter: { isAttacking: false },
                    ranger: { isAttacking: false },
                    mage: { isAttacking: false },
                  });
            }
            if (playerAttacking === this.fighter.name) {
                this.$patch({
                    monster: { isAttacking: false },
                    fighter: {isAttacking: true, totalDamageApplied: this.fighter.totalDamageApplied + damage },
                    ranger: { isAttacking: false },
                    mage: { isAttacking: false },
                });
            }
            if (playerAttacking === this.ranger.name) {
                this.$patch({
                    monster: { isAttacking: false },
                    fighter: { isAttacking: false },
                    ranger: { isAttacking: true, totalDamageApplied: this.ranger.totalDamageApplied + damage },
                    mage: { isAttacking: false },
                });
            }
            if (playerAttacking === this.mage.name) {
                this.$patch({
                    monster: { isAttacking: false },
                    fighter: { isAttacking: false },
                    ranger: { isAttacking: false },
                    mage: { isAttacking: true, totalDamageApplied: this.mage.totalDamageApplied + damage },
                });
            }

        },
        handlePlayerIsUnderAttack(message: Types.Message) {
            if (this.teamHasWon !== undefined) return;
            const playerName: string = message.data.name;
            const characterClass: CharacterClass = message.data.characterClass as CharacterClass;
            const health: number = message.data.health;
            const damage: number = message.data.damage;
            const isDefeated: boolean = message.data.isDefeated;
            const messageText = `${playerName} received ${damage} damage`;
            this.writeMessage(messageText);
            this.updatePlayer(playerName, characterClass, health, damage, isDefeated, false, true);
        },
        async handleCheckPlayerTurn(message: Types.Message) {
            if (this.teamHasWon !== undefined) return;
            this.writeMessage(message.data.message);
            this.currentPlayer = message.data.name;
        },
        writeMessage(message: string) {
            this.messages.unshift(message);
            console.log(message);
        },
        reset() {
            this.$reset();
        }
    },
});
