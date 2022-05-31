using System.Collections.Generic;
using FluentAssertions;
using Xunit;
using AblyLabs.ServerlessWebsocketsQuest.Models;

namespace AblyLabs.ServerlessWebsocketsQuest.Test.Models
{

    public class GameStateTests
    {
        public static IEnumerable<object[]> GetPlayers()
        {
            yield return new object[] { new List<string> { "monster", "abc" }, "abc", "monster" };
            yield return new object[] { new List<string> { "monster", "abc", "def" }, "monster", "abc" };
            yield return new object[] { new List<string> { "monster", "abc", "def" }, "abc", "def" };
            yield return new object[] { new List<string> { "monster", "abc" }, null, "monster" };
        }

        [Theory()]
        [MemberData(nameof(GetPlayers))]
        public void GetNextPlayerId(List<string> playerIds, string? currentPlayerId, string expectedPlayerId)
        {
            var gameState = new GameState() { PlayerIds = playerIds };
            gameState.GetNextPlayerId(currentPlayerId).Should().Be(expectedPlayerId);
        }

        [Theory()]
        [MemberData(nameof(GetPlayers))]
        public void GetRandomPlayerId(List<string> playerIds, string? currentPlayerId, string expectedPlayerId)
        {
            var gameState = new GameState() { PlayerIds = playerIds };
            gameState.GetRandomPlayerId().Should().NotBe("monster");
        }
    }
}