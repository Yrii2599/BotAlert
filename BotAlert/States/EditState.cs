using System.Threading.Tasks;
using BotAlert.Models;
using BotAlert.Helpers;
using BotAlert.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotAlert.States
{
    public class EditState : IState
    {
        private readonly IEventProvider _eventProvider;
        private readonly IStateProvider _stateProvider;

        public EditState(IEventProvider eventProvider, IStateProvider stateProvider)
        {
            _eventProvider = eventProvider;
            _stateProvider = stateProvider;
        }

        public async Task<ContextState> BotOnMessageReceived(ITelegramBotClient botClient, Message message) 
            => await PrintMessage(botClient, message.Chat.Id, "Выберите один из вариантов");

        public async Task<ContextState> BotOnCallBackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            await botClient.AnswerCallbackQueryAsync(callbackQueryId: callbackQuery.Id);

            return callbackQuery.Data switch
            {
                "Title" => ContextState.InputTitleState,
                "Date" => ContextState.InputDateState,
                "WarnDate" => ContextState.InputWarnDateState,
                "Description" => ContextState.InputDescriptionState,
                "Back" => ContextState.GetNotificationDetailsState,

                _ => ContextState.EditState
            };
        }

        public void BotSendMessage(ITelegramBotClient botClient, long chatId)
        {
            var eventId = _stateProvider.GetChatState(chatId).ActiveNotificationId;
            var eventObj = _eventProvider.GetEventById(eventId);

            var options = new InlineKeyboardMarkup(new[]
            {
                new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData("Изменить название", "Title") },
                new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData("Изменить дату события", "Date") },
                new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData("Изменить дату оповещения", "WarnDate") },
                new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData("Изменить описание", "Description") },
                new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData("Назад", "Back") },
            });

            InteractionHelper.SendInlineKeyboard(botClient, chatId, eventObj.ToString(), options);
        }

        private async Task<ContextState> PrintMessage(ITelegramBotClient botClient, long chatId, string message)
        {
            await botClient.SendTextMessageAsync(chatId, message);

            return ContextState.EditState;
        }
    }
}
