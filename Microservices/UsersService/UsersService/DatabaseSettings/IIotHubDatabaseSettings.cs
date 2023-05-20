
namespace IoTHubAPI.DatabaseSettings
{
    public interface IIotHubDatabaseSettings
    {
        string UsersCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
