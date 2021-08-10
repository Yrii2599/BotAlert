using BotAlert.Models;
using BotAlert.Service;
using FakeItEasy;
using MongoDB.Driver;
using System.Threading;
using Xunit;

namespace BotAlert.Tests
{
    public class StateProviderTests
    {
        IMongoCollection<ChatState> chatsCollectionMock;
        IMongoDatabase mongoDatabaseMock;
        StateProvider stateDBService;

        public StateProviderTests()
        {
            mongoDatabaseMock = A.Fake<IMongoDatabase>();
            chatsCollectionMock = A.Fake<IMongoCollection<ChatState>>();
            A.CallTo(() => mongoDatabaseMock.GetCollection<ChatState>(A<string>.Ignored, A<MongoCollectionSettings>.Ignored))
                            .Returns(chatsCollectionMock);
            stateDBService = new StateProvider(mongoDatabaseMock);
        }

        [Fact]
        public void CreateChat_ShouldSaveStateToDB()
        {
            var chatState = new ChatState(123);

            stateDBService.CreateChat(chatState);

            A.CallTo(() => chatsCollectionMock.InsertOne(A<ChatState>.Ignored, A<InsertOneOptions>.Ignored, A<CancellationToken>.Ignored))
                            .MustHaveHappenedOnceExactly();
        }
    }
}
