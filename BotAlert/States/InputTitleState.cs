using System;
using System.Threading.Tasks;
using BotAlert.Models;
using BotAlert.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotAlert.States
{
    public class InputTitleState : IState
    {
        private readonly IEventProvider _eventProvider;
        private readonly IStateProvider _stateProvider;

        public InputTitleState(IEventProvider eventProvider, IStateProvider stateProvider)
        {
            _eventProvider = eventProvider;
            _stateProvider = stateProvider;
        }

        public async Task<ContextState> BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            if (message.Text == null) 
            { 
                return await PrintMessage(botClient, message.Chat.Id, "Неверный формат сообщения");
            }

            var chat = _stateProvider.GetChatState(message.Chat.Id);

            if (chat.ActiveNotificationId != Guid.Empty)
            {
                var eventObj = _eventProvider.GetEventById(chat.ActiveNotificationId);
                
                if (eventObj == null)
                {
                    chat.ActiveNotificationId = Guid.Empty;
                    _stateProvider.SaveChatState(chat);
                    await botClient.SendTextMessageAsync(chat.ChatId, "Данное событие уже произошло");

                    return ContextState.MainState;
                }

                eventObj.Title = message.Text;
                _eventProvider.UpdateEvent(eventObj);

                return ContextState.EditState;
            }
            else
            {
                _eventProvider.CreateEvent(new Event(message.Chat.Id, message.Text));
                chat.ActiveNotificationId = _eventProvider.GetDraftEventByChatId(message.Chat.Id).Id;
                _stateProvider.SaveChatState(chat);

                return ContextState.InputEventTimeZoneKeyboardState;
            }
        }

        public async Task<ContextState> BotOnCallBackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            await botClient.AnswerCallbackQueryAsync(callbackQueryId: callbackQuery.Id);

            return ContextState.InputTitleState;
        }

        public void BotSendMessage(ITelegramBotClient botClient, long chatId)
        {
            botClient.SendTextMessageAsync(chatId, $"Введите название события:");
        }

        private async Task<ContextState> PrintMessage(ITelegramBotClient botClient, long chatId, string message)
        {
            await botClient.SendTextMessageAsync(chatId, message);

            return ContextState.InputTitleState;
        }
    }
}
