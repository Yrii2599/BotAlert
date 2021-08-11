using BotAlert.Helpers;
using BotAlert.Interfaces;
using BotAlert.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotAlert.States
{
    public class UserInputDescriptionKeyboardState : IState
    {
        private readonly IEventProvider _eventProvider;

        public UserInputDescriptionKeyboardState(IEventProvider eventProvider)
        {
            _eventProvider = eventProvider;
        }

        public async Task<ContextState> BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            if(message.Text != null)
            {
                if (message.Text.ToLower() == "да") return HandleAcceptInput();
                else if (message.Text.ToLower() == "нет") return HandleDeclineInput(message.Chat.Id);
            }

            return HandleInvalidInput(botClient, message.Chat.Id);
        }

        public async Task<ContextState> BotOnCallBackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            await botClient.AnswerCallbackQueryAsync(
                callbackQueryId: callbackQuery.Id,
                text: $"Received {callbackQuery.Data}");

            if (callbackQuery.Data == "y")
                return HandleAcceptInput();

            return HandleDeclineInput(callbackQuery.Message.Chat.Id);
        }

        public void BotSendMessage(ITelegramBotClient botClient, long chatId)
        {
            var options = new InlineKeyboardMarkup(new[] { InlineKeyboardButton.WithCallbackData("Да", "y") , 
                                                           InlineKeyboardButton.WithCallbackData("Нет", "n") });
            InteractionHelper.SendInlineKeyboard(botClient, chatId, "Желаете добавить описание?", options);
        }

        public ContextState HandleInvalidInput(ITelegramBotClient botClient, long chatId)
        {
            botClient.SendTextMessageAsync(chatId, "Выберите один из вариантов");
            return ContextState.UserInputDescriptionKeyboardState;
        }

        private ContextState HandleAcceptInput()
        {
            return ContextState.UserInputDescriptionState;
        }

        private ContextState HandleDeclineInput(long chatId)
        {
            var eventObj = _eventProvider.GetDraftEventByChatId(chatId);
            eventObj.Status = EventStatus.Created;

            _eventProvider.UpdateEvent(eventObj);
            return ContextState.MainState;
        }
    }
}
