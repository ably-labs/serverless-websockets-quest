namespace AblyLabs.ServerlessWebsocketsQuest.Models
{
    public interface IGameState
    {
        public void SetHost(string host);
        public void AddPlayerId(string playerId);
    }
}