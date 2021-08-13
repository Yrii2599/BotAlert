﻿using System;
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
    }
}