using System.Collections.Generic;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using BotAlert.Interfaces;
using BotAlert.Models;
using BotAlert.States;
using FakeItEasy;
using Xunit;

using System;


namespace BotAlert.Tests
{
    public class InputTimeZoneStateTests
    {
        private readonly ITelegramBotClient _botClientMock;
        private readonly IStateProvider _stateProviderMock;

        private readonly IEventProvider _eventProviderMock;
        private readonly Message _messageStub;
        private readonly CallbackQuery _callbackQueryStub;
        private readonly ChatState _chatStub;
        private readonly Event _eventStub;


        private readonly ContextState _currentState;

        private readonly InputTimeZoneState _inputTimeZoneState;

        public InputTimeZoneStateTests()
        {
            _botClientMock = A.Fake<ITelegramBotClient>();
            _stateProviderMock = A.Fake<IStateProvider>();
            _eventProviderMock = A.Fake<IEventProvider>();

            _messageStub = new Message
            {
                Chat = new Chat()
            };
            _callbackQueryStub = new CallbackQuery
            {
                Message = _messageStub
            };
            _chatStub = new ChatState(_messageStub.Chat.Id, _currentState);
            _eventStub = new Event(_messageStub.Chat.Id, "Title");

            _currentState = ContextState.InputTimeZoneState;

            _inputTimeZoneState = new InputTimeZoneState(_stateProviderMock, _eventProviderMock);

        }

        [Fact]
        public void BotOnMessageReceived_MessageTextNull_ReturnsCurrentState()
        {
            var expected = _currentState;

            var actual = _inputTimeZoneState.BotOnMessageReceived(_botClientMock, _messageStub).Result;

            A.CallTo(() => _botClientMock.SendTextMessageAsync(_messageStub.Chat.Id,
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
        public void BotOnMessageReceived_MessageTextNotAnInt_ReturnsCurrentState()
        {
            _messageStub.Text = "not an int";
            var expected = _currentState;

            var actual = _inputTimeZoneState.BotOnMessageReceived(_botClientMock, _messageStub).Result;

            A.CallTo(() => _botClientMock.SendTextMessageAsync(_messageStub.Chat.Id,
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
        public void BotOnMessageReceived_MessageIntLowerThanNegative12_ReturnsCurrentState()
        {
            _messageStub.Text = "-13";
            var expected = _currentState;

            var actual = _inputTimeZoneState.BotOnMessageReceived(_botClientMock, _messageStub).Result;

            A.CallTo(() => _botClientMock.SendTextMessageAsync(_messageStub.Chat.Id,
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
        public void BotOnMessageReceived_MessageIntBiggerThan14_ReturnsCurrentState()
        {
            _messageStub.Text = "15";
            var expected = _currentState;

            var actual = _inputTimeZoneState.BotOnMessageReceived(_botClientMock, _messageStub).Result;

            A.CallTo(() => _botClientMock.SendTextMessageAsync(_messageStub.Chat.Id,
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
        public void BotOnMessageReceived_MessageTextValidButNoActiveNotification_SavesChatStateAndReturnsMainState()
        {
            var timeOffSet = 3;
            _messageStub.Text = timeOffSet.ToString();
            _chatStub.ActiveNotificationId = Guid.Empty;
            var expected = ContextState.MainState;
            A.CallTo(() => _stateProviderMock.GetChatState(_messageStub.Chat.Id))
             .Returns(_chatStub);

            var actual = _inputTimeZoneState.BotOnMessageReceived(_botClientMock, _messageStub).Result;

            A.CallTo(() => _stateProviderMock.SaveChatState(_chatStub)).MustHaveHappenedOnceExactly();
            Assert.Equal(expected, actual);
            Assert.Equal(timeOffSet, _chatStub.TimeOffSet);
        }

        [Fact]
        public void BotOnMessageReceived_MessageTextValidButEventObjNull_SavesChatStateAndReturnsMainState()
        {
            var timeOffSet = 3;
            _messageStub.Text = timeOffSet.ToString();
            _chatStub.ActiveNotificationId = Guid.NewGuid();
            var expected = ContextState.MainState;
            A.CallTo(() => _stateProviderMock.GetChatState(_messageStub.Chat.Id))
             .Returns(_chatStub);
            A.CallTo(() => _eventProviderMock.GetEventById(_chatStub.ActiveNotificationId)).Returns(null);

            var actual = _inputTimeZoneState.BotOnMessageReceived(_botClientMock, _messageStub).Result;

            A.CallTo(() => _stateProviderMock.SaveChatState(_chatStub)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _botClientMock.SendTextMessageAsync(_chatStub.ChatId,
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
        public void BotOnMessageReceived_MessageTextValid_SavesChatStateAndReturnsInputDateState()
        {
            var timeOffSet = 3;
            _messageStub.Text = timeOffSet.ToString();
            _chatStub.ActiveNotificationId = Guid.NewGuid();
            var expected = ContextState.InputDateState;
            A.CallTo(() => _stateProviderMock.GetChatState(_messageStub.Chat.Id)).Returns(_chatStub);
            A.CallTo(() => _eventProviderMock.GetEventById(_chatStub.ActiveNotificationId)).Returns(_eventStub);


            var actual = _inputTimeZoneState.BotOnMessageReceived(_botClientMock, _messageStub).Result;

            A.CallTo(() => _eventProviderMock.UpdateEvent(_eventStub)).MustHaveHappenedOnceExactly();
            Assert.Equal(expected, actual);
            Assert.Equal(timeOffSet, _eventStub.TimeOffSet);

        }

        [Fact]
        public void BotOnCallBackQueryReceived_ReturnsCurrentState()
        {
            var expected = _currentState;

            var actual = _inputTimeZoneState.BotOnCallBackQueryReceived(_botClientMock, _callbackQueryStub).Result;

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
            _inputTimeZoneState.BotSendMessage(_botClientMock, _messageStub.Chat.Id);

            A.CallTo(() => _botClientMock.SendTextMessageAsync(_messageStub.Chat.Id,
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

