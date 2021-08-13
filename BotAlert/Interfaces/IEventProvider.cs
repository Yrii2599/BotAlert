using System;
using System.Collections.Generic;
using BotAlert.Models;

namespace BotAlert.Interfaces
{
    public interface IEventProvider
    {
        public void CreateEvent(Event eventObj);
        public List<Event> GetUserEventsOnPage(long chatId);
        public bool UserEventsPreviousPageExists(long chatId);
        public bool UserEventsNextPageExists(long chatId);
        public List<Event> GetAllEventsInDateRange(DateTime rangeStart, DateTime rangeEnd);
        public Event GetEventById(Guid id);
        public void UpdateEvent(Event eventObj);
        public void DeleteEvent(Guid id);
        Event GetDraftEventByChatId(long chatId);
        public void UpdateDraftEventByChatId<T>(long chatId, string updatingField, T newValue);
    }
}
