using System.Threading.Tasks;
using BotAlert.Models;
using BotAlert.Helpers;
using BotAlert.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotAlert.States
{
    public class InputWarnDateKeyboardState : IState
    {
        private readonly IEventProvider _eventProvider;

        public InputWarnDateKeyboardState(IEventProvider eventProvider)
        {
            _eventProvider = eventProvider;
        }

        public async Task<ContextState> BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            return PrintMessage(botClient, message.Chat.Id, "Выберите один из вариантов");
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

        public void BotSendMessage(ITelegramBotClient botClient, long chatId)
        {
            var options = new InlineKeyboardMarkup(new[] {
                                                    new[] { InlineKeyboardButton.WithCallbackData("5 м.", "5"),
                                                            InlineKeyboardButton.WithCallbackData("15 м.", "15") },
                                                    new[] { InlineKeyboardButton.WithCallbackData("30 м.", "30"),
                                                            InlineKeyboardButton.WithCallbackData("1 ч.", "60") },
                                                    new[] { InlineKeyboardButton.WithCallbackData("Ввести свое значение", "own") } 
                                                   });
            InteractionHelper.SendInlineKeyboard(botClient, chatId, "Когда вас уведомить ?", options);
        }

        private ContextState HandleWarnDateOptions(long chatId, string warnInMinutes)
        {
            var minutes = int.Parse(warnInMinutes);

            var eventObj = _eventProvider.GetDraftEventByChatId(chatId);
            eventObj.WarnDate = eventObj.Date.AddMinutes(-minutes);
            _eventProvider.UpdateEvent(eventObj);

            return ContextState.InputDescriptionKeyboardState;
        }

        private ContextState PrintMessage(ITelegramBotClient botClient, long chatId, string message)
        {
            botClient.SendTextMessageAsync(chatId, message);
            return ContextState.InputWarnDateKeyboardState;
        }
    }
}
