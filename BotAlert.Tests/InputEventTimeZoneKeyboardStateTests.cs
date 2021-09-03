using System.Threading;
using System.Collections.Generic;
using BotAlert.Interfaces;
using BotAlert.States;
using BotAlert.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using FakeItEasy;
using Xunit;
using System;

namespace BotAlert.Tests
{
    public class InputEventTimeZoneKeyboardStateTests
    {
        private readonly ITelegramBotClient _botClientMock;
        private readonly IStateProvider _stateProviderMock;
        private readonly IEventProvider _eventProviderMock;
        private readonly Message _messageMock;
        private readonly CallbackQuery _callbackQueryMock;
        private readonly ChatState _chatStub;
        private readonly Event _eventStub;

        private readonly ContextState _currentState;

        private readonly InputEventTimeZoneKeyboardState _inputEventTimeZoneKeyboardState;

        public InputEventTimeZoneKeyboardStateTests()
        {
            _botClientMock = A.Fake<ITelegramBotClient>();
            _stateProviderMock = A.Fake<IStateProvider>();
            _eventProviderMock = A.Fake<IEventProvider>();
            _messageMock = A.Fake<Message>();
            _messageMock.Chat = A.Fake<Chat>();
            _callbackQueryMock = A.Fake<CallbackQuery>();
            _callbackQueryMock.Message = _messageMock;

            _currentState = ContextState.InputEventTimeZoneKeyboardState;

            _chatStub = new ChatState(_messageMock.Chat.Id, _currentState);
            _eventStub = new Event(_messageMock.Chat.Id, "Title");

            _inputEventTimeZoneKeyboardState = new InputEventTimeZoneKeyboardState(_stateProviderMock, _eventProviderMock);
        }

        [Fact]
        public void BotOnMessageReceived_TextNull_ReturnsCurrentState()
        {
            var expected = _currentState;

            var actual = _inputEventTimeZoneKeyboardState.BotOnMessageReceived(_botClientMock, _messageMock).Result;

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
        public void BotOnMessageReceived_TextAccept_ReturnsInputTimeZoneState()
        {
            var expected = ContextState.InputTimeZoneState;
            _messageMock.Text = "да";

            var actual = _inputEventTimeZoneKeyboardState.BotOnMessageReceived(_botClientMock, _messageMock).Result;

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
                                                              .MustNotHaveHappened();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BotOnMessageReceived_TextDecline_UpdatesEventReturnsInputDateState()
        {
            _messageMock.Text = "нет";
            var expected = ContextState.InputDateState;
            var expectedTimeOffSet = 3;
            _eventStub.Id = Guid.NewGuid();
            _chatStub.ActiveNotificationId = _eventStub.Id;
            _chatStub.TimeOffSet = expectedTimeOffSet;
            A.CallTo(() => _stateProviderMock.GetChatState(_messageMock.Chat.Id)).Returns(_chatStub);
            A.CallTo(() => _eventProviderMock.GetEventById(_chatStub.ActiveNotificationId)).Returns(_eventStub);

            var actual = _inputEventTimeZoneKeyboardState.BotOnMessageReceived(_botClientMock, _messageMock).Result;
            var actualTimeOffSet = _eventStub.TimeOffSet;

            A.CallTo(() => _eventProviderMock.UpdateEvent(_eventStub)).MustHaveHappenedOnceExactly();
            Assert.Equal(expected, actual);
            Assert.Equal(expectedTimeOffSet, actualTimeOffSet);
        }

        [Fact]
        public void BotOnCallBackQueryReceived_DataAccept_ReturnsInputTimeZoneState()
        {
            _callbackQueryMock.Data = "да";
            var expected = ContextState.InputTimeZoneState;

            var actual = _inputEventTimeZoneKeyboardState.BotOnCallBackQueryReceived(_botClientMock, _callbackQueryMock).Result;

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
        public void BotOnCallBackQueryReceived_DataDecline_ReturnsInputDateState()
        {
            _callbackQueryMock.Data = "нет";
            var expected = ContextState.InputDateState;
            var expectedTimeOffSet = 3;
            _eventStub.Id = Guid.NewGuid();
            _chatStub.ActiveNotificationId = _eventStub.Id;
            _chatStub.TimeOffSet = expectedTimeOffSet;
            A.CallTo(() => _stateProviderMock.GetChatState(_messageMock.Chat.Id)).Returns(_chatStub);
            A.CallTo(() => _eventProviderMock.GetEventById(_chatStub.ActiveNotificationId)).Returns(_eventStub);


            var actual = _inputEventTimeZoneKeyboardState.BotOnCallBackQueryReceived(_botClientMock, _callbackQueryMock).Result;
            var actualTimeOffSet = _eventStub.TimeOffSet;


            A.CallTo(() => _botClientMock.AnswerCallbackQueryAsync(A<string>.Ignored,
                                                                   A<string>.Ignored,
                                                                   A<bool>.Ignored,
                                                                   A<string>.Ignored,
                                                                   A<int>.Ignored,
                                                                   A<CancellationToken>.Ignored))
                                                                  .MustHaveHappenedOnceExactly();
            A.CallTo(() => _eventProviderMock.UpdateEvent(_eventStub)).MustHaveHappenedOnceExactly();
            Assert.Equal(expected, actual);
            Assert.Equal(expectedTimeOffSet, actualTimeOffSet);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BotOnCallBackQueryReceived_DataNotValid_ReturnsCurrentState()
        {
            _callbackQueryMock.Data = "fsdkngn";
            var expected = _currentState;

            var actual = _inputEventTimeZoneKeyboardState.BotOnCallBackQueryReceived(_botClientMock, _callbackQueryMock).Result;

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
            A.CallTo(() => _stateProviderMock.GetChatState(_messageMock.Chat.Id)).Returns(_chatStub);

            _inputEventTimeZoneKeyboardState.BotSendMessage(_botClientMock, _messageMock.Chat.Id);

            A.CallTo(() => _botClientMock.SendTextMessageAsync(_messageMock.Chat.Id,
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
