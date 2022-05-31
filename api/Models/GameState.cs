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
        [JsonProperty("host")]
        public string Host { get; set; }
        public void SetHost(string host) => Host = host;

        [JsonProperty("players")]
        public List<string> PlayerIds { get; set; }
        public void AddPlayerId(string playerId)
        {
            if (PlayerIds == null)
            {
                PlayerIds =  new List<string> { playerId };
            }
            else
            {
                PlayerIds.Add(playerId);
            }
        }

        public string GetNextPlayerId(string? currentPlayerId)
        {
            string nextPlayer;
            if (string.IsNullOrEmpty(currentPlayerId))
            {
                nextPlayer = PlayerIds[0];
            }
            else
            {
                var currentIndex = PlayerIds.FindIndex(0, PlayerIds.Count, p => p == currentPlayerId);
                nextPlayer = currentIndex == PlayerIds.Count - 1 ? PlayerIds[0] : PlayerIds[currentIndex + 1];
            }

            return nextPlayer;
        }

        public bool IsMonsterTurn(string currentPlayerId)
        {
            var currentIndex = PlayerIds.FindIndex(0, PlayerIds.Count, p => p == currentPlayerId);
            return currentIndex == PlayerIds.Count - 1 ? true : false;
        }

        public string GetRandomPlayerId()
        {
            var playerIdsWithoutMonster = PlayerIds.Where(p => p != Monster.ID).ToList();
            var index = new Random().Next(0, playerIdsWithoutMonster.Count - 1);
            return playerIdsWithoutMonster[index];
        }

        [FunctionName(nameof(GameState))]
        public static Task Run([EntityTrigger] IDurableEntityContext ctx)
            => ctx.DispatchAsync<GameState>();
    }
}