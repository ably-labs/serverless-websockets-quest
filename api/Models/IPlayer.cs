namespace AblyLabs.ServerlessWebsocketsQuest.Models
{
    public interface IPlayer
    {
        public void SetId(string id);
        public void SetHealth(int health);
        public void ApplyDamage(int damage);
    }
}