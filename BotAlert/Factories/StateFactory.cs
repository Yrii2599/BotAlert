using System;
using System.Collections.Generic;
using BotAlert.Interfaces;
using BotAlert.Models;

namespace BotAlert.Factories
{
    public class StateFactory : Dictionary<ContextState, Func<IState>>, IStateFactory
    {
        public IState GetState(ContextState state) => this[state]();
    }
}
