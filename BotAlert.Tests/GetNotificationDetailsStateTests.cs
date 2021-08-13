using System;
using System.Threading;
using System.Collections.Generic;
using BotAlert.States;
using BotAlert.Models;
using BotAlert.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using FakeItEasy;
using Xunit;

namespace BotAlert.Tests
{
    public class GetNotificationDetailsStateTests
    {
        private readonly ITelegramBotClient _botClientMock;
        private readonly IEventProvider _eventProviderMock;
        private readonly IStateProvider _stateProviderMock;
        private readonly Message _messageMock;
        private readonly CallbackQuery _callbackQueryMock;

        private readonly ContextState _currentState;

        private readonly GetNotificationDetailsState _getNotificationDetailsState;

        public GetNotificationDetailsStateTests()
        {
            _botClientMock = A.Fake<ITelegramBotClient>();
            _eventProviderMock = A.Fake<IEventProvider>();
            _stateProviderMock = A.Fake<IStateProvider>();
            _messageMock = A.Fake<Message>();
            _messageMock.Chat = A.Fake<Chat>();
            _callbackQueryMock = A.Fake<CallbackQuery>();
            _callbackQueryMock.Message = _messageMock;

            _currentState = ContextState.GetNotificationDetailsState;

            _getNotificationDetailsState = new GetNotificationDetailsState(_eventProviderMock, _stateProviderMock);
        }

        [Fact]
        public void BotOnMessageReceived_ReturnsCurrentState()
        {
            var expected = _currentState;

            var actual = _getNotificationDetailsState.BotOnMessageReceived(_botClientMock, _messageMock).Result;

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
        public void BotOnCallBackQueryReceived_DataBack_GetAllNotificationsState()
        {
            _callbackQueryMock.Data = "Back";
            var expected = ContextState.GetAllNotificationsState;

            var actual = _getNotificationDetailsState.BotOnCallBackQueryReceived(_botClientMock, _callbackQueryMock).Result;

            A.CallTo(() => _botClientMock.AnswerCallbackQueryAsync(A<string>.Ignored,
                                                                   A<string>.Ignored,
                                                                   A<bool>.Ignored,
                                                                   A<string>.Ignored,
                                                                   A<int>.Ignored,
                                                                   A<CancellationToken>.Ignored))
                                                                  .MustHaveHappenedOnceExactly();
            Assert.Equal(expected, actual);
        }

        //[Fact]
        //public void BotOnCallBackQueryReceived_DataEdit_ReturnsEditState()
        //{
        //    _callbackQueryMock.Data = "Edit";
        //    var expected = ContextState.EditState

        //    var actual = _getNotificationDetailsState.BotOnCallBackQueryReceived(_botClientMock, _callbackQueryMock).Result;

        //    A.CallTo(() => _botClientMock.AnswerCallbackQueryAsync(A<string>.Ignored,
        //                                                           A<string>.Ignored,
        //                                                           A<bool>.Ignored,
        //                                                           A<string>.Ignored,
        //                                                           A<int>.Ignored,
        //                                                           A<CancellationToken>.Ignored))
        //                                                          .MustHaveHappenedOnceExactly();
        //    Assert.Equal(expected, actual);
        //}

        //[Fact]
        //public void BotOnCallBackQueryReceived_DataDelete_ReturnsDeleteKeyboardState()
        //{
        //    _callbackQueryMock.Data = "Delete";
        //    var expected = ContextState.DeleteKeyboardState;

        //    var actual = _getNotificationDetailsState.BotOnCallBackQueryReceived(_botClientMock, _callbackQueryMock).Result;


        //    A.CallTo(() => _botClientMock.AnswerCallbackQueryAsync(A<string>.Ignored,
        //                                                           A<string>.Ignored,
        //                                                           A<bool>.Ignored,
        //                                                           A<string>.Ignored,
        //                                                           A<int>.Ignored,
        //                                                           A<CancellationToken>.Ignored))
        //                                                          .MustNotHaveHappened();
        //    Assert.Equal(expected, actual);
        //}

        [Fact]
        public void BotOnCallBackQueryReceived_DataDefault_ReturnsInputWarnDateState()
        {
            var expected = _currentState;

            var actual = _getNotificationDetailsState.BotOnCallBackQueryReceived(_botClientMock, _callbackQueryMock).Result;

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
            _getNotificationDetailsState.BotSendMessage(_botClientMock, _messageMock.Chat.Id);

            A.CallTo(() => _eventProviderMock.GetEventById(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
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
    }
}
