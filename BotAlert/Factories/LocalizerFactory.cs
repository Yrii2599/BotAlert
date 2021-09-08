using System;
using System.Collections.Generic;
using BotAlert.Interfaces;
using BotAlert.Models;

namespace BotAlert.Factories
{
    public class LocalizerFactory : Dictionary<LanguageType, Func<ILocalizeHelper>>, ILocalizerFactory
    {
        public ILocalizeHelper GetLocalizer(LanguageType language) => this[language]();
    }
}
