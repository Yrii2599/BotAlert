using System;
using System.Globalization;
using System.Threading.Tasks;
using BotAlert.Interfaces;
using BotAlert.Models;
using BotAlert.Settings;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotAlert.States
{
    public class UserInputWarnDateState : IState
    {
        private readonly IEventProvider _eventProvider;

        public UserInputWarnDateState(IEventProvider eventProvider)
        {
            _eventProvider = eventProvider;
        }

        public async Task<ContextState> BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            if (message.Text == null || !DateTime.TryParse(message.Text,
                                                          new CultureInfo(TelegramSettings.DateTimeFormat),
                                                          DateTimeStyles.None,
                                                          out var warnDate))
            {
                return HandleInvalidInput(botClient, message.Chat.Id);
            }

            var eventObj = _eventProvider.GetDraftEventByChatId(message.Chat.Id);
            if(warnDate > eventObj.Date) return HandleInvalidInput(botClient, message.Chat.Id);
            // Можно добавить .AddHours(3) тк. оно хранит в UTC+0, а у нас UTC+3
            eventObj.WarnDate = warnDate.AddHours(3);

            _eventProvider.UpdateEvent(eventObj);
            return ContextState.UserInputDescriptionState;
        }

        public void BotSendMessage(ITelegramBotClient botClient, long chatId)
        {
            botClient.SendTextMessageAsync(chatId, $"Введите дату и время для оповещения (DD.MM.YYYY HH:MM:SS):");
        }

        public ContextState HandleInvalidInput(ITelegramBotClient botClient, long chatId)
        {
            botClient.SendTextMessageAsync(chatId, "Неверный формат даты и времени");
            return ContextState.UserInputWarnDateState;
        }
    }
}
