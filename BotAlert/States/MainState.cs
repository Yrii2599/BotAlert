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
                "/start" => PrintMessage(botClient, message.Chat.Id, $"Рады вас приветствовать, {message.Chat.FirstName}!"),
                "/create" => CreateNotification(),
                "/get_notifications" => GetAllNotifications(),

                _ => PrintMessage(botClient, message.Chat.Id, "Выберите пожалуйста одну из команд!")
            };
        }

        public async Task<ContextState> BotOnCallBackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
            => ContextState.MainState;

        public void BotSendMessage(ITelegramBotClient botClient, long chatId)
        {
            botClient.SendTextMessageAsync(chatId, $"Выберите одну из команд:\n" +
                                                   $"/create - Создать новое событие\n" +
                                                   $"/get_notifications - Получить список событий");
        }

        private ContextState PrintMessage(ITelegramBotClient botClient, long chatId, string message)
        {
            botClient.SendTextMessageAsync(chatId, message);

            return ContextState.MainState;
        }

        private ContextState CreateNotification() => ContextState.InputTitleState;
        
        private ContextState GetAllNotifications() => ContextState.GetAllNotificationsState;
    }
}
