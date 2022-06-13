using System.Collections.Generic;
using FluentAssertions;
using Xunit;
using AblyLabs.ServerlessWebsocketsQuest.Models;
using NSubstitute;
using IO.Ably;

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
        public void GetNextPlayerName(List<string> playerNames, string? currentPlayerName, string expectedPlayerName)
        {
            var realtimeClient = Substitute.For<IRealtimeClient>();
            var gameState = new GameState(realtimeClient) { PlayerNames = playerNames };
            gameState.GetNextPlayerName(currentPlayerName).Should().Be(expectedPlayerName);
        }

        [Theory()]
        [MemberData(nameof(GetPlayers))]
        public void GetRandomPlayerName(List<string> playerNames, string? currentPlayerName, string expectedPlayerName)
        {
            var realtimeClient = Substitute.For<IRealtimeClient>();
            var gameState = new GameState(realtimeClient) { PlayerNames = playerNames };
            gameState.GetRandomPlayerName().Should().NotBe("monster");
        }
    }
}