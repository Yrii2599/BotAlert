using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Hosting;
using BotAlert.Models;
using BotAlert.Interfaces;
using Telegram.Bot;

public class NotificationSenderService : IHostedService
{
    const int OneMinuteInMilliseconds = 60000;

    private readonly IEventProvider _eventProvider;
    private readonly ITelegramBotClient _botClient;

    public NotificationSenderService(ITelegramBotClient botClient, IEventProvider eventProvider)
    {
        _botClient = botClient;
        _eventProvider = eventProvider;
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
            SendMessages(_eventProvider.GetAllNotificationsToBeSentNow());
            await Task.Delay(OneMinuteInMilliseconds);
        }
    }

    private async Task SendMessages(List<Event> eventsList)
    {
        foreach (var eventObj in eventsList)
        {
            _botClient.SendTextMessageAsync(eventObj.ChatId, eventObj.ToString());
            _eventProvider.DeleteEvent(eventObj.Id);
        }
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }
}