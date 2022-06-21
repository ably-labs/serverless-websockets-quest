using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using AblyLabs.ServerlessWebsocketsQuest.Models;
using IO.Ably;

namespace AblyLabs.ServerlessWebsocketsQuest
{
    public class CreateQuest
    {
        private IRestClient _ablyClient;

        public CreateQuest(IRestClient ablyClient)
        {
            _ablyClient = ablyClient;
        }

        /// The CreateQuest function is called by the host (the first player).
        /// The function creates the initial GameState and stores this as a Durable Entity.
        [FunctionName(nameof(CreateQuest))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequestMessage req,
            [DurableClient] IDurableClient durableClient,
            ILogger log)
        {
            if (req.Content != null)
            {
                var questId = await req.Content.ReadAsStringAsync();
                var channel = _ablyClient.Channels.Get(questId);
                var gameEngine = new GameEngine(durableClient, questId, channel);
                var gamePhase = await gameEngine.CreateQuestAsync();
                return new OkObjectResult(gamePhase);
            }
            else
            {
                return new BadRequestObjectResult("QuestId is required");
            }
        }
    }
}
