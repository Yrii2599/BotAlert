using System.Threading.Tasks;
using BotAlert.Interfaces;
using BotAlert.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotAlert.States
{
    public class MainState : IState
    {
        public async Task<ContextState> BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            if (message.Type != MessageType.Text)
                return ContextState.MainState;

            return message.Text switch
            {
                "/start" => GreetUser(botClient, message.Chat.Id),
                "/create" => CreateNotification(),
                /*"/get_notifications" => GetAllNotifications(botClient, message),
                "/get_notification" => GetNotification(botClient, message),
                "/edit" => EditNotification(botClient, message),
                "/delete" => DeleteNotification(botClient, message),*/
                _ => HandleInvalidInput(botClient, message.Chat.Id)
            };
        }

        public void BotSendMessage(ITelegramBotClient botClient, long chatId)
        {
            botClient.SendTextMessageAsync(chatId, $"Выберите одну из команд:\n" +
                                                   $"/create - Создать новое событие\n" +
                                                   $"/get_notifications - Получить список событий");
        }

        public ContextState HandleInvalidInput(ITelegramBotClient botClient, long chatId)
        {
            botClient.SendTextMessageAsync(chatId, "Выберите пожалуйста одну из команд!");
            return ContextState.MainState;
        }

        private ContextState GreetUser(ITelegramBotClient botClient, long chatId)
        {
            botClient.SendTextMessageAsync(chatId, $"Рады вас приветствовать!");
            return ContextState.MainState;
        }

        private ContextState CreateNotification()
        {
            return ContextState.UserInputTitleState;
        }
        
    }
}
