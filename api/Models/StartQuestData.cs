namespace AblyLabs.ServerlessWebsocketsQuest.Models
{
    public class StartQuestData
    {
        public StartQuestData(string questId, string[] playerIds)
        {
            QuestId = questId;
            PlayerIds = playerIds;
        }

        public string QuestId { get; set; }
        public string[] PlayerIds { get; set; }
    }
}