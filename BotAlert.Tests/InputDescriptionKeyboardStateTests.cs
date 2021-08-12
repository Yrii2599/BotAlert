using System.Collections.Generic;
using System.Threading;
using FakeItEasy;
using Xunit;
using BotAlert.States;
using BotAlert.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;


namespace BotAlert.Tests
{
    public class InputDescriptionKeyboardStateTests
    {
        private readonly ITelegramBotClient _botClientMock;
        private readonly Message _messageMock;
        private readonly CallbackQuery _callbackQueryMock;

        private readonly ContextState _currentState;

        private readonly InputDescriptionKeyboardState _inputDescriptionKeyboardState;

        public InputDescriptionKeyboardStateTests()
        {
            _botClientMock = A.Fake<ITelegramBotClient>();
            _messageMock = A.Fake<Message>();
            _messageMock.Chat = A.Fake<Chat>();
            _callbackQueryMock = A.Fake<CallbackQuery>();
            _callbackQueryMock.Message = _messageMock;

            _currentState = ContextState.InputDescriptionKeyboardState;

            _inputDescriptionKeyboardState = new InputDescriptionKeyboardState();
        }

        [Fact]
        public void BotOnMessageReceived_TextNull_ReturnsCurrentState()
        {
            var expected = _currentState;

            var actual = _inputDescriptionKeyboardState.BotOnMessageReceived(_botClientMock, _messageMock).Result;

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
        public void BotOnMessageReceived_TextAccept_ReturnsInputDescriptionState()
        {
            var expected = ContextState.InputDescriptionState;
            _messageMock.Text = "да";

            var actual = _inputDescriptionKeyboardState.BotOnMessageReceived(_botClientMock, _messageMock).Result;

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
                                                              .MustNotHaveHappened();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BotOnMessageReceived_TextDecline_ReturnsSaveState()
        {
            var expected = ContextState.SaveState;
            _messageMock.Text = "нет";

            var actual = _inputDescriptionKeyboardState.BotOnMessageReceived(_botClientMock, _messageMock).Result;

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
                                                              .MustNotHaveHappened();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BotOnCallBackQueryReceived_DataAccept_ReturnsInputWarnDateState()
        {
            _callbackQueryMock.Data = "y";
            var expected = ContextState.InputDescriptionState;

            var actual = _inputDescriptionKeyboardState.BotOnCallBackQueryReceived(_botClientMock, _callbackQueryMock).Result;

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
        public void BotOnCallBackQueryReceived_DataNullOrDecline_ReturnsSaveState()
        {
            var expected = ContextState.SaveState;

            var actual = _inputDescriptionKeyboardState.BotOnCallBackQueryReceived(_botClientMock, _callbackQueryMock).Result;

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
            _inputDescriptionKeyboardState.BotSendMessage(_botClientMock, _messageMock.Chat.Id);

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

            var actual = _inputDescriptionKeyboardState.HandleInvalidInput(_botClientMock, _messageMock.Chat.Id);

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
