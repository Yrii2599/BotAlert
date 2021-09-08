using System;
using System.Linq;
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
        private readonly ILocalizerFactory _localizerFactory;

        public GetAllNotificationsState(IEventProvider eventProvider, IStateProvider stateProvider, ILocalizerFactory localizerFactory)
        {
            _eventProvider = eventProvider;
            _stateProvider = stateProvider;
            _localizerFactory = localizerFactory;
        }

        public async Task<ContextState> BotOnCallBackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            await botClient.AnswerCallbackQueryAsync(callbackQueryId: callbackQuery.Id);

            var chat = _stateProvider.GetChatState(callbackQuery.Message.Chat.Id);
            var res = ContextState.GetAllNotificationsState;

            switch (callbackQuery.Data)
            {
                case "Back":
                    chat.NotificationsPage = 0;
                    res = ContextState.MainState;

                    break;

                case "Prev":
                    if (_eventProvider.UserEventsPreviousPageExists(callbackQuery.Message.Chat.Id))
                    {
                        chat.NotificationsPage--;
                    }

                    break;

                case "Next":
                    if (_eventProvider.UserEventsNextPageExists(callbackQuery.Message.Chat.Id))
                    {
                        chat.NotificationsPage++;
                    }

                    break;

                default:
                    var eventId = Guid.Parse(callbackQuery.Data);

                    if (_eventProvider.GetEventById(eventId) != null)
                    {
                        chat.ActiveNotificationId = eventId;
                        res = ContextState.GetNotificationDetailsState;
                    }

                    break;
            }

            _stateProvider.SaveChatState(chat);

            return res;
        }

        public async Task<ContextState> BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            var localizer = _localizerFactory.GetLocalizer(_stateProvider.GetChatState(message.Chat.Id).Language);

            return await PrintMessage(botClient, message.Chat.Id, localizer.GetMessage(MessageKeyConstants.InvalidChoiceInput));
        }

        public void BotSendMessage(ITelegramBotClient botClient, long chatId)
        {
            var options = new List<InlineKeyboardButton[]>();

            var chat = _stateProvider.GetChatState(chatId);
            var events = _eventProvider.GetUserEventsOnPage(chatId);

            var localizer = _localizerFactory.GetLocalizer(chat.Language);

            if(!events.Any())
            {
                if (chat.NotificationsPage == 0)
                {
                    botClient.SendTextMessageAsync(chatId, localizer.GetMessage(MessageKeyConstants.NoEvents));
                    chat.State = ContextState.MainState;
                    _stateProvider.SaveChatState(chat);

                    return;
                }
                else
                {
                    chat.NotificationsPage--;
                    _stateProvider.SaveChatState(chat);
                    events = _eventProvider.GetUserEventsOnPage(chatId);
                }
            }

            foreach (var eventObj in events)
            {
                options.Add(new[] { InlineKeyboardButton.WithCallbackData(eventObj.Title, eventObj.Id.ToString()) });
            }

            var prevNextBtns = new List<InlineKeyboardButton>();

            if (_eventProvider.UserEventsPreviousPageExists(chatId)) 
            {
                prevNextBtns.Add(InlineKeyboardButton.WithCallbackData("◀", "Prev"));
            }

            if (_eventProvider.UserEventsNextPageExists(chatId))
            {
                prevNextBtns.Add(InlineKeyboardButton.WithCallbackData("▶", "Next"));
            }

            options.Add(prevNextBtns.ToArray());
            options.Add(new[] { InlineKeyboardButton.WithCallbackData(localizer.GetMessage(MessageKeyConstants.Back), "Back") });

            InteractionHelper.SendInlineKeyboard(botClient, chatId, localizer.GetMessage(MessageKeyConstants.ChooseEvent), options.ToArray());
        }

        private async Task<ContextState> PrintMessage(ITelegramBotClient botClient, long chatId, string message)
        {
            await botClient.SendTextMessageAsync(chatId, message);

            return ContextState.GetAllNotificationsState;
        }
    }
}
