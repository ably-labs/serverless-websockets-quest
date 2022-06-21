using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using AutoFixture;
using NSubstitute;
using Xunit;
using AblyLabs.ServerlessWebsocketsQuest.Models;
using IO.Ably.Rest;

namespace AblyLabs.ServerlessWebsocketsQuest.Test.Models
{

    public class GameEngineTests
    {
        private readonly IFixture _fixture;

        public GameEngineTests()
        {
            _fixture = new Fixture();
            var customization = new SupportMutableValueTypesCustomization();
            customization.Customize(_fixture);
        }

        [Fact]
        public async Task CreateQuestShouldUpdateGameStateEntity()
        {
            // Arrange
            var durableClient = Substitute.For<IDurableClient>();
            var channel = Substitute.For<IRestChannel>();
            string questId = _fixture.Create<string>();
            var gameEngine = new GameEngine(durableClient, questId, channel);

            // Act
            await gameEngine.CreateQuestAsync();

            // Assert
            await durableClient.Received(2).SignalEntityAsync<IGameState>(
                Arg.Any<EntityId>(),
                Arg.Any<Action<IGameState>>());
        }

        [Fact]
        public async Task JoinQuestShouldUpdateGameStateEntity()
        {
            // Arrange
            var durableClient = Substitute.For<IDurableClient>();
            var channel = Substitute.For<IRestChannel>();
            string questId = _fixture.Create<string>();
            var gameEngine = new GameEngine(durableClient, questId, channel);
            string playerName = _fixture.Create<string>();
            string className = "fighter";

            // Act
            await gameEngine.AddPlayerAsync(playerName, className);

            // Assert
            await durableClient.Received(1).SignalEntityAsync<IGameState>(
                Arg.Any<EntityId>(),
                Arg.Any<Action<IGameState>>());
        }

        [Fact]
        public async Task JoinQuestShouldUpdatePlayerEntity()
        {
            // Arrange
            var durableClient = Substitute.For<IDurableClient>();
            var channel = Substitute.For<IRestChannel>();
            string questId = _fixture.Create<string>();
            var gameEngine = new GameEngine(durableClient, questId, channel);
            string playerName = _fixture.Create<string>();
            string className = "fighter";

            // Act
            await gameEngine.AddPlayerAsync(playerName, className);

            // Assert
            await durableClient.Received(1).SignalEntityAsync<IPlayer>(
                Arg.Any<EntityId>(),
                Arg.Any<Action<IPlayer>>());
        }

        [Fact]
        public async Task ExecuteTurnForMonsterShouldUpdatePlayerEntity()
        {
            // Arrange
            var durableClient = Substitute.For<IDurableClient>();
            var playerNames = _fixture.CreateMany<string>(3).ToList();
            playerNames = playerNames.Prepend(CharacterClassDefinitions.Monster.CharacterClass).ToList();
            var gameState = _fixture.Build<GameState>()
                .With(g => g.PlayerNames, playerNames)
                .Create();
            var entityStateResponseGameState = new EntityStateResponse<GameState>
            {
                EntityExists = true,
                EntityState = gameState
            };
            durableClient.ReadEntityStateAsync<GameState>(Arg.Any<EntityId>()).Returns(Task.FromResult(entityStateResponseGameState));

            var player = _fixture.Create<Player>();
            var entityStateResponsePlayer = new EntityStateResponse<Player>
            {
                EntityExists = true,
                EntityState = player
            };
            durableClient.ReadEntityStateAsync<Player>(Arg.Any<EntityId>()).Returns(Task.FromResult(entityStateResponsePlayer));
            var channel = Substitute.For<IRestChannel>();
            string questId = _fixture.Create<string>();
            var gameEngine = new GameEngine(durableClient, questId, channel);

            // Act
            await gameEngine.ExecuteTurnAsync(CharacterClassDefinitions.Monster.Name);

            // Assert
            await durableClient.Received(1).SignalEntityAsync<IPlayer>(
                Arg.Any<EntityId>(),
                Arg.Any<Action<IPlayer>>());

            await channel.Received(1).PublishAsync(
                "update-player",
                Arg.Any<object>());

            await channel.Received(1).PublishAsync(
                "check-player-turn",
                Arg.Any<object>());
        }

    }
}
