using System.Threading;
using BotAlert.Models;
using BotAlert.Services;
using BotAlert.Interfaces;
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

        private readonly StateProvider _stateDBService;

        public StateProviderTests()
        {
            _stateFactoryMock = A.Fake<IStateFactory>();
            _mongoDatabaseMock = A.Fake<IMongoDatabase>();
            _chatsCollectionMock = A.Fake<IMongoCollection<ChatState>>();

            A.CallTo(() => _mongoDatabaseMock.GetCollection<ChatState>(A<string>.Ignored, A<MongoCollectionSettings>.Ignored))
                            .Returns(_chatsCollectionMock);

            _stateDBService = new StateProvider(_stateFactoryMock, _mongoDatabaseMock);
        }

        [Fact]
        public void GetChatState_ShouldSaveStateToTheCollection()
        {
            long chatId = 1234;

            _stateDBService.GetChatState(chatId);

            A.CallTo(() => _chatsCollectionMock.InsertOne(A<ChatState>.Ignored, A<InsertOneOptions>.Ignored, A<CancellationToken>.Ignored))
                            .MustHaveHappenedOnceExactly();
        }

        /*[Fact]
        public void GetChatState_ShouldGetStateFromCollection()
        {
            long chatId = 1234;
            var temp = new ChatState(chatId);

            _stateDBService.GetChatState(chatId);

            A.CallTo(() => _chatsCollectionMock.Find(A<FilterDefinition<ChatState>>.Ignored, A<FindOptions>.Ignored));

            A.CallTo(() => _chatsCollectionMock.InsertOne(A<ChatState>.Ignored, A<InsertOneOptions>.Ignored, A<CancellationToken>.Ignored))
                            .MustNotHaveHappened();
        }*/

        [Fact]
        public void SaveChatState_ShouldUpdateState()
        {
            var temp = new ChatState(1234);

            _stateDBService.SaveChatState(temp);

            A.CallTo(() => _chatsCollectionMock.UpdateOne(A<FilterDefinition<ChatState>>.Ignored, A<UpdateDefinition<ChatState>>.Ignored, A<UpdateOptions>.Ignored, A<CancellationToken>.Ignored))
                            .MustHaveHappenedOnceExactly();
        }
    }
}
