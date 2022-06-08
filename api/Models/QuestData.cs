namespace AblyLabs.ServerlessWebsocketsQuest.Models
{
    public class QuestData
    {
        public QuestData(string questId, string playerId, string characterClass)
        {
            QuestId = questId;
            PlayerId = playerId;
            CharacterClass = characterClass;
        }

        public string QuestId { get; set; }
        public string PlayerId { get; set; }
        public string CharacterClass { get; set; }
    }
}