using System.Threading.Tasks;
using BotAlert.Models;
using BotAlert.Helpers;
using BotAlert.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotAlert.States
{
    public class SaveState : IState
    {
        private readonly IEventProvider _eventProvider;

        public SaveState(IEventProvider eventProvider)
        {
            _eventProvider = eventProvider;
        }

        public async Task<ContextState> BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            if (message.Text != null)
            {
                if (message.Text.ToLower() == "сохранить") 
                {
                    return HandleAcceptInput(botClient, message.Chat.Id);
                }
                else if (message.Text.ToLower() == "отменить") 
                {
                    return HandleDeclineInput(botClient, message.Chat.Id);
                }
            }

            return PrintMessage(botClient, message.Chat.Id, "Выберите один из вариантов");
        }

        public async Task<ContextState> BotOnCallBackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            await botClient.AnswerCallbackQueryAsync(callbackQueryId: callbackQuery.Id);

            if (callbackQuery.Data == "Save")
            {
                return HandleAcceptInput(botClient, callbackQuery.Message.Chat.Id);
            }

            return HandleDeclineInput(botClient, callbackQuery.Message.Chat.Id);
        }

        public void BotSendMessage(ITelegramBotClient botClient, long chatId)
        {
            var eventObj = _eventProvider.GetDraftEventByChatId(chatId);
            var message = eventObj.ToString() +
                          $"****************************\n" +
                          $"Сохранить событие?";

            var options = new InlineKeyboardMarkup(new[] { InlineKeyboardButton.WithCallbackData("Сохранить", "Save"),
                                                           InlineKeyboardButton.WithCallbackData("Отменить", "Cancel") });

            InteractionHelper.SendInlineKeyboard(botClient, chatId, message, options);
        }

        private ContextState PrintMessage(ITelegramBotClient botClient, long chatId, string message)
        {
            botClient.SendTextMessageAsync(chatId, message);

            return ContextState.SaveState;
        }

        private ContextState HandleAcceptInput(ITelegramBotClient botClient, long chatId)
        {
            _eventProvider.UpdateDraftEventByChatId(chatId, x => x.Status, EventStatus.Created);

            botClient.SendTextMessageAsync(chatId, "Запись успешно сохранена!");

            return ContextState.MainState;
        }

        private ContextState HandleDeclineInput(ITelegramBotClient botClient, long chatId)
        {
            var eventObj = _eventProvider.GetDraftEventByChatId(chatId);
            _eventProvider.DeleteEvent(eventObj.Id);

            botClient.SendTextMessageAsync(chatId, "Запись успешно удалена!");

            return ContextState.MainState;
        }
    }
}
