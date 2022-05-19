using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json;

namespace AblyLabs.ServerlessWebsocketsQuest.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Monster : IPlayer
    {
        public Monster(string id, int health)
        {
            Id = id;
            Health = health;
        }

        [JsonProperty("id")]
        public string Id { get; set; }
        public void SetId(string id) => Id = id;

        [JsonProperty("health")]
        public int Health { get; set; }
        public void SetHealth(int health) => Health = health;
        public void ApplyDamage(int damage) => Health = damage > Health ? 0 : Health - damage;

        [FunctionName(nameof(Monster))]
        public static Task Run(
            [EntityTrigger] IDurableEntityContext ctx)
            => ctx.DispatchAsync<Monster>();
    }
}