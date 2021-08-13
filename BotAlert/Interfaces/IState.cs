using System.Threading.Tasks;
using BotAlert.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotAlert.Interfaces 
{
    public interface IState 
    {
        Task<ContextState> BotOnMessageReceived(ITelegramBotClient botClient, Message message);

        Task<ContextState> BotOnCallBackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery);

        void BotSendMessage(ITelegramBotClient botClient, long chatId);
    }
}
