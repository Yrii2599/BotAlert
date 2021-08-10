using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotAlert.States;

namespace BotAlert.Interfaces
{
    interface IStateProvider
    {
        Context GetChatContext(long chatId);
    }
}
