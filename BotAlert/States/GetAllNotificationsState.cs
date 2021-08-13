using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using BotAlert.Models;
using BotAlert.Helpers;
using BotAlert.Interfaces;
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

            var chat = _stateProvider.GetChatState(callbackQuery.Message.Chat.Id);
            var res = ContextState.GetNotificationDetails;

            switch (callbackQuery.Data)
            {
                case "ToMain":
                    chat.NotificationsPage = 0;
                    res = ContextState.MainState;
                    break;
                case "Prev":
                    chat.NotificationsPage--;
                    res = ContextState.GetAllNotificationsState;
                    break;
                case "Next":
                    chat.NotificationsPage++;
                    res = ContextState.GetAllNotificationsState;
                    break;
                default:
                    chat.ActiveNotificationId = Guid.Parse(callbackQuery.Data);
                    break;
            }

            _stateProvider.SaveChatState(chat);

            return res;
        }

        public async Task<ContextState> BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            return PrintMessage(botClient, message.Chat.Id, "Выберите один из вариантов");
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

        private ContextState PrintMessage(ITelegramBotClient botClient, long chatId, string message)
        {
            botClient.SendTextMessageAsync(chatId, message);
            return ContextState.GetAllNotificationsState;
        }
    }
}
