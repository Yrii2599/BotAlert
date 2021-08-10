using System.Threading.Tasks;
using BotAlert.Models;
using BotAlert.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotAlert.States
{
    public class UserInputTitleState : State
    {
        private Event _eventObj { get; set; }

        public UserInputTitleState(Event eventObj)
        {
            _eventObj = eventObj;
        }

        public override async Task BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            return;

            //if (message.Type != MessageType.Text
            // || _eventObj.Status == EventStatus.Deleted)
            //    return;

            //_eventObj.Title = message.Text;

            //if (_eventObj.Status == EventStatus.InProgress) {
            //    new EventProvider().CreateEvent(_eventObj);
            //    botClient.SendTextMessageAsync(message.Chat.Id, "Введите дату события: ");
            //    ContextObj.ChangeState(new UserInputDateState(_eventObj));
            //}
            //else ContextObj.ChangeState(new MainState());
        }
    }
}
