using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

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

        public ChatState(long chatId, ContextState state = ContextState.MainState)
        {
            ChatId = chatId;
            State = state;
        }
    }
}
