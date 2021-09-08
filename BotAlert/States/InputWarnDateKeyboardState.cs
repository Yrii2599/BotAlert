using System.Threading.Tasks;
using BotAlert.Models;
using BotAlert.Helpers;
using BotAlert.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using System;

namespace BotAlert.States
{
    public class InputWarnDateKeyboardState : IState
    {
        private readonly IEventProvider _eventProvider;
        private readonly IStateProvider _stateProvider;
        private readonly ILocalizerFactory _localizerFactory;

        public InputWarnDateKeyboardState(IEventProvider eventProvider, IStateProvider stateProvider, ILocalizerFactory localizerFactory)
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

            if (callbackQuery.Data == "own")
            {
                return ContextState.InputWarnDateState;
            }

            return await HandleWarnDateOptions(botClient, callbackQuery.Message.Chat.Id, callbackQuery.Data);
        }

        public void BotSendMessage(ITelegramBotClient botClient, long chatId)
        {
            var localizer = _localizerFactory.GetLocalizer(_stateProvider.GetChatState(chatId).Language);

            InteractionHelper.SendInlineKeyboard(botClient, chatId, 
                localizer.GetMessage(MessageKeyConstants.WhenToRemind), 
                localizer.GetInlineKeyboardMarkUp(MessageKeyConstants.WarnDateMarkUp));
        }

        private async Task<ContextState> HandleWarnDateOptions(ITelegramBotClient botClient, long chatId, string warnInMinutes)
        {
            var minutes = int.Parse(warnInMinutes);

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


            if (eventObj.Date < DateTime.UtcNow)

            {
                await botClient.SendTextMessageAsync(chatId, localizer.GetMessage(MessageKeyConstants.ExpiredDate));

                return ContextState.InputDateState;
            }

            if(eventObj.Date.AddMinutes(-minutes) < DateTime.UtcNow)

            {
                return await PrintMessage(botClient, chatId, localizer.GetMessage(MessageKeyConstants.ExpiredWarnDate));
            }

            eventObj.WarnDate = eventObj.Date.AddMinutes(-minutes);

            _eventProvider.UpdateEvent(eventObj);

            if (eventObj.Status == EventStatus.InProgress)
            {
                return ContextState.InputDescriptionKeyboardState;
            }
            else
            {
                return ContextState.EditState;
            }
        }

        private async Task<ContextState> PrintMessage(ITelegramBotClient botClient, long chatId, string message)
        {
            await botClient.SendTextMessageAsync(chatId, message);

            return ContextState.InputWarnDateKeyboardState;
        }
    }
}
