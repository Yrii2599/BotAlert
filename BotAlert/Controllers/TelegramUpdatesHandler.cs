﻿using System;
using System.Threading;
using System.Threading.Tasks;
using BotAlert.Interfaces;
using BotAlert.Models;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotAlert.Controllers
{
    public class TelegramUpdatesHandler : ITelegramUpdatesHandler
    {
        private readonly IStateProvider _stateProvider;
        private readonly IStateFactory _stateFactory;

        public TelegramUpdatesHandler(IStateProvider stateProvider, IStateFactory stateFactory)
        {
            _stateProvider = stateProvider;
            _stateFactory = stateFactory;
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Message == null && update.CallbackQuery == null)
            {
                return;
            }

            var chatId = update.Message != null ? update.Message.Chat.Id : update.CallbackQuery.Message.Chat.Id;
            var state = _stateProvider.GetChatState(chatId);

            var handler = update.Type switch
            {
                UpdateType.Message => state.BotOnMessageReceived(botClient, update.Message),
                UpdateType.EditedMessage => state.BotOnMessageReceived(botClient, update.Message),

                UpdateType.CallbackQuery => state.BotOnCallBackQueryReceived(botClient, update.CallbackQuery),

                // Заменить на IState.HandleInvalidInput()
                _ => UnknownUpdateHandlerAsync(botClient, update)
            };

            try
            {
                var nextState = await handler;
                _stateProvider.SaveChatState(new ChatState(chatId, nextState));
                _stateFactory.GetState(nextState).BotSendMessage(botClient, chatId);
            }
            catch (Exception exception)
            {
                await HandleErrorAsync(botClient, exception, cancellationToken);
            }
        }

        public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);

            return Task.CompletedTask;
        }

        private Task<ContextState> UnknownUpdateHandlerAsync(ITelegramBotClient botClient, Update update)
        {
            botClient.SendTextMessageAsync(update.Message.Chat.Id, "Sorry, something went worng, please try again later!");

            return Task.Run(() => ContextState.MainState);
        }
    }
}
