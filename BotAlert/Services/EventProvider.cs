﻿using System;
using System.Collections.Generic;
using BotAlert.Interfaces;
using BotAlert.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BotAlert.Services
{
    public class EventProvider : IEventProvider
    {
        private IMongoCollection<Event> eventsCollection;

        private readonly FilterDefinitionBuilder<Event> filterBuilder = Builders<Event>.Filter;

        public EventProvider(IMongoDatabase database)
        {
            eventsCollection = database.GetCollection<Event>("events");
        }

        public void CreateEvent(Event eventObj)
        {
            eventsCollection.InsertOne(eventObj);
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

        public void UpdateEvent(Event eventObj)
        {
            var filter = filterBuilder.Eq(x => x.Id, eventObj.Id);
            var update = Builders<Event>.Update.Set(x => x.Status, eventObj.Status)
                                               .Set(x => x.Title, eventObj.Title)
                                               .Set(x => x.Date, eventObj.Date)
                                               .Set(x => x.WarnDate, eventObj.WarnDate)
                                               .Set(x => x.Description, eventObj.Description);
            eventsCollection.UpdateOne(filter, update);
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

        public void UpdateDraftEventByChatId<T>(long chatId, string updatingField, T newValue)
        {
            var filter1 = filterBuilder.Eq(x => x.ChatId, chatId);
            var filter2 = filterBuilder.Eq(x => x.Status, EventStatus.InProgress);
            var finalFilter = filterBuilder.And(new List<FilterDefinition<Event>> { filter1, filter2 });

            var update = Builders<Event>.Update.Set(updatingField, newValue);

            eventsCollection.UpdateOne(finalFilter, update);
        }
    }
}
