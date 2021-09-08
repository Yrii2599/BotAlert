using System.Threading.Tasks;
using BotAlert.Helpers;
using BotAlert.Interfaces;
using BotAlert.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotAlert.States
{
    public class InputEventTimeZoneKeyboardState : IState
    {
        private readonly IStateProvider _stateProvider;
        private readonly IEventProvider _eventProvider;
        private readonly ILocalizerFactory _localizerFactory;

        public InputEventTimeZoneKeyboardState(IStateProvider stateProvider, IEventProvider eventProvider, ILocalizerFactory localizerFactory)
        {
            _stateProvider = stateProvider;
            _eventProvider = eventProvider;
            _localizerFactory = localizerFactory;
        }

        public async Task<ContextState> BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            if (message.Text != null)
            {
                if (message.Text.ToLower() == "да" || message.Text.ToLower() == "yes")
                {
                    return await HandleAcceptInput();
                }

                if (message.Text.ToLower() == "нет" || message.Text.ToLower() == "no")
                {
                    return await HandleDeclineInput(message.Chat.Id);
                }
            }

            var localizer = _localizerFactory.GetLocalizer(_stateProvider.GetChatState(message.Chat.Id).Language);

            return await PrintMessage(botClient, message.Chat.Id, localizer.GetMessage(MessageKeyConstants.InvalidChoiceInput));
        }

        public async Task<ContextState> BotOnCallBackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            await botClient.AnswerCallbackQueryAsync(callbackQueryId: callbackQuery.Id);

            if (callbackQuery.Data == "да")
            {
                return await HandleAcceptInput();
            }

            if (callbackQuery.Data == "нет")
            {
                return await HandleDeclineInput(callbackQuery.Message.Chat.Id);
            }

            var localizer = _localizerFactory.GetLocalizer(_stateProvider.GetChatState(callbackQuery.Message.Chat.Id).Language);

            return await PrintMessage(botClient, callbackQuery.Message.Chat.Id, localizer.GetMessage(MessageKeyConstants.InvalidChoiceInput));
        }

        public void BotSendMessage(ITelegramBotClient botClient, long chatId)
        {
            var chat = _stateProvider.GetChatState(chatId);
            var localizer = _localizerFactory.GetLocalizer(chat.Language);

            InteractionHelper.SendInlineKeyboard(botClient, chatId, 
                localizer.GetTimeZone(chat.TimeOffSet) + localizer.GetMessage(MessageKeyConstants.WantToChangeTimeZone), 
                localizer.GetInlineKeyboardMarkUp(MessageKeyConstants.YesOrNoMarkUp));
        }

        private async Task<ContextState> PrintMessage(ITelegramBotClient botClient, long chatId, string message)
        {
            await botClient.SendTextMessageAsync(chatId, message);

            return ContextState.InputEventTimeZoneKeyboardState;
        }

        private Task<ContextState> HandleAcceptInput() => Task.FromResult(ContextState.InputTimeZoneState);

        private Task<ContextState> HandleDeclineInput(long chatId)
        {
            var chat = _stateProvider.GetChatState(chatId);
            var eventObj = _eventProvider.GetEventById(chat.ActiveNotificationId);
            eventObj.TimeOffSet = chat.TimeOffSet;
            _eventProvider.UpdateEvent(eventObj);

            return Task.FromResult(ContextState.InputDateState);
        }
    }
}

