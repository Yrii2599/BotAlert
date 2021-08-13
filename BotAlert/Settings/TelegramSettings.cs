namespace BotAlert.Settings
{
    public class TelegramSettings
    {
        public const string DateTimeFormat = "de-DE";

        public const int EventsPerPage = 5;

        public const string ConfigKey = "TelegramSettings";

        public string BotApiKey { get; set; }

    }
}
