using System.Threading.Tasks;
using BotAlert.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotAlert.Interfaces {
    public interface IState {
        void BotSendMessage(ITelegramBotClient botClient, long chatId);

        async Task<ContextState> BotOnMessageReceived(ITelegramBotClient botClient, Message message) {
            return ContextState.MainState;
        }

        async Task<ContextState> BotOnCallBackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery) {
            return ContextState.MainState;
        }
    }
}
