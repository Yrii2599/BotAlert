using System;
using System.Threading.Tasks;
using BotAlert.Models;
using BotAlert.Helpers;
using BotAlert.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotAlert.States
{
    public class InputTitleState : IState
    {
        private readonly IEventProvider _eventProvider;
        private readonly IStateProvider _stateProvider;
        private readonly ILocalizerFactory _localizerFactory;

        public InputTitleState(IEventProvider eventProvider, IStateProvider stateProvider, ILocalizerFactory localizerFactory)
        {
            _eventProvider = eventProvider;
            _stateProvider = stateProvider;
            _localizerFactory = localizerFactory;
        }

        public async Task<ContextState> BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            var chat = _stateProvider.GetChatState(message.Chat.Id);
            var localizer = _localizerFactory.GetLocalizer(chat.Language);

            if (message.Text == null) 
            { 
                return await PrintMessage(botClient, message.Chat.Id, localizer.GetMessage(MessageKeyConstants.InvalidTextInput));
            }


            if (chat.ActiveNotificationId != Guid.Empty)
            {
                var eventObj = _eventProvider.GetEventById(chat.ActiveNotificationId);
                
                if (eventObj == null)
                {
                    chat.ActiveNotificationId = Guid.Empty;
                    _stateProvider.SaveChatState(chat);
                    await botClient.SendTextMessageAsync(chat.ChatId, localizer.GetMessage(MessageKeyConstants.ExpiredDate));

                    return ContextState.MainState;
                }

                eventObj.Title = message.Text;
                _eventProvider.UpdateEvent(eventObj);

                return ContextState.EditState;
            }
            else
            {
                _eventProvider.CreateEvent(new Event(message.Chat.Id, message.Text));
                chat.ActiveNotificationId = _eventProvider.GetDraftEventByChatId(message.Chat.Id).Id;
                _stateProvider.SaveChatState(chat);

                return ContextState.InputEventTimeZoneKeyboardState;
            }
        }

        public async Task<ContextState> BotOnCallBackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            await botClient.AnswerCallbackQueryAsync(callbackQueryId: callbackQuery.Id);

            return ContextState.InputTitleState;
        }

        public void BotSendMessage(ITelegramBotClient botClient, long chatId)
        {
            var localizer = _localizerFactory.GetLocalizer(_stateProvider.GetChatState(chatId).Language);

            botClient.SendTextMessageAsync(chatId, localizer.GetMessage(MessageKeyConstants.EnterTitle));
        }

        private async Task<ContextState> PrintMessage(ITelegramBotClient botClient, long chatId, string message)
        {
            await botClient.SendTextMessageAsync(chatId, message);

            return ContextState.InputTitleState;
        }
    }
}
