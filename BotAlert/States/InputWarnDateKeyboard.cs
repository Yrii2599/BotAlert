using System.Threading.Tasks;
using BotAlert.Helpers;
using BotAlert.Interfaces;
using BotAlert.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotAlert.States
{
    public class InputWarnDateKeyboard : IState
    {

        private readonly IEventProvider _eventProvider;

        public InputWarnDateKeyboard(IEventProvider eventProvider)
        {
            _eventProvider = eventProvider;
        }

        public async Task<ContextState> BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            return HandleInvalidInput(botClient, message.Chat.Id, "Выберите один из вариантов");
        }

        public async Task<ContextState> BotOnCallBackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            await botClient.AnswerCallbackQueryAsync(callbackQueryId: callbackQuery.Id);

            if (callbackQuery.Data == "own")
            {
                return ContextState.InputWarnDateState;
            }

            return HandleWarnDateOptions(callbackQuery.Message.Chat.Id, callbackQuery.Data);
        }

        private ContextState HandleWarnDateOptions(long chatId, string warnInMinutes)
        {
            var minutes = int.Parse(warnInMinutes);

            var eventObj = _eventProvider.GetDraftEventByChatId(chatId);
            eventObj.WarnDate = eventObj.Date.AddMinutes(-minutes);
            _eventProvider.UpdateEvent(eventObj);

            return ContextState.InputDescriptionKeyboardState;
        }

        public void BotSendMessage(ITelegramBotClient botClient, long chatId)
        {
            var options = new InlineKeyboardMarkup(new[] {
                                                    new[] { InlineKeyboardButton.WithCallbackData("5 м.", "5"),
                                                            InlineKeyboardButton.WithCallbackData("15 м.", "15") },
                                                    new[] { InlineKeyboardButton.WithCallbackData("30 м.", "30"),
                                                            InlineKeyboardButton.WithCallbackData("1 ч.", "60") },
                                                    new[] { InlineKeyboardButton.WithCallbackData("Ввести свое значение", "own") } 
                                                   });
            InteractionHelper.SendInlineKeyboard(botClient, chatId, "Желаете добавить описание?", options);
        }

        public ContextState HandleInvalidInput(ITelegramBotClient botClient, long chatId, string message)
        {
            botClient.SendTextMessageAsync(chatId, message);
            return ContextState.InputWarnDateKeyboard;
        }
    }
}
