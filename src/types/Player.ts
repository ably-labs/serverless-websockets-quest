import { CharacterClass } from "./CharacterClass";

export type Player = {
    characterClass: CharacterClass;
    name: string,
    health: number,
    damage: number,
    totalDamageApplied: number,
    isAvailable: boolean,
    isAttacking: boolean,
    isUnderAttack: boolean,
    isDefeated: boolean,
  }