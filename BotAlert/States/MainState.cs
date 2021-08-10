using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotAlert.Models;
using BotAlert.Service;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotAlert.States
{
    public class MainState : State
    {
        public MainState()
        {
            //new StateDBService().CreateOrUpdate(chatId, ContextState.MainState);
        }

        public override async Task BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            if (message.Type != MessageType.Text)
                return;

            switch (message.Text)
            {
                case "/create":
                    CreateNotification(botClient, message);
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
                    botClient.SendTextMessageAsync(message.Chat.Id, "Something went wrong");
                    break;
            }
        }

        private async void CreateNotification(ITelegramBotClient botClient, Message message)
        {
            var eventObj = new Event(message.Chat.Id);
            botClient.SendTextMessageAsync(message.Chat.Id, "Введите название события: ");
            ContextObj.ChangeState(new UserInputTitleState(eventObj));
        }
    }
}
