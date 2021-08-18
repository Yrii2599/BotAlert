﻿using System.Threading;
using System.Collections.Generic;
using BotAlert.Interfaces;
using BotAlert.Models;
using BotAlert.States;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using FakeItEasy;
using Xunit;

namespace BotAlert.Tests
{
    public class SaveStateTests
    {
        private readonly IEventProvider _eventProviderMock;
        private readonly IStateProvider _stateProviderMock;
        private readonly ITelegramBotClient _botClientMock;
        private readonly Message _messageStub;
        private readonly CallbackQuery _callbackQueryStub;
        private readonly ChatState _chatStateStub;
        private readonly Event _eventStub;

        private readonly IState _saveState;

        public SaveStateTests()
        {
            _eventProviderMock = A.Fake<IEventProvider>();
            _stateProviderMock = A.Fake<IStateProvider>();
            _botClientMock = A.Fake<ITelegramBotClient>();
            _messageStub = new Message();
            _messageStub.Chat = new Chat();
            _callbackQueryStub = new CallbackQuery();
            _callbackQueryStub.Message = _messageStub;
            _chatStateStub = new ChatState(1234);
            _eventStub = new Event(1234, "Title");

            _saveState = new SaveState(_eventProviderMock, _stateProviderMock);
        }

        [Fact]
        public void BotOnMessageReceived_ShouldSaveEventSendMessageAndReturnNextState()
        {
            var expected = ContextState.MainState;
            _messageStub.Text = "сохранить";
            A.CallTo(() => _stateProviderMock.GetChatState(_messageStub.Chat.Id)).Returns(_chatStateStub);
            A.CallTo(() => _eventProviderMock.GetEventById(_chatStateStub.ActiveNotificationId)).Returns(_eventStub);

            var actual = _saveState.BotOnMessageReceived(_botClientMock, _messageStub).Result;

            A.CallTo(() => _stateProviderMock.GetChatState(_messageStub.Chat.Id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _eventProviderMock.GetEventById(_chatStateStub.ActiveNotificationId)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _eventProviderMock.UpdateEvent(_eventStub)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _botClientMock.SendTextMessageAsync(_messageStub.Chat.Id, A<string>.Ignored, A<ParseMode>.Ignored,
                                                           A<IEnumerable<MessageEntity>>.Ignored, A<bool>.Ignored,
                                                           A<bool>.Ignored, A<int>.Ignored, A<bool>.Ignored,
                                                           A<IReplyMarkup>.Ignored, A<CancellationToken>.Ignored))
                                                           .MustHaveHappenedOnceExactly();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BotOnMessageReceived_ShouldDeleteEventSendMessageAndReturnNextState()
        {
            var expected = ContextState.MainState;
            _messageStub.Text = "отменить";
            A.CallTo(() => _stateProviderMock.GetChatState(_messageStub.Chat.Id)).Returns(_chatStateStub);

            var actual = _saveState.BotOnMessageReceived(_botClientMock, _messageStub).Result;

            A.CallTo(() => _stateProviderMock.GetChatState(_messageStub.Chat.Id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _eventProviderMock.DeleteEvent(_chatStateStub.ActiveNotificationId)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _stateProviderMock.SaveChatState(_chatStateStub)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _botClientMock.SendTextMessageAsync(_messageStub.Chat.Id, A<string>.Ignored, A<ParseMode>.Ignored,
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

            var actual = _saveState.BotOnMessageReceived(_botClientMock, _messageStub).Result;

            A.CallTo(() => _botClientMock.SendTextMessageAsync(_messageStub.Chat.Id, A<string>.Ignored, A<ParseMode>.Ignored,
                                                           A<IEnumerable<MessageEntity>>.Ignored, A<bool>.Ignored,
                                                           A<bool>.Ignored, A<int>.Ignored, A<bool>.Ignored,
                                                           A<IReplyMarkup>.Ignored, A<CancellationToken>.Ignored))
                                                           .MustHaveHappenedOnceExactly();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BotOnCallBackQueryReceived_ShouldSaveEventSendMessageAndReturnNextState()
        {
            var expected = ContextState.MainState;
            _callbackQueryStub.Data = "Save";
            A.CallTo(() => _stateProviderMock.GetChatState(_messageStub.Chat.Id)).Returns(_chatStateStub);
            A.CallTo(() => _eventProviderMock.GetEventById(_chatStateStub.ActiveNotificationId)).Returns(_eventStub);

            var actual = _saveState.BotOnCallBackQueryReceived(_botClientMock, _callbackQueryStub).Result;

            A.CallTo(() => _botClientMock.AnswerCallbackQueryAsync(_callbackQueryStub.Id,
                                                                   A<string>.Ignored,
                                                                   A<bool>.Ignored,
                                                                   A<string>.Ignored,
                                                                   A<int>.Ignored,
                                                                   A<CancellationToken>.Ignored))
                                                                  .MustHaveHappenedOnceExactly();
            A.CallTo(() => _stateProviderMock.GetChatState(_messageStub.Chat.Id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _eventProviderMock.GetEventById(_chatStateStub.ActiveNotificationId)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _eventProviderMock.UpdateEvent(_eventStub)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _botClientMock.SendTextMessageAsync(_messageStub.Chat.Id, A<string>.Ignored, A<ParseMode>.Ignored,
                                                           A<IEnumerable<MessageEntity>>.Ignored, A<bool>.Ignored,
                                                           A<bool>.Ignored, A<int>.Ignored, A<bool>.Ignored,
                                                           A<IReplyMarkup>.Ignored, A<CancellationToken>.Ignored))
                                                           .MustHaveHappenedOnceExactly();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BotOnCallBackQueryReceived_ShouldDeleteEventSendMessageAndReturnNextState()
        {
            var expected = ContextState.MainState;
            _callbackQueryStub.Data = "c";
            A.CallTo(() => _stateProviderMock.GetChatState(_messageStub.Chat.Id)).Returns(_chatStateStub);

            var actual = _saveState.BotOnCallBackQueryReceived(_botClientMock, _callbackQueryStub).Result;

            A.CallTo(() => _stateProviderMock.GetChatState(_messageStub.Chat.Id)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _eventProviderMock.DeleteEvent(_chatStateStub.ActiveNotificationId)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _stateProviderMock.SaveChatState(_chatStateStub)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _botClientMock.SendTextMessageAsync(_messageStub.Chat.Id, A<string>.Ignored, A<ParseMode>.Ignored,
                                                           A<IEnumerable<MessageEntity>>.Ignored, A<bool>.Ignored,
                                                           A<bool>.Ignored, A<int>.Ignored, A<bool>.Ignored,
                                                           A<IReplyMarkup>.Ignored, A<CancellationToken>.Ignored))
                                                           .MustHaveHappenedOnceExactly();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BotSendMessage_ShouldSendTextMessage()
        {
            _saveState.BotSendMessage(_botClientMock, _messageStub.Chat.Id);

            A.CallTo(() => _botClientMock.SendTextMessageAsync(_messageStub.Chat.Id, A<string>.Ignored, A<ParseMode>.Ignored,
                                                           A<IEnumerable<MessageEntity>>.Ignored, A<bool>.Ignored,
                                                           A<bool>.Ignored, A<int>.Ignored, A<bool>.Ignored,
                                                           A<IReplyMarkup>.Ignored, A<CancellationToken>.Ignored))
                                                           .MustHaveHappenedOnceExactly();
        }
    }
}
