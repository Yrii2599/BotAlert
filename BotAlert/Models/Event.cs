using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;

namespace BotAlert.Models
{
    public class Event
    {
        [BsonId]
        public Guid Id { get; set; }

        [Required]
        public long ChatId { get; set; }

        [Required]
        public EventStatus Status { get; set; }

        [Required]
        public string Title { get; set; }

        public DateTime Date { get; set; }

        public DateTime WarnDate { get; set; }

        public string Description { get; set; }

        public Event(long chatId, string title)
        {
            ChatId = chatId;
            Title = title;
            Status = EventStatus.InProgress;
        }

        public override string ToString()
        {
            return $"\tTitle: {Title}\n" +
                $"Date and time: {Date.ToLocalTime()}\n" +
                $"Date and time of notification: {WarnDate.ToLocalTime()}\n" +
                $"{Description}\n";
        }
    }
}
