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
            var eventObj = _eventProvider.GetDraftEventByChatId(message.Chat.Id);
            // Можно добавить .AddHours(3) тк. оно хранит в UTC+0, а у нас UTC+3
            eventObj.WarnDate = DateTime.Parse(message.Text, new CultureInfo(TelegramSettings.DateTimeFormat));
            _eventProvider.UpdateEvent(eventObj);
            return ContextState.UserInputDescriptionState;
        }

        public void BotSendMessage(ITelegramBotClient botClient, long chatId)
        {
            botClient.SendTextMessageAsync(chatId, $"Введите дату и время для оповещения:");
        }
    }
}
