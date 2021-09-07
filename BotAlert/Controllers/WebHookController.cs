using System;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BotAlert.Interfaces;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotAlert.Controllers
{
    [Route("[controller]")]
    public class WebHookController : ControllerBase
    {
        private readonly ITelegramBotClient _botClient;
        private readonly ITelegramUpdatesHandler _telegramUpdateService;

        public WebHookController(ITelegramBotClient botClient, ITelegramUpdatesHandler telegramUpdateService)
        {
            _botClient = botClient;
            _telegramUpdateService = telegramUpdateService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Update update)
        {
            try
            {
                var cts = new CancellationTokenSource();
                await _telegramUpdateService.HandleUpdateAsync(_botClient, update, cts.Token);
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
            }

            return Ok();
        }
    }
}
