namespace AblyLabs.ServerlessWebsocketsQuest.Models
{
    public class StartQuestData
    {
        public StartQuestData(string questId, string playerId)
        {
            QuestId = questId;
            PlayerId = playerId;
        }

        public string QuestId { get; set; }
        public string PlayerId { get; set; }
    }
}