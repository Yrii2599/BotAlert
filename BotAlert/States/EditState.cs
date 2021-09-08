using System.Threading.Tasks;
using BotAlert.Models;
using BotAlert.Helpers;
using BotAlert.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotAlert.States
{
    public class EditState : IState
    {
        private readonly IEventProvider _eventProvider;
        private readonly IStateProvider _stateProvider;
        private readonly ILocalizerFactory _localizerFactory;

        public EditState(IEventProvider eventProvider, IStateProvider stateProvider, ILocalizerFactory localizerFactory)
        { 
            _eventProvider = eventProvider;
            _stateProvider = stateProvider;
            _localizerFactory = localizerFactory;
        }

        public async Task<ContextState> BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            var localizer = _localizerFactory.GetLocalizer(_stateProvider.GetChatState(message.Chat.Id).Language);

            return await PrintMessage(botClient, message.Chat.Id, localizer.GetMessage(MessageKeyConstants.InvalidChoiceInput));
        }

        public async Task<ContextState> BotOnCallBackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            await botClient.AnswerCallbackQueryAsync(callbackQueryId: callbackQuery.Id);

            return callbackQuery.Data switch
            {
                "Title" => ContextState.InputTitleState,
                "Date" => ContextState.InputDateState,
                "WarnDate" => ContextState.InputWarnDateState,
                "Description" => ContextState.InputDescriptionState,
                "Back" => ContextState.GetNotificationDetailsState,

                _ => ContextState.EditState
            };
        }

        public void BotSendMessage(ITelegramBotClient botClient, long chatId)
        {
            var chat = _stateProvider.GetChatState(chatId);
            var eventObj = _eventProvider.GetEventById(chat.ActiveNotificationId);

            var localizer = _localizerFactory.GetLocalizer(chat.Language);

            InteractionHelper.SendInlineKeyboard(botClient, chatId, eventObj.ToString(localizer), localizer.GetInlineKeyboardMarkUp(MessageKeyConstants.EditMarkUp));
        }

        private async Task<ContextState> PrintMessage(ITelegramBotClient botClient, long chatId, string message)
        {
            await botClient.SendTextMessageAsync(chatId, message);

            return ContextState.EditState;
        }
    }
}
