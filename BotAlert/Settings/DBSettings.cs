namespace BotAlert.Settings
{
    public class DBSettings
    {
        public const string ConfigKey = "MongoDBSettings";

        public string ConnectionString { get; set; }

        public string DatabaseName { get; set; }
    }
}
