using System.Threading.Tasks;
using BotAlert.Models;
using BotAlert.Helpers;
using BotAlert.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using System;

namespace BotAlert.States
{
    public class InputWarnDateKeyboardState : IState
    {
        private readonly IEventProvider _eventProvider;
        private readonly IStateProvider _stateProvider;

        public InputWarnDateKeyboardState(IEventProvider eventProvider, IStateProvider stateProvider)
        {
            _eventProvider = eventProvider;
            _stateProvider = stateProvider;
        }

        public async Task<ContextState> BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            return await PrintMessage(botClient, message.Chat.Id, "Выберите один из вариантов");
        }

        public async Task<ContextState> BotOnCallBackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            await botClient.AnswerCallbackQueryAsync(callbackQueryId: callbackQuery.Id);

            if (callbackQuery.Data == "own")
            {
                return ContextState.InputWarnDateState;
            }

            return await HandleWarnDateOptions(botClient, callbackQuery.Message.Chat.Id, callbackQuery.Data);
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
            InteractionHelper.SendInlineKeyboard(botClient, chatId, "Когда вас уведомить?", options);
        }

        private async Task<ContextState> HandleWarnDateOptions(ITelegramBotClient botClient, long chatId, string warnInMinutes)
        {
            var minutes = int.Parse(warnInMinutes);

            var chat = _stateProvider.GetChatState(chatId);
            var eventObj = _eventProvider.GetEventById(chat.ActiveNotificationId);

            if (eventObj.Date < DateTime.Now)
            {
                await botClient.SendTextMessageAsync(chatId, "Ваше событие уже прошло, введите новое значение");
                return ContextState.InputDateState;
            }

            if(eventObj.Date.AddMinutes(-minutes) < DateTime.Now)
            {
                return await PrintMessage(botClient, chatId, "Оповещение уже произошло");
            }

            eventObj.WarnDate = eventObj.Date.AddMinutes(-minutes);

            _eventProvider.UpdateEvent(eventObj);

            if (eventObj.Status == EventStatus.InProgress)
            {
                return ContextState.InputDescriptionKeyboardState;
            }
            else
            {
                return ContextState.EditState;
            }
        }

        private async Task<ContextState> PrintMessage(ITelegramBotClient botClient, long chatId, string message)
        {
            await botClient.SendTextMessageAsync(chatId, message);

            return ContextState.InputWarnDateKeyboardState;
        }
    }
}
