using System;
using System.Collections.Generic;
using BotAlert.Models;

namespace BotAlert.Interfaces
{
    public interface IEventProvider
    {
        public void CreateEvent(Event eventObj);
        public List<Event> GetAllEvents();
        public List<Event> GetAllEventsInDateRange(DateTime rangeStart, DateTime rangeEnd);
        public Event GetEventById(Guid id);
        public Event GetEventByTitle(string title);
        public void UpdateEvent(Event eventObj);
        public void DeleteEvent(Guid id);
        Event GetDraftEventByChatId(long chatId);
    }
}
