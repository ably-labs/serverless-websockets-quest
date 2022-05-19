using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using IO.Ably;
using System.Net.Http;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using AblyLabs.ServerlessWebsocketsQuest.Models;

namespace AblyLabs.ServerlessWebsocketsQuest
{
    public class StartQuest
    {
        private AblyRealtime _realtime;

        public StartQuest(AblyRealtime realtime)
        {
            _realtime = realtime;
        }

        /// The StartQuest function is called when all players have joined the quest.
        /// The PlayerIds will be stored in a Durable Entity.
        /// The monster will attack first.
        [FunctionName(nameof(StartQuest))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequestMessage req,
            [DurableClient] IDurableClient durableClient,
            ILogger log)
        {
            var startQuestData = await req.Content.ReadAsAsync<StartQuestData>();
            var entityId = new EntityId(nameof(GameState), startQuestData.QuestId);
            await durableClient.SignalEntityAsync<IGameState>(entityId, proxy => proxy.SetPlayers(startQuestData.Players));
            var entityStateResponse = await durableClient.ReadEntityStateAsync<GameState>(entityId);
            if (entityStateResponse.EntityExists)
            {
                var channel = _realtime.Channels.Get(startQuestData.QuestId);
                await channel.PublishAsync(
                    "update-player", 
                    new { 
                        playerId = entityStateResponse.EntityState.GetRandomPlayer(),
                        damage = entityStateResponse.EntityState.GetMonsterAttackDamage()
                    }
                );
                await channel.PublishAsync(
                    "check-player-turn", 
                    new { 
                        playerId = entityStateResponse.EntityState.GetNextPlayer(null)
                    }
                );
            }

            return new AcceptedResult();
        }
    }
}
