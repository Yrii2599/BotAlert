using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Hosting;
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
    public class NotificationSenderServiceTests
    {
        private readonly ITelegramBotClient _botClientMock;
        private readonly IEventProvider _eventProviderMock;
        private readonly CancellationToken _cancellationToken;

        private readonly IHostedService _notificationSender;

        public NotificationSenderServiceTests()
        {
            _botClientMock = A.Fake<ITelegramBotClient>();
            _eventProviderMock = A.Fake<IEventProvider>();
            _cancellationToken = new CancellationToken();

            _notificationSender = new NotificationSenderService(_botClientMock, _eventProviderMock);
        }

        [Fact]
        public void StartAsync_WorkCorrectlyWithZeroElements()
        {
            var funcRes = new List<Event>();

            var expected = Task.CompletedTask;

            A.CallTo(() => _eventProviderMock.GetAllNotificationsToBeSentNow()).Returns(funcRes);

            var actual = _notificationSender.StartAsync(_cancellationToken);

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
        public void StartAsync_WorkCorrectlyWithOneElement()
        {
            var funcRes = new List<Event>() { new Event(1234, "title") };

            var expected = Task.CompletedTask;

            A.CallTo(() => _eventProviderMock.GetAllNotificationsToBeSentNow()).Returns(funcRes);

            var actual = _notificationSender.StartAsync(_cancellationToken);

            A.CallTo(() => _botClientMock.SendTextMessageAsync(funcRes.First().ChatId,
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
        public void StopAsync_WorkCorrectly()
        {
            var expected = Task.CompletedTask;

            var actual = _notificationSender.StopAsync(_cancellationToken);

            Assert.Equal(expected, actual);
        }
    }
}
