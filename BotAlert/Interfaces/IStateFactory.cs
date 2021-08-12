using BotAlert.Models;

namespace BotAlert.Interfaces
{
    public interface IStateFactory
    {
        public IState GetState(ContextState state);
    }
}
