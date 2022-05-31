using Xunit;
using AblyLabs.ServerlessWebsocketsQuest.Models;
using FluentAssertions;

namespace AblyLabs.ServerlessWebsocketsQuest.Test.Models
{

    public class PlayerTests
    {
        [Theory()]
        [InlineData(100, 0, 100)]
        [InlineData(100, 90, 10)]
        [InlineData(100, 100, 0)]
        [InlineData(100, 110, 0)]
        public void ApplyDamageToPlayer(int health, int damage, int result)
        {
            var player = new Player() { Health = health };
            player.ApplyDamage(damage);
            player.Health.Should().Be(result);
        }

        [Theory()]
        [InlineData(100, 90, false)]
        [InlineData(100, 100, true)]
        [InlineData(100, 110, true)]
        public void IsGameOver(int health, int damage, bool isGameOver)
        {
            var monster = new Player() { Health = health };
            monster.ApplyDamage(damage);
            monster.IsDefeated.Should().Be(isGameOver);
        }
    }
}