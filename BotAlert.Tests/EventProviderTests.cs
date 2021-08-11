using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BotAlert.Interfaces;
using BotAlert.Models;
using FakeItEasy;
using MongoDB.Driver;
using Xunit;

namespace BotAlert.Tests
{
    public class EventProviderTests
    {
        private readonly IMongoDatabase _mongoDatabaseMock;

        private readonly IEventProvider _eventProvider;

        public EventProviderTests(IMongoDatabase mongoDatabaseMock)
        {
            _mongoDatabaseMock = mongoDatabaseMock;
        }


        /*[Fact]
        public void CreateEvent_WorksCorrectly()
        {
            var eventObj = new Event(123);

            _eventProvider.CreateEvent(eventObj);

            A.CallTo(() => _mongoDatabaseMock.GetCollection<Event>("events").InsertOne(A<Event>.Ignored,))
                            .MustHaveHappenedOnceExactly();
        }*/
    }
}
