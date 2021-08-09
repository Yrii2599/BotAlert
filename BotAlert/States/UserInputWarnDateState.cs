﻿using System;
using System.Threading.Tasks;
using BotAlert.Helpers;
using BotAlert.Models;
using BotAlert.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotAlert.States
{
    public class UserInputWarnDateState : State
    {
        private Event _eventObj { get; set; }

        public UserInputWarnDateState(Event eventObj)
        {
            _eventObj = eventObj;
        }

        public override async Task BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            if (message.Type != MessageType.Text)
                return;

            _eventObj.Date = DateTime.Parse(message.Text);
            if (_eventObj.Status == EventStatus.InProgress) {
                var options = new InlineKeyboardMarkup(new[] { new[] { InlineKeyboardButton.WithCallbackData("Да", "y"),
                                                                       InlineKeyboardButton.WithCallbackData("Нет", "n") } }); ;
                InteractionHelper.SendInlineKeyboard(botClient, message, "Хотите добавить описание ?" , options);
            } else ContextObj.ChangeState(new MainState());
        }

        public override async Task BotOnCallBackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery) {

            await botClient.AnswerCallbackQueryAsync(callbackQueryId: callbackQuery.Id);

            if (callbackQuery.Data == "y") {
                botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Введите описнаие события:");
                ContextObj.ChangeState(new UserInputDescriptionState(_eventObj)); 
            }
            else
            {
                EventDBService.CreateEvent(_eventObj);
                ContextObj.ChangeState(new MainState());
            }
        }

    }
}
