using BotAlert.Factories;
using BotAlert.Helpers.Localizers;
using BotAlert.Interfaces;
using BotAlert.Models;
using System;
using Xunit;

namespace BotAlert.Tests
{
    public class LocalizerFactoryTests
    {
        private readonly ILocalizerFactory _localizerFactory;

        public LocalizerFactoryTests()
        {
            _localizerFactory = new LocalizerFactory
            {
                { LanguageType.English, () => new EngLocalizeHelper() },
                { LanguageType.Russian, () => new RusLocalizeHelper() },
            };
        }

        [Theory]
        [InlineData(LanguageType.English, typeof(EngLocalizeHelper))]
        [InlineData(LanguageType.Russian, typeof(RusLocalizeHelper))]
        public void GetLocalizer_ReturnsCorrectLocalizer(LanguageType language, Type expectedHelperType)
        {
            var actualHelperType = _localizerFactory.GetLocalizer(language).GetType();

            Assert.Equal(expectedHelperType, actualHelperType);
        }
    }
}
