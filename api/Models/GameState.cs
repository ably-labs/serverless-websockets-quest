using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IO.Ably;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json;

namespace AblyLabs.ServerlessWebsocketsQuest.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class GameState : IGameState
    {
        private const int NumberOfPlayers = 4;
        private readonly IRealtimeClient _realtimeClient;

        public GameState(IRealtimeClient realtimeClient)
        {
            _realtimeClient = realtimeClient;
        }
        
        [JsonProperty("questId")]
        public string QuestId { get; set; }
        public async Task InitGameState(string[] gameStateFields)
        {
            QuestId = gameStateFields[0];
            Phase = gameStateFields[1];
            await PublishUpdatePhase(Phase);
        }

        [JsonProperty("phase")]
        public string Phase { get; set; }
        public async Task UpdatePhase(string phase)
        {
            Phase = phase;
            await PublishUpdatePhase(Phase);
        }

        [JsonProperty("players")]
        public List<string> PlayerNames { get; set; }
        public async Task AddPlayerName(string playerName)
        {
            if (PlayerNames == null)
            {
                PlayerNames =  new List<string> { playerName };
            }
            else
            {
                PlayerNames.Add(playerName);
            }

            if (IsPartyComplete)
            {
                await UpdatePhase(GamePhases.Play);
            }
        }

        public string GetNextPlayerName(string? currentPlayerName)
        {
            string nextPlayer;
            if (string.IsNullOrEmpty(currentPlayerName))
            {
                nextPlayer = PlayerNames[0];
            }
            else
            {
                var currentIndex = PlayerNames.FindIndex(0, PlayerNames.Count, p => p == currentPlayerName);
                nextPlayer = currentIndex == PlayerNames.Count - 1 ? PlayerNames[0] : PlayerNames[currentIndex + 1];
            }

            return nextPlayer;
        }

        public bool IsPartyComplete => PlayerNames.Count == NumberOfPlayers;

        public string GetRandomPlayerName()
        {
            var playerNamesWithoutMonster = PlayerNames.Where(p => p != CharacterClassDefinitions.Monster.Name).ToList();
            var index = new Random().Next(0, playerNamesWithoutMonster.Count - 1);
            return playerNamesWithoutMonster[index];
        }

        [FunctionName(nameof(GameState))]
        public static Task Run([EntityTrigger] IDurableEntityContext ctx)
            => ctx.DispatchAsync<GameState>();

        private async Task PublishUpdatePhase(string phase)
        {
            var channel = _realtimeClient.Channels.Get(QuestId);
            await channel.PublishAsync(
                "update-phase",
                    new
                    {
                        phase = phase
                    }
                );
        }
    }
}