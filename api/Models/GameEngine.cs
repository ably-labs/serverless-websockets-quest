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

        public async Task CreateQuestAsync(int monsterHealth)
        {
            await CreateMonsterAsync(monsterHealth);
        }

        private async Task SetHostAsync(string hostId)
        {
            var gameStateEntityId = new EntityId(nameof(GameState), _questId);
            await _durableClient.SignalEntityAsync<IGameState>(gameStateEntityId, proxy => proxy.SetHost(hostId));
        }

        private async Task CreateMonsterAsync(int health)
        {
            var monsterEntityId = new EntityId(nameof(Monster), Monster.GetEntityId(_questId));
            await _durableClient.SignalEntityAsync<IMonster>(monsterEntityId, proxy => proxy.SetHealth(health));

            var gameStateEntityId = new EntityId(nameof(GameState), _questId);
            await _durableClient.SignalEntityAsync<IGameState>(gameStateEntityId, proxy => proxy.AddPlayerId(Monster.ID));
        }

        public async Task AddplayerAsync(string playerId, int health)
        {
            var gameStateEntityId = new EntityId(nameof(GameState), _questId);
            await _durableClient.SignalEntityAsync<IGameState>(gameStateEntityId, proxy => proxy.AddPlayerId(playerId));

            var playerEntityId = new EntityId(nameof(Player), Player.GetEntityId(_questId, playerId));
            await _durableClient.SignalEntityAsync<IPlayer>(playerEntityId, proxy => proxy.SetHealth(health));
        }

        public async Task<bool> CheckQuestExistsAsync()
        {
            var entityId = new EntityId(nameof(GameState), _questId);
            var gameState = await _durableClient.ReadEntityStateAsync<GameState>(entityId);

            return gameState.EntityExists;
        }

        public async Task ExecuteTurnAsync(string playerId)
        {
            var entityId = new EntityId(nameof(GameState), _questId);
            var gameState = await _durableClient.ReadEntityStateAsync<GameState>(entityId);

            if (playerId == Monster.ID)
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
            var damage = Monster.GetAttackDamage();

            var playerEntityId = new EntityId(nameof(Player), Player.GetEntityId(_questId, playerId));
            await _durableClient.SignalEntityAsync<IPlayer>(playerEntityId, proxy => proxy.ApplyDamage(damage));
            var player = await _durableClient.ReadEntityStateAsync<Player>(playerEntityId);

            if (_channel != null)
            {
                await PublishUpdatePlayer(playerId, player.EntityState.Health, damage);

                var nextPlayerId = gameState.GetNextPlayerId(null);
                await PublishPlayerTurnAsync(nextPlayerId);
            }
        }

        private async Task AttackByPlayerAsync(string playerId, GameState gameState)
        {
            var monsterEntityId = new EntityId(nameof(Monster), Monster.GetEntityId(_questId));
            var damage = Player.GetAttackDamage();
            await _durableClient.SignalEntityAsync<IMonster>(monsterEntityId, proxy => proxy.ApplyDamage(damage));
            var monster = await _durableClient.ReadEntityStateAsync<Monster>(monsterEntityId);

            if (_channel != null)
            {
                await PublishUpdatePlayer(Monster.ID, monster.EntityState.Health, damage);

                var nextPlayerId = gameState.GetNextPlayerId(playerId);
                await PublishPlayerTurnAsync(nextPlayerId);
            }
        }

        private async Task PublishUpdatePlayer(string playerId, int health, int damage)
        {
            if (_channel != null)
            {
                await _channel.PublishAsync(
                    "update-player",
                        new
                        {
                            playerId = playerId,
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