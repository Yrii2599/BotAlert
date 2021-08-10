using System;
using System.Threading.Tasks;
using BotAlert.Models;
using BotAlert.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotAlert.States
{
    public class UserInputDateState : State
    {
        private Event _eventObj { get; set; }

        public UserInputDateState(Event eventObj)
        {
            _eventObj = eventObj;

        }

        public override async Task BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            if (message.Type != MessageType.Text)
                return;

            _eventObj.Date = DateTime.Parse(message.Text);

            if (_eventObj.Status == EventStatus.InProgress) {
                new EventDBService().UpdateEvent(_eventObj);
                botClient.SendTextMessageAsync(message.Chat.Id, "Введите дату оповещения: ");
                ContextObj.ChangeState(new UserInputWarnDateState(_eventObj));
            }
            else ContextObj.ChangeState(new MainState());
        }
    }
}
