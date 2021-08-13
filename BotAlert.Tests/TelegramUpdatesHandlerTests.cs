using System;
using System.Threading;
using System.Threading.Tasks;
using BotAlert.Models;
using BotAlert.Handlers;
using BotAlert.Interfaces;
using FakeItEasy;
using Telegram.Bot;
using Telegram.Bot.Types;
<<<<<<< HEAD
using Telegram.Bot.Exceptions;
=======
>>>>>>> 68cb7c5bf0461bf2c37cf1e16edbb6e1859d75b2
using Xunit;
using System.Collections.Generic;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.Enums;

namespace BotAlert.Tests
{
    public class TelegramUpdatesHandlerTests
    {
        private readonly IStateProvider _stateProviderMock;
        private readonly IStateFactory _stateFactoryMock;
        private readonly ITelegramBotClient _botClientMock;
        private readonly ChatState _chatStateMock;
        private readonly IState _stateMock1;
        private readonly Update _updateMock;
        private readonly Message _messageMock;
        private readonly CallbackQuery _callbackQueryMock;
        private readonly CancellationToken _cts;

        private readonly TelegramUpdatesHandler _updatesHandler;

        public TelegramUpdatesHandlerTests()
        {
            _stateFactoryMock = A.Fake<IStateFactory>();
            _stateProviderMock = A.Fake<IStateProvider>();
            _botClientMock = A.Fake<ITelegramBotClient>();
            _chatStateMock = A.Fake<ChatState>();
            _stateMock1 = A.Fake<IState>();
            _updateMock = A.Fake<Update>();
            _messageMock = A.Fake<Message>();
            _messageMock.Chat = A.Fake<Chat>();
            _callbackQueryMock = A.Fake<CallbackQuery>();
            _callbackQueryMock.Message = _messageMock;

            _cts = new CancellationToken();

            _updatesHandler = new TelegramUpdatesHandler(_stateProviderMock, _stateFactoryMock);
        }

        [Fact]
        public void HandleUpdateAsync_ShouldNotStart_MessageAndCallBackQueryAreNull()
        {
            _updatesHandler.HandleUpdateAsync(_botClientMock, _updateMock, _cts);

            A.CallTo(() => _stateProviderMock.GetChatState(A<long>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public void HandleUpdateAsync_WorksCorrectlyWithUpdateTypeMessage()
        {
            _updateMock.Message = _messageMock;

            A.CallTo(() => _stateFactoryMock.GetState(A<ContextState>.Ignored)).Returns(_stateMock1);

            A.CallTo(() => _stateProviderMock.GetChatState(A<long>.Ignored)).Returns(_chatStateMock);

            _updatesHandler.HandleUpdateAsync(_botClientMock, _updateMock, _cts);

            A.CallTo(() => _stateProviderMock.GetChatState(A<long>.Ignored)).MustHaveHappenedTwiceExactly();

            A.CallTo(() => _stateProviderMock.GetChatState(A<long>.Ignored)).MustHaveHappenedTwiceExactly();

            A.CallTo(() => _stateMock1.BotOnMessageReceived(_botClientMock, _updateMock.Message)).MustHaveHappenedOnceExactly();

            A.CallTo(() => _stateProviderMock.SaveChatState(_chatStateMock)).MustHaveHappenedOnceExactly();

        }

        [Fact]
        public void HandleUpdateAsync_WorksCorrectlyWithUpdateTypeCallbackQuery()
        {
            _updateMock.CallbackQuery = _callbackQueryMock;

            A.CallTo(() => _stateFactoryMock.GetState(A<ContextState>.Ignored)).Returns(_stateMock1);
            
            A.CallTo(() => _stateProviderMock.GetChatState(A<long>.Ignored)).Returns(_chatStateMock);

            _updatesHandler.HandleUpdateAsync(_botClientMock, _updateMock, _cts);
            
            A.CallTo(() => _stateProviderMock.GetChatState(A<long>.Ignored)).MustHaveHappenedTwiceExactly();

            A.CallTo(() => _stateProviderMock.GetChatState(A<long>.Ignored)).MustHaveHappenedTwiceExactly();
            
            A.CallTo(() => _stateMock1.BotOnCallBackQueryReceived(_botClientMock, _updateMock.CallbackQuery)).MustHaveHappenedOnceExactly();

            A.CallTo(()=> _stateProviderMock.SaveChatState(_chatStateMock)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void HandleErrorAsync_HandlesApiRequestException()
        {
            var expected = Task.CompletedTask;

            var exception = A.Fake<ApiRequestException>();

            var actual = _updatesHandler.HandleErrorAsync(_botClientMock, exception, _cts);

            A.CallTo(() => exception.ToString()).MustNotHaveHappened();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void HandleErrorAsync_HandlesRegularException()
        {
            var expected = Task.CompletedTask;

            var exception = A.Fake<Exception>();

            var actual = _updatesHandler.HandleErrorAsync(_botClientMock, exception, _cts);

            A.CallTo(() => exception.ToString()).MustHaveHappenedOnceExactly();

            Assert.Equal(expected, actual);
        }
    }
}
