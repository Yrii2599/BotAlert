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
            _eventProvider.CreateEvent(new Event(message.Chat.Id, message.Text));
            return ContextState.UserInputDateState;
        }

        public void BotSendMessage(ITelegramBotClient botClient, long chatId)
        {
            botClient.SendTextMessageAsync(chatId, $"Введите название события:");
        }
    }
}
