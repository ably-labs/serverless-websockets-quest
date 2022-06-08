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
        private IRealtimeClient _realtime;

        public CreateQuest(IRealtimeClient realtime)
        {
            _realtime = realtime;
        }
        
        /// The CreateQuest function is called by the host (the first player).
        /// The function creates the initial GameState and stores this as a Durable Entity.
        [FunctionName(nameof(CreateQuest))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequestMessage req,
            [DurableClient] IDurableClient durableClient,
            ILogger log)
        {
            var questId = await req.Content.ReadAsStringAsync();
            var channel = _realtime.Channels.Get(questId);
            var gameEngine = new GameEngine(durableClient, questId, channel);
            var gamePhase = await gameEngine.CreateQuestAsync();

            return new OkObjectResult(gamePhase);
        }
    }
}
