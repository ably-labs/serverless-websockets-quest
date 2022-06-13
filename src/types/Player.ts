import { CharacterClass } from "./CharacterClass";

export type Player = {
    characterClass: CharacterClass;
    name: string,
    health: number,
    damage: number,
    isAvailable: boolean
  }