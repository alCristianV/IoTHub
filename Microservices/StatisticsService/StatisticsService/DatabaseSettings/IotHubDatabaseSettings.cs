
namespace IoTHubAPI.DatabaseSettings
{
    public class IotHubDatabaseSettings : IIotHubDatabaseSettings
    {
        public string MessagesCollectionName { get; set; }
        public string DeviceDataFieldsCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
