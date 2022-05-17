using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.Net.Http;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using IO.Ably;
using AblyLabs.ServerlessWebsocketsQuest.Models;

namespace AblyLabs.ServerlessWebsocketsQuest
{
    public class CreateQuest
    {
        private AblyRealtime _realtime;

        public CreateQuest(AblyRealtime realtime)
        {
            _realtime = realtime;
        }

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
            var channel = _realtime.Channels.Get(newQuestData.QuestId);

            return new OkResult();
        }
    }
}
