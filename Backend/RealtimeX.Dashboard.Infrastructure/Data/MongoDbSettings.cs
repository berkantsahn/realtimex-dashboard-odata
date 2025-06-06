namespace RealtimeX.Dashboard.Infrastructure.Data
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string RealTimeDataCollectionName { get; set; } = string.Empty;
        public string AnnouncementsCollectionName { get; set; } = string.Empty;
        public string ChatMessagesCollectionName { get; set; } = string.Empty;
        public string UsersCollectionName { get; set; } = string.Empty;
    }
} 