using System;
using System.Threading.Tasks;
using BotAlert.Models;
using BotAlert.Helpers;
using BotAlert.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotAlert.States
{
    public class GetNotificationDetailsState : IState
    {
        private readonly IEventProvider _eventProvider;
        private readonly IStateProvider _stateProvider;

        public GetNotificationDetailsState(IEventProvider eventProvider, IStateProvider stateProvider)
        {
            _eventProvider = eventProvider;
            _stateProvider = stateProvider;
        }

        public Task<ContextState> BotOnMessageReceived(ITelegramBotClient botClient, Message message) 
            => Task.FromResult(ContextState.GetNotificationDetailsState);

        public async Task<ContextState> BotOnCallBackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            await botClient.AnswerCallbackQueryAsync(callbackQueryId: callbackQuery.Id);

            return callbackQuery.Data switch 
            {
                "Edit" => ContextState.EditState,
                "Delete" => ContextState.InputDeleteKeyboardState,
                "Back" => MoveBack(callbackQuery.Message.Chat.Id),

                _ => ContextState.GetNotificationDetailsState
            };
        }

        public void BotSendMessage(ITelegramBotClient botClient, long chatId)
        {
            var eventId = _stateProvider.GetChatState(chatId).ActiveNotificationId;
            var eventObj = _eventProvider.GetEventById(eventId);

            var options = new InlineKeyboardMarkup(new[] 
            { 
                new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData("Изменить", "Edit") },
                new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData("Удалить", "Delete") },
                new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData("Назад", "Back") } 
            });

            InteractionHelper.SendInlineKeyboard(botClient, chatId, eventObj.ToString(), options);
        }

        private ContextState MoveBack(long chatId)
        {
            var chat = _stateProvider.GetChatState(chatId);
            chat.ActiveNotificationId = Guid.Empty;
            _stateProvider.SaveChatState(chat);

            return ContextState.GetAllNotificationsState;
        }
    }
}
