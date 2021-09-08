using System;
using System.Threading.Tasks;
using BotAlert.Helpers;
using Telegram.Bot;
using Telegram.Bot.Types;
using BotAlert.Interfaces;
using BotAlert.Models;

namespace BotAlert.States
{
    public class InputTimeZoneState : IState
    {
        private readonly IStateProvider _stateProvider;
        private readonly IEventProvider _eventProvider;
        private readonly ILocalizerFactory _localizerFactory;

        public InputTimeZoneState(IStateProvider stateProvider, IEventProvider eventProvider, ILocalizerFactory localizerFactory)
        {
            _stateProvider = stateProvider;
            _eventProvider = eventProvider;
            _localizerFactory = localizerFactory;
        }

    public async Task<ContextState> BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            var chat = _stateProvider.GetChatState(message.Chat.Id);

            var localizer = _localizerFactory.GetLocalizer(chat.Language);

            if (message.Text == null || !int.TryParse(message.Text, out var timeOffSet) || timeOffSet < -12 || timeOffSet > 14)
            {
                return await PrintMessage(botClient, message.Chat.Id, localizer.GetMessage(MessageKeyConstants.InvalidTextInput));
            }

            if (chat.ActiveNotificationId == Guid.Empty)
            {
                chat.TimeOffSet = timeOffSet;
                _stateProvider.SaveChatState(chat);

                return ContextState.MainState;
            }

            var eventObj = _eventProvider.GetEventById(chat.ActiveNotificationId);
            
            if (eventObj == null)
            {
                chat.ActiveNotificationId = Guid.Empty;
                _stateProvider.SaveChatState(chat);
                await botClient.SendTextMessageAsync(chat.ChatId, localizer.GetMessage(MessageKeyConstants.ExpiredDate));

                return ContextState.MainState;
            }

            eventObj.TimeOffSet = timeOffSet;
            _eventProvider.UpdateEvent(eventObj);

            return ContextState.InputDateState;

        }

        public async Task<ContextState> BotOnCallBackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            await botClient.AnswerCallbackQueryAsync(callbackQueryId: callbackQuery.Id);

            var localizer = _localizerFactory.GetLocalizer(_stateProvider.GetChatState(callbackQuery.Message.Chat.Id).Language);

            return await PrintMessage(botClient, callbackQuery.Message.Chat.Id, localizer.GetMessage(MessageKeyConstants.EnterTimeZone));

        }

        public void BotSendMessage(ITelegramBotClient botClient, long chatId)
        {
            var chat = _stateProvider.GetChatState(chatId);
            var localizer = _localizerFactory.GetLocalizer(chat.Language);

            Task.FromResult(botClient.SendTextMessageAsync(chatId, 
                localizer.GetTimeZone(chat.TimeOffSet)+
                "****************************\n" +
                localizer.GetMessage(MessageKeyConstants.EnterTimeZone)));

        }

        private async Task<ContextState> PrintMessage(ITelegramBotClient botClient, long chatId, string message)
        {
            await botClient.SendTextMessageAsync(chatId, message);

            return ContextState.InputTimeZoneState;
        }
    }
}
