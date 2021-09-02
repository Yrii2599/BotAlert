using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using BotAlert.Interfaces;
using BotAlert.Models;

namespace BotAlert.States
{
    public class InputTimeZoneState : IState
    {
        private readonly IStateProvider _stateProvider;

        public InputTimeZoneState(IStateProvider stateProvider)
        {
            _stateProvider = stateProvider;
        }

        public async Task<ContextState> BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            if (message.Text == null || !int.TryParse(message.Text, out var timeOffSet) || timeOffSet < -12 || timeOffSet > 14)
            {
                return await PrintMessage(botClient, message.Chat.Id, "Неправильный формат ввода");
            }

            var chat = _stateProvider.GetChatState(message.Chat.Id);
            chat.TimeOffSet = timeOffSet;
            _stateProvider.SaveChatState(chat);

            return ContextState.MainState;
        }

        public async Task<ContextState> BotOnCallBackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            await botClient.AnswerCallbackQueryAsync(callbackQueryId: callbackQuery.Id);

            return await PrintMessage(botClient, callbackQuery.Message.Chat.Id, "");
        }

        public void BotSendMessage(ITelegramBotClient botClient, long chatId)
        {

            Task.FromResult(botClient.SendTextMessageAsync(chatId, 
                $"Текущий часовой пояс: UTC {_stateProvider.GetChatState(chatId).TimeOffSet}:00 \n" +
                $"****************************\n" +
                $"Введите часовой новый пояс (от -12 до +14):"));
        }

        private async Task<ContextState> PrintMessage(ITelegramBotClient botClient, long chatId, string message)
        {
            await botClient.SendTextMessageAsync(chatId, message);

            return ContextState.InputTimeZoneState;
        }
    }
}
