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
                if (message.Text.ToLower() == "сохранить") return HandleAcceptInput();
                else if (message.Text.ToLower() == "отменить") return HandleDeclineInput(botClient, message.Chat.Id);
            }

            return HandleInvalidInput(botClient, message.Chat.Id);
        }

        public async Task<ContextState> BotOnCallBackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            await botClient.AnswerCallbackQueryAsync(callbackQueryId: callbackQuery.Id);

            if (callbackQuery.Data == "s")
                return HandleAcceptInput();

            return HandleDeclineInput(botClient, callbackQuery.Message.Chat.Id);
        }

        public void BotSendMessage(ITelegramBotClient botClient, long chatId)
        {
            //Нужно заменить метод на GetEventBuChatId
            //_eventProvider.GetEventById()

            var options = new InlineKeyboardMarkup(new[] { InlineKeyboardButton.WithCallbackData("Сохранить", "s"),
                                                           InlineKeyboardButton.WithCallbackData("Отменить", "c") });
            InteractionHelper.SendInlineKeyboard(botClient, chatId, "Сохранить событие?", options);
        }

        public ContextState HandleInvalidInput(ITelegramBotClient botClient, long chatId)
        {
            botClient.SendTextMessageAsync(chatId, "Выберите один из вариантов");
            return ContextState.SaveState;
        }

        private ContextState HandleAcceptInput()
        {
            return ContextState.MainState;
        }

        private ContextState HandleDeclineInput(ITelegramBotClient botClient, long chatId)
        {
            //Нужно заменить метод на GetEventBuChatId
            //_eventProvider.GetEventById()

            botClient.SendTextMessageAsync(chatId, "Запись успешно удалена!");
            return ContextState.MainState;
        }
    }
}
