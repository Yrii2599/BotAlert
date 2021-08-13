using System;
using BotAlert.Models;

namespace BotAlert.Interfaces
{
    public interface IStateProvider
    {
        IState GetChatState(long chatId);

        int GetChatPage(long chatId);

        public Guid GetCurrentlyViewingNotificationId(long chatId);

        void SaveChatState(ChatState chatState);

        public void ResetChatPage(long chatId);

        public void IncrementChatPage(long chatId, int incrementValue);

        public void UpdateCurrentlyViewingNotification(long chatId, string eventId);
    }
}
