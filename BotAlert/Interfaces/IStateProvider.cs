using BotAlert.Models;

namespace BotAlert.Interfaces
{
    public interface IStateProvider
    {
        ChatState GetChatState(long chatId);

        void SaveChatState(ChatState chatState);
    }
}
