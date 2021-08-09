﻿using System;
using System.Collections.Generic;
using System.Linq;
using BotAlert.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BotAlert.Services
{
    public class EventDBService : Controller
    {
        private static IMongoCollection<Event> eventsCollection;

        private readonly FilterDefinitionBuilder<Event> filterBuilder = Builders<Event>.Filter;

        static EventDBService()
        {
            var database = new MongoClient(Settings.Settings.DBConnectionString).GetDatabase(Settings.Settings.DatabaseName);
            eventsCollection = database.GetCollection<Event>(Settings.Settings.CollectionName);
        }

        public static void CreateEvent(Event eventObj)
        {
            eventObj.Status = EventStatus.Created;
            Console.WriteLine("Pre");
            eventsCollection.InsertOne(eventObj);
            Console.WriteLine("Post");
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
    }
}
