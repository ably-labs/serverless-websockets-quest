using System.Threading.Tasks;
using AblyLabs.ServerlessWebsocketsQuest.Models;
using IO.Ably.Realtime;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace AblyLabs.ServerlessWebsocketsQuest
{
    public class GameEngine
    {
        private readonly IDurableClient _durableClient;
        private readonly IRealtimeChannel _channel;
        private readonly string _questId;

        public GameEngine(IDurableClient durableClient, IRealtimeChannel channel, string questId)
        {
            _durableClient = durableClient;
            _channel = channel;
            _questId = questId;
        }

        public async Task AttackByMonster(EntityStateResponse<GameState> gameState)
        {
            var playerId = gameState.EntityState.GetRandomPlayerId();
            var damage = Monster.GetAttackDamage();

            var playerEntityId = new EntityId(nameof(Player), Player.GetEntityId(_questId, playerId));
            await _durableClient.SignalEntityAsync<IPlayer>(playerEntityId, proxy => proxy.ApplyDamage(damage));
            var player = await _durableClient.ReadEntityStateAsync<Player>(playerEntityId);

            await _channel.PublishAsync(
            "update-player",
                new
                {
                    playerId = playerId,
                    health = player.EntityState.Health,
                    damage = damage
                }
            );

            var nextPlayerId = gameState.EntityState.GetNextPlayerId(null);
            await PublishPlayerTurn(nextPlayerId);
        }

        public async Task AttackByPlayer(string playerId, EntityStateResponse<GameState> gameState)
        {
            var monsterEntityId = new EntityId(nameof(Monster), Monster.GetEntityId(_questId));
            var damage = Player.GetAttackDamage();
            await _durableClient.SignalEntityAsync<IMonster>(monsterEntityId, proxy => proxy.ApplyDamage(damage));
            var monster = await _durableClient.ReadEntityStateAsync<Monster>(monsterEntityId);

            await _channel.PublishAsync(
                "update-monster",
                new
                {
                    damage = damage,
                    health = monster.EntityState.Health
                }
            );

            var nextPlayerId = gameState.EntityState.GetNextPlayerId(playerId);
            await PublishPlayerTurn(nextPlayerId);
        }

        private async Task PublishPlayerTurn(string playerId)
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