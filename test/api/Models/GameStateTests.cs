using Xunit;
using AblyLabs.ServerlessWebsocketsQuest.Models;
using FluentAssertions;

namespace AblyLabs.ServerlessWebsocketsQuest.Test.Models
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
        [InlineData(new string[] {"abc", "def"}, null, "abc")]
        public void GetNextPlayer(string[] playerIds, string? currentPlayerId, string expectedPlayerId)
        {
            var gameState = new GameState() { Players = playerIds };
            gameState.GetNextPlayer(currentPlayerId).Should().Be(expectedPlayerId);
        }

        [Theory()]
        [InlineData(new string[] {"abc"}, "abc", true)]
        [InlineData(new string[] {"abc", "def"}, "def", true)]
        [InlineData(new string[] {"abc", "def", "ghi"}, "def", false)]
        [InlineData(new string[] {"abc", "def", "ghi"}, "abc", false)]
        public void IsMonsterTurn(string[] playerIds, string currentPlayerId, bool isMonsterTurn)
        {
            var gameState = new GameState() { Players = playerIds };
            gameState.IsMonsterTurn(currentPlayerId).Should().Be(isMonsterTurn);
        }
    }
}