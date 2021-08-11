using System.Threading.Tasks;
using BotAlert.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotAlert.Interfaces {
    public interface IState {
        void BotSendMessage(ITelegramBotClient botClient, long chatId);
        ContextState HandleInvalidInput(ITelegramBotClient botClient, long chatId);

        //Можно ли убрать async
        async Task<ContextState> BotOnMessageReceived(ITelegramBotClient botClient, Message message) {
            return ContextState.MainState;
        }

        async Task<ContextState> BotOnCallBackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery) {
            return ContextState.MainState;
        }
    }
}
