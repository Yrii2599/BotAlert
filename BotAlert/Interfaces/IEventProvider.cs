using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using BotAlert.Models;

namespace BotAlert.Interfaces
{
    public interface IEventProvider
    {
        void CreateEvent(Event eventObj);
        List<Event> GetUserEventsOnPage(long chatId);
        bool UserEventsPreviousPageExists(long chatId);
        bool UserEventsNextPageExists(long chatId);
        Event GetEventById(Guid id);
        void UpdateEvent(Event eventObj);
        void DeleteEvent(Guid id);
        Event GetDraftEventByChatId(long chatId);
        public void UpdateDraftEventByChatId<T>(long chatId, Expression<Func<Event, T>> updatingField, T newValue);
    }
}
