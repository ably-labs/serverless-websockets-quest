namespace AblyLabs.ServerlessWebsocketsQuest.Models
{
    public interface IGameState
    {
        public void SetPhase(string phase);
        public void AddPlayerId(string playerId);
    }
}