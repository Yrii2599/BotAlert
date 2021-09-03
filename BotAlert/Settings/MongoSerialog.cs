using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Formatting.Compact;

namespace BotAlert.Settings
{
    public static class HostBuilderSettings
    {
        public static IHostBuilder UseConfiguredSerilog(this IHostBuilder hostBuilder)
        {
            hostBuilder.UseSerilog((context, services, configuration) => configuration
                                         .Enrich.FromLogContext()
                                         .WriteTo.AzureBlobStorage(context.Configuration.GetSection("LoggerSettings")
                                        .Get<LoggerSetting>().ConnectionString, 
                                         Serilog.Events.LogEventLevel.Debug, null, "{yyyy}-{MM}/{dd}-log.txt"));

            return hostBuilder;
        }
    }
}