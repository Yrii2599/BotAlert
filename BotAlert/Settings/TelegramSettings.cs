namespace BotAlert.Settings
{
    public class TelegramSettings
    {
        public static int EventsPerPage { get; } = 5;

        public static string DateTimeFormat { get; } = "de-DE";

        public static string ConfigKey { get; } = "TelegramSettings";

        public string BotApiKey { get; set; }
    }
}
