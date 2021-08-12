using BotAlert.States;
using BotAlert.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using FakeItEasy;
using Xunit;
using Telegram.Bot.Types.Enums;
using System.Collections.Generic;
using Telegram.Bot.Types.ReplyMarkups;
using System.Threading;
using BotAlert.Models;

namespace BotAlert.Tests
{
    public class InputTitleStateTests
    {

        private readonly IEventProvider _eventProviderMock;
        private readonly ITelegramBotClient _botClientMock;
        private readonly Message _messageMock;
        private readonly CallbackQuery _callbackQueryMock;

        private readonly IState _inputTitleState;

        public InputTitleStateTests()
        {
            _eventProviderMock = A.Fake<IEventProvider>();
            _botClientMock = A.Fake<ITelegramBotClient>();
            _messageMock = A.Fake<Message>();
            _messageMock.Chat = A.Fake<Chat>();
            _callbackQueryMock = A.Fake<CallbackQuery>();

            _inputTitleState = new InputTitleState(_eventProviderMock);
        }

        [Fact]
        public void BotOnMessageReceived_ShouldCreateEvent()
        {
            _messageMock.Text = "";

            var expected = ContextState.InputDateState;

            var actual = _inputTitleState.BotOnMessageReceived(_botClientMock, _messageMock).Result;

            A.CallTo(() => _eventProviderMock.CreateEvent(A<Event>.Ignored)).MustHaveHappenedOnceExactly();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BotOnMessageReceived_ShouldHandleMessagelessInput()
        {
            var expected = ContextState.InputTitleState;

            var actual = _inputTitleState.BotOnMessageReceived(_botClientMock, _messageMock).Result;

            A.CallTo(() => _botClientMock.SendTextMessageAsync(A<ChatId>.Ignored, A<string>.Ignored, A<ParseMode>.Ignored,
                                                           A<IEnumerable<MessageEntity>>.Ignored, A<bool>.Ignored,
                                                           A<bool>.Ignored, A<int>.Ignored, A<bool>.Ignored,
                                                           A<IReplyMarkup>.Ignored, A<CancellationToken>.Ignored))
                                                           .MustHaveHappenedOnceExactly();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BotOnCallBackQueryReceived_ShouldReturnSameState()
        {
            var expected = ContextState.InputTitleState;

            var actual = _inputTitleState.BotOnCallBackQueryReceived(_botClientMock, _callbackQueryMock).Result;

            A.CallTo(() => _botClientMock.AnswerCallbackQueryAsync(A<string>.Ignored, A<string>.Ignored, A<bool>.Ignored,
                                                               A<string>.Ignored, A<int>.Ignored, A<CancellationToken>.Ignored))
                                                               .MustHaveHappenedOnceExactly();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BotSendMessage_ShouldSendTextMessage()
        {
            _inputTitleState.BotSendMessage(_botClientMock, _messageMock.Chat.Id);

            A.CallTo(() => _botClientMock.SendTextMessageAsync(A<ChatId>.Ignored, A<string>.Ignored, A<ParseMode>.Ignored,
                                                           A<IEnumerable<MessageEntity>>.Ignored, A<bool>.Ignored,
                                                           A<bool>.Ignored, A<int>.Ignored, A<bool>.Ignored,
                                                           A<IReplyMarkup>.Ignored, A<CancellationToken>.Ignored))
                                                           .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void HandleInvalidInput_ShouldSendTextMessageAndReturnSameState()
        {
            var expected = ContextState.InputTitleState;

            var actual = _inputTitleState.HandleInvalidInput(_botClientMock, _messageMock.Chat.Id);

            A.CallTo(() => _botClientMock.SendTextMessageAsync(A<ChatId>.Ignored, A<string>.Ignored, A<ParseMode>.Ignored,
                                                           A<IEnumerable<MessageEntity>>.Ignored, A<bool>.Ignored,
                                                           A<bool>.Ignored, A<int>.Ignored, A<bool>.Ignored,
                                                           A<IReplyMarkup>.Ignored, A<CancellationToken>.Ignored))
                                                           .MustHaveHappenedOnceExactly();

            Assert.Equal(expected, actual);
        }
    }
}
