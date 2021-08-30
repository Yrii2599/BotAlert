using System.Security.Authentication;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BotAlert.Handlers;
using BotAlert.Models;
using BotAlert.States;
using BotAlert.Services;
using BotAlert.Settings;
using BotAlert.Factories;
using BotAlert.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using MongoDB.Driver;
using SimpleInjector;

namespace BotAlert
{
    public class Startup
    {
        private readonly Container _container = new Container();

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSimpleInjector(_container, options =>
            {
                options.AddHostedService<NotificationSenderService>();
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
            });

            _container.Verify();

            InitializeTelegramListener();
        }

        private void InitializeContainer()
        {
            var mongoDbSettings = Configuration.GetSection(DBSettings.ConfigKey).Get<DBSettings>();
            var telegramSettings = Configuration.GetSection(TelegramSettings.ConfigKey).Get<TelegramSettings>();

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

            //Register services
            _container.Register<IStateProvider, StateProvider>(Lifestyle.Singleton);
            _container.Register<IEventProvider, EventProvider>(Lifestyle.Singleton);

            _container.Register(typeof(TelegramUpdatesHandler));
            _container.Register<ITelegramUpdatesHandler, TelegramUpdatesHandler>();

            //Register mongo
            var mongoDatabase = InitializeMongoDatabase(mongoDbSettings);
            _container.RegisterInstance(mongoDatabase);

            //Register config settings
            _container.RegisterInstance(Configuration.GetSection(TelegramSettings.ConfigKey).Get<TelegramSettings>());
            _container.RegisterInstance(mongoDbSettings);

            //Register ITelegramBotClient
            _container.RegisterInstance(CreateClient(telegramSettings.BotApiKey));
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

        private void InitializeTelegramListener()
        {
            var handler = _container.GetInstance<ITelegramUpdatesHandler>();

            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            var source = new CancellationTokenSource();
            _container.GetInstance<ITelegramBotClient>().StartReceiving(new DefaultUpdateHandler(handler.HandleUpdateAsync, handler.HandleErrorAsync), source.Token);
        }
    }
}
