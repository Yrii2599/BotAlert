using System.Collections.Generic;
using System.Threading;
using BotAlert.Helpers;
using FakeItEasy;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Xunit;

namespace BotAlert.Tests
{
   public class InteractionHelperTests
    {
        private readonly ITelegramBotClient _botClientMock;
        private InlineKeyboardMarkup _keyboardMock;
        private Message _messageMock;

        public InteractionHelperTests()
        {
            _keyboardMock = A.Fake<InlineKeyboardMarkup>();
            _botClientMock = A.Fake<ITelegramBotClient>();
            _messageMock = A.Fake<Message>();
            _messageMock.Chat = A.Fake<Chat>();
        }

        [Fact]
        public void SendInlineKeyboard_ReturnSendTextMessageAsync()
        {
            InteractionHelper.SendInlineKeyboard(_botClientMock, _messageMock.Chat.Id, _messageMock.Text,
                                                 _keyboardMock);
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
