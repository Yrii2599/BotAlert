using System;
using System.Collections.Generic;
using System.Linq;
using BotAlert.Interfaces;
using BotAlert.Models;
using BotAlert.Settings;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BotAlert.Services
{
    public class EventProvider : IEventProvider
    {
        private readonly IMongoCollection<Event> _eventsCollection;
        private readonly IStateProvider _stateProvider;

        private readonly FilterDefinitionBuilder<Event> _filterBuilder = Builders<Event>.Filter;

        public EventProvider(IMongoDatabase database, IStateProvider stateProvider)
        {
            _eventsCollection = database.GetCollection<Event>("events");
            _stateProvider = stateProvider;
        }

        public void CreateEvent(Event eventObj)
        {
            _eventsCollection.InsertOne(eventObj);
        }

        public List<Event> GetAllUserEvents(long chatId)
        {
            return _eventsCollection.Find<Event>(x => x.ChatId == chatId)
                                    .SortBy(x => x.Date)
                                    .ToList();
        }

        public List<Event> GetUserEventsOnPage(long chatId)
        {
            var page = _stateProvider.GetChatPage(chatId);
            var eventsPerPag = TelegramSettings.EventsPerPage;

            return GetAllUserEvents(chatId).Skip(page * eventsPerPag)
                                           .Take(eventsPerPag)
                                           .ToList();
        }

        public bool UserEventsPreviousPageExists(long chatId) {
            return _stateProvider.GetChatPage(chatId) != 0 ? true : false;
        }

        public bool UserEventsNextPageExists(long chatId)
        {
            var eventsPerPag = TelegramSettings.EventsPerPage;
            var totalUserPages = Math.Ceiling((double)GetAllUserEvents(chatId).Count() / eventsPerPag);
            var userPage = _stateProvider.GetChatPage(chatId);

            // Тк. в БД подсчет начинается с 0
            return userPage < totalUserPages - 1;
        }

        public List<Event> GetAllEventsInDateRange(DateTime rangeStart, DateTime rangeEnd)
        {
            var filter1 = _filterBuilder.Gte(x => x.Date, rangeStart);
            var filter2 = _filterBuilder.Lte(x => x.Date, rangeEnd);
            var finalFilter = _filterBuilder.Or(new List<FilterDefinition<Event>> { filter1, filter2 });

            return _eventsCollection.Find(finalFilter).ToList();
        }

        public Event GetEventById(Guid id)
        {
            var filter = _filterBuilder.Eq(x => x.Id, id);

            return _eventsCollection.Find(filter).SingleOrDefault();
        }

        public void UpdateEvent(Event eventObj)
        {
            var filter = _filterBuilder.Eq(x => x.Id, eventObj.Id);
            var update = Builders<Event>.Update.Set(x => x.Status, eventObj.Status)
                                               .Set(x => x.Title, eventObj.Title)
                                               .Set(x => x.Date, eventObj.Date)
                                               .Set(x => x.WarnDate, eventObj.WarnDate)
                                               .Set(x => x.Description, eventObj.Description);
            _eventsCollection.UpdateOne(filter, update);
        }

        public void DeleteEvent(Guid id)
        {
            var filter = _filterBuilder.Eq(x => x.Id, id);
            _eventsCollection.DeleteOne(filter);
        }

        public Event GetDraftEventByChatId(long chatId)
        {
            var filter1 = _filterBuilder.Eq(x => x.ChatId, chatId);
            var filter2 = _filterBuilder.Eq(x => x.Status, EventStatus.InProgress);
            var finalFilter = _filterBuilder.And(new List<FilterDefinition<Event>> { filter1, filter2 }); 

            return _eventsCollection.Find(finalFilter).SingleOrDefault();
        }

        public void UpdateDraftEventByChatId<T>(long chatId, string updatingField, T newValue)
        {
            var filter1 = _filterBuilder.Eq(x => x.ChatId, chatId);
            var filter2 = _filterBuilder.Eq(x => x.Status, EventStatus.InProgress);
            var finalFilter = _filterBuilder.And(new List<FilterDefinition<Event>> { filter1, filter2 });

            var update = Builders<Event>.Update.Set(updatingField, newValue);

            _eventsCollection.UpdateOne(finalFilter, update);
        }
    }
}
