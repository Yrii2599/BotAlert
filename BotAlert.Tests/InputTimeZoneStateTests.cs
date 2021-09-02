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

namespace BotAlert.Tests
{
    public class InputTimeZoneStateTests
    {
        private readonly ITelegramBotClient _botClientMock;
        private readonly IStateProvider _stateProviderMock;
        private readonly Message _messageStub;
        private readonly CallbackQuery _callbackQueryStub;

        private readonly ContextState _currentState;

        private readonly InputTimeZoneState _inputTimeZoneState;

        public InputTimeZoneStateTests()
        {
            _botClientMock = A.Fake<ITelegramBotClient>();
            _stateProviderMock = A.Fake<IStateProvider>();
            _messageStub = new Message
            {
                Chat = new Chat()
            };
            _callbackQueryStub = new CallbackQuery
            {
                Message = _messageStub
            };

            _currentState = ContextState.InputTimeZoneState;

            _inputTimeZoneState = new InputTimeZoneState(_stateProviderMock);
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
        public void BotOnMessageReceived_MessageTextValid_SavesChatStateAndReturnsMainState()
        {
            var timeOffSet = 3;
            _messageStub.Text = timeOffSet.ToString();
            var expected = ContextState.MainState;
            var chatStub = new ChatState(_messageStub.Chat.Id, _currentState);
            A.CallTo(() => _stateProviderMock.GetChatState(_messageStub.Chat.Id))
             .Returns(chatStub);

            var actual = _inputTimeZoneState.BotOnMessageReceived(_botClientMock, _messageStub).Result;

            A.CallTo(() => _stateProviderMock.SaveChatState(chatStub)).MustHaveHappenedOnceExactly();
            Assert.Equal(expected, actual);
            Assert.Equal(timeOffSet, chatStub.TimeOffSet);
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

