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
    public class StartQuest
    {
        private AblyRealtime _realtime;

        public StartQuest(AblyRealtime realtime)
        {
            _realtime = realtime;
        }

        /// The StartQuest function is called when all players have joined the quest and the host starts the quest.
        /// The monster will attack first.
        [FunctionName(nameof(StartQuest))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequestMessage req,
            [DurableClient] IDurableClient durableClient,
            ILogger log)
        {
            var questData = await req.Content.ReadAsAsync<QuestData>();

            var gameStateEntityId = new EntityId(nameof(GameState), questData.QuestId);
            var gameState = await durableClient.ReadEntityStateAsync<GameState>(gameStateEntityId);

            var monsterEntityId = new EntityId(nameof(Monster), Monster.GetEntityId(questData.QuestId));
            await durableClient.SignalEntityAsync<IPlayer>(monsterEntityId, proxy => proxy.SetHealth(100));
            var monster = await durableClient.ReadEntityStateAsync<Monster>(monsterEntityId);

            if (gameState.EntityExists && monster.EntityExists)
            {
                var channel = _realtime.Channels.Get(questData.QuestId);
                var gameEngine = new GameEngine(durableClient, channel, questData.QuestId);
                await gameEngine.AttackByMonster(gameState);

                return new AcceptedResult();
            }
            else
            {
                return new BadRequestObjectResult("No game data was found. Please start a new quest.");
            }
        }
    }
}
