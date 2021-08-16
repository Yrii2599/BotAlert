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
using System;

namespace BotAlert.Tests
{
    public class InputTitleStateTests
    {
        private readonly IEventProvider _eventProviderMock;
        private readonly IStateProvider _stateProviderMock;
        private readonly ITelegramBotClient _botClientMock;
        private readonly Message _messageMock;
        private readonly CallbackQuery _callbackQueryMock;

        private readonly IState _inputTitleState;

        public InputTitleStateTests()
        {
            _eventProviderMock = A.Fake<IEventProvider>();
            _stateProviderMock = A.Fake<IStateProvider>();
            _botClientMock = A.Fake<ITelegramBotClient>();
            _messageMock = A.Fake<Message>();
            _messageMock.Chat = A.Fake<Chat>();
            _callbackQueryMock = A.Fake<CallbackQuery>();

            _inputTitleState = new InputTitleState(_eventProviderMock, _stateProviderMock);
        }

        [Fact]
        public void BotOnMessageReceived_WithTextAndNoActiveNotification_ShouldCreateEvent()
        {
            var expected = ContextState.InputDateState;
            _messageMock.Text = "";

            var actual = _inputTitleState.BotOnMessageReceived(_botClientMock, _messageMock).Result;

            A.CallTo(() => _eventProviderMock.CreateEvent(A<Event>.That.Matches(e => e.ChatId == _messageMock.Chat.Id)))
                                             .MustHaveHappenedOnceExactly();
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void BotOnMessageReceived_WithTextAndActiveNotification_ShouldUpdateEvent()
        {
            var chatStateMock = A.Fake<ChatState>();
            chatStateMock.ActiveNotificationId = Guid.NewGuid();
            A.CallTo(() => _stateProviderMock.GetChatState(_messageMock.Chat.Id)).Returns(chatStateMock);

            _messageMock.Text = "";
            var expected = ContextState.EditState;


            var actual = _inputTitleState.BotOnMessageReceived(_botClientMock, _messageMock).Result;


            A.CallTo(() => _eventProviderMock.UpdateEvent(A<Event>.That.Matches(e => e.ChatId == _messageMock.Chat.Id)))
                                             .MustHaveHappenedOnceExactly();
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
    }
}
