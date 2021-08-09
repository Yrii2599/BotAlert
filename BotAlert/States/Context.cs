using System;

namespace BotAlert.States
{
    public class Context
    {
        State state;
        // Constructor
        public Context(State state)
        {
            this.State = state;
            State.ContextObj = this;
        }
        // Gets or sets the state
        public State State
        {
            get { return state; }
            set
            {
                state = value;
                Console.WriteLine("State: " + state.GetType().Name);
            }
        }

        public virtual void ChangeState(State newState)
        {
            State = newState;
            State._contextObj = this;
        }
    }
}