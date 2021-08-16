using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using BotAlert.Models;
using BotAlert.Settings;
using BotAlert.Interfaces;
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
            eventObj.WarnDate = new DateTime(eventObj.WarnDate.Year,
                                        eventObj.WarnDate.Month,
                                        eventObj.WarnDate.Day,
                                        eventObj.WarnDate.Hour,
                                        eventObj.WarnDate.Minute,
                                        0,
                                        0);

            _eventsCollection.InsertOne(eventObj);
        } 
        
        public List<Event> GetUserEventsOnPage(long chatId)
        {
            var page = _stateProvider.GetChatState(chatId).NotificationsPage;

            return getAllUserEvents(chatId).Skip(page * TelegramSettings.EventsPerPage)
                                           .Take(TelegramSettings.EventsPerPage)
                                           .ToList();
        }

        public List<Event> GetAllNotificationsToBeSentNow()
        {
            var date = DateTime.Now;

            date = new DateTime(date.Year,
                                       date.Month,
                                       date.Day,
                                       date.Hour,
                                       date.Minute,
                                       0,
                                       0);

            return _eventsCollection.Find<Event>(x => x.WarnDate == date).ToList();
        }

        public bool UserEventsPreviousPageExists(long chatId) {
            return _stateProvider.GetChatState(chatId).NotificationsPage != 0 ? true : false;
        }

        public bool UserEventsNextPageExists(long chatId)
        {
            var totalUserPages = Math.Ceiling((double)getAllUserEvents(chatId).Count() / TelegramSettings.EventsPerPage);
            var userPage = _stateProvider.GetChatState(chatId).NotificationsPage;

            // Тк. в БД подсчет начинается с 0
            return userPage < totalUserPages - 1;
        }

        public Event GetEventById(Guid id)
        {
            return _eventsCollection.Find(getIdFilter(id)).SingleOrDefault();
        }

        public void UpdateEvent(Event eventObj)
        {
            var update = Builders<Event>.Update.Set(x => x.Status, eventObj.Status)
                                               .Set(x => x.Title, eventObj.Title)
                                               .Set(x => x.Date, eventObj.Date)
                                               .Set(x => x.WarnDate, eventObj.WarnDate)
                                               .Set(x => x.Description, eventObj.Description);

            _eventsCollection.UpdateOne(getIdFilter(eventObj.Id), update);
        }

        public void DeleteEvent(Guid id)
        {
            _eventsCollection.DeleteOne(getIdFilter(id));
        }

        public Event GetDraftEventByChatId(long chatId)
        {
            return _eventsCollection.Find(getDraftEventFilter(chatId)).SingleOrDefault();
        }

        public void UpdateDraftEventByChatId<T>(long chatId, Expression<Func<Event, T>> updatingField, T newValue)
        {
            var update = Builders<Event>.Update.Set(updatingField, newValue);

            _eventsCollection.UpdateOne(getDraftEventFilter(chatId), update);
        }
        
        private List<Event> getAllUserEvents(long chatId)
        {
            return _eventsCollection.Find<Event>(x => x.ChatId == chatId)
                                    .SortBy(x => x.Date)
                                    .ToList();
        }

        private FilterDefinition<Event> getIdFilter(Guid id)
        {
            return _filterBuilder.Eq(x => x.Id, id);
        }

        private FilterDefinition<Event> getDraftEventFilter(long chatId)
        {
            var chatIdFilter = _filterBuilder.Eq(x => x.ChatId, chatId);
            var draftEventFilter = _filterBuilder.Eq(x => x.Status, EventStatus.InProgress);
            return _filterBuilder.And(new List<FilterDefinition<Event>> { chatIdFilter, draftEventFilter });
        }
    }
}
