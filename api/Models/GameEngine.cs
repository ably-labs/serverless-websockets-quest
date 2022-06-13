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
        private readonly Publisher _publisher;

        public GameEngine(IDurableClient durableClient, string questId, IRealtimeChannel channel)
        {
            _durableClient = durableClient;
            _questId = questId;
            _channel = channel;
            _publisher = new Publisher(_channel);
        }

        public async Task<string> CreateQuestAsync()
        {
            await InitializeGameStateAsync(GamePhases.Character);
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

        private async Task InitializeGameStateAsync(string phase)
        {
            var monsterEntityId = new EntityId(nameof(Player), Player.GetEntityId(_questId, CharacterClassDefinitions.Monster.Name));
            await _durableClient.SignalEntityAsync<IPlayer>(monsterEntityId, proxy => proxy.InitPlayer(
                new object[] {
                    _questId,
                    CharacterClassDefinitions.Monster.Name,
                    CharacterClassDefinitions.Monster.CharacterClass,
                    CharacterClassDefinitions.Monster.InitialHealth
                    }));

            var gameStateEntityId = new EntityId(nameof(GameState), _questId);
            await _durableClient.SignalEntityAsync<IGameState>(gameStateEntityId, proxy => proxy.InitGameState(new[] { _questId, phase }));
            await _durableClient.SignalEntityAsync<IGameState>(gameStateEntityId, proxy => proxy.AddPlayerName(CharacterClassDefinitions.Monster.Name));
        }

        public async Task AddPlayerAsync(string playerName, string characterClass)
        {
            var gameStateEntityId = new EntityId(nameof(GameState), _questId);
            var gameState = await _durableClient.ReadEntityStateAsync<GameState>(gameStateEntityId);
            if (!gameState.EntityState.IsPartyComplete)
            {
                var playerEntityId = new EntityId(nameof(Player), Player.GetEntityId(_questId, playerName));
                await _durableClient.SignalEntityAsync<IPlayer>(playerEntityId, proxy => proxy.InitPlayer(
                    new object[] {
                        _questId,
                        playerName,
                        characterClass,
                        CharacterClassDefinitions.GetInitialHealthFor(characterClass)
                    }));

                await _durableClient.SignalEntityAsync<IGameState>(gameStateEntityId, proxy => proxy.AddPlayerName(playerName));
            }
            else
            {
                await _publisher.PublishUpdateMessage(_questId, Texts.MaxNumberOfPlayers, true);
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
            var playerAttacking = CharacterClassDefinitions.Monster.Name;
            var playerUnderAttack = gameState.GetRandomPlayerName();
            var damage = CharacterClassDefinitions.GetDamageFor(CharacterClassDefinitions.Monster.CharacterClass);
            await AttackAsync(playerAttacking, playerUnderAttack, gameState, damage);
        }

        private async Task AttackByPlayerAsync(string playerAttacking, GameState gameState)
        {
            var playerEntityId = new EntityId(nameof(Player), Player.GetEntityId(_questId, playerAttacking));
            var player = await _durableClient.ReadEntityStateAsync<Player>(playerEntityId);
            var damage = CharacterClassDefinitions.GetDamageFor(player.EntityState.CharacterClass);
            await AttackAsync(playerAttacking, CharacterClassDefinitions.Monster.Name, gameState, damage);
        }

        private async Task AttackAsync(string playerAttacking, string playerUnderAttack, GameState gameState, int damage)
        {
            var message1 = $"{playerAttacking} attacks {playerUnderAttack}!";
            await  _publisher.PublishUpdateMessage(_questId, message1, false);
            Task.Delay(1500).Wait();
            var playerEntityId = new EntityId(nameof(Player), Player.GetEntityId(_questId, playerUnderAttack));
            await _durableClient.SignalEntityAsync<IPlayer>(playerEntityId, proxy => proxy.ApplyDamage(damage));
            var nextPlayerName = gameState.GetNextPlayerName(playerAttacking);
            Task.Delay(1500).Wait();
            var message2 = $"Next turn: {nextPlayerName}";
            await _publisher.PublishPlayerTurnAsync(_questId, message2, nextPlayerName);
        }
    }
}