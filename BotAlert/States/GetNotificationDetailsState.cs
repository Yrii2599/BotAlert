using System;
using System.Threading.Tasks;
using BotAlert.Models;
using BotAlert.Helpers;
using BotAlert.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotAlert.States
{
    public class GetNotificationDetailsState : IState
    {
        private readonly IEventProvider _eventProvider;
        private readonly IStateProvider _stateProvider;
        private readonly ILocalizerFactory _localizerFactory;

        public GetNotificationDetailsState(IEventProvider eventProvider, IStateProvider stateProvider, ILocalizerFactory localizerFactory)
        {
            _eventProvider = eventProvider;
            _stateProvider = stateProvider;
            _localizerFactory = localizerFactory;
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
            var chat = _stateProvider.GetChatState(chatId);
            var eventObj = _eventProvider.GetEventById(chat.ActiveNotificationId);

            var localizer = _localizerFactory.GetLocalizer(chat.Language);

            var message = eventObj?.ToString(localizer) ?? localizer.GetMessage(MessageKeyConstants.ExpiredEventForDetails);

            InteractionHelper.SendInlineKeyboard(botClient, chatId, message, localizer.GetInlineKeyboardMarkUp(MessageKeyConstants.DetailsMarkUp));
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
