using System.Threading;
using BotAlert.Handlers;
using BotAlert.Factories;
using BotAlert.Interfaces;
using BotAlert.Models;
using BotAlert.Service;
using BotAlert.Services;
using BotAlert.Settings;
using BotAlert.States;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

            services.AddSimpleInjector(_container);
            InitializeContainer();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if(env.IsDevelopment())
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

            //Register factories
            _container.RegisterInstance<IStateFactory>(new StateFactory 
            {
                { ContextState.MainState, () => _container.GetInstance<MainState>() },
                { ContextState.InputTitleState, () => _container.GetInstance<InputTitleState>() },
                { ContextState.InputDateState, () => _container.GetInstance<InputDateState>() },
                { ContextState.InputWarnDateKeyboard, () => _container.GetInstance<InputWarnDateKeyboard>() },
                { ContextState.InputWarnDateState, () => _container.GetInstance<InputWarnDateState>() },
                { ContextState.InputDescriptionKeyboardState, () => _container.GetInstance<InputDescriptionKeyboardState>() },
                { ContextState.InputDescriptionState, () => _container.GetInstance<InputDescriptionState>() },
                { ContextState.SaveState, () => _container.GetInstance<SaveState>() },
                { ContextState.GetAllNotificationsState, () => _container.GetInstance<GetAllNotificationsState>() },
                { ContextState.GetNotificationDetails, () => _container.GetInstance<GetNotificationDetails>() }
            });

            //Register states
            // Lifestyle.Scoped выдает ошибку
            _container.Register<MainState>();
            _container.Register<InputTitleState>();
            _container.Register<InputDateState>();
            _container.Register<InputWarnDateKeyboard>();
            _container.Register<InputWarnDateState>();
            _container.Register<InputDescriptionKeyboardState>();
            _container.Register<InputDescriptionState>();
            _container.Register<SaveState>();
            _container.Register<GetAllNotificationsState>();
            _container.Register<GetNotificationDetails>();

            //Register services
            _container.Register<IStateProvider, StateProvider>();
            _container.Register<IEventProvider, EventProvider>();

            _container.Register(typeof(TelegramUpdatesHandler));
            _container.Register<ITelegramUpdatesHandler, TelegramUpdatesHandler>();

            //Register mongo
            var mongoDatabase = InitializeMongoDatabase(mongoDbSettings);
            _container.RegisterInstance(mongoDatabase);

            //Register config settings
            _container.RegisterInstance(Configuration.GetSection(TelegramSettings.ConfigKey).Get<TelegramSettings>());
            _container.RegisterInstance(mongoDbSettings);
        }

        private IMongoDatabase InitializeMongoDatabase(DBSettings mongoDbSettings)
        {
            var client = new MongoClient(mongoDbSettings.ConnectionString);
            var database = client.GetDatabase(mongoDbSettings.DatabaseName);

            return database;
        }

        private void InitializeTelegramListener()
        {
            var telegramSettings = _container.GetInstance<TelegramSettings>();
            var handler = _container.GetInstance<ITelegramUpdatesHandler>();
            TelegramBotExtensions.StartListeningAsync(telegramSettings.BotApiKey, new CancellationToken(), handler);
        }
    }
}
