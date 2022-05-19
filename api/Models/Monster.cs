using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json;

namespace AblyLabs.ServerlessWebsocketsQuest.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Monster : IMonster
    {
        public const string ID = "monster";

        [JsonProperty("health")]
        public int Health { get; set; }
        public void SetHealth(int health) => Health = health;
        public void ApplyDamage(int damage) => Health = damage > Health ? 0 : Health - damage;

        public bool IsDefeated => Health <= 0;

        public static int GetAttackDamage()
        {
            return new Random().Next(15, 30);
        }

        public static string GetEntityId(string questId) => $"{questId}-{ID}";

        [FunctionName(nameof(Monster))]
        public static Task Run(
            [EntityTrigger] IDurableEntityContext ctx)
            => ctx.DispatchAsync<Monster>();
    }
}