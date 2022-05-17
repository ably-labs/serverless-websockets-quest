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
    public class ExecuteTurn
    {
        private AblyRealtime _realtime;

        public ExecuteTurn(AblyRealtime realtime)
        {
            _realtime = realtime;
        }

        [FunctionName(nameof(ExecuteTurn))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequestMessage req,
            [DurableClient] IDurableClient durableClient,
            ILogger log)
        {
            // Read Turn object from Ably Message
            var turn = await req.Content.ReadAsAsync<TurnData>();
            var entityId = new EntityId(nameof(GameState), turn.QuestId);
            var entityStateResponse = await durableClient.ReadEntityStateAsync<GameState>(entityId);
            if (entityStateResponse.EntityExists)
            {
                // Determine if next turn is user or monster
                
                var channel = _realtime.Channels.Get(turn.QuestId);
                await channel.PublishAsync(
                    "monster-attack", 
                    new { 
                        playerId = entityStateResponse.EntityState.GetRandomPlayer(),
                        damage = entityStateResponse.EntityState.GetMonsterAttackDamage() 
                    }
                );
            }

            return new OkResult();
        }
    }
}
