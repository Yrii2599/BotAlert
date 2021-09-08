using System.Collections.Generic;
using BotAlert.Interfaces;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotAlert.Helpers.Localizers
{
    public class EngLocalizeHelper : ILocalizeHelper
    {
        private readonly Dictionary<string, string> _messages = new()
        {
            { MessageKeyConstants.Start, "Glad to see you" },
            
            { MessageKeyConstants.EnterTitle, "Enter event title:" },

            { MessageKeyConstants.EnterDate, "Enter date and time for the event\n(DD.MM.YYYY HH:MM):" },

            { MessageKeyConstants.EnterWarnDate, "Enter date and time for the notification\n(DD.MM.YYYY HH:MM):" },

            { MessageKeyConstants.EnterDescription, "Enter event description:" },

            { MessageKeyConstants.EnterTimeZone, "Enter time zone (from -12 to +14):" },

            { MessageKeyConstants.EnterLanguage, "What language do you prefer?" },

            { MessageKeyConstants.ChooseEvent, "\nChoose an event:" },

            { MessageKeyConstants.InvalidChoiceInput, "Please choose one of the options!" },

            { MessageKeyConstants.InvalidTextInput, "Invalid message format!" },

            { MessageKeyConstants.InvalidDateInput, "Invalid date and time format!" },

            { MessageKeyConstants.InvalidWarnDateInput, "The notification cannot come after the event!" },

            {
                MessageKeyConstants.CommandChoicePanel,
                "Choose one of the commands:\n" +
                                                   "/create - Create new event\n" +
                                                   "/get_notifications - Get notification list\n" +
                                                   "/set_time_zone - Set your time zone\n" +
                                                   "/set_language - Set your preferred language"
            },

            { MessageKeyConstants.ExpiredDate, "The event has already passed" },

            { MessageKeyConstants.ExpiredEventForDetails, "The event has been already deleted" },

            { MessageKeyConstants.ExpiredWarnDate, "The notification has already occurred" },

            { MessageKeyConstants.NoEvents, "You have no upcoming events!" },

            { MessageKeyConstants.WhenToRemind, "When should I notify you?" },

            { MessageKeyConstants.WantToAddDescription, "Would you like to add a description?" },

            { MessageKeyConstants.WantToChangeTimeZone, "Would you like to enter a different time zone?" },

            { MessageKeyConstants.WantToSaveEvent, "Save event?" },

            { MessageKeyConstants.DeleteEventAssurance, "Are you sure you want to delete this event?" },

            { MessageKeyConstants.DeleteSuccess, "The event was successfully deleted!" },

            { MessageKeyConstants.SaveSuccess, "The event was successfully saved!" },

            { MessageKeyConstants.Back, "Back" },

            { MessageKeyConstants.Title, "Title:" },

            { MessageKeyConstants.Date, "Event date:" },

            { MessageKeyConstants.WarnDate, "Notification date:" },

            { MessageKeyConstants.CurrentLanguage, "Your current language is English" }
        };

        private readonly Dictionary<string, InlineKeyboardMarkup> _markUps = new()
        {
            {
                MessageKeyConstants.WarnDateMarkUp,
                new InlineKeyboardMarkup(new[] {
                    new[] { InlineKeyboardButton.WithCallbackData("5 min", "5"),
                        InlineKeyboardButton.WithCallbackData("15 min", "15") },
                    new[] { InlineKeyboardButton.WithCallbackData("30 min", "30"),
                        InlineKeyboardButton.WithCallbackData("1 h", "60") },
                    new[] { InlineKeyboardButton.WithCallbackData("Own value", "own") }
                })
            },

            {
                MessageKeyConstants.EditMarkUp,
                new InlineKeyboardMarkup(new[] {
                    new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData("Edit title", "Title") },
                    new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData("Edit event date", "Date") },
                    new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData("Edit notification date", "WarnDate") },
                    new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData("Edit description", "Description") },
                    new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData("Back", "Back") },
                })
            },

            {
                MessageKeyConstants.DetailsMarkUp,
                new InlineKeyboardMarkup(new[]
                {
                    new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData("Edit", "Edit") },
                    new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData("Delete", "Delete") },
                    new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData("Back", "Back") }
                })
            },

            {
                MessageKeyConstants.SaveMarkUp,
                new InlineKeyboardMarkup(new[] { InlineKeyboardButton.WithCallbackData("Save", "Save"),
                                                           InlineKeyboardButton.WithCallbackData("Cancel", "Cancel") })
            },

            {
                MessageKeyConstants.YesOrNoMarkUp,
                new InlineKeyboardMarkup(new[] { InlineKeyboardButton.WithCallbackData("Yes", "да") ,
                                                           InlineKeyboardButton.WithCallbackData("No", "нет") })
            },
        };

        public string GetMessage(string key) => _messages[key];

        public InlineKeyboardMarkup GetInlineKeyboardMarkUp(string key) => _markUps[key];

        public string GetTimeZone(int timeZone)
        {
            if (timeZone > 0)
            {
                return $"Current time zone: UTC +{timeZone}:00 \n";
            }
            else
            {
                return $"Current time zone: UTC {timeZone}:00 \n";
            }
        }
    }
}

