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
    public class UserInputDateState : IState
    {
        private readonly IEventProvider _eventProvider;

        public UserInputDateState(IEventProvider eventProvider)
        {
            _eventProvider = eventProvider;
        }

        public async Task<ContextState> BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            if(message.Text == null || !DateTime.TryParse(message.Text, 
                                                          new CultureInfo(TelegramSettings.DateTimeFormat), 
                                                          DateTimeStyles.None, 
                                                          out var date))
            {
                return HandleInvalidInput(botClient, message.Chat.Id);
            }

            if (DateTime.Now > date) 
            { 
                return HandleInvalidInput(botClient, message.Chat.Id);
            }

            var eventObj = _eventProvider.GetDraftEventByChatId(message.Chat.Id);

            eventObj.Date = date;

            _eventProvider.UpdateEvent(eventObj);
            return ContextState.UserInputWarnDateState;
        }

        public async Task<ContextState> BotOnCallBackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            await botClient.AnswerCallbackQueryAsync(callbackQueryId: callbackQuery.Id);
            return ContextState.UserInputDateState;
        }

        public void BotSendMessage(ITelegramBotClient botClient, long chatId)
        {
            botClient.SendTextMessageAsync(chatId, $"Введите дату и время события (DD.MM.YYYY HH:MM:SS):");
        }

        public ContextState HandleInvalidInput(ITelegramBotClient botClient, long chatId)
        {
            botClient.SendTextMessageAsync(chatId, "Неверный формат даты и времени");
            return ContextState.UserInputDateState;
        }
    }
}
