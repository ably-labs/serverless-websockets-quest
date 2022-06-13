import { Types } from "ably";
import { Realtime } from "ably/promises";
import { Player } from "./Player";

export type GameState = RealtimeState & {
    clientId: string;
    playerName: string;
    isHost: boolean;
    questId: string;
    title: string;
    phase: string;
    characterClass: string;
    monster: Player;
    fighter: Player;
    ranger: Player;
    mage: Player;
    players: Array<string>;
    isPlayerAdded: boolean;
    currentPlayer: string;
    messages: Array<string>;
};

export type RealtimeState = {
    realtimeClient: Types.RealtimePromise | undefined;
    channelInstance: Types.RealtimeChannelPromise | undefined;
    isConnected: boolean;
};
