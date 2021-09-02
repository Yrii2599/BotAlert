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
    public class InputWarnDateState : IState
    {
        private readonly IEventProvider _eventProvider;
        private readonly IStateProvider _stateProvider;

        public InputWarnDateState(IEventProvider eventProvider, IStateProvider stateProvider)
        {
            _eventProvider = eventProvider;
            _stateProvider = stateProvider;
        }

        public async Task<ContextState> BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            if (message.Text == null || !DateTime.TryParse(message.Text,
                                                          new CultureInfo(TelegramSettings.DateTimeFormat),
                                                          DateTimeStyles.None,
                                                          out var warnDate))
            {
                return await PrintMessage(botClient, message.Chat.Id, "Неверный формат даты и времени");
            }

            var chat = _stateProvider.GetChatState(message.Chat.Id);
            var eventObj = _eventProvider.GetEventById(chat.ActiveNotificationId);

            if (eventObj == null)
            {
                chat.ActiveNotificationId = Guid.Empty;
                _stateProvider.SaveChatState(chat);
                await botClient.SendTextMessageAsync(chat.ChatId, "Данное событие уже произошло");

                return ContextState.MainState;
            }

            if (warnDate < DateTime.UtcNow.AddHours(chat.TimeOffSet))
            {
                return await PrintMessage(botClient, message.Chat.Id, "Оповещение уже произошло");
            }

            if (eventObj.Date < DateTime.UtcNow.AddHours(chat.TimeOffSet))
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Ваше событие уже прошло, введите новое значение");

                return ContextState.InputDateState;
            }

            if (warnDate > eventObj.Date)
            {
                 return await PrintMessage(botClient, message.Chat.Id, "Оповещение не может прийти после события");
            }

            eventObj.WarnDate = warnDate.TrimSecondsAndMilliseconds();

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

        public async Task<ContextState> BotOnCallBackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            await botClient.AnswerCallbackQueryAsync(callbackQueryId: callbackQuery.Id);

            return ContextState.InputWarnDateState;
        }

        public void BotSendMessage(ITelegramBotClient botClient, long chatId)
        {
            botClient.SendTextMessageAsync(chatId, $"Введите дату и время для оповещения\n(DD.MM.YYYY HH:MM):");
        }

        private async Task<ContextState> PrintMessage(ITelegramBotClient botClient, long chatId, string message)
        {
            await botClient.SendTextMessageAsync(chatId, message);

            return ContextState.InputWarnDateState;
        }
    }
}
