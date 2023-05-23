
namespace IoTHubAPI.DatabaseSettings
{
    public interface IIotHubDatabaseSettings
    {
        string UsersCollectionName { get; set; }
        string DevicesCollectionName { get; set; }
        string NotificationCollectionName { get; set; }
        string DeviceDataFieldsCollectionName { get; set; }
        string ActionsCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
