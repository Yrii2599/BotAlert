using System;
using System.Threading;
using BotAlert.Interfaces;
using BotAlert.Models;
using BotAlert.Services;
using MongoDB.Driver;
using FakeItEasy;
using Xunit;

namespace BotAlert.Tests
{
    public class EventProviderTests
    {
        private readonly IMongoDatabase _mongoDatabaseMock;
        private readonly IStateProvider _stateProviderMock;
        private readonly IMongoCollection<Event> _eventsCollectionMock;

        private readonly EventProvider _eventProvider;

        public EventProviderTests()
        {
            _mongoDatabaseMock = A.Fake<IMongoDatabase>();
            _stateProviderMock = A.Fake<IStateProvider>();
            _eventsCollectionMock = A.Fake<IMongoCollection<Event>>();

            A.CallTo(() => _mongoDatabaseMock.GetCollection<Event>(A<string>.Ignored, A<MongoCollectionSettings>.Ignored))
                            .Returns(_eventsCollectionMock);

            _eventProvider = new EventProvider(_mongoDatabaseMock, _stateProviderMock);
        }

        [Fact]
        public void CreateEvent_WorksCorrectly()
        {
            var eventObj = new Event(123, "Title");

            _eventProvider.CreateEvent(eventObj);

            A.CallTo(() => _eventsCollectionMock.InsertOne(A<Event>.That.Matches(x => x.Id == eventObj.Id), A<InsertOneOptions>.Ignored, A<CancellationToken>.Ignored))
                            .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void UpdateEvent_WorksCorrectly()
        {
            var eventObj = new Event(123, "Title");
            _eventsCollectionMock.InsertOne(eventObj);
            eventObj.Date = DateTime.Now;

            _eventProvider.UpdateEvent(eventObj);

            A.CallTo(() => _eventsCollectionMock.UpdateOne(A<FilterDefinition<Event>>.Ignored,
                             A<UpdateDefinition<Event>>.Ignored,
                             A<UpdateOptions>.Ignored,
                             A<CancellationToken>.Ignored))
                            .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void DeleteEvent_WorksCorrectly()
        {
            var eventObj = new Event(123, "Title");

            _eventProvider.DeleteEvent(eventObj.Id);

            A.CallTo(() => _eventsCollectionMock.DeleteOne(A<FilterDefinition<Event>>.Ignored, A<CancellationToken>.Ignored))
                            .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void UserEventsNextPageExists_CallGetChatState()
        {
            long chatid = 5;

            _eventProvider.UserEventsNextPageExists(chatid);

            A.CallTo(() => _stateProviderMock.GetChatState(chatid));
        }

        [Fact]
        public void UserEventsPreviousPageExists_PageDoesntExist()
        {
            long chatid = 5;

            var actual = _eventProvider.UserEventsPreviousPageExists(chatid);

            Assert.False(actual);
        }
    }
}
