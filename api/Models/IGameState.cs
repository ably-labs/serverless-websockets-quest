using System.Threading.Tasks;

namespace AblyLabs.ServerlessWebsocketsQuest.Models
{
    public interface IGameState
    {
        public void SetQuestId(string questId);
        public void SetPhase(string phase);
        public Task AddPlayerName(string playerName);
    }
}