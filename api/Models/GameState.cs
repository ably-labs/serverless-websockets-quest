using System;
using System.Collections.Generic;
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

        [JsonProperty("host")]
        public string Host { get; set; }
        public void SetHost(string host) => Host = host;

        [JsonProperty("players")]
        public List<Player> Players { get; set; }
        public void SetPlayers(List<Player> players) => Players = players;
        public Player GetNextPlayer(string? currentPlayerId)
        {
            Player nextPlayer;
            if (string.IsNullOrEmpty(currentPlayerId))
            {
                nextPlayer = Players[0];
            }
            else
            {
                var currentIndex = Players.FindIndex(0, Players.Count, p => p.Id == currentPlayerId);
                nextPlayer = currentIndex == Players.Count - 1 ? Players[0] : Players[currentIndex + 1];
            }

            return nextPlayer;
        }

        public bool IsMonsterTurn(string currentPlayerId)
        {
            var currentIndex = Players.FindIndex(0, Players.Count, p => p.Id == currentPlayerId);
            return currentIndex == Players.Count - 1 ? true : false;
        }

        public Player GetRandomPlayer()
        {
            var index = new Random().Next(0, Players.Count - 1);
            return Players[index];
        }

        public int GetMonsterAttackDamage()
        {
            return new Random().Next(MonsterHealth/10, MonsterHealth/5);
        }

        public int GetPlayerAttackDamage()
        {
            return new Random().Next(MonsterHealth/20, MonsterHealth/10);
        }

        public bool IsGameOver => MonsterHealth <= 0;

        public static Task Run(
            [EntityTrigger] IDurableEntityContext ctx)
            => ctx.DispatchAsync<GameState>();

    }
}