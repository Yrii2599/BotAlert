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
                    return await HandleAcceptInput();
                }
                else if (message.Text.ToLower() == "нет")
                {
                    return await HandleDeclineInput();
                }
            }

            return await PrintMessage(botClient, message.Chat.Id, "Выберите один из вариантов");
        }

        public async Task<ContextState> BotOnCallBackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            await botClient.AnswerCallbackQueryAsync(callbackQueryId: callbackQuery.Id);

            if (callbackQuery.Data == "да")
            {
                return await HandleAcceptInput();
            }

            return await HandleDeclineInput();
        }

        public void BotSendMessage(ITelegramBotClient botClient, long chatId)
        {
            var options = new InlineKeyboardMarkup(new[] { InlineKeyboardButton.WithCallbackData("Да", "да") , 
                                                           InlineKeyboardButton.WithCallbackData("Нет", "нет") });
            InteractionHelper.SendInlineKeyboard(botClient, chatId, "Желаете добавить описание?", options);
        }

        private async Task<ContextState> PrintMessage(ITelegramBotClient botClient, long chatId, string message)
        {
            await botClient.SendTextMessageAsync(chatId, message);

            return ContextState.InputDescriptionKeyboardState;
        }

        private Task<ContextState> HandleAcceptInput() => Task.FromResult(ContextState.InputDescriptionState);

        private Task<ContextState> HandleDeclineInput() => Task.FromResult(ContextState.SaveState);
    }
}
