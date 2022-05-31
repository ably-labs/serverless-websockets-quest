using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using AutoFixture;
using IO.Ably.Realtime;
using NSubstitute;
using Xunit;
using AblyLabs.ServerlessWebsocketsQuest.Models;

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
            var realtimeChannel = Substitute.For<IRealtimeChannel>();
            string questId = _fixture.Create<string>();
            var gameEngine = new GameEngine(durableClient, questId, realtimeChannel);
            string hostId = _fixture.Create<string>();
            int monsterHealth = _fixture.Create<int>();
            
            // Act
            await gameEngine.CreateQuestAsync(hostId, monsterHealth);

            // Assert
            await durableClient.Received(2).SignalEntityAsync<IGameState>(
                Arg.Any<EntityId>(), 
                Arg.Any<Action<IGameState>>());
        }

        [Fact]
        public async Task CreateQuestShouldUpdateMonsterEntity()
        {
            // Arrange
            var durableClient = Substitute.For<IDurableClient>();
            var realtimeChannel = Substitute.For<IRealtimeChannel>();
            string questId = _fixture.Create<string>();
            var gameEngine = new GameEngine(durableClient, questId, realtimeChannel);
            string hostId = _fixture.Create<string>();
            int monsterHealth = _fixture.Create<int>();
            
            // Act
            await gameEngine.CreateQuestAsync(hostId, monsterHealth);

            // Assert
            await durableClient.Received(1).SignalEntityAsync<IMonster>(
                Arg.Any<EntityId>(), 
                Arg.Any<Action<IMonster>>());
        }

        [Fact]
        public async Task JoinQuestShouldUpdateGameStateEntity()
        {
            // Arrange
            var durableClient = Substitute.For<IDurableClient>();
            var realtimeChannel = Substitute.For<IRealtimeChannel>();
            string questId = _fixture.Create<string>();
            var gameEngine = new GameEngine(durableClient, questId, realtimeChannel);
            string playerId = _fixture.Create<string>();
            int playerHealth = _fixture.Create<int>();
            
            // Act
            await gameEngine.JoinQuestAsync(playerId, playerHealth);

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
            var realtimeChannel = Substitute.For<IRealtimeChannel>();
            string questId = _fixture.Create<string>();
            var gameEngine = new GameEngine(durableClient, questId, realtimeChannel);
            string playerId = _fixture.Create<string>();
            int playerHealth = _fixture.Create<int>();
            
            // Act
            await gameEngine.JoinQuestAsync(playerId, playerHealth);

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
            var playerIds = _fixture.CreateMany<string>(3).ToList();
            playerIds = playerIds.Prepend(Monster.ID).ToList();
            var gameState = _fixture.Build<GameState>()
                .With(g => g.PlayerIds, playerIds)
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
            var realtimeChannel = Substitute.For<IRealtimeChannel>();
            string questId = _fixture.Create<string>();
            var gameEngine = new GameEngine(durableClient, questId, realtimeChannel);
            
            // Act
            await gameEngine.ExecuteTurnAsync(Monster.ID);

            // Assert
            await durableClient.Received(1).SignalEntityAsync<IPlayer>(
                Arg.Any<EntityId>(), 
                Arg.Any<Action<IPlayer>>());

            await realtimeChannel.Received(1).PublishAsync(
                "update-player", 
                Arg.Any<object>());
            
            await realtimeChannel.Received(1).PublishAsync(
                "check-player-turn", 
                Arg.Any<object>());
        }

    }
}