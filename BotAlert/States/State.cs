using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotAlert.States
{
    public abstract class State
    {
        public Context ContextObj { get; set; }
        public virtual async Task BotOnMessageReceived(ITelegramBotClient botClient, Message message) { }
        public virtual async Task BotOnCallBackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery) { }
    }
}