using System;
using System.Linq;
using System.Collections.Generic;
using BotAlert.Models;
using BotAlert.Helpers;
using BotAlert.Settings;
using BotAlert.Interfaces;
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

        public List<Event> GetUserEventsOnPage(long chatId)
        {
            var page = _stateProvider.GetChatState(chatId).NotificationsPage;

            return GetAllUserEvents(chatId).Skip(page * TelegramSettings.EventsPerPage)
                                           .Take(TelegramSettings.EventsPerPage)
                                           .ToList();
        }

        public List<Event> GetAllNotificationsToBeSentNow()
        {
            return _eventsCollection.Find(x => x.WarnDate == DateTime.UtcNow
                                        .AddHours(_stateProvider.GetChatState(x.ChatId).TimeOffSet)
                                        .TrimSecondsAndMilliseconds())
                                    .ToList();
        }

        public bool UserEventsPreviousPageExists(long chatId)
        {
            return _stateProvider.GetChatState(chatId).NotificationsPage != 0;
        }

        public bool UserEventsNextPageExists(long chatId)
        {
            var totalUserPages = Math.Ceiling((double)GetAllUserEvents(chatId).Count / TelegramSettings.EventsPerPage);
            var userPage = _stateProvider.GetChatState(chatId).NotificationsPage;

            // Тк. в БД подсчет начинается с 0
            return userPage < totalUserPages - 1;
        }

        public Event GetEventById(Guid id)
        {
            return _eventsCollection.Find(GetIdFilter(id)).SingleOrDefault();
        }

        public void UpdateEvent(Event eventObj)
        {
            var update = Builders<Event>.Update.Set(x => x.Status, eventObj.Status)
                                               .Set(x => x.Title, eventObj.Title)
                                               .Set(x => x.Date, eventObj.Date)
                                               .Set(x => x.WarnDate, eventObj.WarnDate)
                                               .Set(x => x.Description, eventObj.Description);

            _eventsCollection.UpdateOne(GetIdFilter(eventObj.Id), update);
        }

        public void DeleteEvent(Guid id)
        {
            _eventsCollection.DeleteOne(GetIdFilter(id));
        }

        public Event GetDraftEventByChatId(long chatId)
        {
            return _eventsCollection.Find(GetDraftEventFilter(chatId)).SingleOrDefault();
        }

        private List<Event> GetAllUserEvents(long chatId)
        {
            var events = _eventsCollection
                .Find(x => x.ChatId == chatId)
                .ToList()
                .OrderBy(x => x.Date)
                .ToList();

            return events;
        }

        private FilterDefinition<Event> GetIdFilter(Guid id)
        {
            return _filterBuilder.Eq(x => x.Id, id);
        }

        private FilterDefinition<Event> GetDraftEventFilter(long chatId)
        {
            var chatIdFilter = _filterBuilder.Eq(x => x.ChatId, chatId);
            var draftEventFilter = _filterBuilder.Eq(x => x.Status, EventStatus.InProgress);

            return _filterBuilder.And(new List<FilterDefinition<Event>> { chatIdFilter, draftEventFilter });
        }
    }
}
