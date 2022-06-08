using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using IO.Ably.Realtime;

namespace AblyLabs.ServerlessWebsocketsQuest.Models
{
    public class GameEngine
    {
        private readonly IDurableClient _durableClient;
        private readonly string _questId;
        private readonly IRealtimeChannel? _channel;

        public GameEngine(IDurableClient durableClient, string questId, IRealtimeChannel? channel)
        {
            _durableClient = durableClient;
            _questId = questId;
            _channel = channel;
        }

        public async Task<string> CreateQuestAsync()
        {
            await CreateMonsterAsync();
            await SetPhaseAsync(GamePhases.Character);
            return GamePhases.Character;
        }

        public async Task<(bool QuestExists, string Phase, string Message)> DoesQuestExistAsync()
        {
            var gameStateEntityId = new EntityId(nameof(GameState), _questId);
            var gameState = await _durableClient.ReadEntityStateAsync<GameState>(gameStateEntityId);
            if (gameState.EntityExists)
            {
                if (!gameState.EntityState.IsPartyComplete)
                {
                    return (true, GamePhases.Character, "");
                }
                else
                {
                    return(false, GamePhases.Start, "Quest reached maximum number of players ¯\\_(ツ)_/¯");
                }
            }
            else
            {
                return (false, GamePhases.Start, "No quest was found with this ID (╥﹏╥)");
            }
        }

        private async Task SetPhaseAsync(string phaseId)
        {
            var gameStateEntityId = new EntityId(nameof(GameState), _questId);
            await _durableClient.SignalEntityAsync<IGameState>(gameStateEntityId, proxy => proxy.SetPhase(phaseId));
        }

        private async Task CreateMonsterAsync()
        {
            var monsterEntityId = new EntityId(nameof(Player), Player.GetEntityId(_questId, CharacterClassDefinitions.Monster.CharacterClass));
            await _durableClient.SignalEntityAsync<IPlayer>(monsterEntityId, proxy => proxy.SetCharacterClass(CharacterClassDefinitions.Monster.CharacterClass));
            var health = CharacterClassDefinitions.GetInitialHealthFor(CharacterClassDefinitions.Monster.CharacterClass);
            await _durableClient.SignalEntityAsync<IPlayer>(monsterEntityId, proxy => proxy.SetHealth(CharacterClassDefinitions.Monster.InitialHealth));

            var gameStateEntityId = new EntityId(nameof(GameState), _questId);
            await _durableClient.SignalEntityAsync<IGameState>(gameStateEntityId, proxy => proxy.AddPlayerId(CharacterClassDefinitions.Monster.CharacterClass));
        }

        public async Task AddPlayerAsync(string playerId, string characterClass)
        {
            var gameStateEntityId = new EntityId(nameof(GameState), _questId);
            var gameState = await _durableClient.ReadEntityStateAsync<GameState>(gameStateEntityId);
            if (!gameState.EntityState.IsPartyComplete)
            {
                if (gameState.EntityState.PlayerIds.Count == 1) {
                    // Publish monster data now there is a channel
                    var monsterEntityId = new EntityId(nameof(Player), Player.GetEntityId(_questId, CharacterClassDefinitions.Monster.CharacterClass));
                    var monster = await _durableClient.ReadEntityStateAsync<Player>(monsterEntityId);
                    await PublishUpdatePlayer(CharacterClassDefinitions.Monster.CharacterClass, monster.EntityState.CharacterClass, monster.EntityState.Health, null);
                }
                
                var playerEntityId = new EntityId(nameof(Player), Player.GetEntityId(_questId, playerId));
                await _durableClient.SignalEntityAsync<IPlayer>(playerEntityId, proxy => proxy.SetCharacterClass(characterClass));
                var health = CharacterClassDefinitions.GetInitialHealthFor(characterClass);
                await _durableClient.SignalEntityAsync<IPlayer>(playerEntityId, proxy => proxy.SetHealth(health));
                await PublishUpdatePlayer(playerId, characterClass, health, null);

                await _durableClient.SignalEntityAsync<IGameState>(gameStateEntityId, proxy => proxy.AddPlayerId(playerId));
                await _durableClient.ReadEntityStateAsync<GameState>(gameStateEntityId);
                if (gameState.EntityState.IsPartyComplete)
                {
                    await PublishUpdatePhase(GamePhases.Play);
                }
            }
            else
            {
                await PublishUpdateMessage("Quest reached maximum number of players", true);
            }
        }

        public async Task ExecuteTurnAsync(string playerId)
        {
            var entityId = new EntityId(nameof(GameState), _questId);
            var gameState = await _durableClient.ReadEntityStateAsync<GameState>(entityId);

            if (playerId == CharacterClassDefinitions.Monster.CharacterClass)
            {
                await AttackByMonsterAsync(gameState.EntityState);
            }
            else
            {
                await AttackByPlayerAsync(playerId, gameState.EntityState);
            }
        }

        private async Task AttackByMonsterAsync(GameState gameState)
        {
            var playerId = gameState.GetRandomPlayerId();
            var damage = CharacterClassDefinitions.GetDamageFor(CharacterClassDefinitions.Monster.CharacterClass);

            var playerEntityId = new EntityId(nameof(Player), Player.GetEntityId(_questId, playerId));
            await _durableClient.SignalEntityAsync<IPlayer>(playerEntityId, proxy => proxy.ApplyDamage(damage));
            var player = await _durableClient.ReadEntityStateAsync<Player>(playerEntityId);

            await PublishUpdatePlayer(playerId, player.EntityState.CharacterClass, player.EntityState.Health, damage);

            var nextPlayerId = gameState.GetNextPlayerId(null);
            await PublishPlayerTurnAsync(nextPlayerId);
        }

        private async Task AttackByPlayerAsync(string playerId, GameState gameState)
        {
            var playerEntityId = new EntityId(nameof(Player), Player.GetEntityId(_questId, playerId));
            var player = await _durableClient.ReadEntityStateAsync<Player>(playerEntityId);
            var damage = CharacterClassDefinitions.GetDamageFor(player.EntityState.CharacterClass);

            var monsterEntityId = new EntityId(nameof(Player), Player.GetEntityId(_questId, CharacterClassDefinitions.Monster.CharacterClass));
            await _durableClient.SignalEntityAsync<IPlayer>(monsterEntityId, proxy => proxy.ApplyDamage(damage));
            var monster = await _durableClient.ReadEntityStateAsync<Player>(monsterEntityId);

            await PublishUpdatePlayer(CharacterClassDefinitions.Monster.CharacterClass, CharacterClassDefinitions.Monster.CharacterClass, monster.EntityState.Health, damage);

            var nextPlayerId = gameState.GetNextPlayerId(playerId);
            await PublishPlayerTurnAsync(nextPlayerId);
        }

        private async Task PublishUpdatePhase(string phase)
        {
            if (_channel != null)
            {
                await _channel.PublishAsync(
                    "update-phase",
                        new
                        {
                            phase = phase
                        }
                );
            }
        }

        private async Task PublishUpdateMessage(string message, bool isError)
        {
            if (_channel != null)
            {
                await _channel.PublishAsync(
                    "update-message",
                        new
                        {
                            message = message,
                            isError = isError
                        }
                    );
            }
        }

        private async Task PublishUpdatePlayer(string playerId, string characterClass, int health, int? damage)
        {
            if (_channel != null)
            {
                await _channel.PublishAsync(
                    "update-player",
                        new
                        {
                            playerId = playerId,
                            characterClass = characterClass,
                            health = health,
                            damage = damage
                        }
                    );
            }
        }

        private async Task PublishPlayerTurnAsync(string playerId)
        {
            if (_channel != null)
            {
                await _channel.PublishAsync(
                    "check-player-turn",
                    new
                    {
                        playerId = playerId
                    }
                );
            }
        }
    }
}