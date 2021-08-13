using System.Threading.Tasks;
using BotAlert.Models;
using BotAlert.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotAlert.States
{
    public class InputDescriptionState : IState
    {
        private readonly IEventProvider _eventProvider;

        public InputDescriptionState(IEventProvider eventProvider)
        {
            _eventProvider = eventProvider;
        }

        public async Task<ContextState> BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            if (message.Text == null) 
            { 
                return PrintMessage(botClient, message.Chat.Id, "Неверный формат сообщения");
            }

            _eventProvider.UpdateDraftEventByChatId(message.Chat.Id, x => x.Description, message.Text);

            return ContextState.SaveState;
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
