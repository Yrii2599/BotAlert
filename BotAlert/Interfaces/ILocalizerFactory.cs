
using BotAlert.Models;

namespace BotAlert.Interfaces
{
    public interface ILocalizerFactory
    {
        public ILocalizeHelper GetLocalizer(LanguageType language);
    }
}
