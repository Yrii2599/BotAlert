using System.Threading.Tasks;
using BotAlert.Interfaces;
using BotAlert.Helpers;
using BotAlert.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotAlert.States
{
    public class InputLanguageKeyboardState : IState
    {
        private readonly IStateProvider _stateProvider;
        private readonly ILocalizerFactory _localizerFactory;

        public InputLanguageKeyboardState(IStateProvider stateProvider, ILocalizerFactory localizerFactory)
        {
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

            var chat = _stateProvider.GetChatState(callbackQuery.Message.Chat.Id);

            switch(callbackQuery.Data)
            {
                case "English": 
                    chat.Language = LanguageType.English;
                    break;

                case "Russian":
                    chat.Language = LanguageType.Russian;
                    break;

                default:
                    return await PrintMessage(botClient, 
                        callbackQuery.Message.Chat.Id, 
                        _localizerFactory.GetLocalizer(chat.Language).GetMessage(MessageKeyConstants.InvalidChoiceInput));
            }

            _stateProvider.SaveChatState(chat);

            var localizer = _localizerFactory.GetLocalizer(chat.Language);
            botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, localizer.GetMessage(MessageKeyConstants.CurrentLanguage));

            return ContextState.MainState;
        }

        public void BotSendMessage(ITelegramBotClient botClient, long chatId)
        {
            var chat = _stateProvider.GetChatState(chatId);

            var localizer = _localizerFactory.GetLocalizer(chat.Language);

            var options = new InlineKeyboardMarkup(new[] { InlineKeyboardButton.WithCallbackData("English", "English"),
                                                           InlineKeyboardButton.WithCallbackData("Русский", "Russian")});

            InteractionHelper.SendInlineKeyboard(botClient, chatId, localizer.GetMessage(MessageKeyConstants.EnterLanguage), options);
        }

        private async Task<ContextState> PrintMessage(ITelegramBotClient botClient, long chatId, string message)
        {
            await botClient.SendTextMessageAsync(chatId, message);

            return ContextState.InputLanguageKeyboardState;
        }
    }
}
