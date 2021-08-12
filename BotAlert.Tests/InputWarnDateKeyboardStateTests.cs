using System.Collections.Generic;
using System.Threading;
using FakeItEasy;
using Xunit;
using BotAlert.Interfaces;
using BotAlert.States;
using BotAlert.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System;

namespace BotAlert.Tests
{
    public class InputWarnDateKeyboardStateTests
    {
        private readonly IEventProvider _eventProviderMock;
        private readonly ITelegramBotClient _botClientMock;
        private readonly Message _messageMock;
        private readonly CallbackQuery _callbackQueryMock;

        private readonly ContextState _currentState;

        private readonly InputWarnDateKeyboard _inputWarnDateKeyboardState;

        public InputWarnDateKeyboardStateTests()
        {
            _eventProviderMock = A.Fake<IEventProvider>();
            _botClientMock = A.Fake<ITelegramBotClient>();
            _messageMock = A.Fake<Message>();
            _messageMock.Chat = A.Fake<Chat>();
            _callbackQueryMock = A.Fake<CallbackQuery>();
            _callbackQueryMock.Message = _messageMock;

            _currentState = ContextState.InputWarnDateKeyboard;

            _inputWarnDateKeyboardState = new InputWarnDateKeyboard(_eventProviderMock);
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
        public void BotOnCallbackQueryReceived_HardcodedData_ReturnsInputDescriptionKeyboardState()
        {
            _callbackQueryMock.Data = "30";

            var expected = ContextState.InputDescriptionKeyboardState;

            // Захардкодили чтоб не писать вверху с зависимостями 
            A.CallTo(() => _eventProviderMock.GetDraftEventByChatId(A<long>.Ignored)).Returns(new Event(1234, "Title") { Date = DateTime.Now});

            var actual = _inputWarnDateKeyboardState.BotOnCallBackQueryReceived(_botClientMock, _callbackQueryMock).Result;

            A.CallTo(() => _botClientMock.AnswerCallbackQueryAsync(A<string>.Ignored,
                                                                   A<string>.Ignored,
                                                                   A<bool>.Ignored,
                                                                   A<string>.Ignored,
                                                                   A<int>.Ignored,
                                                                   A<CancellationToken>.Ignored))
                                                                  .MustHaveHappenedOnceExactly();

            A.CallTo(() => _eventProviderMock.UpdateEvent(A<Event>.Ignored)).MustHaveHappenedOnceExactly();

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

        [Fact]
        public void HandleInvalidInput_ReturnsCurrentState()
        {
            var expected = _currentState;

            var actual = _inputWarnDateKeyboardState.HandleInvalidInput(_botClientMock, _messageMock.Chat.Id, String.Empty);

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
    }
}
