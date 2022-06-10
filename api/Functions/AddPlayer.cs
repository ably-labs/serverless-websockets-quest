using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using AblyLabs.ServerlessWebsocketsQuest.Models;
using IO.Ably;

namespace AblyLabs.ServerlessWebsocketsQuest
{
    public class AddPlayer
    {
        private IRealtimeClient _realtime;

        public AddPlayer(IRealtimeClient realtime)
        {
            _realtime = realtime;
        }
        
        /// The AddPlayer function is called when a player joins a quest created by the host.
        /// The Player Id & Health will be stored in a Durable Entity.
        [FunctionName(nameof(AddPlayer))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequestMessage req,
            [DurableClient] IDurableClient durableClient,
            ILogger log)
        {
            var questData = await req.Content.ReadAsAsync<QuestData>();
            var channel = _realtime.Channels.Get(questData.QuestId);
            var gameEngine = new GameEngine(durableClient, questData.QuestId, channel);
            try
            {
                await gameEngine.AddPlayerAsync(questData.PlayerName, questData.CharacterClass);
                return new AcceptedResult();
            }
            catch (System.Exception ex)
            {
                return new ObjectResult(ex) { StatusCode = 500 };
            }
        }
    }
}
