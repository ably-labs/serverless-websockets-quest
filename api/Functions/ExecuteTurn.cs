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
            var turn = await req.Content.ReadAsAsync<Turn>();
            var entityId = new EntityId(nameof(GameState), turn.QuestId);
            // Update entityId
            // Publish monster damage and next player turn
            var responseJson = new { title = "This is a title from the api call!" };
            return new OkObjectResult(responseJson);
        }
    }
}
