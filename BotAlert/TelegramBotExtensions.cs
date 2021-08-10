﻿using System.Threading;
using System.Threading.Tasks;
using BotAlert.Controllers;
using BotAlert.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;

namespace BotAlert
{
    public class TelegramBotExtensions
    {
        public static async Task StartListeningAsync(string apiKey, CancellationToken cancellationToken, ITelegramUpdatesHandler handler)
        {
            var Bot = new TelegramBotClient(apiKey);

            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            Bot.StartReceiving(new DefaultUpdateHandler(handler.HandleUpdateAsync, handler.HandleErrorAsync), cancellationToken);
        }
    }
}
