using System.Threading.Tasks;
using IO.Ably;
using IO.Ably.Realtime;

namespace AblyLabs.ServerlessWebsocketsQuest.Models
{
    public class Publisher
    {
        private readonly IRealtimeClient? _realtimeClient;
        private readonly IRealtimeChannel? _realtimeChannel;

        public Publisher(IRealtimeClient realtimeClient)
        {
            _realtimeClient = realtimeClient;
        }

        public Publisher(IRealtimeChannel realtimeChannel)
        {
            _realtimeChannel = realtimeChannel;
        }

        public async Task PublishAddPlayer(string questId, string playerName, string characterClass, int health)
        {
           if (_realtimeClient != null)
            {
                var channel = _realtimeClient.Channels.Get(questId);
                await channel.PublishAsync(
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

        public async Task PublishUpdatePlayer(string questId, string playerName, string characterClass, int health, int? damage)
        {
            if (_realtimeClient != null)
            {
                var channel = _realtimeClient.Channels.Get(questId);
                await channel.PublishAsync(
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

        public async Task PublishUpdatePhase(string questId, string phase)
        {
            if (_realtimeClient != null)
            {
            var channel = _realtimeClient.Channels.Get(questId);
            await channel.PublishAsync(
                "update-phase",
                    new
                    {
                        phase = phase
                    }
                );
            }
        }
    }
}