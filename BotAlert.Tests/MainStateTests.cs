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
    public class MainStateTests
    {
        private readonly ITelegramBotClient _botClientMock;
        private readonly Message _messageStub;
        private readonly CallbackQuery _callbackQueryStub;

        private readonly IState _mainState;

        public MainStateTests()
        {
            _botClientMock = A.Fake<ITelegramBotClient>();
            _messageStub = new Message();
            _messageStub.Chat = new Chat();
            _callbackQueryStub = new CallbackQuery();
            _callbackQueryStub.Message = _messageStub;

            _mainState = new MainState();
        }

        [Fact]
        public void BotOnMessageReceived_GreetsUserCorrectly()
        {
            var expected = ContextState.InputTimeZoneState;
            _messageStub.Text = "/start";

            var actual = _mainState.BotOnMessageReceived(_botClientMock, _messageStub).Result;

            A.CallTo(() => _botClientMock.SendTextMessageAsync(_messageStub.Chat.Id, A<string>.Ignored, A<ParseMode>.Ignored,
                                                           A<IEnumerable<MessageEntity>>.Ignored, A<bool>.Ignored,
                                                           A<bool>.Ignored, A<int>.Ignored, A<bool>.Ignored,
                                                           A<IReplyMarkup>.Ignored, A<CancellationToken>.Ignored))
                                                           .MustHaveHappenedOnceExactly();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BotOnMessageReceived_CreatesEventCorrectly()
        {
            var expected = ContextState.InputTitleState;
            _messageStub.Text = "/create";

            var actual = _mainState.BotOnMessageReceived(_botClientMock, _messageStub).Result;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BotOnMessageReceived_GetsEventsCorrectly()
        {
            var expected = ContextState.GetAllNotificationsState;
            _messageStub.Text = "/get_notifications";

            var actual = _mainState.BotOnMessageReceived(_botClientMock, _messageStub).Result;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BotOnMessageReceived_HandlesIncorrectMessages()
        {
            var expected = ContextState.MainState;
            _messageStub.Text = "test";

            var actual = _mainState.BotOnMessageReceived(_botClientMock, _messageStub).Result;

            A.CallTo(() => _botClientMock.SendTextMessageAsync(_messageStub.Chat.Id, A<string>.Ignored, A<ParseMode>.Ignored,
                                                           A<IEnumerable<MessageEntity>>.Ignored, A<bool>.Ignored,
                                                           A<bool>.Ignored, A<int>.Ignored, A<bool>.Ignored,
                                                           A<IReplyMarkup>.Ignored, A<CancellationToken>.Ignored))
                                                           .MustHaveHappenedOnceExactly();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BotOnMessageReceived_HandlesNonTextTypeMessages()
        {
            var expected = ContextState.MainState;

            var actual = _mainState.BotOnMessageReceived(_botClientMock, _messageStub).Result;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BotOnCallBackQueryReceived_ReturnsSameState()
        {
            var expected = ContextState.MainState;

            var actual = _mainState.BotOnCallBackQueryReceived(_botClientMock, _callbackQueryStub).Result;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BotSendMessage_ShouldSendTextMessage()
        {
            _mainState.BotSendMessage(_botClientMock, _messageStub.Chat.Id);

            A.CallTo(() => _botClientMock.SendTextMessageAsync(_messageStub.Chat.Id, A<string>.Ignored, A<ParseMode>.Ignored,
                                                           A<IEnumerable<MessageEntity>>.Ignored, A<bool>.Ignored,
                                                           A<bool>.Ignored, A<int>.Ignored, A<bool>.Ignored,
                                                           A<IReplyMarkup>.Ignored, A<CancellationToken>.Ignored))
                                                           .MustHaveHappenedOnceExactly();
        }
    }
}
