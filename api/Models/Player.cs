using System;
using System.Threading.Tasks;
using IO.Ably;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json;

namespace AblyLabs.ServerlessWebsocketsQuest.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Player : IPlayer
    {
        private readonly IRealtimeClient _realtimeClient;

        public Player(IRealtimeClient realtimeClient)
        {
            _realtimeClient = realtimeClient;
        }

        [JsonProperty("questId")]
        public string QuestId { get; set; }
        
        [JsonProperty("playerName")]
        public string PlayerName { get; set; }
        
        [JsonProperty("characterClass")]
        public string CharacterClass { get; set; }

        [JsonProperty("health")]
        public int Health { get; set; }
        public async Task InitPlayer(object[] playerFields)
        {
            QuestId = (string)playerFields[0];
            PlayerName = (string)playerFields[1];
            CharacterClass = (string)playerFields[2];
            Health = Convert.ToInt32(playerFields[3]);
            await PublishAddPlayer(PlayerName, CharacterClass, Health);
        }
        
        public async Task ApplyDamage(int damage)
        {
            Health = damage > Health ? 0 : Health - damage;
            await PublishUpdatePlayer(PlayerName, CharacterClass, Health, damage);
        } 
        public bool IsDefeated => Health <= 0;

        public static string GetEntityId(string questId, string playerName) => $"{questId}-{playerName}";

        [FunctionName(nameof(Player))]
        public static Task Run(
            [EntityTrigger] IDurableEntityContext ctx)
            => ctx.DispatchAsync<Player>();

        private async Task PublishAddPlayer(string playerName, string characterClass, int health)
        {
            var channel = _realtimeClient.Channels.Get(QuestId);
            {
                await channel.PublishAsync(
                    "add-player",
                        new
                        {
                            name = playerName,
                            characterClass = characterClass,
                            health = health
                        }
                    );
            }
        }

        private async Task PublishUpdatePlayer(string playerName, string characterClass, int health, int? damage)
        {
            var channel = _realtimeClient.Channels.Get(QuestId);
            {
                await channel.PublishAsync(
                    "update-player",
                        new
                        {
                            name = playerName,
                            characterClass = characterClass,
                            health = health,
                            damage = damage
                        }
                    );
            }
        }
    }
}