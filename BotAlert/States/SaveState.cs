using System.Threading.Tasks;
using BotAlert.Helpers;
using BotAlert.Interfaces;
using BotAlert.Models;
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
                if (message.Text.ToLower() == "сохранить") return HandleAcceptInput(botClient, message.Chat.Id);
                else if (message.Text.ToLower() == "отменить") return HandleDeclineInput(botClient, message.Chat.Id);
            }

            return HandleInvalidInput(botClient, message.Chat.Id);
        }

        public async Task<ContextState> BotOnCallBackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            await botClient.AnswerCallbackQueryAsync(callbackQueryId: callbackQuery.Id);

            if (callbackQuery.Data == "s")
                return HandleAcceptInput(botClient, callbackQuery.Message.Chat.Id);

            return HandleDeclineInput(botClient, callbackQuery.Message.Chat.Id);
        }

        public void BotSendMessage(ITelegramBotClient botClient, long chatId)
        {
            var eventObj = _eventProvider.GetDraftEventByChatId(chatId);
            var description = eventObj.Description != null ? eventObj.Description : "No description";
            var message = $"Title: {eventObj.Title}\n" +
                $"Date and time: {eventObj.Date.ToLocalTime()}\n" +
                $"Date and time of notification: {eventObj.WarnDate.ToLocalTime()}\n" +
                $"Description: {description}\n" +
                $"****************************\n" +
                $"Сохранить событие?";

            var options = new InlineKeyboardMarkup(new[] { InlineKeyboardButton.WithCallbackData("Сохранить", "s"),
                                                           InlineKeyboardButton.WithCallbackData("Отменить", "c") });
            InteractionHelper.SendInlineKeyboard(botClient, chatId, message, options);
        }

        public ContextState HandleInvalidInput(ITelegramBotClient botClient, long chatId)
        {
            botClient.SendTextMessageAsync(chatId, "Выберите один из вариантов");
            return ContextState.SaveState;
        }

        private ContextState HandleAcceptInput(ITelegramBotClient botClient, long chatId)
        {
            _eventProvider.UpdateDraftEventByChatId(chatId, "Status", EventStatus.Created);

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
