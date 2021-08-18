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
    public class SaveState : IState
    {
        private readonly IEventProvider _eventProvider;
        private readonly IStateProvider _stateProvider;

        public SaveState(IEventProvider eventProvider, IStateProvider stateProvider)
        {
            _eventProvider = eventProvider;
            _stateProvider = stateProvider;
        }

        public async Task<ContextState> BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            if (message.Text != null)
            {
                if (message.Text.ToLower() == "сохранить") 
                {
                    return await HandleAcceptInput(botClient, message.Chat.Id);
                }
                else if (message.Text.ToLower() == "отменить") 
                {
                    return await HandleDeclineInput(botClient, message.Chat.Id);
                }
            }

            return await PrintMessage(botClient, message.Chat.Id, "Выберите один из вариантов");
        }

        public async Task<ContextState> BotOnCallBackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            await botClient.AnswerCallbackQueryAsync(callbackQueryId: callbackQuery.Id);

            if (callbackQuery.Data == "Save")
            {
                return await HandleAcceptInput(botClient, callbackQuery.Message.Chat.Id);
            }

            return await HandleDeclineInput(botClient, callbackQuery.Message.Chat.Id);
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

        private async Task<ContextState> PrintMessage(ITelegramBotClient botClient, long chatId, string message)
        {
            await botClient.SendTextMessageAsync(chatId, message);

            return ContextState.SaveState;
        }

        private async Task<ContextState> HandleAcceptInput(ITelegramBotClient botClient, long chatId)
        {
            var chat = _stateProvider.GetChatState(chatId);
            var eventObj = _eventProvider.GetEventById(chat.ActiveNotificationId);

            eventObj.Status = EventStatus.Created;
            _eventProvider.UpdateEvent(eventObj);

            chat.ActiveNotificationId = Guid.Empty;
            _stateProvider.SaveChatState(chat);

            await botClient.SendTextMessageAsync(chatId, "Запись успешно сохранена!");

            return ContextState.MainState;
        }

        private async Task<ContextState> HandleDeclineInput(ITelegramBotClient botClient, long chatId)
        {
            var chat = _stateProvider.GetChatState(chatId);
            _eventProvider.DeleteEvent(chat.ActiveNotificationId);

            chat.ActiveNotificationId = Guid.Empty;
            _stateProvider.SaveChatState(chat);

            await botClient.SendTextMessageAsync(chatId, "Запись успешно удалена!");

            return ContextState.MainState;
        }
    }
}
