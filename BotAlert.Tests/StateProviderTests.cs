using BotAlert.Models;
using BotAlert.Service;
using FakeItEasy;
using MongoDB.Driver;
using System.Threading;
using BotAlert.Interfaces;
using Xunit;

namespace BotAlert.Tests
{
    public class StateProviderTests
    {
        private readonly IMongoCollection<ChatState> _chatsCollectionMock;
        private readonly IMongoDatabase _mongoDatabaseMock;
        private readonly IEventProvider _eventProviderMock;

        private readonly StateProvider _stateDBService;

        public StateProviderTests()
        {
            _eventProviderMock = A.Fake<IEventProvider>();
            _mongoDatabaseMock = A.Fake<IMongoDatabase>();
            _chatsCollectionMock = A.Fake<IMongoCollection<ChatState>>();

            A.CallTo(() => _mongoDatabaseMock.GetCollection<ChatState>(A<string>.Ignored, A<MongoCollectionSettings>.Ignored))
                            .Returns(_chatsCollectionMock);

            _stateDBService = new StateProvider(_mongoDatabaseMock, _eventProviderMock);
        }

        [Fact]
        public void CreateChat_ShouldSaveStateToDB()
        {
            var chatState = new ChatState(123);

            _stateDBService.CreateChat(chatState);

            A.CallTo(() => _chatsCollectionMock.InsertOne(A<ChatState>.Ignored, A<InsertOneOptions>.Ignored, A<CancellationToken>.Ignored))
                            .MustHaveHappenedOnceExactly();
        }
    }
}
