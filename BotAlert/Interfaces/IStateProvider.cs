using BotAlert.Models;

namespace BotAlert.Interfaces
{
    public interface IStateProvider
    {
        IState GetChatState(long chatId);

        void SaveChatState(ChatState chatState);
    }
}
