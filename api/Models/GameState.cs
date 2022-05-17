using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json;

namespace AblyLabs.ServerlessWebsocketsQuest.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class GameState : IGameState
    {
        [JsonProperty("monsterHealth")]
        public int MonsterHealth { get; set; }
        public void SetMonsterHealth(int health) => MonsterHealth = health;
        public void ApplyDamageToMonster(int damage) => 
            MonsterHealth = damage < MonsterHealth ? MonsterHealth - damage : 0;

        [JsonProperty("players")]
        public string[] Players { get; set; }
        public void SetPlayers(string[] players) => Players = players;
        public string GetNextPlayer(string currentPlayerId)
        {
            var currentIndex = Array.IndexOf(Players, currentPlayerId);
            return (currentIndex == Players.Length - 1) ? Players[0] : Players[currentIndex + 1];
        }

        public string GetNextTurn(string currentPlayerId)
        {
            var currentIndex = Array.IndexOf(Players, currentPlayerId);
            return currentIndex == Players.Length - 1 ? "monster" : GetNextPlayer(currentPlayerId);
        }

        public string GetRandomPlayer()
        {
            var index = new Random().Next(0, Players.Length - 1);
            return Players[index];
        }

        public int GetMonsterAttackDamage()
        {
            return new Random().Next(10, 20);
        }

        public bool IsGameOver => MonsterHealth <= 0;

        public static Task Run(
            [EntityTrigger] IDurableEntityContext ctx)
            => ctx.DispatchAsync<GameState>();

    }
}