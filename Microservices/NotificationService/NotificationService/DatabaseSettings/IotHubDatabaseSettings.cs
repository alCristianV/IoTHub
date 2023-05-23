
namespace IoTHubAPI.DatabaseSettings
{
    public class IotHubDatabaseSettings : IIotHubDatabaseSettings
    {
        public string UsersCollectionName { get; set; }
        public string DevicesCollectionName { get; set; }
        public string NotificationCollectionName { get; set; }
        public string DeviceDataFieldsCollectionName { get; set; }
        public string ActionsCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
