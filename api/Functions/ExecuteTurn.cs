using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using AblyLabs.ServerlessWebsocketsQuest.Models;

namespace AblyLabs.ServerlessWebsocketsQuest
{
    public class ExecuteTurn
    {
        private Publisher _publisher;

        public ExecuteTurn(Publisher publisher)
        {
            _publisher = publisher;
        }

        /// The ExecuteTurn function is called by a player that performs a turn.
        [FunctionName(nameof(ExecuteTurn))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequestMessage req,
            [DurableClient] IDurableClient durableClient,
            ILogger log)
        {
            var questData = await req.Content.ReadAsAsync<QuestData>();
            var gameEngine = new GameEngine(durableClient, questData.QuestId, _publisher);
            await gameEngine.ExecuteTurnAsync(questData.PlayerName);

            return new AcceptedResult();
        }
    }
}
