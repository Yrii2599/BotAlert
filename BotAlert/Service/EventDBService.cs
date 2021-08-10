using System;
using System.Collections.Generic;
using System.Linq;
using BotAlert.Models;
using BotAlert.Settings;
using Microsoft.AspNetCore.Mvc.Razor;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BotAlert.Services
{
    public class EventDBService
    {
        private IMongoCollection<Event> eventsCollection;

        public static DBSettings Settings;

        private readonly FilterDefinitionBuilder<Event> filterBuilder = Builders<Event>.Filter;

        public EventDBService()
        {
            var database = new MongoClient(Settings.ConnectionString).GetDatabase(Settings.DatabaseName);
            eventsCollection = database.GetCollection<Event>("events");
        }

        public void CreateEvent(Event eventObj)
        {
            try
            {
                eventsCollection.InsertOne(eventObj);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public List<Event> GetAllEvents()
        {
            return eventsCollection.Find(new BsonDocument()).ToList();
        }

        public List<Event> GetAllEventsInDateRange(DateTime rangeStart, DateTime rangeEnd)
        {
            var filter1 = filterBuilder.Gte(x => x.Date, rangeStart);
            var filter2 = filterBuilder.Lte(x => x.Date, rangeEnd);
            var finalFilter = filterBuilder.Or(new List<FilterDefinition<Event>> { filter1, filter2 });
            return eventsCollection.Find(finalFilter).ToList();
        }

        public Event GetEventById(Guid id)
        {
            var filter = filterBuilder.Eq(x => x.Id, id);
            return eventsCollection.Find(filter).SingleOrDefault();
        }

        public Event GetEventByTitle(string title)
        {
            var filter = filterBuilder.Eq(x => x.Title, title);
            return eventsCollection.Find(filter).SingleOrDefault();
        }

        public void UpdateEvent(Event eventObj)
        {
            var filter = filterBuilder.Eq(x => x.Id, eventObj.Id);
            eventsCollection.ReplaceOne(filter, eventObj);
        }

        public void DeleteEvent(Guid id)
        {
            var filter = filterBuilder.Eq(x => x.Id, id);
            eventsCollection.DeleteOne(filter);
        }

        public Event GetDraftEventByChatId(long chatId)
        {
            var filter1 = filterBuilder.Eq(x => x.ChatId, chatId);
            var filter2 = filterBuilder.Eq(x => x.Status, EventStatus.InProgress);
            var finalFilter = filterBuilder.And(new List<FilterDefinition<Event>> { filter1, filter2 }); 

            return eventsCollection.Find(finalFilter).SingleOrDefault();
        }
    }
}
