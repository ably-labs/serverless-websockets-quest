using Xunit;
using AblyLabs.ServerlessWebsocketsQuest.Models;
using FluentAssertions;
using System.Collections.Generic;

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

        public static IEnumerable<object[]> GetPlayers()
        {
            var abc = new Player("abc", 50);
            var def = new Player("def", 50);
            var ghi = new Player("ghi", 50);
            yield return new object[] { new List<Player> { abc }, "abc", "abc" };
            yield return new object[] { new List<Player> { abc, def }, "def", "abc" };
            yield return new object[] { new List<Player> { abc, def, ghi }, "def", "ghi" };
            yield return new object[] { new List<Player> { abc, def, ghi }, "abc", "def" };
            yield return new object[] { new List<Player> { abc, def }, null, "abc" };
        }

        [Theory()]
        [MemberData(nameof(GetPlayers))]
        public void GetNextPlayer(List<Player> players, string? currentPlayerId, string expectedPlayerId)
        {
            var gameState = new GameState() { Players = players };
            gameState.GetNextPlayer(currentPlayerId).Id.Should().Be(expectedPlayerId);
        }

        // [Theory()]
        // [InlineData(new string[] {"abc"}, "abc", true)]
        // [InlineData(new string[] {"abc", "def"}, "def", true)]
        // [InlineData(new string[] {"abc", "def", "ghi"}, "def", false)]
        // [InlineData(new string[] {"abc", "def", "ghi"}, "abc", false)]
        // public void IsMonsterTurn(string[] playerIds, string currentPlayerId, bool isMonsterTurn)
        // {
        //     var gameState = new GameState() { Players = playerIds };
        //     gameState.IsMonsterTurn(currentPlayerId).Should().Be(isMonsterTurn);
        // }

        [Theory()]
        [InlineData(100, 10, 20)]
        [InlineData(20, 2, 4)]
        public void GetMonsterAttackDamage(int monsterHealth, int min, int max)
        {
            var gameState = new GameState() { MonsterHealth = monsterHealth };
            gameState.GetMonsterAttackDamage().Should().BeInRange(min, max);
        }

        [Theory()]
        [InlineData(100, 5, 10)]
        [InlineData(20, 1, 2)]
        public void GetPlayerAttackDamage(int monsterHealth, int min, int max)
        {
            var gameState = new GameState() { MonsterHealth = monsterHealth };
            gameState.GetPlayerAttackDamage().Should().BeInRange(min, max);
        }
    }
}