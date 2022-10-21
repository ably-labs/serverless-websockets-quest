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
        private const int NumberOfPlayers = 4;
        private readonly Publisher _publisher;

        public GameState(Publisher publisher)
        {
            _publisher = publisher;
            QuestId = string.Empty;
            Phase = string.Empty;
            PlayerNames = new List<string>();
        }

        [JsonProperty("questId")]
        public string QuestId { get; set; }

        [JsonProperty("phase")]
        public string Phase { get; set; }

        [JsonProperty("players")]
        public List<string> PlayerNames { get; set; }

        public async Task InitGameState(string[] gameStateFields)
        {
            QuestId = gameStateFields[0];
            Phase = gameStateFields[1];
            await _publisher.PublishUpdatePhase(QuestId, Phase);
        }

        public async Task UpdatePhase(string phase)
        {
            Phase = phase;
            await _publisher.PublishUpdatePhase(QuestId, Phase);
        }

        public async Task AddPlayerName(string playerName)
        {
            if (PlayerNames == null)
            {
                PlayerNames = new List<string> { playerName };
            }
            else
            {
                PlayerNames.Add(playerName);
            }

            if (IsPartyComplete)
            {
                await UpdatePhase(GamePhases.Play);
                await Task.Delay(1000);
                await AttackByMonster();
            }
        }

        public async Task RemovePlayerName(string playerName)
        {
            PlayerNames.Remove(playerName);

            if (PlayerNames.Count == 0)
            {
                var teamHasWon = false;
                await _publisher.PublishUpdatePhase(QuestId, GamePhases.End, teamHasWon);
            }
        }

        private async Task AttackByMonster()
        {
            var playerAttacking = CharacterClassDefinitions.Monster.Name;
            var playerUnderAttack = GetRandomPlayerName();
            var damage = CharacterClassDefinitions.GetDamageFor(CharacterClassDefinitions.Monster.CharacterClass);
            await _publisher.PublishPlayerAttacking(QuestId, playerAttacking, playerUnderAttack, damage);
            await Task.Delay(1000);
            var playerEntityId = new EntityId(nameof(Player), Player.GetEntityId(QuestId, playerUnderAttack));
            Entity.Current.SignalEntity<IPlayer>(playerEntityId, proxy => proxy.ApplyDamage(damage));
            await Task.Delay(1000);
            var nextPlayerName = GetNextPlayerName(CharacterClassDefinitions.Monster.Name);
            await _publisher.PublishPlayerTurnAsync(QuestId, $"Next turn: {nextPlayerName}", nextPlayerName);
        }

        public string GetNextPlayerName(string currentPlayerName)
        {
            var currentIndex = PlayerNames.FindIndex(0, PlayerNames.Count, p => p == currentPlayerName);
            string nextPlayer = currentIndex == PlayerNames.Count - 1 ? PlayerNames[0] : PlayerNames[currentIndex + 1];

            return nextPlayer;
        }

        public bool IsPartyComplete => PlayerNames.Count == NumberOfPlayers;

        public string GetRandomPlayerName()
        {
            var playerNamesWithoutMonster = PlayerNames.Where(p => p != CharacterClassDefinitions.Monster.Name).ToList();
            var index = new Random().Next(0, playerNamesWithoutMonster.Count);
            return playerNamesWithoutMonster[index];
        }

        [FunctionName(nameof(GameState))]
        public static Task Run([EntityTrigger] IDurableEntityContext ctx)
            => ctx.DispatchAsync<GameState>();
    }
}
