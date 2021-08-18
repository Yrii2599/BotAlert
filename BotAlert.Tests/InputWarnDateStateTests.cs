using System;
using System.Threading;
using System.Collections.Generic;
using BotAlert.States;
using BotAlert.Models;
using BotAlert.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using FakeItEasy;
using Xunit;

namespace BotAlert.Tests
{
    public class InputWarnDateStateTests
    {
        private readonly IEventProvider _eventProviderMock;
        private readonly IStateProvider _stateProviderMock;
        private readonly ITelegramBotClient _botClientMock;
        private readonly Message _messageMock;
        private readonly Event _eventMock;

        private readonly ContextState _currentState;

        private readonly InputWarnDateState _inputWarnDateState;

        public InputWarnDateStateTests()
        {
            _eventProviderMock = A.Fake<IEventProvider>();
            _stateProviderMock = A.Fake<IStateProvider>();
            _botClientMock = A.Fake<ITelegramBotClient>();
            _messageMock = A.Fake<Message>();
            _messageMock.Chat = A.Fake<Chat>();
            _eventMock = A.Fake<Event>();
            _eventMock.ChatId = _messageMock.Chat.Id;
            _eventMock.Date = new DateTime(9999, 12, 31);

            _currentState = ContextState.InputWarnDateState;

            _inputWarnDateState = new InputWarnDateState(_eventProviderMock, _stateProviderMock);
        }

        [Fact]
        public void BotOnMessageReceived_MessageTextNull_ReturnsCurrentState()
        {
            var expected = _currentState;

            var actual = _inputWarnDateState.BotOnMessageReceived(_botClientMock, _messageMock).Result;

            A.CallTo(() => _botClientMock.SendTextMessageAsync(A<ChatId>.Ignored,
                                                               A<string>.Ignored,
                                                               A<ParseMode>.Ignored,
                                                               A<IEnumerable<MessageEntity>>.Ignored,
                                                               A<bool>.Ignored,
                                                               A<bool>.Ignored,
                                                               A<int>.Ignored,
                                                               A<bool>.Ignored,
                                                               A<IReplyMarkup>.Ignored,
                                                               A<CancellationToken>.Ignored))
                                                              .MustHaveHappenedOnceExactly();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BotOnMessageReceived_MessageTextIsNotADate_ReturnsCurrentState()
        {
            var expected = _currentState;
            _messageMock.Text = "Text";

            var actual = _inputWarnDateState.BotOnMessageReceived(_botClientMock, _messageMock).Result;

            A.CallTo(() => _botClientMock.SendTextMessageAsync(A<ChatId>.Ignored,
                                                               A<string>.Ignored,
                                                               A<ParseMode>.Ignored,
                                                               A<IEnumerable<MessageEntity>>.Ignored,
                                                               A<bool>.Ignored,
                                                               A<bool>.Ignored,
                                                               A<int>.Ignored,
                                                               A<bool>.Ignored,
                                                               A<IReplyMarkup>.Ignored,
                                                               A<CancellationToken>.Ignored))
                                                              .MustHaveHappenedOnceExactly();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BotOnMessageReceived_MessageTextIsADateSmallerThanEventDateAndInProgressEvent_ReturnsInputDescriptionKeyboardState()
        {
            _eventMock.Status = EventStatus.InProgress;
            A.CallTo(() => _eventProviderMock.GetEventById(A<Guid>.Ignored)).Returns(_eventMock);
            _messageMock.Text = "30.12.9999";
            var expected = ContextState.InputDescriptionKeyboardState;

            var actual = _inputWarnDateState.BotOnMessageReceived(_botClientMock, _messageMock).Result;

            A.CallTo(() => _eventProviderMock.UpdateEvent(A<Event>.Ignored)).MustHaveHappenedOnceExactly();

            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void BotOnMessageReceived_MessageTextIsADateSmallerThanEventDateAndCreatedEvent_ReturnsEditState()
        {
            var chatStateMock = A.Fake<ChatState>();
            chatStateMock.ActiveNotificationId = Guid.NewGuid();
            _eventMock.Status = EventStatus.Created;
            A.CallTo(() => _stateProviderMock.GetChatState(_messageMock.Chat.Id)).Returns(chatStateMock);
            A.CallTo(() => _eventProviderMock.GetEventById(chatStateMock.ActiveNotificationId)).Returns(_eventMock);

            _messageMock.Text = "30.12.9999";
            var expected = ContextState.EditState;


            var actual = _inputWarnDateState.BotOnMessageReceived(_botClientMock, _messageMock).Result;


            A.CallTo(() => _eventProviderMock.UpdateEvent(A<Event>.That.Matches(e => e.ChatId == _messageMock.Chat.Id)))
                                             .MustHaveHappenedOnceExactly();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BotOnMessageReceived_MessageTextIsAnExpiredDateAndNoActiveNotification_ReturnsCurrentState()
        {
            _messageMock.Text = "01.01.0001";
            var expected = _currentState;

            var actual = _inputWarnDateState.BotOnMessageReceived(_botClientMock, _messageMock).Result;

            A.CallTo(() => _botClientMock.SendTextMessageAsync(A<ChatId>.That.Matches(id => id == _messageMock.Chat.Id),
                                                               A<string>.Ignored,
                                                               A<ParseMode>.Ignored,
                                                               A<IEnumerable<MessageEntity>>.Ignored,
                                                               A<bool>.Ignored,
                                                               A<bool>.Ignored,
                                                               A<int>.Ignored,
                                                               A<bool>.Ignored,
                                                               A<IReplyMarkup>.Ignored,
                                                               A<CancellationToken>.Ignored))
                                                              .MustHaveHappenedOnceExactly();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BotOnMessageReceived_MessageTextIsAnExpiredDateAndActiveNotification_ReturnsCurrentState()
        {
            var chatStateMock = A.Fake<ChatState>();
            chatStateMock.ActiveNotificationId = Guid.NewGuid();
            A.CallTo(() => _stateProviderMock.GetChatState(_messageMock.Chat.Id)).Returns(chatStateMock);

            _messageMock.Text = "01.01.0001";
            var expected = _currentState;


            var actual = _inputWarnDateState.BotOnMessageReceived(_botClientMock, _messageMock).Result;


            A.CallTo(() => _botClientMock.SendTextMessageAsync(A<ChatId>.That.Matches(id => id == _messageMock.Chat.Id),
                                                               A<string>.Ignored,
                                                               A<ParseMode>.Ignored,
                                                               A<IEnumerable<MessageEntity>>.Ignored,
                                                               A<bool>.Ignored,
                                                               A<bool>.Ignored,
                                                               A<int>.Ignored,
                                                               A<bool>.Ignored,
                                                               A<IReplyMarkup>.Ignored,
                                                               A<CancellationToken>.Ignored))
                                                              .MustHaveHappenedOnceExactly();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BotOnCallBackQueryReceived_ReturnsCurrentState()
        {
            var callbackQueryMock = A.Fake<CallbackQuery>();
            callbackQueryMock.Message = _messageMock;
            var expected = _currentState;

            var actual = _inputWarnDateState.BotOnCallBackQueryReceived(_botClientMock, callbackQueryMock).Result;

            A.CallTo(() => _botClientMock.AnswerCallbackQueryAsync(A<string>.Ignored,
                                                                   A<string>.Ignored,
                                                                   A<bool>.Ignored,
                                                                   A<string>.Ignored,
                                                                   A<int>.Ignored,
                                                                   A<CancellationToken>.Ignored))
                                                                  .MustHaveHappenedOnceExactly();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BotSendMessage_SendsTextMessage()
        {
            _inputWarnDateState.BotSendMessage(_botClientMock, _messageMock.Chat.Id);

            A.CallTo(() => _botClientMock.SendTextMessageAsync(A<ChatId>.That.Matches(id => id == _messageMock.Chat.Id),
                                                               A<string>.Ignored,
                                                               A<ParseMode>.Ignored,
                                                               A<IEnumerable<MessageEntity>>.Ignored,
                                                               A<bool>.Ignored,
                                                               A<bool>.Ignored,
                                                               A<int>.Ignored,
                                                               A<bool>.Ignored,
                                                               A<IReplyMarkup>.Ignored,
                                                               A<CancellationToken>.Ignored))
                                                              .MustHaveHappenedOnceExactly();
        }
    }
}
