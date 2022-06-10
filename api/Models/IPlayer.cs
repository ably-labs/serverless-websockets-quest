namespace AblyLabs.ServerlessWebsocketsQuest.Models
{
    public interface IPlayer
    {
        public void SetCharacterClass(string className);
        public void SetHealth(int health);
        public void ApplyDamage(int damage);
    }
}