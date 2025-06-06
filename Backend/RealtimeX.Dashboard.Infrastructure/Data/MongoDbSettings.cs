namespace RealtimeX.Dashboard.Infrastructure.Data
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string RealTimeDataCollectionName { get; set; }
        public string AnnouncementsCollectionName { get; set; }
        public string ChatMessagesCollectionName { get; set; }
        public string UsersCollectionName { get; set; }
    }
} 