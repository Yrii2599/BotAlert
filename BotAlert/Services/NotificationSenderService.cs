using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Hosting;
using BotAlert.Models;
using BotAlert.Interfaces;
using Telegram.Bot;

namespace BotAlert.Services
{
    public class NotificationSenderService : IHostedService
    {
        private const int OneMinuteInMilliseconds = 60000;

        private readonly ITelegramBotClient _botClient;
        private readonly IEventProvider _eventProvider;
        private readonly IStateProvider _stateProvider;
        private readonly ILocalizerFactory _localizerFactory;

        public NotificationSenderService
            (ITelegramBotClient botClient, IEventProvider eventProvider, IStateProvider stateProvider, ILocalizerFactory localizerFactory)
        {
            _botClient = botClient;
            _eventProvider = eventProvider;
            _stateProvider = stateProvider;
            _localizerFactory = localizerFactory;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            Task.Run(() => SendNotifications(stoppingToken));

            return Task.CompletedTask;
        }

        private async void SendNotifications(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await SendMessages(_eventProvider.GetAllNotificationsToBeSentNow());
                await Task.Delay(OneMinuteInMilliseconds);
            }
        }

        private async Task SendMessages(List<Event> eventsList)
        {
            foreach (var eventObj in eventsList)
            {
                await _botClient.SendTextMessageAsync(eventObj.ChatId, 
                    eventObj.ToString(_localizerFactory.GetLocalizer(_stateProvider.GetChatState(eventObj.ChatId).Language)));

                _eventProvider.DeleteEvent(eventObj.Id);
            }
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }
    }
}