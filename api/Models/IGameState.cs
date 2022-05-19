using System.Collections.Generic;

namespace AblyLabs.ServerlessWebsocketsQuest.Models
{
    public interface IGameState
    {
        public void SetMonsterHealth(int health);
        public void ApplyDamageToMonster(int damage);
        public void SetHost(string host);
        public void SetPlayers(List<Player> players);
    }
}