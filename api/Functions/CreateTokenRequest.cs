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
        private IRestClient _ablyClient;

        public CreateTokenRequest(IRestClient ablyClient)
        {
            _ablyClient = ablyClient;
        }

        [FunctionName(nameof(CreateTokenRequest))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "CreateTokenRequest/{clientId?}")] HttpRequestMessage req,
            string? clientId,
            ILogger log)
        {
            var tokenParams = new TokenParams() { ClientId = clientId ?? Guid.NewGuid().ToString() };
            var tokenData = await _ablyClient.Auth.RequestTokenAsync(tokenParams);

            return new OkObjectResult(tokenData);
        }
    }
}
