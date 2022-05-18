using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using AblyLabs.ServerlessWebsocketsQuest.Models;

namespace AblyLabs.ServerlessWebsocketsQuest
{
    public class CreateQuest
    {
        /// The CreateQuest function is called by the host (the firs player).
        /// The function creates the initial GameState and stores this as a Durable Entity.
        [FunctionName(nameof(CreateQuest))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequestMessage req,
            [DurableClient] IDurableClient durableClient,
            ILogger log)
        {
            var newQuestData = await req.Content.ReadAsAsync<NewQuestData>();
            var entityId = new EntityId(nameof(GameState), newQuestData.QuestId);
            const int monsterHealth = 100;
            await durableClient.SignalEntityAsync<IGameState>(entityId, proxy => proxy.SetMonsterHealth(monsterHealth));
            await durableClient.SignalEntityAsync<IGameState>(entityId, proxy => proxy.SetPlayers(new[] {newQuestData.PlayerId}));

            return new OkObjectResult(newQuestData.QuestId);
        }
    }
}
