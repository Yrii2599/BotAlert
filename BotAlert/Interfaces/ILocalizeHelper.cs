using Telegram.Bot.Types.ReplyMarkups;

namespace BotAlert.Interfaces
{
    public interface ILocalizeHelper
    {
        public string GetMessage(string key);

        public InlineKeyboardMarkup GetInlineKeyboardMarkUp(string key);

        public string GetTimeZone(int timeZone);
    }
}
