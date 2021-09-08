﻿using System;
using System.Globalization;
using System.Threading.Tasks;
using BotAlert.Models;
using BotAlert.Helpers;
using BotAlert.Settings;
using BotAlert.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotAlert.States
{
    public class InputDateState : IState
    {
        private readonly IEventProvider _eventProvider;
        private readonly IStateProvider _stateProvider;
        private readonly ILocalizerFactory _localizerFactory;

        public InputDateState(IEventProvider eventProvider, IStateProvider stateProvider, ILocalizerFactory localizerFactory)
        {
            _eventProvider = eventProvider;
            _stateProvider = stateProvider;
            _localizerFactory = localizerFactory;
        }

        public async Task<ContextState> BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            var chat = _stateProvider.GetChatState(message.Chat.Id);

            var localizer = _localizerFactory.GetLocalizer(chat.Language);

            if (message.Text == null || !DateTime.TryParse(message.Text, 
                                                          new CultureInfo(TelegramSettings.DateTimeFormat), 
                                                          DateTimeStyles.None, 
                                                          out var date))
            {
                return await PrintMessage(botClient, message.Chat.Id, localizer.GetMessage(MessageKeyConstants.InvalidDateInput));
            }

            var eventObj = _eventProvider.GetEventById(chat.ActiveNotificationId);

            if (eventObj == null)
            {
                chat.ActiveNotificationId = Guid.Empty;
                _stateProvider.SaveChatState(chat);
                await botClient.SendTextMessageAsync(chat.ChatId, localizer.GetMessage(MessageKeyConstants.ExpiredDate));

                return ContextState.MainState;
            }

            if (date < DateTime.UtcNow.AddHours(eventObj.TimeOffSet)) 
            { 
                return await PrintMessage(botClient, message.Chat.Id, localizer.GetMessage(MessageKeyConstants.ExpiredDate));
            }

            eventObj.Date = date.AddHours(-eventObj.TimeOffSet).TrimSecondsAndMilliseconds();

            _eventProvider.UpdateEvent(eventObj);

            return ContextState.InputWarnDateKeyboardState;
        }

        public async Task<ContextState> BotOnCallBackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            await botClient.AnswerCallbackQueryAsync(callbackQueryId: callbackQuery.Id);

            return ContextState.InputDateState;
        }

        public void BotSendMessage(ITelegramBotClient botClient, long chatId)
        {
            var localizer = _localizerFactory.GetLocalizer(_stateProvider.GetChatState(chatId).Language);

            botClient.SendTextMessageAsync(chatId, localizer.GetMessage(MessageKeyConstants.EnterDate));
        }

        private async Task<ContextState> PrintMessage(ITelegramBotClient botClient, long chatId, string message)
        {
            await botClient.SendTextMessageAsync(chatId, message);

            return ContextState.InputDateState;
        }
    }
}
