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
            var turnData = await req.Content.ReadAsAsync<TurnData>();
            var entityId = new EntityId(nameof(GameState), turnData.QuestId);
            var gameState = await durableClient.ReadEntityStateAsync<GameState>(entityId);
            if (gameState.EntityExists)
            {
                var channel = _realtime.Channels.Get(turnData.QuestId);
                var gameEngine = new GameEngine(durableClient, channel, turnData.QuestId);
                if (turnData.IsMonster)
                {
                    await gameEngine.AttackByMonster(gameState);
                }
                else
                {
                    await gameEngine.AttackByPlayer(turnData.PlayerId, gameState);
                }
            }

            return new AcceptedResult();
        }
    }
}
