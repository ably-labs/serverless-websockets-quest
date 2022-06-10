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
            var monsterEntityId = new EntityId(nameof(Player), Player.GetEntityId(_questId, CharacterClassDefinitions.Monster.Name));
            await _durableClient.SignalEntityAsync<IPlayer>(monsterEntityId, proxy => proxy.SetCharacterClass(CharacterClassDefinitions.Monster.CharacterClass));
            var health = CharacterClassDefinitions.GetInitialHealthFor(CharacterClassDefinitions.Monster.CharacterClass);
            await _durableClient.SignalEntityAsync<IPlayer>(monsterEntityId, proxy => proxy.SetHealth(CharacterClassDefinitions.Monster.InitialHealth));
            var monster = await _durableClient.ReadEntityStateAsync<Player>(monsterEntityId);
            await PublishAddPlayer(CharacterClassDefinitions.Monster.Name, CharacterClassDefinitions.Monster.CharacterClass, health);

            var gameStateEntityId = new EntityId(nameof(GameState), _questId);
            await _durableClient.SignalEntityAsync<IGameState>(gameStateEntityId, proxy => proxy.AddPlayerName(CharacterClassDefinitions.Monster.Name));
        }

        public async Task AddPlayerAsync(string playerName, string characterClass)
        {
            var gameStateEntityId = new EntityId(nameof(GameState), _questId);
            var gameState = await _durableClient.ReadEntityStateAsync<GameState>(gameStateEntityId);
            if (!gameState.EntityState.IsPartyComplete)
            {
                var playerEntityId = new EntityId(nameof(Player), Player.GetEntityId(_questId, playerName));
                await _durableClient.SignalEntityAsync<IPlayer>(playerEntityId, proxy => proxy.SetCharacterClass(characterClass));
                var health = CharacterClassDefinitions.GetInitialHealthFor(characterClass);
                await _durableClient.SignalEntityAsync<IPlayer>(playerEntityId, proxy => proxy.SetHealth(health));
                await PublishAddPlayer(playerName, characterClass, health);

                await _durableClient.SignalEntityAsync<IGameState>(gameStateEntityId, proxy => proxy.AddPlayerName(playerName));
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

        public async Task ExecuteTurnAsync(string playerName)
        {
            var entityId = new EntityId(nameof(GameState), _questId);
            var gameState = await _durableClient.ReadEntityStateAsync<GameState>(entityId);

            if (playerName == CharacterClassDefinitions.Monster.Name)
            {
                await AttackByMonsterAsync(gameState.EntityState);
            }
            else
            {
                await AttackByPlayerAsync(playerName, gameState.EntityState);
            }
        }

        private async Task AttackByMonsterAsync(GameState gameState)
        {
            var playerName = gameState.GetRandomPlayerName();
            var damage = CharacterClassDefinitions.GetDamageFor(CharacterClassDefinitions.Monster.CharacterClass);

            var playerEntityId = new EntityId(nameof(Player), Player.GetEntityId(_questId, playerName));
            await _durableClient.SignalEntityAsync<IPlayer>(playerEntityId, proxy => proxy.ApplyDamage(damage));
            var player = await _durableClient.ReadEntityStateAsync<Player>(playerEntityId);

            await PublishUpdatePlayer(playerName, player.EntityState.CharacterClass, player.EntityState.Health, damage);

            var nextPlayerName = gameState.GetNextPlayerName(null);
            await PublishPlayerTurnAsync(nextPlayerName);
        }

        private async Task AttackByPlayerAsync(string playerName, GameState gameState)
        {
            var playerEntityId = new EntityId(nameof(Player), Player.GetEntityId(_questId, playerName));
            var player = await _durableClient.ReadEntityStateAsync<Player>(playerEntityId);
            var damage = CharacterClassDefinitions.GetDamageFor(player.EntityState.CharacterClass);

            var monsterEntityId = new EntityId(nameof(Player), Player.GetEntityId(_questId, CharacterClassDefinitions.Monster.Name));
            await _durableClient.SignalEntityAsync<IPlayer>(monsterEntityId, proxy => proxy.ApplyDamage(damage));
            var monster = await _durableClient.ReadEntityStateAsync<Player>(monsterEntityId);

            await PublishUpdatePlayer(CharacterClassDefinitions.Monster.Name, CharacterClassDefinitions.Monster.CharacterClass, monster.EntityState.Health, damage);

            var nextPlayerName = gameState.GetNextPlayerName(playerName);
            await PublishPlayerTurnAsync(nextPlayerName);
        }

         private async Task PublishAddPlayer(string playerName, string characterClass, int health)
        {
            if (_channel != null)
            {
                await _channel.PublishAsync(
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

        private async Task PublishUpdatePlayer(string playerName, string characterClass, int health, int? damage)
        {
            if (_channel != null)
            {
                await _channel.PublishAsync(
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

        private async Task PublishPlayerTurnAsync(string playerName)
        {
            if (_channel != null)
            {
                await _channel.PublishAsync(
                    "check-player-turn",
                    new
                    {
                        name = playerName
                    }
                );
            }
        }
    }
}