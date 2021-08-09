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
        public string Title { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public TimeSpan WarnInAdvance { get; set; }

        public string Description { get; set; }
    }
}
