namespace AblyLabs.ServerlessWebsocketsQuest.Models
{
    public interface IPlayer
    {
        public void SetHealth(int health);
        public void ApplyDamage(int damage);
    }
}