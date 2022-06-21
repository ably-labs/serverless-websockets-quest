namespace AblyLabs.ServerlessWebsocketsQuest.Models
{
    public class QuestData
    {
        public QuestData(string questId, string playerName, string characterClass)
        {
            QuestId = questId;
            PlayerName = playerName;
            CharacterClass = characterClass;
        }

        public string QuestId { get; set; }
        public string PlayerName { get; set; }
        public string CharacterClass { get; set; }
    }
}
