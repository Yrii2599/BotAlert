using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BotAlert.Helpers;
using BotAlert.Interfaces;
using BotAlert.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotAlert.States
{
    public class GetAllNotificationsState : IState
    {
        private readonly IEventProvider _eventProvider;
        private readonly IStateProvider _stateProvider;

        public GetAllNotificationsState(IEventProvider eventProvider, IStateProvider stateProvider)
        {
            _eventProvider = eventProvider;
            _stateProvider = stateProvider;
        }

        public async Task<ContextState> BotOnCallBackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            await botClient.AnswerCallbackQueryAsync(callbackQueryId: callbackQuery.Id);

            switch (callbackQuery.Data)
            {
                case "ToMain":
                    _stateProvider.ResetChatPage(callbackQuery.Message.Chat.Id);
                    return ContextState.MainState;
                    break;
                case "Prev":
                    _stateProvider.IncrementChatPage(callbackQuery.Message.Chat.Id, -1);
                    return ContextState.GetAllNotificationsState;
                    break;
                case "Next":
                    _stateProvider.IncrementChatPage(callbackQuery.Message.Chat.Id, 1);
                    return ContextState.GetAllNotificationsState;
                    break;
                default:
                    _stateProvider.UpdateCurrentlyViewingNotification(callbackQuery.Message.Chat.Id, callbackQuery.Data);
                    return ContextState.GetNotificationDetails;
                    break;
            }
        }

        public async Task<ContextState> BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            return HandleInvalidInput(botClient, message.Chat.Id, "Выберите один из вариантов");
        }

        public void BotSendMessage(ITelegramBotClient botClient, long chatId)
        {
            var options = new List<InlineKeyboardButton[]>();
            foreach(var eventObj in _eventProvider.GetUserEventsOnPage(chatId))
            {
                options.Add(new[] { InlineKeyboardButton.WithCallbackData(eventObj.Title, eventObj.Id.ToString()) });
            }
            var prevNextBtns = new List<InlineKeyboardButton>();
            if (_eventProvider.UserEventsPreviousPageExists(chatId)) {
                prevNextBtns.Add(InlineKeyboardButton.WithCallbackData("◀", "Prev"));
            }
            if (_eventProvider.UserEventsNextPageExists(chatId))
            {
                prevNextBtns.Add(InlineKeyboardButton.WithCallbackData("▶", "Next"));
            }
            options.Add(prevNextBtns.ToArray());
            options.Add(new[] { InlineKeyboardButton.WithCallbackData("Back to menu", "ToMain") });
            InteractionHelper.SendInlineKeyboard(botClient, chatId, "\nВыберите событие:", options.ToArray());
        }

        public ContextState HandleInvalidInput(ITelegramBotClient botClient, long chatId, string message)
        {
            botClient.SendTextMessageAsync(chatId, message);
            return ContextState.GetAllNotificationsState;
        }
    }
}
