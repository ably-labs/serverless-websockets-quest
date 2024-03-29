using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Xunit;
using AblyLabs.ServerlessWebsocketsQuest.Models;

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
            var publisher = Substitute.For<Publisher>();
            var player = new Player(publisher) { Health = health };
            await player.ApplyDamage(damage);
            player.Health.Should().Be(result);
        }
    }
}
