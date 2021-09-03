using System;
using System.Threading.Tasks;
using BotAlert.Helpers;
using BotAlert.Interfaces;
using BotAlert.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotAlert.States
{
    public class InputEventTimeZoneKeyboardState : IState
    {
        private readonly IStateProvider _stateProvider;
        private readonly IEventProvider _eventProvider;

        public InputEventTimeZoneKeyboardState(IStateProvider stateProvider, IEventProvider eventProvider)
        {
            _stateProvider = stateProvider;
            _eventProvider = eventProvider;
        }

        public async Task<ContextState> BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            if (message.Text != null)
            {
                if (message.Text.ToLower() == "да")
                {
                    return await HandleAcceptInput();
                }

                if (message.Text.ToLower() == "нет")
                {
                    return await HandleDeclineInput(message.Chat.Id);
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

            if (callbackQuery.Data == "нет")
            {
                return await HandleDeclineInput(callbackQuery.Message.Chat.Id);
            }

            return await PrintMessage(botClient, callbackQuery.Message.Chat.Id, "Выберите один из вариантов");
        }

        public void BotSendMessage(ITelegramBotClient botClient, long chatId)
        {
            var timeOffSet = _stateProvider.GetChatState(chatId).TimeOffSet;
            
            var options = new InlineKeyboardMarkup(new[] { InlineKeyboardButton.WithCallbackData("Да", "да") ,
                                                           InlineKeyboardButton.WithCallbackData("Нет", "нет") });

            InteractionHelper.SendInlineKeyboard(botClient, chatId, TimeZoneHelper.PrintTimeZone(timeOffSet) +
                $"Желаете ввести иной часовой пояс?", options);
        }

        private async Task<ContextState> PrintMessage(ITelegramBotClient botClient, long chatId, string message)
        {
            await botClient.SendTextMessageAsync(chatId, message);

            return ContextState.InputEventTimeZoneKeyboardState;
        }

        private Task<ContextState> HandleAcceptInput() => Task.FromResult(ContextState.InputTimeZoneState);

        private Task<ContextState> HandleDeclineInput(long chatId)
        {
            var chat = _stateProvider.GetChatState(chatId);
            var eventObj = _eventProvider.GetEventById(chat.ActiveNotificationId);
            eventObj.TimeOffSet = chat.TimeOffSet;
            _eventProvider.UpdateEvent(eventObj);

            return Task.FromResult(ContextState.InputDateState);
        }
    }
}

