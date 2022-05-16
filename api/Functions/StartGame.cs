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
    public class StartGame
    {
        private AblyRealtime _realtime;

        public StartGame(AblyRealtime realtime)
        {
            _realtime = realtime;
        }

        [FunctionName(nameof(StartGame))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequestMessage req,
            [DurableClient] IDurableClient durableClient,
            ILogger log)
        {
            var newGame = await req.Content.ReadAsAsync<NewGame>();


            var responseJson = new { title = "This is a title from the api call!" };
            return new OkObjectResult(responseJson);
        }
    }
}
