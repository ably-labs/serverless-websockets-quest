using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json;

namespace AblyLabs.ServerlessWebsocketsQuest.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class GameState : IGameState
    {
        [JsonProperty("phase")]
        public string Phase { get; set; }
        public void SetPhase(string phase) => Phase = phase;

        [JsonProperty("players")]
        public List<string> PlayerNames { get; set; }
        public void AddPlayerName(string playerName)
        {
            if (PlayerNames == null)
            {
                PlayerNames =  new List<string> { playerName };
            }
            else
            {
                PlayerNames.Add(playerName);
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

        private const int NumberOfPlayers = 4;
    }
}