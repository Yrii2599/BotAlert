using System;
using System.Threading.Tasks;
using BotAlert.Models;
using BotAlert.Helpers;
using BotAlert.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotAlert.States
{
    public class SaveState : IState
    {
        private readonly IEventProvider _eventProvider;
        private readonly IStateProvider _stateProvider;
        private readonly ILocalizerFactory _localizerFactory;

        public SaveState(IEventProvider eventProvider, IStateProvider stateProvider, ILocalizerFactory localizerFactory)
        {
            _eventProvider = eventProvider;
            _stateProvider = stateProvider;
            _localizerFactory = localizerFactory;
        }

        public async Task<ContextState> BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            if (message.Text != null)
            {
                if (message.Text.ToLower() == "сохранить" || message.Text.ToLower() == "save") 
                {
                    return await HandleAcceptInput(botClient, message.Chat.Id);
                }
                else if (message.Text.ToLower() == "отменить" || message.Text.ToLower() == "cancel") 
                {
                    return await HandleDeclineInput(botClient, message.Chat.Id);
                }
            }

            var localizer = _localizerFactory.GetLocalizer(_stateProvider.GetChatState(message.Chat.Id).Language);

            return await PrintMessage(botClient, message.Chat.Id, localizer.GetMessage(MessageKeyConstants.InvalidChoiceInput));
        }

        public async Task<ContextState> BotOnCallBackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            await botClient.AnswerCallbackQueryAsync(callbackQueryId: callbackQuery.Id);

            if (callbackQuery.Data == "Save")
            {
                return await HandleAcceptInput(botClient, callbackQuery.Message.Chat.Id);
            }

            return await HandleDeclineInput(botClient, callbackQuery.Message.Chat.Id);
        }

        public void BotSendMessage(ITelegramBotClient botClient, long chatId)
        {
            var eventObj = _eventProvider.GetDraftEventByChatId(chatId);

            var localizer = _localizerFactory.GetLocalizer(_stateProvider.GetChatState(chatId).Language);

            var message = eventObj.ToString(localizer) +
                          "****************************\n" +
                          localizer.GetMessage(MessageKeyConstants.WantToSaveEvent);

            InteractionHelper.SendInlineKeyboard(botClient, chatId, message, localizer.GetInlineKeyboardMarkUp(MessageKeyConstants.SaveMarkUp));
        }

        private async Task<ContextState> PrintMessage(ITelegramBotClient botClient, long chatId, string message)
        {
            await botClient.SendTextMessageAsync(chatId, message);

            return ContextState.SaveState;
        }

        private async Task<ContextState> HandleAcceptInput(ITelegramBotClient botClient, long chatId)
        {
            var chat = _stateProvider.GetChatState(chatId);
            var eventObj = _eventProvider.GetEventById(chat.ActiveNotificationId);

            var localizer = _localizerFactory.GetLocalizer(chat.Language);

            if (eventObj == null)
            {
                chat.ActiveNotificationId = Guid.Empty;
                _stateProvider.SaveChatState(chat);
                await botClient.SendTextMessageAsync(chat.ChatId, localizer.GetMessage(MessageKeyConstants.ExpiredDate));

                return ContextState.MainState;
            }

            eventObj.Status = EventStatus.Created;
            _eventProvider.UpdateEvent(eventObj);

            chat.ActiveNotificationId = Guid.Empty;
            _stateProvider.SaveChatState(chat);

            await botClient.SendTextMessageAsync(chatId, localizer.GetMessage(MessageKeyConstants.SaveSuccess));

            return ContextState.MainState;
        }

        private async Task<ContextState> HandleDeclineInput(ITelegramBotClient botClient, long chatId)
        {
            var chat = _stateProvider.GetChatState(chatId);
            _eventProvider.DeleteEvent(chat.ActiveNotificationId);

            chat.ActiveNotificationId = Guid.Empty;
            _stateProvider.SaveChatState(chat);

            var localizer = _localizerFactory.GetLocalizer(chat.Language);

            await botClient.SendTextMessageAsync(chatId, localizer.GetMessage(MessageKeyConstants.DeleteSuccess));

            return ContextState.MainState;
        }
    }
}
