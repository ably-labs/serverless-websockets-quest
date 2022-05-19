using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using AblyLabs.ServerlessWebsocketsQuest.Models;

namespace AblyLabs.ServerlessWebsocketsQuest
{
    public class JoinQuest
    {
        /// The JoinQuest function is called when a player joins a quest created by the host.
        /// The Player Id & Health will be stored in a Durable Entity.
        [FunctionName(nameof(JoinQuest))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequestMessage req,
            [DurableClient] IDurableClient durableClient,
            ILogger log)
        {
            var questData = await req.Content.ReadAsAsync<QuestData>();

            var gameStateEntityId = new EntityId(nameof(GameState), questData.QuestId);
            await durableClient.SignalEntityAsync<IGameState>(gameStateEntityId, proxy => proxy.AddPlayerId(questData.PlayerId));

            var playerEntityId = new EntityId(nameof(Player), Player.GetEntityId(questData.QuestId, questData.PlayerId));
            await durableClient.SignalEntityAsync<IPlayer>(playerEntityId, proxy => proxy.SetHealth(50));

            return new AcceptedResult();
        }
    }
}
