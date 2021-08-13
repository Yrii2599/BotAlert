using System.Collections.Generic;
using BotAlert.Interfaces;
using BotAlert.Models;
using BotAlert.States;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using FakeItEasy;
using Xunit;
using Telegram.Bot.Types.ReplyMarkups;
using System.Threading;
using System;

namespace BotAlert.Tests
{
    public class SaveStateTests
    {
        private readonly IEventProvider _eventProviderMock;
        private readonly ITelegramBotClient _botClientMock;
        private readonly Message _messageMock;
        private readonly CallbackQuery _callbackQueryMock;

        private readonly IState _saveState;

        public SaveStateTests()
        {
            _eventProviderMock = A.Fake<IEventProvider>();
            _botClientMock = A.Fake<ITelegramBotClient>();
            _messageMock = A.Fake<Message>();
            _messageMock.Chat = A.Fake<Chat>();
            _callbackQueryMock = A.Fake<CallbackQuery>();
            _callbackQueryMock.Message = _messageMock;

            _saveState = new SaveState(_eventProviderMock);
        }

        /*[Fact]
        public void BotOnMessageReceived_ShouldSaveEventSendMessageAndReturnNextState()
        {
            var expected = ContextState.MainState;

            _messageMock.Text = "сохранить";

            var actual = _saveState.BotOnMessageReceived(_botClientMock, _messageMock).Result;

            A.CallTo(() => _eventProviderMock.UpdateDraftEventByChatId(A<long>.Ignored, A<string>.Ignored, A<EventStatus>.Ignored)).MustHaveHappenedOnceExactly();

            A.CallTo(() => _botClientMock.SendTextMessageAsync(A<ChatId>.Ignored, A<string>.Ignored, A<ParseMode>.Ignored,
                                                           A<IEnumerable<MessageEntity>>.Ignored, A<bool>.Ignored,
                                                           A<bool>.Ignored, A<int>.Ignored, A<bool>.Ignored,
                                                           A<IReplyMarkup>.Ignored, A<CancellationToken>.Ignored))
                                                           .MustHaveHappenedOnceExactly();

            Assert.Equal(expected, actual);
        }*/

        [Fact]
        public void BotOnMessageReceived_ShouldDeleteEventSendMessageAndReturnNextState()
        {
            var expected = ContextState.MainState;

            _messageMock.Text = "отменить";

            var actual = _saveState.BotOnMessageReceived(_botClientMock, _messageMock).Result;

            A.CallTo(() => _eventProviderMock.DeleteEvent(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();

            A.CallTo(() => _botClientMock.SendTextMessageAsync(A<ChatId>.Ignored, A<string>.Ignored, A<ParseMode>.Ignored,
                                                           A<IEnumerable<MessageEntity>>.Ignored, A<bool>.Ignored,
                                                           A<bool>.Ignored, A<int>.Ignored, A<bool>.Ignored,
                                                           A<IReplyMarkup>.Ignored, A<CancellationToken>.Ignored))
                                                           .MustHaveHappenedOnceExactly();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BotOnMessageReceived_ShouldHandleMessagelessInput()
        {
            var expected = ContextState.SaveState;

            var actual = _saveState.BotOnMessageReceived(_botClientMock, _messageMock).Result;

            A.CallTo(() => _botClientMock.SendTextMessageAsync(A<ChatId>.Ignored, A<string>.Ignored, A<ParseMode>.Ignored,
                                                           A<IEnumerable<MessageEntity>>.Ignored, A<bool>.Ignored,
                                                           A<bool>.Ignored, A<int>.Ignored, A<bool>.Ignored,
                                                           A<IReplyMarkup>.Ignored, A<CancellationToken>.Ignored))
                                                           .MustHaveHappenedOnceExactly();

            Assert.Equal(expected, actual);
        }

        /*[Fact]
        public void BotOnCallBackQueryReceived_ShouldSaveEventSendMessageAndReturnNextState()
        {
            var expected = ContextState.MainState;

            _callbackQueryMock.Data = "s";

            var actual = _saveState.BotOnCallBackQueryReceived(_botClientMock, _callbackQueryMock).Result;

            A.CallTo(() => _eventProviderMock.UpdateDraftEventByChatId(A<long>.Ignored, A<string>.Ignored, A<EventStatus>.Ignored)).MustHaveHappenedOnceExactly();

            A.CallTo(() => _botClientMock.SendTextMessageAsync(A<ChatId>.Ignored, A<string>.Ignored, A<ParseMode>.Ignored,
                                                           A<IEnumerable<MessageEntity>>.Ignored, A<bool>.Ignored,
                                                           A<bool>.Ignored, A<int>.Ignored, A<bool>.Ignored,
                                                           A<IReplyMarkup>.Ignored, A<CancellationToken>.Ignored))
                                                           .MustHaveHappenedOnceExactly();

            Assert.Equal(expected, actual);
        }*/

        [Fact]
        public void BotOnCallBackQueryReceived_ShouldDeleteEventSendMessageAndReturnNextState()
        {
            var expected = ContextState.MainState;

            _callbackQueryMock.Data = "c";

            var actual = _saveState.BotOnCallBackQueryReceived(_botClientMock, _callbackQueryMock).Result;

            A.CallTo(() => _eventProviderMock.DeleteEvent(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();

            A.CallTo(() => _botClientMock.SendTextMessageAsync(A<ChatId>.Ignored, A<string>.Ignored, A<ParseMode>.Ignored,
                                                           A<IEnumerable<MessageEntity>>.Ignored, A<bool>.Ignored,
                                                           A<bool>.Ignored, A<int>.Ignored, A<bool>.Ignored,
                                                           A<IReplyMarkup>.Ignored, A<CancellationToken>.Ignored))
                                                           .MustHaveHappenedOnceExactly();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BotSendMessage_ShouldSendTextMessage()
        {
            _saveState.BotSendMessage(_botClientMock, _messageMock.Chat.Id);

            A.CallTo(() => _botClientMock.SendTextMessageAsync(A<ChatId>.Ignored, A<string>.Ignored, A<ParseMode>.Ignored,
                                                           A<IEnumerable<MessageEntity>>.Ignored, A<bool>.Ignored,
                                                           A<bool>.Ignored, A<int>.Ignored, A<bool>.Ignored,
                                                           A<IReplyMarkup>.Ignored, A<CancellationToken>.Ignored))
                                                           .MustHaveHappenedOnceExactly();
        }

        /*[Fact]
        public void HandleInvalidInput_ShouldSendTextMessageAndReturnSameState()
        {
            var expected = ContextState.SaveState;

            var actual = _saveState.HandleInvalidInput(_botClientMock, _messageMock.Chat.Id, string.Empty);

            A.CallTo(() => _botClientMock.SendTextMessageAsync(A<ChatId>.Ignored, A<string>.Ignored, A<ParseMode>.Ignored,
                                                           A<IEnumerable<MessageEntity>>.Ignored, A<bool>.Ignored,
                                                           A<bool>.Ignored, A<int>.Ignored, A<bool>.Ignored,
                                                           A<IReplyMarkup>.Ignored, A<CancellationToken>.Ignored))
                                                           .MustHaveHappenedOnceExactly();

            Assert.Equal(expected, actual);
        }*/
    }
}
