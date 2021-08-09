using System;
using System.Threading;
using System.Threading.Tasks;
using BotAlert.Controllers;
using BotAlert.Settings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;

namespace BotAlert
{
    public class Program
    {
        private static TelegramBotClient? Bot;

        public static async Task Main()
        {
            Bot = new TelegramBotClient(Settings.Settings.TelegramConnectionString);

            var me = await Bot.GetMeAsync();
            Console.Title = me.Username;

            using var cts = new CancellationTokenSource();

            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            Bot.StartReceiving(new DefaultUpdateHandler(BaseController.HandleUpdateAsync, BaseController.HandleErrorAsync),
                               cts.Token);

            Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();

            // Send cancellation request to stop bot
            cts.Cancel();

            // CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
