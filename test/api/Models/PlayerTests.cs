using Xunit;
using AblyLabs.ServerlessWebsocketsQuest.Models;
using FluentAssertions;
using NSubstitute;
using IO.Ably;
using System.Threading.Tasks;

namespace AblyLabs.ServerlessWebsocketsQuest.Test.Models
{

    public class PlayerTests
    {
        [Theory()]
        [InlineData(100, 0, 100)]
        [InlineData(100, 90, 10)]
        [InlineData(100, 100, 0)]
        [InlineData(100, 110, 0)]
        public async Task ApplyDamageToPlayer(int health, int damage, int result)
        {
            var realtimeClient = Substitute.For<IRealtimeClient>();
            var player = new Player(realtimeClient) { Health = health };
            await player.ApplyDamage(damage);
            player.Health.Should().Be(result);
        }

        [Theory()]
        [InlineData(100, 90, false)]
        [InlineData(100, 100, true)]
        [InlineData(100, 110, true)]
        public async Task IsGameOver(int health, int damage, bool isGameOver)
        {
            var realtimeClient = Substitute.For<IRealtimeClient>();
            var monster = new Player(realtimeClient) { Health = health };
            await monster.ApplyDamage(damage);
            monster.IsDefeated.Should().Be(isGameOver);
        }
    }
}