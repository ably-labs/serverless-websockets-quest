namespace AblyLabs.ServerlessWebsocketsQuest.Models
{
    public interface IGameState
    {
        public void SetMonsterHealth(int health);
        public void ApplyDamageToMonster(int damage);
        public void SetPlayers(string[] players);
    }
}