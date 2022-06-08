import { Types } from "ably";
import { Realtime } from "ably/promises";
import { Player } from "./Player";

export type GameState = RealtimeState & {
    playerId: string;
    isHost: boolean;
    questId: string;
    phase: string;
    characterClass: string;
    monster: Player;
    fighter: Player;
    ranger: Player;
    mage: Player;
    players: Array<string>;
};

export type RealtimeState = {
    realtimeClient: Realtime | undefined;
    channelInstance: Types.RealtimeChannelPromise | undefined;
    isConnected: boolean;
};
