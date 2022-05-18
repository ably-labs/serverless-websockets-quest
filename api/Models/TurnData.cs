namespace AblyLabs.ServerlessWebsocketsQuest.Models
{
    public class TurnData
    {
        public TurnData(string questId, string playerId)
        {
            QuestId = questId;
            PlayerId = playerId;
        }

        public string QuestId { get; set; }
        public string PlayerId { get; set; }
        public bool IsMonster { get; set; }
    }
}