using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AblyLabs.ServerlessWebsocketsQuest
{
    public static class GetTitle
    {
        [FunctionName("GetTitle")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "getTitleFromApi")] HttpRequest req,
            ILogger log)
        {

            var responseJson = new { title = "This is a title from the api call!" };
            return new OkObjectResult(responseJson);
        }
    }
}
