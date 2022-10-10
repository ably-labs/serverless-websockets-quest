using System.Collections.Generic;
using FluentAssertions;
using NSubstitute;
using Xunit;
using AblyLabs.ServerlessWebsocketsQuest.Models;

namespace AblyLabs.ServerlessWebsocketsQuest.Test.Models
{

    public class GameStateTests
    {
        public static IEnumerable<object[]> GetPlayers()
        {
            yield return new object[] { new List<string> { CharacterClassDefinitions.Monster.Name, "abc" }, "abc", CharacterClassDefinitions.Monster.Name };
            yield return new object[] { new List<string> { CharacterClassDefinitions.Monster.Name, "abc", "def" }, CharacterClassDefinitions.Monster.Name, "abc" };
            yield return new object[] { new List<string> { CharacterClassDefinitions.Monster.Name, "abc", "def" }, "abc", "def" };
            yield return new object[] { new List<string> { CharacterClassDefinitions.Monster.Name, "abc" }, null, CharacterClassDefinitions.Monster.Name };
        }

        [Theory()]
        [MemberData(nameof(GetPlayers))]
        public void GetNextPlayerName(List<string> playerNames, string? currentPlayerName, string expectedPlayerName)
        {
            var publisher = Substitute.For<Publisher>();
            var gameState = new GameState(publisher) { PlayerNames = playerNames };
            gameState.GetNextPlayerName(currentPlayerName).Should().Be(expectedPlayerName);
        }

        [Theory()]
        [MemberData(nameof(GetPlayers))]
        public void GetRandomPlayerName(List<string> playerNames, string? currentPlayerName, string expectedPlayerName)
        {
            var publisher = Substitute.For<Publisher>();
            var gameState = new GameState(publisher) { PlayerNames = playerNames };
            gameState.GetRandomPlayerName().Should().NotBe(CharacterClassDefinitions.Monster.Name);
        }
    }
}
