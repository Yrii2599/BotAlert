using System;
using System.Threading.Tasks;
using BotAlert.Helpers;
using Telegram.Bot;
using Telegram.Bot.Types;
using BotAlert.Interfaces;
using BotAlert.Models;

namespace BotAlert.States
{
    public class InputTimeZoneState : IState
    {
        private readonly IStateProvider _stateProvider;
        private readonly IEventProvider _eventProvider;

        public InputTimeZoneState(IStateProvider stateProvider, IEventProvider eventProvider)
        {
            _stateProvider = stateProvider;
            _eventProvider = eventProvider;

        }

        public async Task<ContextState> BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            if (message.Text == null || !int.TryParse(message.Text, out var timeOffSet) || timeOffSet < -12 || timeOffSet > 14)
            {
                return await PrintMessage(botClient, message.Chat.Id, "Неправильный формат ввода");
            }

            var chat = _stateProvider.GetChatState(message.Chat.Id);


            if (chat.ActiveNotificationId == Guid.Empty)
            {
                chat.TimeOffSet = timeOffSet;
                _stateProvider.SaveChatState(chat);

                return ContextState.MainState;
            }

            var eventObj = _eventProvider.GetEventById(chat.ActiveNotificationId);
            
            if (eventObj == null)
            {
                chat.ActiveNotificationId = Guid.Empty;
                _stateProvider.SaveChatState(chat);
                await botClient.SendTextMessageAsync(chat.ChatId, "Данное событие уже произошло");

                return ContextState.MainState;
            }

            eventObj.TimeOffSet = timeOffSet;
            _eventProvider.UpdateEvent(eventObj);

            return ContextState.InputDateState;

        }

        public async Task<ContextState> BotOnCallBackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            await botClient.AnswerCallbackQueryAsync(callbackQueryId: callbackQuery.Id);


            return await PrintMessage(botClient, callbackQuery.Message.Chat.Id, "Введите целое число");

        }

        public void BotSendMessage(ITelegramBotClient botClient, long chatId)
        {

            var timeOffSet = _stateProvider.GetChatState(chatId).TimeOffSet;

            if (timeOffSet>0)
            {
            Task.FromResult(botClient.SendTextMessageAsync(chatId, 
                TimeZoneHelper.PrintTimeZone(timeOffSet)+
                $"****************************\n" +
                $"Введите новый часовой пояс (от -12 до +14):"));
            }
            else
            {
                Task.FromResult(botClient.SendTextMessageAsync(chatId,
                    TimeZoneHelper.PrintTimeZone(timeOffSet) +
                   $"****************************\n" +
                   $"Введите новый часовой пояс (от -12 до +14):"));
            }

        }

        private async Task<ContextState> PrintMessage(ITelegramBotClient botClient, long chatId, string message)
        {
            await botClient.SendTextMessageAsync(chatId, message);

            return ContextState.InputTimeZoneState;
        }
    }
}
