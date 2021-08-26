using System;
using System.Threading;
using System.Collections.Generic;
using BotAlert.Models;
using BotAlert.States;
using BotAlert.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using FakeItEasy;
using Xunit;

namespace BotAlert.Tests
{
    public class InputDescriptionStateTests
    {
        private readonly IEventProvider _eventProviderMock;
        private readonly IStateProvider _stateProviderMock;
        private readonly ITelegramBotClient _botClientMock;
        private readonly CallbackQuery _callbackQueryStub;
        private readonly ChatState _chatStateStub;
        private readonly Message _messageStub;
        private readonly InputDescriptionState inputDescription;

        public InputDescriptionStateTests()
        {
            _botClientMock = A.Fake<ITelegramBotClient>();
            _eventProviderMock = A.Fake<IEventProvider>();
            _stateProviderMock = A.Fake<IStateProvider>();
            _callbackQueryStub = new CallbackQuery();
            _chatStateStub = new ChatState(1234);
            _messageStub = new Message();
            _messageStub.Chat = new Chat();

            inputDescription = new InputDescriptionState(_eventProviderMock, _stateProviderMock);
        }

        [Fact]
        public void BotOnMessageReceived_ReturnsInvalidInput()
        {
            inputDescription.BotOnMessageReceived(_botClientMock, _messageStub);

            A.CallTo(() => _botClientMock.SendTextMessageAsync(A<ChatId>
                                                                   .Ignored, A<string>
                                                                   .Ignored, A<ParseMode>
                                                                   .Ignored, A<IEnumerable<MessageEntity>>
                                                                   .Ignored, A<bool>
                                                                   .Ignored, A<bool>
                                                                   .Ignored, A<int>
                                                                   .Ignored, A<bool>
                                                                   .Ignored, A<IReplyMarkup>
                                                                   .Ignored, A<CancellationToken>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void BotOnMessageReceived_WithTextAndInProgressEvent__ShouldUpdateDraftEventByChatId()
        {
            _messageStub.Text = "Something";
            
            var actual = inputDescription.BotOnMessageReceived(_botClientMock, _messageStub);

            A.CallTo(() => _eventProviderMock.UpdateEvent(A<Event>.Ignored)).MustHaveHappenedOnceExactly();

            Assert.Equal(ContextState.SaveState, actual.Result);
        }

        [Fact]
        public void BotOnMessageReceived_EventIsNull_ShouldReturnMainState()
        {
            A.CallTo(() => _stateProviderMock.GetChatState(_messageStub.Chat.Id)).Returns(_chatStateStub);
            A.CallTo(() => _eventProviderMock.GetEventById(_chatStateStub.ActiveNotificationId)).Returns(null);
            _messageStub.Text = "Something";
            var expected = ContextState.MainState;

            var actual = inputDescription.BotOnMessageReceived(_botClientMock, _messageStub).Result;

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
        public void BotOnMessageReceived_WithTextAndCreatedEvent__ShouldUpdateEvent()
        {
            var chatStateMock = A.Fake<ChatState>();
            chatStateMock.ActiveNotificationId = Guid.NewGuid();
            var eventMock = A.Fake<Event>();
            eventMock.Status = EventStatus.Created;
            A.CallTo(() => _stateProviderMock.GetChatState(_messageStub.Chat.Id)).Returns(chatStateMock);
            A.CallTo(() => _eventProviderMock.GetEventById(chatStateMock.ActiveNotificationId)).Returns(eventMock);
            _messageStub.Text = "Something";
            
            var actual = inputDescription.BotOnMessageReceived(_botClientMock, _messageStub);

            A.CallTo(() => _eventProviderMock.UpdateEvent(A<Event>.That.Matches(e => e.ChatId == _messageStub.Chat.Id)))
                                              .MustHaveHappenedOnceExactly();
            Assert.Equal(ContextState.EditState, actual.Result);
        }

        [Fact]
        public void BotOnMessageReceived_WithNoText_ReturnsContextStateSaveState()
        {
            var actual= inputDescription.BotOnMessageReceived(_botClientMock, _messageStub);

            Assert.Equal(ContextState.InputDescriptionState,actual.Result);
        }

        [Fact]
        public void BotOnCallBackQueryReceived_Return()
        {
            var actual = inputDescription.BotOnCallBackQueryReceived(_botClientMock, _callbackQueryStub);

            Assert.Equal(ContextState.InputDescriptionState, actual.Result);
        }

        [Fact]
        public void BotOnCallBackQueryReceived_ReturnsAnswerCallbackQueryAsync()
        {
            _messageStub.Text = "Something";
            inputDescription.BotOnCallBackQueryReceived(_botClientMock, _callbackQueryStub);

            A.CallTo(() => _botClientMock.AnswerCallbackQueryAsync(A<string>
                                                                       .Ignored, A<string>
                                                                       .Ignored, A<bool>
                                                                       .Ignored, A<string>
                                                                       .Ignored, A<int>
                                                                       .Ignored, A<CancellationToken>
                                                                       .Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void BotSendMessage()
        {
            inputDescription.BotSendMessage(_botClientMock, _messageStub.Chat.Id);

            A.CallTo(() => _botClientMock.SendTextMessageAsync(A<ChatId>
                                                                   .Ignored, A<string>
                                                                   .Ignored, A<ParseMode>
                                                                   .Ignored, A<IEnumerable<MessageEntity>>
                                                                   .Ignored, A<bool>
                                                                   .Ignored, A<bool>
                                                                   .Ignored, A<int>
                                                                   .Ignored, A<bool>
                                                                   .Ignored, A<IReplyMarkup>
                                                                   .Ignored, A<CancellationToken>.Ignored))
                                                                    .MustHaveHappenedOnceExactly();
        }
    }
}
