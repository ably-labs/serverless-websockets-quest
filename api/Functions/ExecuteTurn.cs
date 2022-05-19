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

        // The ExecuteTurn function is called by a player that performs a turn.
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
                if (turn.IsMonster)
                {
                    await channel.PublishAsync(
                        "update-player",
                        new {
                            playerId = entityStateResponse.EntityState.GetRandomPlayer(),
                            damage = entityStateResponse.EntityState.GetMonsterAttackDamage(),
                        }
                    );
                    await channel.PublishAsync(
                        "check-player-turn", 
                        new {
                            playerId = entityStateResponse.EntityState.GetNextPlayer(null)
                        }
                    );
                }
                else
                {
                    var damage = entityStateResponse.EntityState.GetPlayerAttackDamage();
                    await durableClient.SignalEntityAsync<IGameState>(entityId, proxy => proxy.ApplyDamageToMonster(damage));
                    entityStateResponse = await durableClient.ReadEntityStateAsync<GameState>(entityId);

                    await channel.PublishAsync(
                        "update-monster",
                        new {
                            damage = damage,
                            monsterHealth = entityStateResponse.EntityState.MonsterHealth
                        }
                    );
                    await channel.PublishAsync(
                        "check-player-turn", 
                        new {
                            playerId = entityStateResponse.EntityState.GetNextPlayer(turn.Player.Id)
                        }
                    );
                }
            }

            return new AcceptedResult();
        }
    }
}
