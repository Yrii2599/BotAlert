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
    public class InputDateState : IState
    {
        private readonly IEventProvider _eventProvider;

        public InputDateState(IEventProvider eventProvider)
        {
            _eventProvider = eventProvider;
        }

        public async Task<ContextState> BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            if (message.Text == null || !DateTime.TryParse(message.Text, 
                                                          new CultureInfo(TelegramSettings.DateTimeFormat), 
                                                          DateTimeStyles.None, 
                                                          out var date))
            {
                return HandleInvalidInput(botClient, message.Chat.Id, "Неверный формат даты и времени");
            }

            if (date < DateTime.Now) 
            { 
                return HandleInvalidInput(botClient, message.Chat.Id, "Событие уже прошло");
            }

            _eventProvider.UpdateDraftEventByChatId(message.Chat.Id, "Date", date);

            return ContextState.InputWarnDateKeyboard;
        }

        public async Task<ContextState> BotOnCallBackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            await botClient.AnswerCallbackQueryAsync(callbackQueryId: callbackQuery.Id);
            return ContextState.InputDateState;
        }

        public void BotSendMessage(ITelegramBotClient botClient, long chatId)
        {
            botClient.SendTextMessageAsync(chatId, $"Введите дату и время события в UTC (DD.MM.YYYY HH:MM:SS):");
        }

        public ContextState HandleInvalidInput(ITelegramBotClient botClient, long chatId, string message)
        {
            botClient.SendTextMessageAsync(chatId, message);
            return ContextState.InputDateState;
        }
    }
}
