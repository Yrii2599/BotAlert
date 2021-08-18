using System;
using System.Threading.Tasks;
using BotAlert.Models;
using BotAlert.Helpers;
using BotAlert.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotAlert.States
{
    public class InputDeleteKeyboardState : IState
    {
        private readonly IEventProvider _eventProvider;
        private readonly IStateProvider _stateProvider;

        public InputDeleteKeyboardState(IEventProvider eventProvider, IStateProvider stateProvider)
        {
            _eventProvider = eventProvider;
            _stateProvider = stateProvider;
        }

        public async Task<ContextState> BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            if (message.Text != null)
            {
                if (message.Text.ToLower() == "да")
                {
                    return await HandleAcceptInput(botClient, message.Chat.Id);
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

            return callbackQuery.Data switch
            {
                "да" => await HandleAcceptInput(botClient, callbackQuery.Message.Chat.Id),
                _ => await HandleDeclineInput()
            };
        }

        public void BotSendMessage(ITelegramBotClient botClient, long chatId)
        {
            var options = new InlineKeyboardMarkup(new[]
            {
                InlineKeyboardButton.WithCallbackData("Да", "да"),
                InlineKeyboardButton.WithCallbackData("Нет", "нет")
            });

            InteractionHelper.SendInlineKeyboard(botClient, chatId, "Вы точно хотите удалить это событие?", options);
        }

        private async Task<ContextState> PrintMessage(ITelegramBotClient botClient, long chatId, string message)
        {
            await botClient.SendTextMessageAsync(chatId, message);

            return ContextState.InputDeleteKeyboardState;
        }

        private Task<ContextState> HandleAcceptInput(ITelegramBotClient botClient, long chatId)
        {
            var chatState = _stateProvider.GetChatState(chatId);

            _eventProvider.DeleteEvent(chatState.ActiveNotificationId);

            chatState.ActiveNotificationId = Guid.Empty;

            _stateProvider.SaveChatState(chatState);

            botClient.SendTextMessageAsync(chatId, "Событие успешно удалено!");

            return Task.FromResult(ContextState.GetAllNotificationsState);
        }

        private Task<ContextState> HandleDeclineInput() => Task.FromResult(ContextState.GetNotificationDetailsState);
    }
}