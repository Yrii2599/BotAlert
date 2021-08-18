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
        private readonly CallbackQuery _callbackQueryMock;
        private readonly InputDescriptionState inputDescription;
        private Message _messageMock;

        public InputDescriptionStateTests()
        {
            _botClientMock = A.Fake<ITelegramBotClient>();
            _callbackQueryMock = A.Fake<CallbackQuery>();
            _messageMock = A.Fake<Message>();
            _eventProviderMock = A.Fake<IEventProvider>();
            _stateProviderMock = A.Fake<IStateProvider>();
            _messageMock.Chat = A.Fake<Chat>();
            inputDescription = new InputDescriptionState(_eventProviderMock, _stateProviderMock);
        }

        [Fact]
        public void BotOnMessageReceived_ReturnsInvalidInput()
        {
            inputDescription.BotOnMessageReceived(_botClientMock, _messageMock);

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
            _messageMock.Text = "Something";
            
            var actual = inputDescription.BotOnMessageReceived(_botClientMock, _messageMock);

            A.CallTo(() => _eventProviderMock.UpdateEvent(A<Event>.Ignored)).MustHaveHappenedOnceExactly();

            Assert.Equal(ContextState.SaveState, actual.Result);
        }

        [Fact]
        public void BotOnMessageReceived_WithTextAndCreatedEvent__ShouldUpdateEvent()
        {
            var chatStateMock = A.Fake<ChatState>();
            chatStateMock.ActiveNotificationId = Guid.NewGuid();
            var eventMock = A.Fake<Event>();
            eventMock.Status = EventStatus.Created;
            A.CallTo(() => _stateProviderMock.GetChatState(_messageMock.Chat.Id)).Returns(chatStateMock);
            A.CallTo(() => _eventProviderMock.GetEventById(chatStateMock.ActiveNotificationId)).Returns(eventMock);
            _messageMock.Text = "Something";
            
            var actual = inputDescription.BotOnMessageReceived(_botClientMock, _messageMock);

            A.CallTo(() => _eventProviderMock.UpdateEvent(A<Event>.That.Matches(e => e.ChatId == _messageMock.Chat.Id)))
                                              .MustHaveHappenedOnceExactly();
            Assert.Equal(ContextState.EditState, actual.Result);
        }

        [Fact]
        public void BotOnMessageReceived_WithNoText_ReturnsContextStateSaveState()
        {
            var actual= inputDescription.BotOnMessageReceived(_botClientMock, _messageMock);

            Assert.Equal(ContextState.InputDescriptionState,actual.Result);
        }

        [Fact]
        public void BotOnCallBackQueryReceived_Return()
        {
            var actual = inputDescription.BotOnCallBackQueryReceived(_botClientMock, _callbackQueryMock);

            Assert.Equal(ContextState.InputDescriptionState, actual.Result);
        }

        [Fact]
        public void BotOnCallBackQueryReceived_ReturnsAnswerCallbackQueryAsync()
        {
            _messageMock.Text = "Something";
            inputDescription.BotOnCallBackQueryReceived(_botClientMock, _callbackQueryMock);

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
            inputDescription.BotSendMessage(_botClientMock, _messageMock.Chat.Id);

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
