using System;
using System.Threading.Tasks;
using BotAlert.Models;
using BotAlert.Helpers;
using BotAlert.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotAlert.States
{
    public class InputDeleteKeyboardState : IState
    {
        private readonly IEventProvider _eventProvider;
        private readonly IStateProvider _stateProvider;
        private readonly ILocalizerFactory _localizerFactory;

        public InputDeleteKeyboardState(IEventProvider eventProvider, IStateProvider stateProvider, ILocalizerFactory localizerFactory)
        {
            _eventProvider = eventProvider;
            _stateProvider = stateProvider;
            _localizerFactory = localizerFactory;
        }

        public async Task<ContextState> BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            if (message.Text != null)
            {
                if (message.Text.ToLower() == "да" || message.Text.ToLower() == "yes")
                {
                    return await HandleAcceptInput(botClient, message.Chat.Id);
                }
                else if (message.Text.ToLower() == "нет" || message.Text.ToLower() == "no")
                {
                    return await HandleDeclineInput();
                }
            }

            var localizer = _localizerFactory.GetLocalizer(_stateProvider.GetChatState(message.Chat.Id).Language);

            return await PrintMessage(botClient, message.Chat.Id, localizer.GetMessage(MessageKeyConstants.InvalidChoiceInput));
        }

        public async Task<ContextState> BotOnCallBackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            await botClient.AnswerCallbackQueryAsync(callbackQueryId: callbackQuery.Id);

            return callbackQuery.Data switch
            {
                "да" => await HandleAcceptInput(botClient, callbackQuery.Message.Chat.Id),
                _ => await HandleDeclineInput()
            };
        }

        public void BotSendMessage(ITelegramBotClient botClient, long chatId)
        {
            var localizer = _localizerFactory.GetLocalizer(_stateProvider.GetChatState(chatId).Language);

            InteractionHelper.SendInlineKeyboard(botClient, chatId, 
                localizer.GetMessage(MessageKeyConstants.DeleteEventAssurance), 
                localizer.GetInlineKeyboardMarkUp(MessageKeyConstants.YesOrNoMarkUp));
        }

        private async Task<ContextState> PrintMessage(ITelegramBotClient botClient, long chatId, string message)
        {
            await botClient.SendTextMessageAsync(chatId, message);

            return ContextState.InputDeleteKeyboardState;
        }

        private Task<ContextState> HandleAcceptInput(ITelegramBotClient botClient, long chatId)
        {
            var chat = _stateProvider.GetChatState(chatId);

            var localizer = _localizerFactory.GetLocalizer(chat.Language);

            _eventProvider.DeleteEvent(chat.ActiveNotificationId);

            chat.ActiveNotificationId = Guid.Empty;

            _stateProvider.SaveChatState(chat);

            botClient.SendTextMessageAsync(chatId, localizer.GetMessage(MessageKeyConstants.DeleteSuccess));

            return Task.FromResult(ContextState.GetAllNotificationsState);
        }

        private Task<ContextState> HandleDeclineInput() => Task.FromResult(ContextState.GetNotificationDetailsState);
    }
}