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
  public  class GetAllNotificationsStateTests
    {
        private readonly IEventProvider _eventProviderMock;
        private readonly IStateProvider _stateProviderMock;
        private readonly ITelegramBotClient _botClientMock;
        private readonly ChatState _chatStateMock;
        private readonly CallbackQuery _callbackQueryMock;
        private readonly Message _messageMock;

        private readonly IState _getAllNotificationsState;

        public GetAllNotificationsStateTests()
        {
            _eventProviderMock = A.Fake<IEventProvider>();
            _stateProviderMock = A.Fake<IStateProvider>();
            _botClientMock = A.Fake<ITelegramBotClient>();
            _chatStateMock = A.Fake<ChatState>();
            _messageMock = A.Fake<Message>();
            _messageMock.Chat = A.Fake<Chat>();
            _callbackQueryMock = A.Fake<CallbackQuery>();
            _callbackQueryMock.Message = A.Fake<Message>();
            _callbackQueryMock.Message.Chat = A.Fake<Chat>();
            _callbackQueryMock.Message.Chat.Id = 5;

            _getAllNotificationsState = new GetAllNotificationsState(_eventProviderMock,_stateProviderMock);
        }

        [Fact]
        public void BotOnCallBackQueryReceived_AnswerCallbackQueryAsync()
        {
            _getAllNotificationsState.BotOnCallBackQueryReceived(_botClientMock, _callbackQueryMock);

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
        public void BotOnCallBackQueryReceived_WorksCorrectly_WhenCallBackDataToMain()
        {
            var expected = ContextState.MainState;

            _callbackQueryMock.Data = "ToMain";

            A.CallTo(() => _stateProviderMock.GetChatState(A<long>.Ignored)).Returns(_chatStateMock);

            var actual = _getAllNotificationsState.BotOnCallBackQueryReceived(_botClientMock, _callbackQueryMock).Result;

            A.CallTo(() => _stateProviderMock.GetChatState(A<long>.Ignored)).MustHaveHappenedOnceExactly();

            A.CallTo(() => _stateProviderMock.SaveChatState(_chatStateMock)).MustHaveHappenedOnceExactly();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BotOnCallBackQueryReceived_WorksCorrectly_WhenCallBackDataPrev()
        {
            var expected = ContextState.GetAllNotificationsState;

            _callbackQueryMock.Data = "Prev";

            A.CallTo(() => _stateProviderMock.GetChatState(A<long>.Ignored)).Returns(_chatStateMock);

            var actual = _getAllNotificationsState.BotOnCallBackQueryReceived(_botClientMock, _callbackQueryMock).Result;

            A.CallTo(() => _stateProviderMock.GetChatState(A<long>.Ignored)).MustHaveHappenedOnceExactly();

            A.CallTo(() => _stateProviderMock.SaveChatState(_chatStateMock)).MustHaveHappenedOnceExactly();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BotOnCallBackQueryReceived_WorksCorrectly_WhenCallBackDataNext()
        {
            var expected = ContextState.GetAllNotificationsState;

            _callbackQueryMock.Data = "Next";

            A.CallTo(() => _stateProviderMock.GetChatState(A<long>.Ignored)).Returns(_chatStateMock);

            var actual = _getAllNotificationsState.BotOnCallBackQueryReceived(_botClientMock, _callbackQueryMock).Result;

            A.CallTo(() => _stateProviderMock.GetChatState(A<long>.Ignored)).MustHaveHappenedOnceExactly();

            A.CallTo(() => _stateProviderMock.SaveChatState(_chatStateMock)).MustHaveHappenedOnceExactly();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BotOnCallBackQueryReceived_WorksCorrectly_WhenCallBackDataUnknown()
        {
            var expected = ContextState.GetNotificationDetailsState;

            _callbackQueryMock.Data = Guid.Empty.ToString();

            A.CallTo(() => _stateProviderMock.GetChatState(A<long>.Ignored)).Returns(_chatStateMock);

            var actual = _getAllNotificationsState.BotOnCallBackQueryReceived(_botClientMock, _callbackQueryMock).Result;

            A.CallTo(() => _stateProviderMock.GetChatState(A<long>.Ignored)).MustHaveHappenedOnceExactly();

            A.CallTo(() => _stateProviderMock.SaveChatState(_chatStateMock)).MustHaveHappenedOnceExactly();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BotOnMessageReceived_ReturnsSameStateAndPrintsMessage()
        {
            var expected = ContextState.GetAllNotificationsState;

           var actual = _getAllNotificationsState.BotOnMessageReceived(_botClientMock, _messageMock).Result;

           A.CallTo(() => _botClientMock.SendTextMessageAsync(A<ChatId>.Ignored, A<string>.Ignored, A<ParseMode>.Ignored,
                                                              A<IEnumerable<MessageEntity>>.Ignored, A<bool>.Ignored,
                                                              A<bool>.Ignored, A<int>.Ignored, A<bool>.Ignored,
                                                              A<IReplyMarkup>.Ignored, A<CancellationToken>.Ignored))
                                                            .MustHaveHappenedOnceExactly();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BotSendMessage_EventListCount0AndPage0_WorksCorrectly()
        {
            A.CallTo(() => _stateProviderMock.GetChatState(A<long>.Ignored)).Returns(_chatStateMock);

            _getAllNotificationsState.BotSendMessage(_botClientMock, _messageMock.Chat.Id);

            A.CallTo(() => _eventProviderMock.GetUserEventsOnPage(A<long>.Ignored)).MustHaveHappenedOnceExactly();

            A.CallTo(() => _botClientMock.SendTextMessageAsync(A<ChatId>.Ignored, A<string>.Ignored, A<ParseMode>.Ignored,
                                                              A<IEnumerable<MessageEntity>>.Ignored, A<bool>.Ignored,
                                                              A<bool>.Ignored, A<int>.Ignored, A<bool>.Ignored,
                                                              A<IReplyMarkup>.Ignored, A<CancellationToken>.Ignored))
                                                            .MustHaveHappenedOnceExactly();
            A.CallTo(() => _stateProviderMock.SaveChatState(A<ChatState>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void BotSendMessage_EventListCount0AndPageNot0_WorksCorrectly()
        {
            _chatStateMock.NotificationsPage = 1;
            A.CallTo(() => _stateProviderMock.GetChatState(A<long>.Ignored)).Returns(_chatStateMock);


            _getAllNotificationsState.BotSendMessage(_botClientMock, _messageMock.Chat.Id);

            A.CallTo(() => _eventProviderMock.GetUserEventsOnPage(A<long>.Ignored)).MustHaveHappenedTwiceExactly();

            A.CallTo(() => _botClientMock.SendTextMessageAsync(A<ChatId>.Ignored, A<string>.Ignored, A<ParseMode>.Ignored,
                                                              A<IEnumerable<MessageEntity>>.Ignored, A<bool>.Ignored,
                                                              A<bool>.Ignored, A<int>.Ignored, A<bool>.Ignored,
                                                              A<IReplyMarkup>.Ignored, A<CancellationToken>.Ignored))
                                                            .MustHaveHappenedOnceExactly();
            A.CallTo(() => _stateProviderMock.SaveChatState(A<ChatState>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void BotSendMessage_EventListCountNot0_WorksCorrectly()
        {
            _chatStateMock.NotificationsPage = 1;
            A.CallTo(() => _stateProviderMock.GetChatState(A<long>.Ignored)).Returns(_chatStateMock);


            _getAllNotificationsState.BotSendMessage(_botClientMock, _messageMock.Chat.Id);

            A.CallTo(() => _eventProviderMock.GetUserEventsOnPage(A<long>.Ignored)).MustHaveHappenedTwiceExactly();
            A.CallTo(() => _eventProviderMock.UserEventsPreviousPageExists(A<long>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _eventProviderMock.UserEventsNextPageExists(A<long>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _botClientMock.SendTextMessageAsync(A<ChatId>.Ignored, A<string>.Ignored, A<ParseMode>.Ignored,
                                                              A<IEnumerable<MessageEntity>>.Ignored, A<bool>.Ignored,
                                                              A<bool>.Ignored, A<int>.Ignored, A<bool>.Ignored,
                                                              A<IReplyMarkup>.Ignored, A<CancellationToken>.Ignored))
                                                            .MustHaveHappenedOnceExactly();
        }
    }
}
