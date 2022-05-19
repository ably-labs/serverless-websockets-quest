using System.Collections.Generic;

namespace AblyLabs.ServerlessWebsocketsQuest.Models
{
    public class StartQuestData
    {
        public StartQuestData(string questId, List<Player> players)
        {
            QuestId = questId;
            Players = players;
        }

        public string QuestId { get; set; }
        public List<Player> Players { get; set; }
    }
}