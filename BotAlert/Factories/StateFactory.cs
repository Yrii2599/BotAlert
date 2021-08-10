using BotAlert.Interfaces;
using BotAlert.Models;
using BotAlert.States;

namespace BotAlert.Factories
{
    public class StateFactory : IStateFactory
    {

        public IState GetState(ContextState state)
        {
            return state switch
            {
                ContextState.MainState => GetMainState(),
                ContextState.UserInputTitleState => GetUserInputTitleState(),
                ContextState.UserInputDateState => GetUserInputDateState(),
                ContextState.UserInputWarnDateState => GetUserInputWarnDateState(),
                ContextState.UserInputDescriptionState => GetUserInputDescriptionState()
            };
        }

        private IState GetUserInputDescriptionState()
        {
            return new UserInputDescriptionState();
        }

        private IState GetUserInputWarnDateState()
        {
            return new UserInputWarnDateState();
        }

        private IState GetUserInputDateState()
        {
            return new UserInputDateState();
        }

        private IState GetUserInputTitleState()
        {
            return new UserInputTitleState();
        }

        private IState GetMainState()
        {
            return new MainState();
        }
    }
}
