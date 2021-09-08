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
    public class InputTitleStateTests
    {
        private readonly IEventProvider _eventProviderMock;
        private readonly IStateProvider _stateProviderMock;
        private readonly ILocalizerFactory _localizerFactory;
        private readonly ITelegramBotClient _botClientMock;
        private readonly Message _messageStub;
        private readonly ChatState _chatStateStub;
        private readonly CallbackQuery _callbackQueryStub;

        private readonly IState _inputTitleState;

        public InputTitleStateTests()
        {
            _eventProviderMock = A.Fake<IEventProvider>();
            _stateProviderMock = A.Fake<IStateProvider>();
            _localizerFactory = A.Fake<ILocalizerFactory>();
            _botClientMock = A.Fake<ITelegramBotClient>();
            _messageStub = new Message();
            _messageStub.Chat = new Chat();
            _callbackQueryStub = new CallbackQuery();
            _chatStateStub = new ChatState(1234);

            _inputTitleState = new InputTitleState(_eventProviderMock, _stateProviderMock, _localizerFactory);
        }

        [Fact]
        public void BotOnMessageReceived_WithTextAndNoActiveNotification_ShouldCreateEvent()
        {
            var expected = ContextState.InputEventTimeZoneKeyboardState;
            _messageStub.Text = "";
            A.CallTo(() => _stateProviderMock.GetChatState(_messageStub.Chat.Id)).Returns(_chatStateStub);

            var actual = _inputTitleState.BotOnMessageReceived(_botClientMock, _messageStub).Result;

            A.CallTo(() => _eventProviderMock.CreateEvent(A<Event>.That.Matches(e => e.ChatId == _messageStub.Chat.Id)))
                                             .MustHaveHappenedOnceExactly();
            A.CallTo(() => _stateProviderMock.SaveChatState(_chatStateStub)).MustHaveHappenedOnceExactly();
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void BotOnMessageReceived_WithTextAndActiveNotification_ShouldUpdateEvent()
        {
            var eventStub = new Event(1234, "Title");
            _chatStateStub.ActiveNotificationId = Guid.NewGuid();
            A.CallTo(() => _stateProviderMock.GetChatState(_messageStub.Chat.Id)).Returns(_chatStateStub);
            A.CallTo(() => _eventProviderMock.GetEventById(_chatStateStub.ActiveNotificationId)).Returns(eventStub);
            _messageStub.Text = "";
            var expected = ContextState.EditState;

            var actual = _inputTitleState.BotOnMessageReceived(_botClientMock, _messageStub).Result;

            A.CallTo(() => _eventProviderMock.GetEventById(_chatStateStub.ActiveNotificationId)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _eventProviderMock.UpdateEvent(eventStub)).MustHaveHappenedOnceExactly();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BotOnMessageReceived_ShouldHandleMessagelessInput()
        {
            var expected = ContextState.InputTitleState;

            var actual = _inputTitleState.BotOnMessageReceived(_botClientMock, _messageStub).Result;

            A.CallTo(() => _botClientMock.SendTextMessageAsync(_messageStub.Chat.Id, A<string>.Ignored, A<ParseMode>.Ignored,
                                                           A<IEnumerable<MessageEntity>>.Ignored, A<bool>.Ignored,
                                                           A<bool>.Ignored, A<int>.Ignored, A<bool>.Ignored,
                                                           A<IReplyMarkup>.Ignored, A<CancellationToken>.Ignored))
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
            _messageStub.Text = "";
            var expected = ContextState.MainState;

            var actual = _inputTitleState.BotOnMessageReceived(_botClientMock, _messageStub).Result;

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
        public void BotOnCallBackQueryReceived_ShouldReturnSameState()
        {
            var expected = ContextState.InputTitleState;

            var actual = _inputTitleState.BotOnCallBackQueryReceived(_botClientMock, _callbackQueryStub).Result;

            A.CallTo(() => _botClientMock.AnswerCallbackQueryAsync(_callbackQueryStub.Id, A<string>.Ignored, A<bool>.Ignored,
                                                               A<string>.Ignored, A<int>.Ignored, A<CancellationToken>.Ignored))
                                                               .MustHaveHappenedOnceExactly();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BotSendMessage_ShouldSendTextMessage()
        {
            _inputTitleState.BotSendMessage(_botClientMock, _messageStub.Chat.Id);

            A.CallTo(() => _botClientMock.SendTextMessageAsync(_messageStub.Chat.Id, A<string>.Ignored, A<ParseMode>.Ignored,
                                                           A<IEnumerable<MessageEntity>>.Ignored, A<bool>.Ignored,
                                                           A<bool>.Ignored, A<int>.Ignored, A<bool>.Ignored,
                                                           A<IReplyMarkup>.Ignored, A<CancellationToken>.Ignored))
                                                           .MustHaveHappenedOnceExactly();
        }
    }
}
