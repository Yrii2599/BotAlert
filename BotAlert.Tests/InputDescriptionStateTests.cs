using System;
using System.Threading;
using System.Linq.Expressions;
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
            _messageMock.Chat = A.Fake<Chat>();
            inputDescription = new InputDescriptionState(_eventProviderMock);
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
        public void BotOnMessageReceived_ReturnsUpdateDraftEventByChatId()
        {
            _messageMock.Text = "Something";
            inputDescription.BotOnMessageReceived(_botClientMock, _messageMock);

            A.CallTo(() => _eventProviderMock.UpdateDraftEventByChatId<string>(A<long>
                                                                                   .Ignored, A<Expression<Func<Event, string>>>
                                                                                   .Ignored, A<string>
                                                                                   .Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void BotOnMessageReceived_ReturnsContextStateSaveState()
        {
            var actual= inputDescription.BotOnMessageReceived(_botClientMock, _messageMock);

            Assert.Equal(ContextState.InputDescriptionState,actual.Result);
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
        public void BotOnMessageReceived_Return()
        {
            var actual = inputDescription.BotOnCallBackQueryReceived(_botClientMock, _callbackQueryMock);

            Assert.Equal(ContextState.InputDescriptionState, actual.Result);
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
