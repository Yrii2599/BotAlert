using System;
using BotAlert.Interfaces;

namespace BotAlert.States
{
    public class Context
    {
        private IState state;

        public IState State
        {
            get { return state; }
            set
            {
                state = value;
                State.ContextObj = this;
                Console.WriteLine("State: " + state.GetType().Name);
            }
        }
    }
}