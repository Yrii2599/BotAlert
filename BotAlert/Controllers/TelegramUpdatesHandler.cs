using System;
using System.Threading;
using System.Threading.Tasks;
using BotAlert.Interfaces;
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
            var context = _stateProvider.GetOrCreateChatContext(update.Message.Chat.Id);

            var handler = update.Type switch
            {
                UpdateType.Message => context.State.BotOnMessageReceived(botClient, update.Message),
                UpdateType.EditedMessage => context.State.BotOnMessageReceived(botClient, update.Message),

                UpdateType.CallbackQuery => context.State.BotOnCallBackQueryReceived(botClient, update.CallbackQuery),

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

        private Task UnknownUpdateHandlerAsync(ITelegramBotClient botClient, Update update)
        {
            botClient.SendTextMessageAsync(update.Message.Chat.Id, "Sorry, something went worng, please try again later!");

            return Task.CompletedTask;
        }
    }
}
