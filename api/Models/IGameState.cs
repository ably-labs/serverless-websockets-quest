using System.Threading.Tasks;

namespace AblyLabs.ServerlessWebsocketsQuest.Models
{
    public interface IGameState
    {
        public Task InitGameState(string[] gameStateFields);
        public Task UpdatePhase(string phase);
        public Task AddPlayerName(string playerName);
        public Task RemovePlayerName(string playerName);
    }
}
