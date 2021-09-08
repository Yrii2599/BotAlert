using System.Threading.Tasks;
using BotAlert.Models;
using BotAlert.Helpers;
using BotAlert.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotAlert.States
{
    public class InputDescriptionKeyboardState : IState
    {
        private readonly IStateProvider _stateProvider;
        private readonly ILocalizerFactory _localizerFactory;

        public InputDescriptionKeyboardState(IStateProvider stateProvider, ILocalizerFactory localizerFactory)
        {
            _stateProvider = stateProvider;
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

            if (callbackQuery.Data == "да")
            {
                return await HandleAcceptInput();
            }

            return await HandleDeclineInput();
        }

        public void BotSendMessage(ITelegramBotClient botClient, long chatId)
        {
            var localizer = _localizerFactory.GetLocalizer(_stateProvider.GetChatState(chatId).Language);

            InteractionHelper.SendInlineKeyboard(botClient, chatId, 
                localizer.GetMessage(MessageKeyConstants.WantToAddDescription), 
                localizer.GetInlineKeyboardMarkUp(MessageKeyConstants.YesOrNoMarkUp));
        }

        private async Task<ContextState> PrintMessage(ITelegramBotClient botClient, long chatId, string message)
        {
            await botClient.SendTextMessageAsync(chatId, message);

            return ContextState.InputDescriptionKeyboardState;
        }

        private Task<ContextState> HandleAcceptInput() => Task.FromResult(ContextState.InputDescriptionState);

        private Task<ContextState> HandleDeclineInput() => Task.FromResult(ContextState.SaveState);
    }
}
