import { Realtime } from 'ably/promises';
import { gameStore } from '.'
import { defineStore } from 'pinia'
import { Types } from 'ably';

const mainStore = gameStore();

export type RealtimeState = {
  ablyClientId: string,
  ablyRealtimeClient: Realtime | undefined,
  channelName: string,
  channelInstance: Types.RealtimeChannelPromise | undefined,
  isConnected: boolean,
  players: Array<string>,
}; 

export const realtimeStore = defineStore('realtime', {
  state: () => ({
    ablyClientId: "",
    ablyRealtimeClient: undefined,
    channelName: "",
    channelInstance: undefined,
    isConnected: false,
    players: [],
  }) as RealtimeState,
  getters: {
    havePlayersJoined: (state) => state.players.length > 1,
    numberOfPlayersJoined: (state) => state.players.length,
  },
  actions: {
    addParticipantJoined(clientId: string) {
      if (!this.players.includes(clientId)) {
        this.players.push(clientId);
      }
    },
    removeParticipantJoined(clientId: string) {
      this.players.splice(
        this.players.findIndex(
          (participant: any) => participant.id === clientId,
        ),
        1,
      );
    },
    instantiateAblyConnection(clientId: string, sessionId: string) {
      if (!this.isConnected) {
        const realtimeClient = new Realtime({
          authUrl: '/api/createTokenRequest',
          echoMessages: false,
        });
        realtimeClient.connection.on('connected', () => {
          this.isConnected = true;
          this.ablyClientId = clientId ?? realtimeClient.auth.clientId;
          this.ablyRealtimeClient = realtimeClient;
          if (sessionId) {
            this.channelName = sessionId;
            mainStore.questId = sessionId;
          }
          this.attachToAblyChannels().then(() => {
            this.enterClientInAblyPresenceSet();
            this.getExistingAblyPresenceSet().then(() => {
              this.subscribeToAblyPresence();
            });
          });
        });

        realtimeClient.connection.on('disconnected', () => {
          this.isConnected = false;
        });
      }
    },
    closeAblyConnection() {
      this.ablyRealtimeClient?.connection.close();
    },

    async attachToAblyChannels() {
      const channelInstance = this.ablyRealtimeClient?.channels.get(
        this.channelName,
        {
          params: { rewind: '2m' },
        },
      );
      this.channelInstance = channelInstance;
      this.subscribeToAblyVoting();
    },

    enterClientInAblyPresenceSet() {
      this.channelInstance?.presence.enter({
        id: this.ablyClientId,
      });
    },

    async getExistingAblyPresenceSet() {
      var presenceMessages = await this.channelInstance?.presence.get();
      if (presenceMessages !== undefined) {
        for (let i = 0; i < presenceMessages.length; i++) {
          this.addParticipantJoined(presenceMessages[i].id);
        }
      }
    },
    subscribeToAblyPresence() {
      this.channelInstance?.presence.subscribe('enter', (msg) => {
        this.handleNewParticipantEntered(msg);
      });
      this.channelInstance?.presence.subscribe('leave', (msg) => {
        this.handleExistingParticipantLeft(msg);
      });
    },

    handleNewParticipantEntered(participant: any) {
      this.addParticipantJoined(participant.clientId);
    },

    handleExistingParticipantLeft(participant: any) {
      // this.removeParticipantJoined(participant.clientId);
      // const cardNumber = this.selectedCardForClient(participant.clientId);
      // if (cardNumber !== null) {
      //   commit('removeParticipantVoted', {
      //     clientId: participant.clientId,
      //     cardNumber,
      //   });
      // }
    },

    subscribeToAblyVoting() {
      this.channelInstance?.subscribe('vote', (msg) => {
        this.handleVoteReceived(msg);
      });
      this.channelInstance?.subscribe('undo-vote', (msg) => {
        this.handleUndoVoteReceived(msg);
      });
      this.channelInstance?.subscribe('show-results', (msg) => {
        this.handleShowResultsReceived(msg);
      });
      this.channelInstance?.subscribe('reset-voting', (msg) => {
        this.handleResetVotingReceived(msg);
      });
    },

    handleVoteReceived(msg: any) {
      // this.addParticipantV
      // commit('addParticipantVoted', {
      //   clientId: msg.data.clientId,
      //   cardNumber: msg.data.cardNumber,
      // });
    },
    handleUndoVoteReceived(msg: any) {
      // commit('removeParticipantVoted', {
      //   clientId: msg.data.clientId,
      //   cardNumber: msg.data.cardNumber,
      // });
    },
    handleShowResultsReceived(msg: any) {
      // if (msg.data.showResults) {
      //   commit('setShowResults', true);
      // } else {
      //   commit('setShowResults', false);
      // }
    },
    // eslint-disable-next-line no-unused-vars
    handleResetVotingReceived(msg: any) {
      // dispatch('commonResetVoting');
    },

    publishVoteToAbly(clientVote: any) {
      // state.channelInstances.voting.publish('vote', clientVote);
    },
    publishUndoVoteToAbly(clientVote: any) {
      // state.channelInstances.voting.publish('undo-vote', clientVote);
    },
    publishShowResultsToAbly(showResults: any) {
      // state.channelInstances.voting.publish('show-results', {
      //   showResults,
      // });
    },
    publishResetVotingToAbly() {
      // state.channelInstances.voting.publish('reset-voting', {});
    },
  },
});
