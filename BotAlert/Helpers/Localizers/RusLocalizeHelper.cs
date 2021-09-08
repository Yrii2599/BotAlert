using System.Collections.Generic;
using BotAlert.Interfaces;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotAlert.Helpers.Localizers
{
    public class RusLocalizeHelper : ILocalizeHelper
    {
        private readonly Dictionary<string, string> _messages = new()
        {
            { MessageKeyConstants.Start, "Добро пожаловать" },
            
            { MessageKeyConstants.EnterTitle, "Введите название события:" },

            { MessageKeyConstants.EnterDate, "Введите дату и время события\n(DD.MM.YYYY HH:MM):" },

            { MessageKeyConstants.EnterWarnDate, "Введите дату и время оповещения\n(DD.MM.YYYY HH:MM):" },

            { MessageKeyConstants.EnterDescription, "Введите описание оповещения:" },

            { MessageKeyConstants.EnterTimeZone, "Введите новый часовой пояс (от -12 до +14):" },

            { MessageKeyConstants.EnterLanguage, "Какой язык вы предпочитаете?" },

            { MessageKeyConstants.ChooseEvent, "\nВыберите событие:" },

            { MessageKeyConstants.InvalidChoiceInput, "Выберите пожалуйста один из вариантов!" },

            { MessageKeyConstants.InvalidTextInput, "Неверный формат сообщения!" },

            { MessageKeyConstants.InvalidDateInput, "Неверный формат даты и времени!" },

            { MessageKeyConstants.InvalidWarnDateInput, "Оповещение не может прийти после события!" },

            { MessageKeyConstants.CommandChoicePanel,"Выберите одну из команд:\n" +
                                                   "/create - Создать новое событие\n" +
                                                   "/get_notifications - Получить список событий\n" +
                                                   "/set_time_zone - Установить часовой пояс\n" +
                                                   "/set_language - Изменить язык"
            },

            { MessageKeyConstants.ExpiredDate, "Данное событие уже прошло" },

            { MessageKeyConstants.ExpiredEventForDetails, "Событие удалено" },

            { MessageKeyConstants.ExpiredWarnDate, "Оповещение уже произошло" },

            { MessageKeyConstants.NoEvents, "У вас нет предстоящих событий" },

            { MessageKeyConstants.WhenToRemind, "Когда вас уведомить?" },

            { MessageKeyConstants.WantToAddDescription, "Желаете добавить описание?" },

            { MessageKeyConstants.WantToChangeTimeZone, "Желаете ввести иной часовой пояс?" },

            { MessageKeyConstants.WantToSaveEvent, "Сохранить событие?" },

            { MessageKeyConstants.DeleteEventAssurance, "Вы точно хотите удалить это событие?" },

            { MessageKeyConstants.DeleteSuccess, "Запись успешно удалена!" },

            { MessageKeyConstants.SaveSuccess, "Запись успешно сохранена!" },

            { MessageKeyConstants.Back, "Назад" },

            { MessageKeyConstants.Title, "Название:" },

            { MessageKeyConstants.Date, "Дата события:" },

            { MessageKeyConstants.WarnDate, "Дата оповещения:" },

            { MessageKeyConstants.CurrentLanguage, "Ваш текущий язык - Русский" }
            };

        private readonly Dictionary<string, InlineKeyboardMarkup> _markUps = new()
        {
            {
                MessageKeyConstants.WarnDateMarkUp,
                new InlineKeyboardMarkup(new[] {
                    new[] { InlineKeyboardButton.WithCallbackData("5 м.", "5"),
                        InlineKeyboardButton.WithCallbackData("15 м.", "15") },
                    new[] { InlineKeyboardButton.WithCallbackData("30 м.", "30"),
                        InlineKeyboardButton.WithCallbackData("1 ч.", "60") },
                    new[] { InlineKeyboardButton.WithCallbackData("Ввести свое значение", "own") }
                })
            },

            {
                MessageKeyConstants.EditMarkUp,
                new InlineKeyboardMarkup(new[] {
                    new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData("Изменить название", "Title") },
                    new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData("Изменить дату события", "Date") },
                    new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData("Изменить дату оповещения", "WarnDate") },
                    new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData("Изменить описание", "Description") },
                    new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData("Назад", "Back") },
                })
            },

            {
                MessageKeyConstants.DetailsMarkUp,
                new InlineKeyboardMarkup(new[]
                {
                    new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData("Изменить", "Edit") },
                    new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData("Удалить", "Delete") },
                    new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData("Назад", "Back") }
                })
            },

            { 
                MessageKeyConstants.SaveMarkUp,
                new InlineKeyboardMarkup(new[] { InlineKeyboardButton.WithCallbackData("Сохранить", "Save"),
                                                 InlineKeyboardButton.WithCallbackData("Отменить", "Cancel") })
            },

            {
                MessageKeyConstants.YesOrNoMarkUp,
                new InlineKeyboardMarkup(new[] { InlineKeyboardButton.WithCallbackData("Да", "да") ,
                                                 InlineKeyboardButton.WithCallbackData("Нет", "нет") })
            },
        };

        public string GetMessage(string key) => _messages[key];

        public InlineKeyboardMarkup GetInlineKeyboardMarkUp(string key) => _markUps[key];

        public string GetTimeZone(int timeZone)
        {
            if (timeZone > 0)
            {
                return $"Текущий часовой пояс: UTC +{timeZone}:00 \n";
            }
            else
            {
                return $"Текущий часовой пояс: UTC {timeZone}:00 \n";
            }
        }
    }
}
