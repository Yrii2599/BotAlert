using BotAlert.Models;

namespace BotAlert.Interfaces
{
    public interface IStateFactory
    {
        IState GetState(ContextState state);
    }
}
