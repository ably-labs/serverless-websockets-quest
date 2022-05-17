using Xunit;
using AblyLabs.ServerlessWebsocketsQuest.Models;
using FluentAssertions;

namespace AblyLabs.ServerlessWebsocketsQuest.Models
{

    public class GameStateTests
    {
        [Theory()]
        [InlineData(100, 0, 100)]
        [InlineData(100, 90, 10)]
        [InlineData(100, 100, 0)]
        [InlineData(100, 110, 0)]
        public void ApplyDamageToMonster(int health, int damage, int result)
        {
            var gameState = new GameState() { MonsterHealth = health };
            gameState.ApplyDamageToMonster(damage);
            gameState.MonsterHealth.Should().Be(result);
        }

        [Theory()]
        [InlineData(100, 90, false)]
        [InlineData(100, 100, true)]
        [InlineData(100, 110, true)]
        public void IsGameOver(int health, int damage, bool isGameOver)
        {
            var gameState = new GameState() { MonsterHealth = health };
            gameState.ApplyDamageToMonster(damage);
            gameState.IsGameOver.Should().Be(isGameOver);
        }

        [Theory()]
        [InlineData(new string[] {"abc"}, "abc", "abc")]
        [InlineData(new string[] {"abc", "def"}, "def", "abc")]
        [InlineData(new string[] {"abc", "def", "ghi"}, "def", "ghi")]
        [InlineData(new string[] {"abc", "def", "ghi"}, "abc", "def")]
        public void GetNextPlayer(string[] playerIds, string currentPlayerId, string expectedPlayerId)
        {
            var gameState = new GameState() { Players = playerIds };
            gameState.GetNextPlayer(currentPlayerId).Should().Be(expectedPlayerId);
        }

        [Theory()]
        [InlineData(new string[] {"abc"}, "abc", "monster")]
        [InlineData(new string[] {"abc", "def"}, "def", "monster")]
        [InlineData(new string[] {"abc", "def", "ghi"}, "def", "ghi")]
        [InlineData(new string[] {"abc", "def", "ghi"}, "abc", "def")]
        public void GetNextTurn(string[] playerIds, string currentPlayerId, string expectedTurn)
        {
            var gameState = new GameState() { Players = playerIds };
            gameState.GetNextTurn(currentPlayerId).Should().Be(expectedTurn);
        }
    }
}