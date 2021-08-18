using System;
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

namespace BotAlert.Tests
{
    public class InputWarnDateKeyboardStateTests
    {
        private readonly IEventProvider _eventProviderMock;
        private readonly IStateProvider _stateProviderMock;
        private readonly ITelegramBotClient _botClientMock;
        private readonly Message _messageMock;
        private readonly CallbackQuery _callbackQueryMock;
        private readonly Event _eventMock;

        private readonly ContextState _currentState;

        private readonly InputWarnDateKeyboardState _inputWarnDateKeyboardState;

        public InputWarnDateKeyboardStateTests()
        {
            _eventProviderMock = A.Fake<IEventProvider>();
            _stateProviderMock = A.Fake<IStateProvider>();
            _botClientMock = A.Fake<ITelegramBotClient>();
            _messageMock = A.Fake<Message>();
            _messageMock.Chat = A.Fake<Chat>();
            _callbackQueryMock = A.Fake<CallbackQuery>();
            _callbackQueryMock.Message = _messageMock;
            _eventMock = A.Fake<Event>();
            _eventMock.ChatId = _messageMock.Chat.Id;
            _eventMock.Date = DateTime.Now;

            _currentState = ContextState.InputWarnDateKeyboardState;

            _inputWarnDateKeyboardState = new InputWarnDateKeyboardState(_eventProviderMock, _stateProviderMock);
        }

        [Fact]
        public void BotOnMessageReceived_ReturnsCurrentState()
        {
            var expected = _currentState;

            var actual = _inputWarnDateKeyboardState.BotOnMessageReceived(_botClientMock, _messageMock).Result;

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
        public void BotOnCallBackQueryReceived_OwnData_ReturnsInputWarnDateState()
        {
            _callbackQueryMock.Data = "own";
            var expected = ContextState.InputWarnDateState;

            var actual = _inputWarnDateKeyboardState.BotOnCallBackQueryReceived(_botClientMock, _callbackQueryMock).Result;

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
        public void BotOnCallbackQueryReceived_HardcodedDataAndInProgressEvent_ReturnsInputDescriptionKeyboardState()
        {
            _eventMock.Status = EventStatus.InProgress;
            A.CallTo(() => _eventProviderMock.GetEventById(A<Guid>.Ignored)).Returns(_eventMock);
            _callbackQueryMock.Data = "30";
            var expected = ContextState.InputDescriptionKeyboardState;


            var actual = _inputWarnDateKeyboardState.BotOnCallBackQueryReceived(_botClientMock, _callbackQueryMock).Result;


            A.CallTo(() => _botClientMock.AnswerCallbackQueryAsync(A<string>.Ignored,
                                                                   A<string>.Ignored,
                                                                   A<bool>.Ignored,
                                                                   A<string>.Ignored,
                                                                   A<int>.Ignored,
                                                                   A<CancellationToken>.Ignored))
                                                                  .MustHaveHappenedOnceExactly();

            A.CallTo(() => _eventProviderMock.UpdateEvent(A<Event>.That.Matches(e => e.ChatId == _messageMock.Chat.Id)))
                                             .MustHaveHappenedOnceExactly();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BotOnCallbackQueryReceived_HardcodedDataAndActiveNotification_ReturnsEditState()
        {
            _eventMock.Status = EventStatus.Created;
            var chatStateMock = A.Fake<ChatState>();
            chatStateMock.ActiveNotificationId = Guid.NewGuid();
            A.CallTo(() => _stateProviderMock.GetChatState(_messageMock.Chat.Id)).Returns(chatStateMock);
            A.CallTo(() => _eventProviderMock.GetEventById(A<Guid>.Ignored)).Returns(_eventMock);
            _callbackQueryMock.Data = "30";
            var expected = ContextState.EditState;


            var actual = _inputWarnDateKeyboardState.BotOnCallBackQueryReceived(_botClientMock, _callbackQueryMock).Result;


            A.CallTo(() => _botClientMock.AnswerCallbackQueryAsync(A<string>.Ignored,
                                                                   A<string>.Ignored,
                                                                   A<bool>.Ignored,
                                                                   A<string>.Ignored,
                                                                   A<int>.Ignored,
                                                                   A<CancellationToken>.Ignored))
                                                                  .MustHaveHappenedOnceExactly();

            A.CallTo(() => _eventProviderMock.UpdateEvent(A<Event>.That.Matches(e => e.ChatId == _messageMock.Chat.Id)))
                                             .MustHaveHappenedOnceExactly();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BotSendMessage_SendsTextMessage()
        {
            _inputWarnDateKeyboardState.BotSendMessage(_botClientMock, _messageMock.Chat.Id);

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
