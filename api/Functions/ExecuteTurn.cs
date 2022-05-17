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
                var channel = _realtime.Channels.Get(turn.QuestId);
                if (entityStateResponse.EntityState.IsMonsterTurn(turn.PlayerId))
                {
                    
                    await channel.PublishAsync(
                        "monster-attacks", 
                        new { 
                            playerId = entityStateResponse.EntityState.GetRandomPlayer(),
                            damage = entityStateResponse.EntityState.GetMonsterAttackDamage(),
                            nextPlayerId = entityStateResponse.EntityState.GetNextPlayer(null)
                        }
                    );
                }
                else
                {
                    var damage = entityStateResponse.EntityState.GetPlayerAttackDamage();
                    // update the monster health entity

                    await channel.PublishAsync(
                        "player-attacks", 
                        new { 
                            nextPlayerId = entityStateResponse.EntityState.GetNextPlayer(turn.PlayerId),
                            damage = damage
                        }
                    );
                }
                
                
            }

            return new OkResult();
        }
    }
}
