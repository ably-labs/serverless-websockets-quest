using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json;

namespace AblyLabs.ServerlessWebsocketsQuest.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Player : IPlayer
    {
        [JsonProperty("characterClass")]
        public string CharacterClass { get; set; }
        public void SetCharacterClass(string characterClass) => CharacterClass = characterClass;
        [JsonProperty("health")]
        public int Health { get; set; }
        public void SetHealth(int health) => Health = health;
        public void ApplyDamage(int damage) => Health = damage > Health ? 0 : Health - damage;
        public bool IsDefeated => Health <= 0;

        public static string GetEntityId(string questId, string playerId) => $"{questId}-{playerId}";

        [FunctionName(nameof(Player))]
        public static Task Run(
            [EntityTrigger] IDurableEntityContext ctx)
            => ctx.DispatchAsync<Player>();
    }
}