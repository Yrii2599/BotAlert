using System.Threading.Tasks;
using BotAlert.Models;
using BotAlert.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotAlert.States
{
    public class UserInputDescriptionState : State
    {
        private Event _eventObj { get; set; }

        public UserInputDescriptionState(Event eventObj)
        {
            _eventObj = eventObj;
        }

        public override async Task BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            if (message.Type != MessageType.Text)
                return;

            _eventObj.Description = message.Text;
            EventDBService.CreateEvent(_eventObj);
            ContextObj.ChangeState(new MainState());
        }
    }
}
