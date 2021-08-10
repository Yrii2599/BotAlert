using System.Threading.Tasks;
using BotAlert.Interfaces;
using BotAlert.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotAlert.States
{
    public class UserInputDateState : IState
    {
        public Context ContextObj { get; set; }

        public async Task BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            return;

            //if (message.Type != MessageType.Text)
            //    return;

            //_eventObj.Date = DateTime.Parse(message.Text);

            //if (_eventObj.Status == EventStatus.InProgress) {
            //    new EventProvider().UpdateEvent(_eventObj);
            //    botClient.SendTextMessageAsync(message.Chat.Id, "Введите дату оповещения: ");
            //    ContextObj.ChangeState(new UserInputWarnDateState(_eventObj));
            //}
            //else ContextObj.ChangeState(new MainState());
        }
    }
}
