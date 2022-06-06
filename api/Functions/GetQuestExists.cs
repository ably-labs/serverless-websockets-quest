using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using AblyLabs.ServerlessWebsocketsQuest.Models;

namespace AblyLabs.ServerlessWebsocketsQuest
{
    public class GetQuestExists
    {
        /// The JoinQuest function is called when a player joins a quest created by the host.
        /// The Player Id & Health will be stored in a Durable Entity.
        [FunctionName(nameof(GetQuestExists))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetQuestExists/{questId}")] HttpRequestMessage req,
            [DurableClient] IDurableClient durableClient,
            string questId,
            ILogger log)
        {
            var gameEngine = new GameEngine(durableClient, questId, null);
            var questExists = await gameEngine.CheckQuestExistsAsync();

            if (questExists) 
            {
                return new OkResult();
            }
            else
            {
                return new NotFoundResult();
            }
        }
    }
}
