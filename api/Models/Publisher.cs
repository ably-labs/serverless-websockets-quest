using System.Threading.Tasks;
using IO.Ably;

namespace AblyLabs.ServerlessWebsocketsQuest.Models
{
    public class Publisher
    {
        private readonly IRestClient? _ablyClient;

        public Publisher()
        {
        }

        public Publisher(IRestClient ablyClient)
        {
            _ablyClient = ablyClient;
        }

        public async Task PublishAddPlayer(string questId, string playerName, string characterClass, int health)
        {
            if (_ablyClient != null)
            {
                var channel = _ablyClient.Channels.Get(questId);
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

        public async Task PublishPlayerUnderAttack(string questId, string playerName, string characterClass, int health, int? damage, bool isDefeated)
        {
            if (_ablyClient != null)
            {
                var channel = _ablyClient.Channels.Get(questId);
                await channel.PublishAsync(
                    "player-under-attack",
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

        public async Task PublishPlayerAttacking(string questId, string playerAttacking, string playerUnderAttack, int damage)
        {
            if (_ablyClient != null)
            {
                var channel = _ablyClient.Channels.Get(questId);
                await channel.PublishAsync(
                    "player-attacking",
                        new
                        {
                            playerAttacking = playerAttacking,
                            playerUnderAttack = playerUnderAttack,
                            damage = damage
                        }
                    );
            }
        }

        public async Task PublishUpdatePhase(string questId, string phase, bool? teamHasWon = null)
        {
            if (_ablyClient != null)
            {
                var channel = _ablyClient.Channels.Get(questId);
                await channel.PublishAsync(
                    "update-phase",
                    new
                    {
                        phase = phase,
                        teamHasWon = teamHasWon
                    }
                );
            }
        }

        public async Task PublishUpdateMessage(string questId, string message, bool isError)
        {
            if (_ablyClient != null)
            {
                var channel = _ablyClient.Channels.Get(questId);
                await channel.PublishAsync(
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
            if (_ablyClient != null)
            {
                var channel = _ablyClient.Channels.Get(questId);
                await channel.PublishAsync(
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
