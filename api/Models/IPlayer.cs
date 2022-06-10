using System.Threading.Tasks;

namespace AblyLabs.ServerlessWebsocketsQuest.Models
{
    public interface IPlayer
    {
        public Task InitPlayer(object[] playerFields);
        public Task ApplyDamage(int damage);
    }
}