using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using IO.Ably;
using System.Net.Http;

namespace AblyLabs.ServerlessWebsocketsQuest
{
    public class CreateTokenRequest
    {
        private IRealtimeClient _realtime;

        public CreateTokenRequest(IRealtimeClient realtime)
        {
            _realtime = realtime;
        }

        [FunctionName(nameof(CreateTokenRequest))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "CreateTokenRequest/{clientId?}")] HttpRequestMessage req,
            string? clientId,
            ILogger log)
        {
            var tokenParams = new TokenParams() { ClientId = clientId ?? Guid.NewGuid().ToString() };
            var tokenData = await _realtime.Auth.RequestTokenAsync(tokenParams);

            return new OkObjectResult(tokenData);
        }
    }
}
