using System.Threading.Tasks;
using BotAlert.States;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotAlert.Interfaces
{
    public interface IState
    {
        Context ContextObj { get; set; }
        async Task BotOnMessageReceived(ITelegramBotClient botClient, Message message) { }
        async Task BotOnCallBackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery) { }
    }
}
