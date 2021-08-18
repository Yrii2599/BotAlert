using System;
using System.Threading;
using System.Threading.Tasks;
using BotAlert.Models;
using BotAlert.Handlers;
using BotAlert.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using FakeItEasy;
using Xunit;

namespace BotAlert.Tests
{
    public class TelegramUpdatesHandlerTests
    {
        private readonly IStateProvider _stateProviderMock;
        private readonly IStateFactory _stateFactoryMock;
        private readonly ITelegramBotClient _botClientMock;
        private readonly IState _stateMock1;
        private readonly IState _stateMock2;
        private readonly Update _updateStub;
        private readonly Message _messageStub;
        private readonly ChatState _chatStateStub;
        private readonly ContextState _contextStateStub;
        private readonly CallbackQuery _callbackQueryStub;
        private readonly CancellationToken _cancellationTokenStub;

        private readonly TelegramUpdatesHandler _updatesHandler;

        public TelegramUpdatesHandlerTests()
        {
            _stateFactoryMock = A.Fake<IStateFactory>();
            _stateProviderMock = A.Fake<IStateProvider>();
            _botClientMock = A.Fake<ITelegramBotClient>();
            _stateMock1 = A.Fake<IState>();
            _stateMock2 = A.Fake<IState>();
            _chatStateStub = new ChatState(1234);
            _contextStateStub = ContextState.MainState;
            _updateStub = new Update();
            _messageStub = new Message();
            _messageStub.Chat = new Chat();
            _callbackQueryStub = new CallbackQuery();
            _callbackQueryStub.Message = _messageStub;
            _cancellationTokenStub = new CancellationToken();

            _updatesHandler = new TelegramUpdatesHandler(_stateProviderMock, _stateFactoryMock);
        }

        [Fact]
        public void HandleUpdateAsync_ShouldNotStart_MessageAndCallBackQueryAreNull()
        {
            _updatesHandler.HandleUpdateAsync(_botClientMock, _updateStub, _cancellationTokenStub);

            A.CallTo(() => _stateProviderMock.GetChatState(A<long>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public void HandleUpdateAsync_WorksCorrectlyWithUpdateTypeMessage()
        {
            _updateStub.Message = _messageStub;
            A.CallTo(() => _stateProviderMock.GetChatState(_messageStub.Chat.Id)).Returns(_chatStateStub);
            A.CallTo(() => _stateFactoryMock.GetState(_chatStateStub.State)).Returns(_stateMock1);
            A.CallTo(() => _stateMock1.BotOnMessageReceived(_botClientMock, _updateStub.Message)).Returns(_contextStateStub);
            A.CallTo(() => _stateFactoryMock.GetState(_contextStateStub)).Returns(_stateMock2);

            _updatesHandler.HandleUpdateAsync(_botClientMock, _updateStub, _cancellationTokenStub);

            A.CallTo(() => _stateProviderMock.GetChatState(_messageStub.Chat.Id)).MustHaveHappenedTwiceExactly();
            A.CallTo(() => _stateFactoryMock.GetState(_chatStateStub.State)).MustHaveHappenedTwiceExactly();
            A.CallTo(() => _stateProviderMock.SaveChatState(_chatStateStub)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _stateMock2.BotSendMessage(_botClientMock, _messageStub.Chat.Id)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void HandleUpdateAsync_WorksCorrectlyWithUpdateTypeCallbackQuery()
        {
            _updateStub.CallbackQuery = _callbackQueryStub;
            A.CallTo(() => _stateProviderMock.GetChatState(_messageStub.Chat.Id)).Returns(_chatStateStub);
            A.CallTo(() => _stateFactoryMock.GetState(_chatStateStub.State)).Returns(_stateMock1);
            A.CallTo(() => _stateMock1.BotOnMessageReceived(_botClientMock, _updateStub.Message)).Returns(_contextStateStub);
            A.CallTo(() => _stateFactoryMock.GetState(_contextStateStub)).Returns(_stateMock2);

            _updatesHandler.HandleUpdateAsync(_botClientMock, _updateStub, _cancellationTokenStub);

            A.CallTo(() => _stateProviderMock.GetChatState(_messageStub.Chat.Id)).MustHaveHappenedTwiceExactly();
            A.CallTo(() => _stateFactoryMock.GetState(_chatStateStub.State)).MustHaveHappenedTwiceExactly();
            A.CallTo(() => _stateProviderMock.SaveChatState(_chatStateStub)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _stateMock2.BotSendMessage(_botClientMock, _callbackQueryStub.Message.Chat.Id)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void HandleErrorAsync_HandlesApiRequestException()
        {
            var expected = Task.CompletedTask;
            var exception = A.Fake<ApiRequestException>();

            var actual = _updatesHandler.HandleErrorAsync(_botClientMock, exception, _cancellationTokenStub);

            A.CallTo(() => exception.ToString()).MustNotHaveHappened();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void HandleErrorAsync_HandlesRegularException()
        {
            var expected = Task.CompletedTask;
            var exceptionStub = A.Fake <Exception>();

            var actual = _updatesHandler.HandleErrorAsync(_botClientMock, exceptionStub, _cancellationTokenStub);

            A.CallTo(() => exceptionStub.ToString()).MustHaveHappenedOnceExactly();
            Assert.Equal(expected, actual);
        }
    }
}
