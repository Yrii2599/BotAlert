using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;

namespace BotAlert.Models
{
    public class ChatState
    {
        [BsonId]
        public Guid Id { get; set; }

        [Required]
        public long ChatId { get; set; }

        [Required]
        public ContextState State { get; set; }

        [Required]
        public int NotificationsPage { get; set; }
        
        [Required]
        public Guid ActiveNotificationId { get; set; }

        [Required]
        public int TimeOffSet { get; set; }

        public ChatState(long chatId, ContextState state = ContextState.MainState)
        {
            ChatId = chatId;
            State = state;
            NotificationsPage = 0;
            ActiveNotificationId = Guid.Empty;
            TimeOffSet = 0;
        }
    }
}
