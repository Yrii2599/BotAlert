using System;
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

        public TelegramUpdatesHandler(IStateProvider stateProvider)
        {
            _stateProvider = stateProvider;
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Message == null) return;

            var state = _stateProvider.GetOrCreateChatState(update.Message.Chat.Id);

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
                _stateProvider.CreateOrUpdateChat(new ChatState(update.Message.Chat.Id, nextState));
                // Изменить
                _stateProvider.GetOrCreateChatState(update.Message.Chat.Id).BotSendMessage(botClient, update.Message.Chat.Id);
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
