using System;
using System.Linq;
using System.Net.Mime;
using System.Security.Authentication;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using BotAlert.Handlers;
using BotAlert.Models;
using BotAlert.States;
using BotAlert.Services;
using BotAlert.Settings;
using BotAlert.Factories;
using BotAlert.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types.Enums;
using MongoDB.Driver;
using SimpleInjector;

namespace BotAlert
{
    public class Startup
    {
        private readonly Container _container = new Container();
        private TelegramSettings _telegramSettings;
        private DBSettings _dbSettings;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            _telegramSettings = Configuration.GetSection(TelegramSettings.ConfigKey).Get<TelegramSettings>();
            _dbSettings = Configuration.GetSection(DBSettings.ConfigKey).Get<DBSettings>();

            services.AddControllers().AddNewtonsoftJson();
            services.AddHttpClient("tgwebhook")
                .AddTypedClient<ITelegramBotClient>(httpClient
                                                        => new TelegramBotClient(_telegramSettings.BotApiKey, httpClient));

            services.AddHealthChecks().AddMongoDb(_dbSettings.ConnectionString);

            services.AddSimpleInjector(_container, options =>
            {
                options.AddHostedService<NotificationSenderService>();
                options.AddAspNetCore().AddControllerActivation();
            });

            InitializeContainer();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapHealthChecks("/Health", new HealthCheckOptions
                {
                    ResponseWriter = async (context, report) =>
                    {
                        var result = JsonSerializer.Serialize(
                            new
                            {
                                status = report.Status.ToString(),
                                monitors = report.Entries.Select(
                                    e => new { key = e.Key, value = Enum.GetName(typeof(HealthStatus), e.Value.Status) })
                            });
                        context.Response.ContentType = MediaTypeNames.Application.Json;
                        await context.Response.WriteAsync(result);
                    }
                });
                endpoints.MapControllerRoute(name: "tgwebhook",
                                             pattern: $"bot/{_telegramSettings.BotApiKey}",
                                             new { controller = "Webhook", action = "Post" });
            });

            _container.Verify();

            if (_telegramSettings.UseWebHook)
            {
                InitializeWebHook();
            }
            else
            {
                InitializePolling();
            }
        }

        private void InitializeContainer()
        {
            //Register factories
            _container.RegisterInstance<IStateFactory>(new StateFactory
            {
                { ContextState.MainState, () => _container.GetInstance<MainState>() },
                { ContextState.InputTitleState, () => _container.GetInstance<InputTitleState>() },
                { ContextState.InputDateState, () => _container.GetInstance<InputDateState>() },
                { ContextState.InputWarnDateKeyboardState, () => _container.GetInstance<InputWarnDateKeyboardState>() },
                { ContextState.InputWarnDateState, () => _container.GetInstance<InputWarnDateState>() },
                { ContextState.InputDescriptionKeyboardState, () => _container.GetInstance<InputDescriptionKeyboardState>() },
                { ContextState.InputDescriptionState, () => _container.GetInstance<InputDescriptionState>() },
                { ContextState.SaveState, () => _container.GetInstance<SaveState>() },
                { ContextState.GetAllNotificationsState, () => _container.GetInstance<GetAllNotificationsState>() },
                { ContextState.GetNotificationDetailsState, () => _container.GetInstance<GetNotificationDetailsState>() },
                { ContextState.InputDeleteKeyboardState, () => _container.GetInstance<InputDeleteKeyboardState>() },
                { ContextState.EditState, () => _container.GetInstance<EditState>() },
                { ContextState.InputTimeZoneState, () => _container.GetInstance<InputTimeZoneState>() },
                { ContextState.InputEventTimeZoneKeyboardState, () => _container.GetInstance<InputEventTimeZoneKeyboardState>() },
            });

            //Register states
            _container.Register<MainState>();
            _container.Register<InputTitleState>();
            _container.Register<InputDateState>();
            _container.Register<InputWarnDateKeyboardState>();
            _container.Register<InputWarnDateState>();
            _container.Register<InputDescriptionKeyboardState>();
            _container.Register<InputDescriptionState>();
            _container.Register<SaveState>();
            _container.Register<GetAllNotificationsState>();
            _container.Register<GetNotificationDetailsState>();
            _container.Register<InputDeleteKeyboardState>();
            _container.Register<EditState>();
            _container.Register<InputTimeZoneState>();
            _container.Register<InputEventTimeZoneKeyboardState>();

            //Register services
            _container.Register<IStateProvider, StateProvider>(Lifestyle.Singleton);
            _container.Register<IEventProvider, EventProvider>(Lifestyle.Singleton);

            _container.Register(typeof(TelegramUpdatesHandler));
            _container.Register<ITelegramUpdatesHandler, TelegramUpdatesHandler>();

            //Register mongo
            var mongoDatabase = InitializeMongoDatabase(_dbSettings);
            _container.RegisterInstance(mongoDatabase);

            //Register config settings
            _container.RegisterInstance(_telegramSettings);
            _container.RegisterInstance(_dbSettings);

            //Register ITelegramBotClient
            _container.RegisterInstance(CreateClient(_telegramSettings.BotApiKey));
        }

        private IMongoDatabase InitializeMongoDatabase(DBSettings mongoDbSettings)
        {
            var settings = MongoClientSettings.FromUrl(
             new MongoUrl(mongoDbSettings.ConnectionString)
         );
            settings.SslSettings =
                new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };
            var client = new MongoClient(settings);
            var database = client.GetDatabase(mongoDbSettings.DatabaseName);

            return database;
        }

        private ITelegramBotClient CreateClient(string apiKey) => new TelegramBotClient(apiKey);

        private void InitializePolling()
        {
            var handler = _container.GetInstance<ITelegramUpdatesHandler>();

            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            var source = new CancellationTokenSource();
            var botClient = _container.GetInstance<ITelegramBotClient>();
            Task.FromResult(botClient.DeleteWebhookAsync(cancellationToken: source.Token));
            botClient.StartReceiving(new DefaultUpdateHandler(handler.HandleUpdateAsync, handler.HandleErrorAsync), source.Token);
        }

        private void InitializeWebHook()
        {
            var botClient = _container.GetInstance<ITelegramBotClient>();
            botClient.DeleteWebhookAsync();
            var webhookAddress = @$"{_telegramSettings.HostAddress}/webhook";
            botClient.SetWebhookAsync(
                url: webhookAddress,
                allowedUpdates: Array.Empty<UpdateType>()).GetAwaiter().GetResult();
        }
    }
}