using System;
using System.Globalization;
using System.Threading.Tasks;
using BotAlert.Models;
using BotAlert.Settings;
using BotAlert.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotAlert.States
{
    public class InputWarnDateState : IState
    {
        private readonly IEventProvider _eventProvider;
        private readonly IStateProvider _stateProvider;

        public InputWarnDateState(IEventProvider eventProvider, IStateProvider stateProvider)
        {
            _eventProvider = eventProvider;
            _stateProvider = stateProvider;
        }

        public async Task<ContextState> BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            if (message.Text == null || !DateTime.TryParse(message.Text,
                                                          new CultureInfo(TelegramSettings.DateTimeFormat),
                                                          DateTimeStyles.None,
                                                          out var warnDate))
            {
                return PrintMessage(botClient, message.Chat.Id, "Неверный формат даты и времени");
            }


            var chat = _stateProvider.GetChatState(message.Chat.Id);

            if (chat.ActiveNotificationId != Guid.Empty)
            {
                var eventObj = _eventProvider.GetEventById(chat.ActiveNotificationId);

                if (warnDate > eventObj.Date.ToLocalTime() || warnDate < DateTime.Now)
                {
                    return PrintMessage(botClient, message.Chat.Id, "Оповещение уже произошло");
                }

                eventObj.WarnDate = warnDate;
                _eventProvider.UpdateEvent(eventObj);

                return ContextState.EditState;
            }
            else
            {
                var eventObj = _eventProvider.GetDraftEventByChatId(message.Chat.Id);

                if (warnDate > eventObj.Date.ToLocalTime() || warnDate < DateTime.Now)
                {
                    return PrintMessage(botClient, message.Chat.Id, "Оповещение уже произошло");
                }

                _eventProvider.UpdateDraftEventByChatId(message.Chat.Id, x => x.WarnDate, warnDate);

                return ContextState.InputDescriptionKeyboardState;
            }
        }

        public async Task<ContextState> BotOnCallBackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            await botClient.AnswerCallbackQueryAsync(callbackQueryId: callbackQuery.Id);

            return ContextState.InputWarnDateState;
        }

        public void BotSendMessage(ITelegramBotClient botClient, long chatId)
        {
            botClient.SendTextMessageAsync(chatId, $"Введите дату и время для оповещения\n(DD.MM.YYYY HH:MM):");
        }

        private ContextState PrintMessage(ITelegramBotClient botClient, long chatId, string message)
        {
            botClient.SendTextMessageAsync(chatId, message);

            return ContextState.InputWarnDateState;
        }
    }
}
