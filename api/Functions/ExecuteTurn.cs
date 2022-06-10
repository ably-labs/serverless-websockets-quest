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
        private IRealtimeClient _realtime;

        public ExecuteTurn(IRealtimeClient realtime)
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
            var questData = await req.Content.ReadAsAsync<QuestData>();
            var channel = _realtime.Channels.Get(questData.QuestId);
            var gameEngine = new GameEngine(durableClient, questData.QuestId, channel);
            await gameEngine.ExecuteTurnAsync(questData.PlayerName);

            return new AcceptedResult();
        }
    }
}
