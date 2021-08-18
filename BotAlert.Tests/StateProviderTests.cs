using System.Threading;
using BotAlert.Models;
using BotAlert.Services;
using MongoDB.Driver;
using FakeItEasy;
using Xunit;

namespace BotAlert.Tests
{
    public class StateProviderTests
    {
        private readonly IMongoCollection<ChatState> _chatsCollectionMock;
        private readonly IMongoDatabase _mongoDatabaseMock;

        private readonly StateProvider _stateDBService;

        public StateProviderTests()
        {
            _mongoDatabaseMock = A.Fake<IMongoDatabase>();
            _chatsCollectionMock = A.Fake<IMongoCollection<ChatState>>();

            A.CallTo(() => _mongoDatabaseMock.GetCollection<ChatState>(A<string>.Ignored, A<MongoCollectionSettings>.Ignored))
                            .Returns(_chatsCollectionMock);

            _stateDBService = new StateProvider(_mongoDatabaseMock);
        }

        [Fact]
        public void GetChatState_ShouldSaveStateToTheCollection()
        {
            long chatId = 1234;

            _stateDBService.GetChatState(chatId);

            A.CallTo(() => _chatsCollectionMock.InsertOne(A<ChatState>.That.Matches(x=>x.ChatId == chatId), A<InsertOneOptions>.Ignored, A<CancellationToken>.Ignored))
                            .MustHaveHappenedOnceExactly();
        }

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
