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
    public class InputDateStateTests
    {
        private readonly IEventProvider _eventProviderMock;
        private readonly IStateProvider _stateProviderMock;
        private readonly ITelegramBotClient _botClientMock;
        private readonly Message _messageMock;

        private readonly ContextState _currentState;

        private readonly InputDateState _inputDateState;

        public InputDateStateTests()
        {
            _eventProviderMock = A.Fake<IEventProvider>();
            _stateProviderMock = A.Fake<IStateProvider>();
            _botClientMock = A.Fake<ITelegramBotClient>();
            _messageMock = A.Fake<Message>();
            _messageMock.Chat = A.Fake<Chat>();

            _currentState = ContextState.InputDateState;

            _inputDateState = new InputDateState(_eventProviderMock, _stateProviderMock);
        }

        [Fact] 
        public void BotOnMessageReceived_MessageTextNull_ReturnsCurrentState()
        {
            var expected = _currentState;

            var actual = _inputDateState.BotOnMessageReceived(_botClientMock, _messageMock).Result;

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

            var actual = _inputDateState.BotOnMessageReceived(_botClientMock, _messageMock).Result;

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
        public void BotOnMessageReceived_MessageTextIsADateAndInProgressEvent_ShouldUpdateDraftEvent()
        {
            var expected = ContextState.InputWarnDateKeyboardState;
            _messageMock.Text = "31.12.9999";

            var actual = _inputDateState.BotOnMessageReceived(_botClientMock, _messageMock).Result;

            A.CallTo(() => _eventProviderMock.UpdateEvent(A<Event>.Ignored))
                                             .MustHaveHappenedOnceExactly();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BotOnMessageReceived_MessageTextIsADateAndCreatedEvent_ShouldUpdateEvent()
        {
            var chatStateMock = A.Fake<ChatState>();
            chatStateMock.ActiveNotificationId = Guid.NewGuid();
            var evenMock = A.Fake<Event>();
            evenMock.Status = EventStatus.Created;
            A.CallTo(() => _stateProviderMock.GetChatState(_messageMock.Chat.Id)).Returns(chatStateMock);
            A.CallTo(() => _eventProviderMock.GetEventById(chatStateMock.ActiveNotificationId)).Returns(evenMock);

            _messageMock.Text = "31.12.9999";
            var expected = ContextState.InputWarnDateKeyboardState;


            var actual = _inputDateState.BotOnMessageReceived(_botClientMock, _messageMock).Result;


            A.CallTo(() => _eventProviderMock.UpdateEvent(A<Event>.That.Matches(e => e.ChatId == _messageMock.Chat.Id)))
                                             .MustHaveHappenedOnceExactly();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BotOnMessageReceived_MessageTextIsAnExpiredDate_ReturnsCurrentState()
        {
            var expected = _currentState;
            _messageMock.Text = "01.01.0001";

            var actual = _inputDateState.BotOnMessageReceived(_botClientMock, _messageMock).Result;

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
        public void BotOnCallBackQueryReceived_ReturnsCurrentState()
        {
            var callbackQueryMock = A.Fake<CallbackQuery>();
            callbackQueryMock.Message = _messageMock;
            var expected = _currentState;

            var actual = _inputDateState.BotOnCallBackQueryReceived(_botClientMock, callbackQueryMock).Result;

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
            _inputDateState.BotSendMessage(_botClientMock, _messageMock.Chat.Id);

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
        }
    }
}
