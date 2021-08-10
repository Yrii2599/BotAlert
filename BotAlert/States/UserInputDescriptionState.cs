using System.Threading.Tasks;
using BotAlert.Interfaces;
using BotAlert.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotAlert.States
{
    public class UserInputDescriptionState : IState
    {
        public Context ContextObj { get; set; }

        public async Task BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            return;

            //if (message.Type != MessageType.Text)
            //    return;

            //_eventObj.Description = message.Text;
            //_eventObj.Status = EventStatus.Created;
            //new EventProvider().UpdateEvent(_eventObj);
            //ContextObj.ChangeState(new MainState());
        }
    }
}
