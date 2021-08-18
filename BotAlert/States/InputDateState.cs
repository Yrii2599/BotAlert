using System;
using System.Globalization;
using System.Threading.Tasks;
using BotAlert.Models;
using BotAlert.Helpers;
using BotAlert.Settings;
using BotAlert.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotAlert.States
{
    public class InputDateState : IState
    {
        private readonly IEventProvider _eventProvider;
        private readonly IStateProvider _stateProvider;

        public InputDateState(IEventProvider eventProvider, IStateProvider stateProvider)
        {
            _eventProvider = eventProvider;
            _stateProvider = stateProvider;
        }

        public async Task<ContextState> BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            if (message.Text == null || !DateTime.TryParse(message.Text, 
                                                          new CultureInfo(TelegramSettings.DateTimeFormat), 
                                                          DateTimeStyles.None, 
                                                          out var date))
            {
                return await PrintMessage(botClient, message.Chat.Id, "Неверный формат даты и времени");
            }

            if (date < DateTime.Now) 
            { 
                return await PrintMessage(botClient, message.Chat.Id, "Событие уже прошло");
            }

            var chat = _stateProvider.GetChatState(message.Chat.Id);
            var eventObj = _eventProvider.GetEventById(chat.ActiveNotificationId);

            eventObj.Date = date.TrimSecondsAndMilliseconds();

            _eventProvider.UpdateEvent(eventObj);

            return ContextState.InputWarnDateKeyboardState;
        }

        public async Task<ContextState> BotOnCallBackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            await botClient.AnswerCallbackQueryAsync(callbackQueryId: callbackQuery.Id);

            return ContextState.InputDateState;
        }

        public void BotSendMessage(ITelegramBotClient botClient, long chatId)
        {
            botClient.SendTextMessageAsync(chatId, $"Введите дату и время события в\n(DD.MM.YYYY HH:MM):");
        }

        private async Task<ContextState> PrintMessage(ITelegramBotClient botClient, long chatId, string message)
        {
            await botClient.SendTextMessageAsync(chatId, message);

            return ContextState.InputDateState;
        }
    }
}
