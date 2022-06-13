using System.Threading.Tasks;
using IO.Ably;
using IO.Ably.Realtime;

namespace AblyLabs.ServerlessWebsocketsQuest.Models
{
    public class Publisher
    {
        private readonly IRealtimeClient? _realtimeClient;
        private IRealtimeChannel? _realtimeChannel;

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

        public async Task PublishUpdatePlayer(string questId, string playerName, string characterClass, int health, int? damage, bool isDefeated)
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
                            damage = damage,
                            isDefeated = isDefeated
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

        public async Task PublishUpdateMessage(string questId, string message, bool isError)
        {
            if (_realtimeClient != null)
            {
                _realtimeChannel = _realtimeClient.Channels.Get(questId);
            }
            if (_realtimeChannel != null) {
                await _realtimeChannel.PublishAsync(
                    "update-message",
                        new
                        {
                            message = message,
                            isError = isError
                        }
                    );
            }
        }

        public async Task PublishPlayerTurnAsync(string questId, string message, string playerName)
        {
            if (_realtimeClient != null)
            {
                _realtimeChannel = _realtimeClient.Channels.Get(questId);
            }
            if (_realtimeChannel != null)
            {
                await _realtimeChannel.PublishAsync(
                    "check-player-turn",
                    new
                    {
                        message = message,
                        name = playerName
                    }
                );
            }
        }
    }
}