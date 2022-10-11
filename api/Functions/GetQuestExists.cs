using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using AblyLabs.ServerlessWebsocketsQuest.Models;

namespace AblyLabs.ServerlessWebsocketsQuest
{
    public class GetQuestExists
    {
        private Publisher _publisher;

        public GetQuestExists(Publisher publisher)
        {
            _publisher = publisher;
        }

        /// The GetQuestExist function is called when a player wants to join a quest created by the host.
        [FunctionName(nameof(GetQuestExists))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetQuestExists/{questId}")] HttpRequestMessage req,
            [DurableClient] IDurableClient durableClient,
            string questId,
            ILogger log)
        {
            var gameEngine = new GameEngine(durableClient, questId, _publisher);
            var result = await gameEngine.DoesQuestExistAsync();

            if (result.QuestExists)
            {
                return new OkObjectResult(result.Phase);
            }
            else
            {
                return new BadRequestObjectResult(result.Message);
            }
        }
    }
}
