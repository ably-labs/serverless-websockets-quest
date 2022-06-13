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
        private readonly Publisher _publisher;

        public GameState(IRealtimeClient realtimeClient)
        {
            _realtimeClient = realtimeClient;
            _publisher = new Publisher(_realtimeClient);
        }
        
        [JsonProperty("questId")]
        public string QuestId { get; set; }
        public async Task InitGameState(string[] gameStateFields)
        {
            QuestId = gameStateFields[0];
            Phase = gameStateFields[1];
            await _publisher.PublishUpdatePhase(QuestId, Phase);
        }

        [JsonProperty("phase")]
        public string Phase { get; set; }
        public async Task UpdatePhase(string phase)
        {
            Phase = phase;
            await _publisher.PublishUpdatePhase(QuestId, Phase);
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
                await AttackByMonster();
                await Task.Delay(2000).ContinueWith( async _ =>
                    {
                        var nextPlayerName = GetNextPlayerName(null);
                        await _publisher.PublishPlayerTurnAsync(QuestId, $"Next turn: {nextPlayerName}", nextPlayerName);
                    }
                );
            }
        }

        private async Task AttackByMonster()
        {
            await _publisher.PublishUpdateMessage(QuestId, "The monster attacks!", "Wait for your turn");
            var playerUnderAttack = GetRandomPlayerName();
            var damage = CharacterClassDefinitions.GetDamageFor(CharacterClassDefinitions.Monster.CharacterClass);
            var playerEntityId = new EntityId(nameof(Player), Player.GetEntityId(QuestId, playerUnderAttack));
            Entity.Current.SignalEntity<IPlayer>(playerEntityId, proxy => proxy.ApplyDamage(damage));
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
    }
}