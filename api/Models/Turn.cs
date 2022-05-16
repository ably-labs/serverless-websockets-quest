namespace AblyLabs.ServerlessWebsocketsQuest.Models
{
    public class Turn
    {
        public string QuestId { get; set; }
        public string PlayerId { get; set; }
        public bool IsUser { get; set; }
        public int Damage { get; set; }
    }
}