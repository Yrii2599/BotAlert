using System.Threading.Tasks;
using BotAlert.Models;
using BotAlert.Interfaces;
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
            {
                return ContextState.MainState;
            }

            return message.Text switch
            {
                "/start" => await PrintMessage(botClient, 
                                               message.Chat.Id, $"Рады вас приветствовать, {message.Chat.FirstName}!", 
                                               ContextState.InputTimeZoneState),
                "/create" => ContextState.InputTitleState,
                "/get_notifications" => ContextState.GetAllNotificationsState,
                "/set_time_zone" => ContextState.InputTimeZoneState,

                _ => await PrintMessage(botClient, message.Chat.Id, "Выберите пожалуйста одну из команд!", ContextState.MainState)
            };
        }

        public Task<ContextState> BotOnCallBackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
            => Task.FromResult(ContextState.MainState);

        public void BotSendMessage(ITelegramBotClient botClient, long chatId)
        {
            botClient.SendTextMessageAsync(chatId, $"Выберите одну из команд:\n" +
                                                   $"/create - Создать новое событие\n" +
                                                   $"/get_notifications - Получить список событий\n" +
                                                   $"/set_time_zone - Установить часовой пояс");
        }

        private async Task<ContextState> PrintMessage(ITelegramBotClient botClient, long chatId, string message, ContextState returnState)
        {
            await botClient.SendTextMessageAsync(chatId, message);

            return returnState;
        }
    }
}
