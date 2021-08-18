using System;
using System.Collections.Generic;
using BotAlert.Models;

namespace BotAlert.Interfaces
{
    public interface IEventProvider
    {
        void CreateEvent(Event eventObj);

        List<Event> GetUserEventsOnPage(long chatId);

        List<Event> GetAllNotificationsToBeSentNow();

        bool UserEventsPreviousPageExists(long chatId);

        bool UserEventsNextPageExists(long chatId);

        Event GetEventById(Guid id);

        void UpdateEvent(Event eventObj);

        void DeleteEvent(Guid id);

        Event GetDraftEventByChatId(long chatId);
    }
}
