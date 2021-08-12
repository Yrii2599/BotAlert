using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BotAlert.Interfaces;
using BotAlert.Models;
using BotAlert.States;
using FakeItEasy;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Xunit;

namespace BotAlert.Tests
{
    public class MainStateTests
    {
        private readonly ITelegramBotClient _botClientMock;
        private readonly Message _messageMock;
        private readonly CallbackQuery _callbackQueryMock;

        private readonly IState _mainState;

        public MainStateTests()
        {
            _botClientMock = A.Fake<ITelegramBotClient>();
            _messageMock = A.Fake<Message>();
            _messageMock.Chat = A.Fake<Chat>();
            _callbackQueryMock = A.Fake<CallbackQuery>();
            _callbackQueryMock.Message = _messageMock;

            _mainState = new MainState();
        }


        [Fact]
        public void BotOnMessageReceived_GreetsUserCorrectly()
        {
            var expected = ContextState.MainState;

            _messageMock.Text = "/start";

            var actual = _mainState.BotOnMessageReceived(_botClientMock, _messageMock).Result;

            A.CallTo(() => _botClientMock.SendTextMessageAsync(A<ChatId>.Ignored, A<string>.Ignored, A<ParseMode>.Ignored,
                                                           A<IEnumerable<MessageEntity>>.Ignored, A<bool>.Ignored,
                                                           A<bool>.Ignored, A<int>.Ignored, A<bool>.Ignored,
                                                           A<IReplyMarkup>.Ignored, A<CancellationToken>.Ignored))
                                                           .MustHaveHappenedOnceExactly();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BotOnMessageReceived_CreatesEventCorrectly()
        {
            var expected = ContextState.InputTitleState;

            _messageMock.Text = "/create";

            var actual = _mainState.BotOnMessageReceived(_botClientMock, _messageMock).Result;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BotOnMessageReceived_HandlesNonIncorrectMessages()
        {
            var expected = ContextState.MainState;

            _messageMock.Text = "test";

            var actual = _mainState.BotOnMessageReceived(_botClientMock, _messageMock).Result;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BotOnMessageReceived_HandlesNonTextTypeMessages()
        {
            var expected = ContextState.MainState;

            var actual = _mainState.BotOnMessageReceived(_botClientMock, _messageMock).Result;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BotOnCallBackQueryReceived_ReturnsSameState()
        {
            var expected = ContextState.MainState;

            var actual = _mainState.BotOnCallBackQueryReceived(_botClientMock, _callbackQueryMock).Result;

            Assert.Equal(expected, actual);
        }


        [Fact]
        public void BotSendMessage_ShouldSendTextMessage()
        {
            _mainState.BotSendMessage(_botClientMock, _messageMock.Chat.Id);

            A.CallTo(() => _botClientMock.SendTextMessageAsync(A<ChatId>.Ignored, A<string>.Ignored, A<ParseMode>.Ignored,
                                                           A<IEnumerable<MessageEntity>>.Ignored, A<bool>.Ignored,
                                                           A<bool>.Ignored, A<int>.Ignored, A<bool>.Ignored,
                                                           A<IReplyMarkup>.Ignored, A<CancellationToken>.Ignored))
                                                           .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void HandleInvalidInput_ShouldSendTextMessageAndReturnSameState()
        {
            var expected = ContextState.MainState;

            var actual = _mainState.HandleInvalidInput(_botClientMock, _messageMock.Chat.Id);

            A.CallTo(() => _botClientMock.SendTextMessageAsync(A<ChatId>.Ignored, A<string>.Ignored, A<ParseMode>.Ignored,
                                                           A<IEnumerable<MessageEntity>>.Ignored, A<bool>.Ignored,
                                                           A<bool>.Ignored, A<int>.Ignored, A<bool>.Ignored,
                                                           A<IReplyMarkup>.Ignored, A<CancellationToken>.Ignored))
                                                           .MustHaveHappenedOnceExactly();

            Assert.Equal(expected, actual);
        }

    }
}
