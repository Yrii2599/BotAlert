using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotAlert.Models;
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
            if (message.Type != MessageType.Text
             || _eventObj.Status == EventStatus.Deleted)
                return;

            _eventObj.Title = message.Text;
            if (_eventObj.Status == EventStatus.InProgress) {
                botClient.SendTextMessageAsync(message.Chat.Id, "Введите дату события: ");
                _contextObj.ChangeState(new UserInputDateState(_eventObj));
            }
            else _contextObj.ChangeState(new MainState());
        }
    }
}
