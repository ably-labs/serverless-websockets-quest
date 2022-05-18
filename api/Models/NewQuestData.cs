namespace AblyLabs.ServerlessWebsocketsQuest.Models
{
    public class NewQuestData
    {
        public NewQuestData(string questId, string playerId)
        {
            QuestId = questId;
            PlayerId = playerId;
        }

        public string QuestId { get; set; }

        public string PlayerId { get; set; }
    }
}