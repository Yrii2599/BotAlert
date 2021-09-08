using System;
using System.ComponentModel.DataAnnotations;
using BotAlert.Interfaces;
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

        public string ToString(ILocalizeHelper localizer)
        {
            return $"\t {localizer.GetMessage(MessageKeyConstants.Title)} {Title}\n" +
                $"{localizer.GetMessage(MessageKeyConstants.Date)} {Date.AddHours(TimeOffSet).ToString("dd.MM.yyyy HH:mm")}\n" +
                $"{localizer.GetMessage(MessageKeyConstants.WarnDate)} {WarnDate.AddHours(TimeOffSet).ToString("dd.MM.yyyy HH:mm")}\n" +
                localizer.GetTimeZone(TimeOffSet) +
                $"{Description}\n";
        }
    }
}
