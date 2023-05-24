
namespace IoTHubAPI.DatabaseSettings
{
    public interface IIotHubDatabaseSettings
    {
        string MessagesCollectionName { get; set; }
        string DeviceDataFieldsCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
