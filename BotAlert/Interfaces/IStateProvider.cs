using BotAlert.Models;

namespace BotAlert.Interfaces
{
    public interface IStateProvider
    {
        IState GetOrCreateChatState(long chatId);

        void CreateOrUpdateChat(ChatState chatState);
    }
}
