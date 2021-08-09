using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotAlert.Helpers
{
    public static class InteractionHelper
    {
        public static async Task<Message> SendInlineKeyboard(ITelegramBotClient botClient, Message message, string prompt, InlineKeyboardMarkup options)
        {
            await botClient.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

            return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                        text: prompt,
                                                        replyMarkup: options);
        }
    }
}