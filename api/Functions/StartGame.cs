using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using IO.Ably;
using System.Net.Http;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

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
