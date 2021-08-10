using BotAlert.Models;
using BotAlert.States;

namespace BotAlert.Interfaces
{
    public interface IStateProvider
    {
        Context GetOrCreateChatContext(long chatId);

        void CreateOrUpdateChat(ChatState chatState);
    }
}
