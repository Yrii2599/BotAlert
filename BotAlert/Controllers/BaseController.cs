﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BotAlert.Service;
using BotAlert.States;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotAlert.Controllers
{

    public class BaseController
    {
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var stateDBService = new StateProvider(new MongoClient("").GetDatabase(""));
            var context = stateDBService.GetChatContext(update.Message.Chat.Id);
            
            if(context == null)
            {
                stateDBService.CreateChat(new Models.ChatState(update.Message.Chat.Id));
                context = new Context(new MainState());
            }

            var handler = update.Type switch
            {
                // UpdateType.Unknown:
                // UpdateType.ChannelPost:
                // UpdateType.EditedChannelPost:
                // UpdateType.ShippingQuery:
                // UpdateType.PreCheckoutQuery:
                // UpdateType.Poll:
                UpdateType.Message => context.State.BotOnMessageReceived(botClient, update.Message),
                // UpdateType.EditedMessage => BotOnMessageReceived(botClient, update.EditedMessage),
                UpdateType.CallbackQuery => context.State.BotOnCallBackQueryReceived(botClient, update.CallbackQuery),
                // UpdateType.InlineQuery => BotOnInlineQueryReceived(botClient, update.InlineQuery),
                // UpdateType.ChosenInlineResult => BotOnChosenInlineResultReceived(botClient, update.ChosenInlineResult),
                _ => UnknownUpdateHandlerAsync(botClient, update)
            };

            try
            {
                await handler;
            }
            catch (Exception exception)
            {
                await HandleErrorAsync(botClient, exception, cancellationToken);
            }
        }

        private static Task UnknownUpdateHandlerAsync(ITelegramBotClient botClient, Update update)
        {
            botClient.SendTextMessageAsync(update.Message.Chat.Id, "Sorry, something went worng, please try again later!");
            return Task.CompletedTask;
        }

        public static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
    }
}
