using System.Collections.Generic;
using System.Threading;
using BotAlert.Interfaces;
using BotAlert.Models;
using BotAlert.States;
using FakeItEasy;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Xunit;

namespace BotAlert.Tests
{
  public  class GetAllNotificationsStateTests
    {
        private readonly IEventProvider _eventProviderMock;
        private readonly IStateProvider _stateProviderMock;
        private readonly GetAllNotificationsState _notification;
        private readonly ITelegramBotClient _botClientMock;
        private readonly CallbackQuery _callbackQueryMock;
        private readonly Message _messageMock;

        public GetAllNotificationsStateTests()
        {
            _eventProviderMock = A.Fake<IEventProvider>();
            _stateProviderMock = A.Fake<IStateProvider>();
            _botClientMock = A.Fake<ITelegramBotClient>();
            _messageMock = A.Fake<Message>();
            _messageMock.Chat = A.Fake<Chat>();
            _callbackQueryMock = A.Fake<CallbackQuery>();
            _callbackQueryMock.Message = A.Fake<Message>();
            _callbackQueryMock.Message.Chat = A.Fake<Chat>();
            _callbackQueryMock.Message.Chat.Id = 5;
            _notification = new GetAllNotificationsState(_eventProviderMock,_stateProviderMock);
        }

        [Fact]
        public void BotOnCallBackQueryReceived_AnswerCallbackQueryAsync()
        {
            _notification.BotOnCallBackQueryReceived(_botClientMock, _callbackQueryMock);

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
        public void BotOnCallBackQueryReceived_WhereCallBackDataToMain()
        {
            var expected = ContextState.MainState;

            _callbackQueryMock.Data = "ToMain";
            var actual=_notification.BotOnCallBackQueryReceived(_botClientMock, _callbackQueryMock).Result;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BotOnCallBackQueryReceived_WhereCallBackDataPrev()
        {
            var expected = ContextState.GetAllNotificationsState;

            _callbackQueryMock.Data = "Prev";
            var actual = _notification.BotOnCallBackQueryReceived(_botClientMock, _callbackQueryMock).Result;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BotOnCallBackQueryReceived_WhereCallBackDataNext()
        {
            var expected = ContextState.GetAllNotificationsState;

            _callbackQueryMock.Data = "Next";
            var actual = _notification.BotOnCallBackQueryReceived(_botClientMock, _callbackQueryMock).Result;

            Assert.Equal(expected, actual);
        }

        //[Fact]
        //public void BotOnCallBackQueryReceived_CallSaveChatState()
        //{
        //    var expected = ContextState.GetAllNotificationsState;

        // _notification.BotOnCallBackQueryReceived(_botClientMock, _callbackQueryMock);

        //    A.CallTo(() => _stateProviderMock.SaveChatState(A<ChatState>.Ignored)).MustHaveHappened();
        //}
        [Fact]
        public void BotOnMessageReceived_ReturnType()
        {
            var expected = ContextState.GetAllNotificationsState;

           var actual= _notification.BotOnMessageReceived(_botClientMock,_messageMock).Result;

           A.CallTo(() => _botClientMock.SendTextMessageAsync(A<ChatId>.Ignored, A<string>.Ignored, A<ParseMode>.Ignored,
                                                              A<IEnumerable<MessageEntity>>.Ignored, A<bool>.Ignored,
                                                              A<bool>.Ignored, A<int>.Ignored, A<bool>.Ignored,
                                                              A<IReplyMarkup>.Ignored, A<CancellationToken>.Ignored))
                                                            .MustHaveHappenedOnceExactly();

            Assert.Equal(expected, actual);
        }
    }
}
