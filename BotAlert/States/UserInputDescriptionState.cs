using System.Threading.Tasks;
using BotAlert.Interfaces;
using BotAlert.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotAlert.States
{
    public class UserInputDescriptionState : IState
    {
        private readonly IEventProvider _eventProvider;

        public UserInputDescriptionState(IEventProvider eventProvider)
        {
            _eventProvider = eventProvider;
        }

        public async Task<ContextState> BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            var eventObj = _eventProvider.GetDraftEventByChatId(message.Chat.Id);
            eventObj.Description = message.Text;
            eventObj.Status = EventStatus.Created;
            _eventProvider.UpdateEvent(eventObj);
            return ContextState.MainState;
        }

        public void BotSendMessage(ITelegramBotClient botClient, long chatId)
        {
            botClient.SendTextMessageAsync(chatId, $"Введите описание оповещения:");
        }
    }
}
