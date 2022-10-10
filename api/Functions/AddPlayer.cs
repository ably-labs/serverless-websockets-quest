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
    public class AddPlayer
    {
        private Publisher _publisher;

        public AddPlayer(Publisher publisher)
        {
            _publisher = publisher;
        }

        /// The AddPlayer function is called when a player joins a quest created by the host.
        /// The Player Id & Health will be stored in a Durable Entity.
        [FunctionName(nameof(AddPlayer))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequestMessage req,
            [DurableClient] IDurableClient durableClient,
            ILogger log)
        {
            var questData = await req.Content.ReadAsAsync<QuestData>();
            var gameEngine = new GameEngine(durableClient, questData.QuestId, _publisher);
            await gameEngine.AddPlayerAsync(questData.PlayerName, questData.CharacterClass);

            return new AcceptedResult();
        }
    }
}
