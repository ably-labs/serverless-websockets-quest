namespace AblyLabs.ServerlessWebsocketsQuest.Models
{
    public class TurnData
    {
        public TurnData(string questId, Player player)
        {
            QuestId = questId;
            Player = player;
        }

        public string QuestId { get; set; }
        public Player Player { get; set; }
        public bool IsMonster { get; set; }
    }
}