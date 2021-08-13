using System.Threading.Tasks;
using BotAlert.Models;
using BotAlert.Helpers;
using BotAlert.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotAlert.States
{
    public class InputDescriptionKeyboardState : IState
    {
        public async Task<ContextState> BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            if (message.Text != null)
            {
                if (message.Text.ToLower() == "да")
                {
                    return HandleAcceptInput();
                }
                else if (message.Text.ToLower() == "нет")
                {
                    return HandleDeclineInput();
                }
            }

            return PrintMessage(botClient, message.Chat.Id, "Выберите один из вариантов");
        }

        public async Task<ContextState> BotOnCallBackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            await botClient.AnswerCallbackQueryAsync(callbackQueryId: callbackQuery.Id);

            if (callbackQuery.Data == "y")
                return HandleAcceptInput();

            return HandleDeclineInput();
        }

        public void BotSendMessage(ITelegramBotClient botClient, long chatId)
        {
            var options = new InlineKeyboardMarkup(new[] { InlineKeyboardButton.WithCallbackData("Да", "y") , 
                                                           InlineKeyboardButton.WithCallbackData("Нет", "n") });
            InteractionHelper.SendInlineKeyboard(botClient, chatId, "Желаете добавить описание?", options);
        }

        private ContextState PrintMessage(ITelegramBotClient botClient, long chatId, string message)
        {
            botClient.SendTextMessageAsync(chatId, message);

            return ContextState.InputDescriptionKeyboardState;
        }

        private ContextState HandleAcceptInput()
        {
            return ContextState.InputDescriptionState;
        }

        private ContextState HandleDeclineInput()
        {
            return ContextState.SaveState;
        }
    }
}
