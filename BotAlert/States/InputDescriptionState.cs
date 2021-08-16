using System.Threading.Tasks;
using BotAlert.Models;
using BotAlert.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using System;

namespace BotAlert.States
{
    public class InputDescriptionState : IState
    {
        private readonly IEventProvider _eventProvider;
        private readonly IStateProvider _stateProvider;

        public InputDescriptionState(IEventProvider eventProvider, IStateProvider stateProvider)
        {
            _eventProvider = eventProvider;
            _stateProvider = stateProvider;
        }

        public async Task<ContextState> BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            if (message.Text == null) 
            { 
                return PrintMessage(botClient, message.Chat.Id, "Неверный формат сообщения");
            }

            var chat = _stateProvider.GetChatState(message.Chat.Id);

            if (chat.ActiveNotificationId != Guid.Empty)
            {
                var eventObj = _eventProvider.GetEventById(chat.ActiveNotificationId);
                eventObj.Description = message.Text;
                _eventProvider.UpdateEvent(eventObj);

                return ContextState.EditState;
            }
            else
            {
                _eventProvider.UpdateDraftEventByChatId(message.Chat.Id, x => x.Description, message.Text);

                return ContextState.SaveState;
            }
        }

        public async Task<ContextState> BotOnCallBackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            await botClient.AnswerCallbackQueryAsync(callbackQueryId: callbackQuery.Id);

            return ContextState.InputDescriptionState;
        }

        public void BotSendMessage(ITelegramBotClient botClient, long chatId)
        {
            botClient.SendTextMessageAsync(chatId, $"Введите описание оповещения:");
        }

        private ContextState PrintMessage(ITelegramBotClient botClient, long chatId, string message)
        {
            botClient.SendTextMessageAsync(chatId, message);

            return ContextState.InputDescriptionState;
        }
    }
}
