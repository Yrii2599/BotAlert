using System;
using System.ComponentModel.DataAnnotations;
using BotAlert.Helpers;
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

        public int TimeOffSet { get; set; }

        public Event(long chatId, string title)
        {
            ChatId = chatId;
            Title = title;
            Status = EventStatus.InProgress;
            TimeOffSet = 0;
        }

        public override string ToString()
        {
            return $"\t Название: {Title}\n" +
                $"Дата события: {Date.AddHours(TimeOffSet).ToString("dd.MM.yyyy HH:mm")}\n" +
                $"Дата оповещения: {WarnDate.AddHours(TimeOffSet).ToString("dd.MM.yyyy HH:mm")}\n" +
                TimeZoneHelper.PrintTimeZone(TimeOffSet) +
                $"{Description}\n";
        }
    }
}
