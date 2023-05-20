
namespace IoTHubAPI.DatabaseSettings
{
    public class IotHubDatabaseSettings : IIotHubDatabaseSettings
    {
        public string UsersCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
