using BotAlert.Models;

namespace BotAlert.Interfaces
{
    public interface IEventProvider
    {
        Event GetDraftEventByChatId(long chatId);
    }
}
