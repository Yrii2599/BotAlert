using System.Threading;
using System.Collections.Generic;
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
    public class InputLanguageKeyboardStateTests
    {
        private readonly ITelegramBotClient _botClientMock;
        private readonly IStateProvider _stateProviderMock;
        private readonly ILocalizerFactory _localizerFactory;
        private readonly Message _messageMock;
        private readonly CallbackQuery _callbackQueryMock;
        private readonly ChatState _chatStub;

        private readonly ContextState _currentState;

        private readonly InputLanguageKeyboardState _inputLanguageKeyboardState;

        public InputLanguageKeyboardStateTests()
        {
            _botClientMock = A.Fake<ITelegramBotClient>();
            _stateProviderMock = A.Fake<IStateProvider>();
            _localizerFactory = A.Fake<ILocalizerFactory>();
            _messageMock = A.Fake<Message>();
            _messageMock.Chat = A.Fake<Chat>();
            _callbackQueryMock = A.Fake<CallbackQuery>();
            _callbackQueryMock.Message = _messageMock;

            _currentState = ContextState.InputLanguageKeyboardState;

            _chatStub = new ChatState(_messageMock.Chat.Id, _currentState);

            _inputLanguageKeyboardState = new InputLanguageKeyboardState(_stateProviderMock, _localizerFactory);
        }

        [Fact]
        public void BotOnMessageReceived_ReturnsCurrentState()
        {
            var expected = _currentState;

            var actual = _inputLanguageKeyboardState.BotOnMessageReceived(_botClientMock, _messageMock).Result;

            A.CallTo(() => _botClientMock.SendTextMessageAsync(_messageMock.Chat.Id,
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
        public void BotOnCallBackQueryReceived_DataEnglish_ChangesLanguageAndReturnsMainState()
        {
            _callbackQueryMock.Data = "English";
            A.CallTo(() => _stateProviderMock.GetChatState(_callbackQueryMock.Message.Chat.Id)).Returns(_chatStub);
            var expected = ContextState.MainState;

            var actual = _inputLanguageKeyboardState.BotOnCallBackQueryReceived(_botClientMock, _callbackQueryMock).Result;

            A.CallTo(() => _stateProviderMock.SaveChatState(_chatStub)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _botClientMock.AnswerCallbackQueryAsync(_callbackQueryMock.Id,
                                                                   A<string>.Ignored,
                                                                   A<bool>.Ignored,
                                                                   A<string>.Ignored,
                                                                   A<int>.Ignored,
                                                                   A<CancellationToken>.Ignored))
                                                                  .MustHaveHappenedOnceExactly();
            Assert.Equal(LanguageType.English, _chatStub.Language);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BotOnCallBackQueryReceived_DataRussian_ChangesLanguageAndReturnsMainState()
        {
            _callbackQueryMock.Data = "Russian";
            A.CallTo(() => _stateProviderMock.GetChatState(_callbackQueryMock.Message.Chat.Id)).Returns(_chatStub);
            var expected = ContextState.MainState;

            var actual = _inputLanguageKeyboardState.BotOnCallBackQueryReceived(_botClientMock, _callbackQueryMock).Result;

            A.CallTo(() => _stateProviderMock.SaveChatState(_chatStub)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _botClientMock.AnswerCallbackQueryAsync(_callbackQueryMock.Id,
                                                                   A<string>.Ignored,
                                                                   A<bool>.Ignored,
                                                                   A<string>.Ignored,
                                                                   A<int>.Ignored,
                                                                   A<CancellationToken>.Ignored))
                                                                  .MustHaveHappenedOnceExactly();
            Assert.Equal(LanguageType.Russian, _chatStub.Language);
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void BotOnCallBackQueryReceived_DataNull_ReturnsCurrentState()
        {
            A.CallTo(() => _stateProviderMock.GetChatState(_callbackQueryMock.Message.Chat.Id)).Returns(_chatStub);
            var expected = _currentState;

            var actual = _inputLanguageKeyboardState.BotOnCallBackQueryReceived(_botClientMock, _callbackQueryMock).Result;

            A.CallTo(() => _stateProviderMock.SaveChatState(_chatStub)).MustNotHaveHappened();
            A.CallTo(() => _botClientMock.AnswerCallbackQueryAsync(_callbackQueryMock.Id,
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
            A.CallTo(() => _stateProviderMock.GetChatState(_messageMock.Chat.Id)).Returns(_chatStub);

            _inputLanguageKeyboardState.BotSendMessage(_botClientMock, _messageMock.Chat.Id);

            A.CallTo(() => _botClientMock.SendTextMessageAsync(_messageMock.Chat.Id,
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
