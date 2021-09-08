using System.Threading.Tasks;
using BotAlert.Models;
using BotAlert.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using BotAlert.Helpers;

namespace BotAlert.States
{
    public class MainState : IState
    {
        private readonly IStateProvider _stateProvider;
        private readonly ILocalizerFactory _localizerFactory;

        public MainState(IStateProvider stateProvider, ILocalizerFactory localizerFactory)
        {
            _stateProvider = stateProvider;
            _localizerFactory = localizerFactory;
        }

        public async Task<ContextState> BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            if (message.Type != MessageType.Text)
            {
                return ContextState.MainState;
            }

            var localizer = _localizerFactory.GetLocalizer(_stateProvider.GetChatState(message.Chat.Id).Language);

            return message.Text switch
            {
                "/start" => await PrintMessage(botClient, 
                                               message.Chat.Id, 
                                               $"{localizer.GetMessage(MessageKeyConstants.Start)}, {message.Chat.FirstName}!", 
                                               ContextState.InputTimeZoneState),
                "/create" => ContextState.InputTitleState,
                "/get_notifications" => ContextState.GetAllNotificationsState,
                "/set_time_zone" => ContextState.InputTimeZoneState,
                "/set_language" => ContextState.InputLanguageKeyboardState,

                _ => await PrintMessage(botClient, message.Chat.Id, localizer.GetMessage(MessageKeyConstants.InvalidChoiceInput), ContextState.MainState)
            };
        }

        public Task<ContextState> BotOnCallBackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
            => Task.FromResult(ContextState.MainState);

        public void BotSendMessage(ITelegramBotClient botClient, long chatId)
        {
            var localizer = _localizerFactory.GetLocalizer(_stateProvider.GetChatState(chatId).Language);

            botClient.SendTextMessageAsync(chatId, localizer.GetMessage(MessageKeyConstants.CommandChoicePanel));
        }

        private async Task<ContextState> PrintMessage(ITelegramBotClient botClient, long chatId, string message, ContextState returnState)
        {
            await botClient.SendTextMessageAsync(chatId, message);

            return returnState;
        }
    }
}
