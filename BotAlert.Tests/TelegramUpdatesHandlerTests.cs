using BotAlert.Controllers;
using BotAlert.Interfaces;
using FakeItEasy;
using Xunit;

namespace BotAlert.Tests
{
    public class TelegramUpdatesHandlerTests
    {
        private readonly IStateProvider _stateProviderMock;
        private readonly IStateFactory _stateFactoryMock;

        private readonly TelegramUpdatesHandler _updatesHandler;

        public TelegramUpdatesHandlerTests()
        {
            _stateFactoryMock = A.Fake<IStateFactory>();
            _stateProviderMock = A.Fake<IStateProvider>();

            _updatesHandler = new TelegramUpdatesHandler(_stateProviderMock, _stateFactoryMock);
        }

    }
}
