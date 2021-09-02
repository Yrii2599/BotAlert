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
        private readonly Message _messageStub;
        private readonly Event _eventStub;
        private readonly ChatState _chatStateStub;

        private readonly ContextState _currentState;

        private readonly InputWarnDateState _inputWarnDateState;

        public InputWarnDateStateTests()
        {
            _eventProviderMock = A.Fake<IEventProvider>();
            _stateProviderMock = A.Fake<IStateProvider>();
            _botClientMock = A.Fake<ITelegramBotClient>();
            _messageStub = new Message();
            _messageStub.Chat = new Chat();
            _eventStub = new Event(_messageStub.Chat.Id, "Title");
            _eventStub.Date = new DateTime(9999, 12, 31);
            _chatStateStub = new ChatState(_messageStub.Chat.Id);

            _currentState = ContextState.InputWarnDateState;

            _inputWarnDateState = new InputWarnDateState(_eventProviderMock, _stateProviderMock);
        }

        [Fact]
        public void BotOnMessageReceived_MessageTextNull_ReturnsCurrentState()
        {
            var expected = _currentState;

            var actual = _inputWarnDateState.BotOnMessageReceived(_botClientMock, _messageStub).Result;

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
            _messageStub.Text = "Text";

            var actual = _inputWarnDateState.BotOnMessageReceived(_botClientMock, _messageStub).Result;

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
            _eventStub.Status = EventStatus.InProgress;
            A.CallTo(() => _eventProviderMock.GetEventById(A<Guid>.Ignored)).Returns(_eventStub);
            _messageStub.Text = "30.12.9999";
            var expected = ContextState.InputDescriptionKeyboardState;

            var actual = _inputWarnDateState.BotOnMessageReceived(_botClientMock, _messageStub).Result;

            A.CallTo(() => _eventProviderMock.UpdateEvent(A<Event>.Ignored)).MustHaveHappenedOnceExactly();

            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void BotOnMessageReceived_MessageTextIsADateSmallerThanEventDateAndCreatedEvent_ReturnsEditState()
        {
            _chatStateStub.ActiveNotificationId = Guid.NewGuid();
            _eventStub.Status = EventStatus.Created;
            A.CallTo(() => _stateProviderMock.GetChatState(_messageStub.Chat.Id)).Returns(_chatStateStub);
            A.CallTo(() => _eventProviderMock.GetEventById(_chatStateStub.ActiveNotificationId)).Returns(_eventStub);

            _messageStub.Text = "30.12.9999";
            var expected = ContextState.EditState;

            var actual = _inputWarnDateState.BotOnMessageReceived(_botClientMock, _messageStub).Result;

            A.CallTo(() => _eventProviderMock.UpdateEvent(A<Event>.That.Matches(e => e.ChatId == _messageStub.Chat.Id)))
                                             .MustHaveHappenedOnceExactly();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BotOnMessageReceived_MessageTextIsAnExpiredDateAndNoActiveNotification_ReturnsCurrentState()
        {
            _messageStub.Text = "01.01.0001";
            var expected = _currentState;

            var actual = _inputWarnDateState.BotOnMessageReceived(_botClientMock, _messageStub).Result;

            A.CallTo(() => _botClientMock.SendTextMessageAsync(A<ChatId>.That.Matches(id => id == _messageStub.Chat.Id),
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
            _chatStateStub.ActiveNotificationId = Guid.NewGuid();
            A.CallTo(() => _stateProviderMock.GetChatState(_messageStub.Chat.Id)).Returns(_chatStateStub);

            _messageStub.Text = "01.01.0001";
            var expected = _currentState;


            var actual = _inputWarnDateState.BotOnMessageReceived(_botClientMock, _messageStub).Result;


            A.CallTo(() => _botClientMock.SendTextMessageAsync(A<ChatId>.That.Matches(id => id == _messageStub.Chat.Id),
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
        public void BotOnMessageReceived_EventIsNull_ShouldReturnMainState()
        {
            var activeId = Guid.NewGuid();
            _chatStateStub.ActiveNotificationId = activeId;
            A.CallTo(() => _stateProviderMock.GetChatState(_messageStub.Chat.Id)).Returns(_chatStateStub);
            A.CallTo(() => _eventProviderMock.GetEventById(_chatStateStub.ActiveNotificationId)).Returns(null);
            _messageStub.Text = "30.12.9999";
            var expected = ContextState.MainState;

            var actual = _inputWarnDateState.BotOnMessageReceived(_botClientMock, _messageStub).Result;

            A.CallTo(() => _stateProviderMock.GetChatState(_messageStub.Chat.Id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _eventProviderMock.GetEventById(activeId)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _stateProviderMock.SaveChatState(_chatStateStub)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _botClientMock.SendTextMessageAsync(_chatStateStub.ChatId,
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
            callbackQueryMock.Message = _messageStub;
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
            _inputWarnDateState.BotSendMessage(_botClientMock, _messageStub.Chat.Id);

            A.CallTo(() => _botClientMock.SendTextMessageAsync(A<ChatId>.That.Matches(id => id == _messageStub.Chat.Id),
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
