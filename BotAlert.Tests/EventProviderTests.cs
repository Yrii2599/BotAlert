using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BotAlert.Interfaces;
using BotAlert.Models;
using BotAlert.Services;
using FakeItEasy;
using MongoDB.Bson;
using MongoDB.Driver;
using Xunit;

namespace BotAlert.Tests
{
    public class EventProviderTests
    {
        private readonly IMongoDatabase _mongoDatabaseMock;
        private readonly IMongoCollection<Event> _eventsCollection;

        private readonly EventProvider _eventProvider;

        public EventProviderTests()
        {
            _mongoDatabaseMock = A.Fake<IMongoDatabase>();
            _eventsCollection = A.Fake<IMongoCollection<Event>>();

            A.CallTo(() => _mongoDatabaseMock.GetCollection<Event>(A<string>.Ignored, A<MongoCollectionSettings>.Ignored))
                            .Returns(_eventsCollection);

            _eventProvider = new EventProvider(_mongoDatabaseMock);
        }


        /*[Fact]
        public void CreateEvent_WorksCorrectly()
        {
            var eventObj = new Event(123, "Title");

            _eventProvider.CreateEvent(eventObj);

            A.CallTo(() => _eventsCollection.Find(A<FilterDefinition<Event>>.Ignored, A<FindOptions>.Ignored)).Returns();

            A.CallTo(() => _mongoDatabaseMock.GetCollection<Event>("events").InsertOne(A<Event>.Ignored,))
                            .MustHaveHappenedOnceExactly();
        }*/
    }
}
