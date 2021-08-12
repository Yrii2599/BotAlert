using System;
using System.Threading;
using BotAlert.Models;
using BotAlert.Services;
using FakeItEasy;
using MongoDB.Driver;
using Xunit;

namespace BotAlert.Tests
{
    public class EventProviderTests
    {
        private readonly IMongoDatabase _mongoDatabaseMock;
        private readonly IMongoCollection<Event> _eventsCollectionMock;

        private readonly EventProvider _eventProvider;

        public EventProviderTests()
        {
            _mongoDatabaseMock = A.Fake<IMongoDatabase>();
            _eventsCollectionMock = A.Fake<IMongoCollection<Event>>();

            A.CallTo(() => _mongoDatabaseMock.GetCollection<Event>(A<string>.Ignored, A<MongoCollectionSettings>.Ignored))
                            .Returns(_eventsCollectionMock);

            _eventProvider = new EventProvider(_mongoDatabaseMock);
        }


        [Fact]
        public void CreateEvent_WorksCorrectly()
        {
            var eventObj = new Event(123, "Title");

            _eventProvider.CreateEvent(eventObj);

            A.CallTo(() => _eventsCollectionMock.InsertOne(A<Event>.Ignored, A<InsertOneOptions>.Ignored, A<CancellationToken>.Ignored))
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
        public void UpdateDraftEventByChatId_WorksCorrectly()
        {
            var eventObj = new Event(123, "Title");
            _eventsCollectionMock.InsertOne(eventObj);

            _eventProvider.UpdateDraftEventByChatId(eventObj.ChatId, "Date", DateTime.Now);

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
            _eventsCollectionMock.InsertOne(eventObj);

            _eventProvider.DeleteEvent(eventObj.Id);

            A.CallTo(() => _eventsCollectionMock.DeleteOne(A<FilterDefinition<Event>>.Ignored, A<CancellationToken>.Ignored))
                            .MustHaveHappenedOnceExactly();
        }

        //[Fact]
        //public void GetEventById_WorksCorrectly()
        //{
        //    var eventObj = new Event(123, "Title");
        //    _eventsCollectionMock.InsertOne(eventObj);

        //    _eventProvider.GetEventById(eventObj.Id);

        //    A.CallTo(() => _eventsCollectionMock.Find(A<FilterDefinition<Event>>.Ignored, A<FindOptions>.Ignored))
        //                    .Returns(eventObj);
        //}

        //[Fact]
        //public void GetEventByTitle_WorksCorrectly()
        //{
        //    var eventObj = new Event(123, "Title");
        //    _eventsCollectionMock.InsertOne(eventObj);

        //    _eventProvider.GetEventByTitle(eventObj.Title);

        //    A.CallTo(() => _eventsCollectionMock.Find(A<FilterDefinition<Event>>.Ignored, A<FindOptions>.Ignored))
        //                    .Returns(eventObj);
        //}

        //[Fact]
        //public void GetDraftEventByChatId_WorksCorrectly()
        //{
        //    var eventObj = new Event(123, "Title");
        //    _eventsCollectionMock.InsertOne(eventObj);

        //    _eventProvider.GetDraftEventByChatId(eventObj.ChatId);

        //    A.CallTo(() => _eventsCollectionMock.Find(A<FilterDefinition<Event>>.Ignored, A<FindOptions>.Ignored))
        //                    .Returns(eventObj);
        //}

        //[Fact]
        //public void GetAllEvents_WorksCorrectly()
        //{

        //}


        //[Fact]
        //public void GetAllEventsInDateRange_WorksCorrectly()
        //{

        //}
    }
}
