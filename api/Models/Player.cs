using System;

namespace AblyLabs.ServerlessWebsocketsQuest.Models
{
    public class Player
    {
        public Player(string id, int health)
        {
            Id = id;
            Health = health;
        }

        public string Id { get; set; }

        public int Health { get; set; }
    }
}