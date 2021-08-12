using System;
using System.Threading;
using System.Threading.Tasks;
using BotAlert.Controllers;
using BotAlert.Interfaces;
using BotAlert.Models;
using FakeItEasy;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Xunit;

namespace BotAlert.Tests
{
    public class TelegramUpdatesHandlerTests
    {
        private readonly IStateProvider _stateProviderMock;
        private readonly IStateFactory _stateFactoryMock;
        private readonly ITelegramBotClient _botClientMock;
        private readonly IState _stateMock1;
        // private readonly IState _stateMock2;
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
            _stateMock1 = A.Fake<IState>();
            // _stateMock2 = A.Fake<IState>();
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

            A.CallTo(() => _stateProviderMock.GetChatState(A<long>.Ignored)).Returns(_stateMock1);

            _updatesHandler.HandleUpdateAsync(_botClientMock, _updateMock, _cts);
            
            A.CallTo(() => _stateMock1.BotOnMessageReceived(A<ITelegramBotClient>.Ignored, A<Message>.Ignored)).MustHaveHappenedOnceExactly();

            A.CallTo(() => _stateProviderMock.SaveChatState(A<ChatState>.Ignored)).MustHaveHappenedOnceExactly();

            // A.CallTo(() => _botClientMock.SendTextMessageAsync(A<ChatId>.Ignored, A<string>.Ignored, A<ParseMode>.Ignored,
            //                                               A<IEnumerable<MessageEntity>>.Ignored, A<bool>.Ignored,
            //                                               A<bool>.Ignored, A<int>.Ignored, A<bool>.Ignored,
            //                                               A<IReplyMarkup>.Ignored, A<CancellationToken>.Ignored))
            //                                               .MustHaveHappened();

            // A.CallTo(() => _stateFactoryMock.GetState(A<ContextState>.Ignored).BotSendMessage(A<ITelegramBotClient>.Ignored, A<long>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void HandleUpdateAsync_WorksCorrectlyWithUpdateTypeCallbackQuery()
        {
            _updateMock.CallbackQuery = _callbackQueryMock;

            A.CallTo(() => _stateProviderMock.GetChatState(A<long>.Ignored)).Returns(_stateMock1);

            _updatesHandler.HandleUpdateAsync(_botClientMock, _updateMock, _cts);

            A.CallTo(() => _stateMock1.BotOnCallBackQueryReceived(A<ITelegramBotClient>.Ignored, A<CallbackQuery>.Ignored)).MustHaveHappenedOnceExactly();

            A.CallTo(() => _stateProviderMock.SaveChatState(A<ChatState>.Ignored)).MustHaveHappenedOnceExactly();

            // A.CallTo(() => _botClientMock.SendTextMessageAsync(A<ChatId>.Ignored, A<string>.Ignored, A<ParseMode>.Ignored,
            //                                               A<IEnumerable<MessageEntity>>.Ignored, A<bool>.Ignored,
            //                                               A<bool>.Ignored, A<int>.Ignored, A<bool>.Ignored,
            //                                               A<IReplyMarkup>.Ignored, A<CancellationToken>.Ignored))
            //                                               .MustHaveHappened();

            // A.CallTo(() => _stateFactoryMock.GetState(A<ContextState>.Ignored).BotSendMessage(A<ITelegramBotClient>.Ignored, A<long>.Ignored)).MustHaveHappenedOnceExactly();
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
