using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotAlert.States
{
    public class MainState : State
    {
        public override async Task BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            if (message.Type != MessageType.Text)
                return;

            switch (message.Text)
            {
                case "/create":
                    //CreateNotification(botClient, message);
                    break;
                case "/get_notifications":
                    //GetAllNotifications(botClient, message);
                    break;
                case "/get_notification":
                    //GetNotification(botClient, message);
                    break;
                case "/edit":
                    //EditNotification(botClient, message);
                    break;
                case "/delete":
                    //DeleteNotification(botClient, message);
                    break;
                default:
                    //await HandleInputError(botClient, message);
                    break;
            }

            botClient.SendTextMessageAsync(message.Chat.Id, message.Text);
        }
    }
}
