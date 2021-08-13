using System.Threading.Tasks;
using BotAlert.Models;
using BotAlert.Helpers;
using BotAlert.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotAlert.States
{
    public class GetNotificationDetails : IState
    {
        private readonly IEventProvider _eventProvider;
        private readonly IStateProvider _stateProvider;

        public GetNotificationDetails(IEventProvider eventProvider, IStateProvider stateProvider)
        {
            _eventProvider = eventProvider;
            _stateProvider = stateProvider;
        }

        public async Task<ContextState> BotOnCallBackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            await botClient.AnswerCallbackQueryAsync(callbackQueryId: callbackQuery.Id);

            switch (callbackQuery.Data)
            {
                case "Back":
                    return ContextState.GetAllNotificationsState;

                    break;

                default:
                    return ContextState.GetNotificationDetails;

                    break;
            }
        }

        public async Task<ContextState> BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            return ContextState.GetNotificationDetails;
        }

        public void BotSendMessage(ITelegramBotClient botClient, long chatId)
        {
            var eventId = _stateProvider.GetChatState(chatId).ActiveNotificationId;
            var eventObj = _eventProvider.GetEventById(eventId);
            var options = new InlineKeyboardMarkup(new[] { new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData("Изменить событие", "Edit") },
                                                           new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData("Удалить событие", "Delete") },
                                                           new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData("Назад", "Back") } 
                                                   });

            InteractionHelper.SendInlineKeyboard(botClient,chatId, eventObj.ToString(), options);
        }
    }
}
