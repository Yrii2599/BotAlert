using BotAlert.Models;
using BotAlert.States;

namespace BotAlert.Interfaces
{
    public interface IStateProvider
    {
        Context GetChatContext(long chatId);

        void CreateChat(ChatState chatState);
    }
}
