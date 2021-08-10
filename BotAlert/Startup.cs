using System.Threading;
using BotAlert.Controllers;
using BotAlert.Interfaces;
using BotAlert.Service;
using BotAlert.Services;
using BotAlert.Settings;
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
            Configuration=configuration;
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
