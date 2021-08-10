using System.Threading;
using BotAlert.Models;
using BotAlert.Service;
using BotAlert.Interfaces;
using BotAlert.States;
using MongoDB.Driver;
using FakeItEasy;
using Xunit;

namespace BotAlert.Tests
{
    public class StateProviderTests
    {
        private readonly IMongoCollection<ChatState> _chatsCollectionMock;
        private readonly IStateFactory _stateFactoryMock;
        private readonly IMongoDatabase _mongoDatabaseMock;
        private readonly IEventProvider _eventProviderMock;

        private readonly StateProvider _stateDBService;

        public StateProviderTests()
        {
            _stateFactoryMock = A.Fake<IStateFactory>();
            _eventProviderMock = A.Fake<IEventProvider>();
            _mongoDatabaseMock = A.Fake<IMongoDatabase>();
            _chatsCollectionMock = A.Fake<IMongoCollection<ChatState>>();

            A.CallTo(() => _mongoDatabaseMock.GetCollection<ChatState>(A<string>.Ignored, A<MongoCollectionSettings>.Ignored))
                            .Returns(_chatsCollectionMock);

            _stateDBService = new StateProvider(_stateFactoryMock, _mongoDatabaseMock, _eventProviderMock);
        }

        [Fact]
        public void CreateOrUpdateChat_Create()
        {
            var chatState = new ChatState(123);

            _stateDBService.CreateOrUpdateChat(chatState);

            A.CallTo(() => _chatsCollectionMock.InsertOne(A<ChatState>.Ignored, A<InsertOneOptions>.Ignored, A<CancellationToken>.Ignored))
                            .MustHaveHappenedOnceExactly();
        }

        //[Fact]
        //public void CreateOrUpdateChat_Update()
        //{
        //    var chatState = new ChatState(123);

        //    _stateDBService.CreateOrUpdateChat(chatState);

        //    A.CallTo(() => _chatsCollectionMock.ReplaceOne(A<ChatState>.Ignored, A<InsertOneOptions>.Ignored, A<CancellationToken>.Ignored))
        //        .MustHaveHappenedOnceExactly();
        //}

        //[Fact]
        //public void GetOrCreateChatContext_Get()
        //{
        //    var chatState = new ChatState(123);

        //    _stateDBService.CreateOrUpdateChat(chatState);

        //    A.CallTo(() => _chatsCollectionMock.InsertOne(A<ChatState>.Ignored, A<InsertOneOptions>.Ignored, A<CancellationToken>.Ignored))
        //        .MustHaveHappenedOnceExactly();
        //}

        [Fact]
        public void GetOrCreateChatContext_Create()
        {
            var chatState = new ChatState(123);

          var actual= _stateDBService.GetOrCreateChatContext(chatState.ChatId).State;
          var expected = new MainState();

            A.CallTo(() => _chatsCollectionMock.InsertOne(A<ChatState>.Ignored, A<InsertOneOptions>.Ignored, A<CancellationToken>.Ignored))
                .MustHaveHappenedOnceExactly();
            Assert.IsType(expected.GetType(), actual);
        }
    }
}
