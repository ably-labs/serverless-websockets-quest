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
        private readonly Publisher _publisher;

        public Player(IRealtimeClient realtimeClient)
        {
            _realtimeClient = realtimeClient;
            _publisher = new Publisher(_realtimeClient);
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
            await _publisher.PublishAddPlayer(QuestId, PlayerName, CharacterClass, Health);
        }
        
        public async Task ApplyDamage(int damage)
        {
            Health = damage > Health ? 0 : Health - damage;
            bool isDefeated = Health <= 0;
            if (isDefeated) {
                var gameStateEntityId = new EntityId(nameof(GameState), QuestId);
                Entity.Current.SignalEntity<IGameState>(gameStateEntityId, proxy => proxy.RemovePlayerName(PlayerName));
                var message = $"{PlayerName} is defeated!";
                await _publisher.PublishUpdateMessage(QuestId, message, false);
                if (PlayerName == CharacterClassDefinitions.Monster.Name)
                {
                    await Task.Delay(1500);
                    await _publisher.PublishUpdatePhase(QuestId, GamePhases.End);
                }
            }
            await _publisher.PublishPlayerUnderAttack(QuestId, PlayerName, CharacterClass, Health, damage, isDefeated);
        }

        public static string GetEntityId(string questId, string playerName) => $"{questId}-{playerName}";

        [FunctionName(nameof(Player))]
        public static Task Run(
            [EntityTrigger] IDurableEntityContext ctx)
            => ctx.DispatchAsync<Player>();
    }
}