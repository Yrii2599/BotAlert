using System.Threading.Tasks;
using BotAlert.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotAlert.States
{
    public class UserInputTitleState : IState
    {
        public Context ContextObj { get; set; }

        public async Task BotOnMessageReceived(ITelegramBotClient botClient, Message message)
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
