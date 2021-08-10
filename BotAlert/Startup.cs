using System.Threading;
using BotAlert.Services;
using BotAlert.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BotAlert
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
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

            var telegramSettings = Configuration.GetSection(TelegramSettings.ConfigKey).Get<TelegramSettings>();
            var dbSettings = Configuration.GetSection(DBSettings.ConfigKey).Get<DBSettings>();

            // Уточнить как передавать dbSettings в каждый новый вызов конструктора DBSettings
            EventDBService.Settings = dbSettings;
            TelegramBotExtensions.StartListeningAsync(telegramSettings.BotApiKey, new CancellationToken());
        }
    }
}
