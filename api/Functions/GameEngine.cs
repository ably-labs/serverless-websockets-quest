using System.Threading.Tasks;
using AblyLabs.ServerlessWebsocketsQuest.Models;
using IO.Ably.Realtime;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace AblyLabs.ServerlessWebsocketsQuest
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

        public async Task SetHostAsync(string playerId)
        {
            var gameStateEntityId = new EntityId(nameof(GameState), _questId);
            await _durableClient.SignalEntityAsync<IGameState>(gameStateEntityId, proxy => proxy.SetHost(playerId));
        }

        public async Task CreateMonsterAsync(int health)
        {
            var monsterEntityId = new EntityId(nameof(Monster), Monster.GetEntityId(_questId));
            await _durableClient.SignalEntityAsync<IMonster>(monsterEntityId, proxy => proxy.SetHealth(health));

            var gameStateEntityId = new EntityId(nameof(GameState), _questId);
            await _durableClient.SignalEntityAsync<IGameState>(gameStateEntityId, proxy => proxy.AddPlayerId(Monster.ID));
        }

        public async Task<GameState> GetGameState()
        {
            var entityId = new EntityId(nameof(GameState), _questId);
            var gameState = await _durableClient.ReadEntityStateAsync<GameState>(entityId);

            return gameState.EntityState;
        }

        public async Task AttackByMonsterAsync(GameState gameState)
        {
            var playerId = gameState.GetRandomPlayerId();
            var damage = Monster.GetAttackDamage();

            var playerEntityId = new EntityId(nameof(Player), Player.GetEntityId(_questId, playerId));
            await _durableClient.SignalEntityAsync<IPlayer>(playerEntityId, proxy => proxy.ApplyDamage(damage));
            var player = await _durableClient.ReadEntityStateAsync<Player>(playerEntityId);

            if (_channel != null)
            {
                await _channel.PublishAsync(
                "update-player",
                    new
                    {
                        playerId = playerId,
                        health = player.EntityState.Health,
                        damage = damage
                    }
                );

                var nextPlayerId = gameState.GetNextPlayerId(null);
                await PublishPlayerTurnAsync(nextPlayerId);
            }
        }

        public async Task AttackByPlayerAsync(string playerId, GameState gameState)
        {
            var monsterEntityId = new EntityId(nameof(Monster), Monster.GetEntityId(_questId));
            var damage = Player.GetAttackDamage();
            await _durableClient.SignalEntityAsync<IMonster>(monsterEntityId, proxy => proxy.ApplyDamage(damage));
            var monster = await _durableClient.ReadEntityStateAsync<Monster>(monsterEntityId);

            if (_channel != null)
            {
                await _channel.PublishAsync(
                    "update-monster",
                    new
                    {
                        damage = damage,
                        health = monster.EntityState.Health
                    }
                );

                var nextPlayerId = gameState.GetNextPlayerId(playerId);
                await PublishPlayerTurnAsync(nextPlayerId);
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