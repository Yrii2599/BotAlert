using System.Threading.Tasks;
using BotAlert.Interfaces;
using BotAlert.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotAlert.States
{
    public class UserInputTitleState : IState
    {
        private readonly IEventProvider _eventProvider;

        public UserInputTitleState(IEventProvider eventProvider)
        {
            _eventProvider = eventProvider;
        }

        public async Task<ContextState> BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            if (message.Text == null) return HandleInvalidInput(botClient, message.Chat.Id);

            _eventProvider.CreateEvent(new Event(message.Chat.Id, message.Text));
            return ContextState.UserInputDateState;
        }

        public async Task<ContextState> BotOnCallBackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            await botClient.AnswerCallbackQueryAsync(callbackQueryId: callbackQuery.Id);
            return ContextState.UserInputTitleState;
        }

        public void BotSendMessage(ITelegramBotClient botClient, long chatId)
        {
            botClient.SendTextMessageAsync(chatId, $"Введите название события:");
        }

        public ContextState HandleInvalidInput(ITelegramBotClient botClient, long chatId)
        {
            botClient.SendTextMessageAsync(chatId, "Неверный формат сообщения");
            return ContextState.UserInputTitleState;
        }
    }
}
