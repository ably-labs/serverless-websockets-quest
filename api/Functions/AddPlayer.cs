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
    public class AddPlayer
    {
        /// The JoinQuest function is called when a player joins a quest created by the host.
        /// The Player Id & Health will be stored in a Durable Entity.
        [FunctionName(nameof(AddPlayer))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequestMessage req,
            [DurableClient] IDurableClient durableClient,
            ILogger log)
        {
            var questData = await req.Content.ReadAsAsync<QuestData>();
            var gameEngine = new GameEngine(durableClient, questData.QuestId, null);
            await gameEngine.AddplayerAsync(questData.PlayerId, 50);

            return new AcceptedResult();
        }
    }
}
